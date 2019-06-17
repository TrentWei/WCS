using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private void funStroreIn()
        {
            funStoreIn_GetStoreInCommandAndWritePLC();
            funStoreIn_CrateCraneCommand();
            funStoreIn_CraneCommandFinish();
        }


        /// <summary>
        /// 读取状态0的入库命名，修改状态=1，同时写入PLC;
        /// </summary>
        private void funStoreIn_GetStoreInCommandAndWritePLC()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                for (int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                {
                    string strBufferName = bCRData[intIndex]._BufferName;
                    int intBufferIndex = bCRData[intIndex]._BufferIndex;

                    #region 码头站口入库
                    //命令为空、目的站为空、模式0、自动、荷有、
                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                        && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                        && bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On
                        && bufferData[intBufferIndex]._ReturnRequest == "0"
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._Mode == Buffer.StnMode.None
                        && strBufferName == STN_NO.StoreInA53)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!---53";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!---53";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion Pallet On Station && BCR Trigger On
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR")
                        {
                            #region Read Error

                            #region BCR Read Fail
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail!";
                            funWriteSysTraceLog(strMsg);

                            bCRData[intIndex].funClear();
                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "2->0|";
                            strMsg += "ERROR|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);
                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear

                            string[] strValues = new string[] { "4" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                            funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "扫码失败！", intBufferIndex);

                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;


                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "扫描成功!";
                            strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                            funWriteSysTraceLog(strMsg);

                            strSQL = "SELECT * FROM CMD_MST WHERE PLT_NO='" + strBCR + "' and CMD_MODE='1' and CMD_STS='0'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strIoType = dtCmdSno.Rows[0]["IO_TYPE"].ToString();
                                string strCmdSno = dtCmdSno.Rows[0]["CMD_SNO"].ToString();

                                if (strIoType == IO_TYPE.StoreIn15 || strIoType == IO_TYPE.StoreIn16)
                                {
                                    string[] strValues = new string[] { strCmdSno, CMDMode.StoreIn, "83" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);

                                        if (strIoType == IO_TYPE.StoreIn15) funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "空托盘入库！", intBufferIndex);
                                        if (strIoType == IO_TYPE.StoreIn16) funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "杂料入库！", intBufferIndex);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                }
                                else if (strIoType == IO_TYPE.StoreIn11)
                                {
                                    #region 删除已存在命令
                                    strMsg = strBufferName + "|";
                                    strMsg += strBCR + "|";
                                    strMsg += "存在未执行命令！";
                                    funWriteSysTraceLog(strMsg);
                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                    {
                                        try
                                        {
                                            strSQL = "INSERT INTO CMD_MST_LOG SELECT * FROM CMD_MST WHERE PLT_NO='" + strBCR + "'";
                                            if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                            {

                                                strMsg = strBufferName + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += "成功备份未执行命令！";
                                                funWriteSysTraceLog(strMsg);
                                                strSQL = "DELETE FROM CMD_MST WHERE  PLT_NO='" + strBCR + "'";
                                                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                                {

                                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                    {
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strBCR + "|";
                                                        strMsg += "成功删除未完成命令！";
                                                        funWriteSysTraceLog(strMsg);
                                                    }
                                                }
                                                else
                                                {
                                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                                    {
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strBCR + "|";
                                                        strMsg += "删除未完成命令失败！";
                                                        funWriteSysTraceLog(strMsg);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                                {
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += "备份未执行命令失败！|";
                                                    funWriteSysTraceLog(strMsg);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                            {
                                                strMsg = strBufferName + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += "备份删除未执行命令时发生异常！|";
                                                strMsg += ex.ToString();
                                                funWriteSysTraceLog(strMsg);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                //算出托盘上所有产品净重量
                                strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                                strSQL += " WHERE B.STATUS IN ('0','W','I')  AND Nvl(LOC,' ')=' ' AND P.PLT_NO='" + strBCR + "'";
                                strSQL += " GROUP BY P.SUB_NO  ";
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    #region 产生新的入库命令
                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                    {
                                        try
                                        {
                                            string strLoaction = "";
                                            string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                            string strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                            string strCommandID = funGetCommandID();
                                            if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strBCR, strBufferName, strActualWeight, " ", " ", Trace.Inital))
                                            {
                                                if (funLockStoreInBox(strSubNo, LoactionState.IN))
                                                {
                                                    string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, "83" };
                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                    {
                                                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                        {
                                                            CommandInfo commandInfo = new CommandInfo();
                                                            commandInfo.CommandID = strCommandID;
                                                            commandInfo.PalletNo = strBCR;
                                                            commandInfo.CommandMode = Convert.ToInt32(CMDMode.StoreIn);


                                                            strMsg = strBufferName + "|";
                                                            strMsg += strBCR + "|";
                                                            strMsg += strCommandID + "|";
                                                            strMsg += "|";
                                                            strMsg += "StoreIn Command And WritePLC Success";
                                                            funWriteSysTraceLog(strMsg);

                                                            bCRData[intIndex].funClear();
                                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                                            strMsg += "2->0|";
                                                            strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                            strMsg += "BCR Clear!";
                                                            funWriteSysTraceLog(strMsg);

                                                            strValues = new string[] { "1" };
                                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);

                                                            funSetKanbanInfo(KanbanModel.IN, strBufferName, commandInfo, intBufferIndex);

                                                        }
                                                        else
                                                        {
                                                            #region StoreInAndTransactionCommit Fail
                                                            InitSys._MPLC.funClearMPLC(bufferData[intBufferIndex]._W_CmdSno);
                                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                            strMsg = strBufferName + "|";
                                                            strMsg += strBCR + "|";
                                                            strMsg += strCommandID + "|";
                                                            strMsg += "|";
                                                            strMsg += "StoreIn And TransactionCommit Fail!";
                                                            funWriteSysTraceLog(strMsg);
                                                            #endregion StoreInAndTransactionCommit Fail

                                                            strValues = new string[] { "1" };
                                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                            funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "事务提交失败！", intBufferIndex);

                                                        }
                                                    }
                                                    else
                                                    {
                                                        #region StoreInAndWriteMPLC Fail
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strBCR + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreInAndWriteMPLC Fail!";
                                                        funWriteSysTraceLog(strMsg);
                                                        #endregion StoreInAndWriteMPLC Fail

                                                        strValues = new string[] { "1" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                        funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "命令写入PLC失败！", intBufferIndex);

                                                    }
                                                }
                                                else
                                                {
                                                    #region Update StoreIn BOX_ID Fail
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += strCommandID + "|";
                                                    strMsg += "|";
                                                    strMsg += "Update StoreIn BOX_ID Fail!";
                                                    funWriteSysTraceLog(strMsg);
                                                    #endregion Update StoreIn PalletNo Fail

                                                    string[] strValues = new string[] { "2" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                    funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "子托盘跟新失败！", intBufferIndex);

                                                }
                                            }
                                            else
                                            {
                                                #region Insert Command Fail
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = strCommandID + "|";
                                                strMsg += strLoaction + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                                strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                                strMsg += "StroreIn Command Insert Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Insert Command Fail

                                                string[] strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "命令产生失败！", intBufferIndex);

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            strMsg = strBufferName + "|";
                                            strMsg += strBCR + "|";
                                            strMsg += "|";
                                            strMsg += "产生命令入库出现异常！";
                                            funWriteSysTraceLog(strMsg);
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Can't Find StroreIn PLT_MST&BOX
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bCRData[intIndex]._ResultID + "|";
                                    strMsg += "Can't Find StroreIn PLT_MST&BOX!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Can't Find StroreIn PLT_MST&BOX

                                    string[] strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                    funSetKanbanInfoERROR(KanbanModel.ERROR, strBufferName, "条码无资料！", intBufferIndex);
                                }
                            }
                            #endregion Read OK
                        }

                    }
                    #endregion

                    #region A41扫码确认

                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                        && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                        && !bufferData[intBufferIndex]._Discharged
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._BufferName == STN_NO.StoreInA41)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion Pallet On Station && BCR Trigger On
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR")
                        {
                            #region Read Error

                            #region BCR Read Fail
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail! 放行";
                            funWriteSysTraceLog(strMsg);

                            bCRData[intIndex].funClear();
                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "2->0|";
                            strMsg += "ERROR|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);
                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear

                            string[] strValues = new string[] { "1" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST ";
                            strSQL += "WHERE PLT_NO='" + strBCR + "' ";
                            strSQL += "AND CMD_MODE in ('2','3')";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strCommandID = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                string strStnNo = dtCmdSno.Rows[0]["STN_NO"].ToString();
                                string strCmdMode = dtCmdSno.Rows[0]["CMD_MODE"].ToString();
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    if (string.IsNullOrEmpty(strStnNo)) strStnNo = "A48";
                                    string[] strValues = new string[] { strCommandID, strCmdMode, strStnNo.Replace("A", "") };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strBCR + "|";
                                        strMsg += strCommandID + "|";
                                        strMsg += strStnNo + "|";
                                        strMsg += "命令站口校验成功!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += "ERROR|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);

                                        strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                    }
                                }
                                else
                                {
                                    strMsg = strBufferName + "|";
                                    strMsg += strBCR + "|";
                                    strMsg += strCommandID + "|";
                                    strMsg += "命令站口分配跟新失败!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += "ERROR|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);


                                    string[] strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                }
                            }
                            else
                            {
                                strMsg = strBufferName + "|";
                                strMsg += strBCR + "|";
                                strMsg += "命令站口分配时托盘未查询到对应命令信息!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);

                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region A90扫码确认

                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                        && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                        && !bufferData[intBufferIndex]._Discharged
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn
                        && strBufferName == STN_NO.StoreInA90)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion Pallet On Station && BCR Trigger On
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR")
                        {
                            #region Read Error

                            #region BCR Read Fail
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail!";
                            funWriteSysTraceLog(strMsg);

                            bCRData[intIndex].funClear();
                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "2->0|";
                            strMsg += "ERROR|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);
                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear

                            string[] strValues = new string[] { "1" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST ";
                            strSQL += "WHERE PLT_NO='" + strBCR + "' ";
                            strSQL += "AND CMD_STS='0'";
                            strSQL += "AND CMD_MODE='1'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strCommandID = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                string strStnNo = dtCmdSno.Rows[0]["STN_NO"].ToString();
                                string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, strStnNo.Replace("A", "") };
                                if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                {
                                    strMsg = strBufferName + "|";
                                    strMsg += strBCR + "|";
                                    strMsg += strCommandID + "|";
                                    strMsg += strStnNo.ToString() + "|";
                                    strMsg += "命令站口校验成功!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += "ERROR|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);

                                    strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                }

                            }
                            else
                            {
                                strMsg = strBufferName + "|";
                                strMsg += strBCR + "|";
                                strMsg += "命令站口分配时托盘未查询到对应命令信息!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);

                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region A83称重机称重

                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                      && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                      && bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On
                      && bufferData[intBufferIndex]._ReturnRequest == "0"
                      && !bufferData[intBufferIndex]._Discharged
                      && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                      && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                      && bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn
                      && strBufferName == STN_NO.StoreInA83)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!";
                            funWriteSysTraceLog(strMsg);
                            if (bCRData[intIndex].funTriggerBCROn2(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion Pallet On Station && BCR Trigger On
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR" && bCRData[intIndex]._ResultID == "0")
                        {
                            #region BCR Read Fail
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail!";
                            funWriteSysTraceLog(strMsg);

                            bCRData[intIndex].funClear();
                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "2->0|";
                            strMsg += "ERROR|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);
                            string[] strValues = new string[] { "4" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR" && bCRData[intIndex]._ResultID != "0")
                        {
                            #region Read OK
                            string intWeight = bCRData[intIndex]._ResultID.Replace(' ', '+').Replace("+0", "").Replace("*", "");
                            funWriteSysTraceLog("A83称重处理1：" + intWeight);
                            //intWeight = Regex.Replace(intWeight, @"[^0-9]+", "");
                            //funWriteSysTraceLog("A83称重处理2：" + intWeight);
                            string[] strTmp = intWeight.Split('+');
                            int num = 0;
                            foreach (string item in strTmp)
                            {
                                intWeight = Regex.Replace(item, @"[^0-9]+", "0");
                                if (string.IsNullOrEmpty(intWeight)) intWeight = "0";
                                funWriteSysTraceLog("A83称重循环处理中：" + intWeight);
                                if (Convert.ToInt32(intWeight) > num) num = Convert.ToInt32(intWeight);
                            }
                            int strWeight = num;
                            bCRData[intIndex].funClear();

                            //int strWeight = int.Parse(Regex.Replace(intWeight.Substring(intWeight.LastIndexOf('+'), (intWeight.Length - intWeight.LastIndexOf('+')) - 1).Replace("+", ""), @"[^0-9]+", ""));
                            //int strWeight = int.Parse(System.Text.RegularExpressions.Regex.Replace(intWeight, "[0-9]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase));

                            funWriteSysTraceLog("A83|称重|处理前重量：" + intWeight + "|处理后重量：" + strWeight.ToString());
                            string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                            string strPltNo = string.Empty;
                            double Pack001 = 0;
                            double Pack002 = 0;
                            double inSKU_GROUP3 = 0;
                            double inBOX_COUNT = 0;

                            strSQL = "SELECT PLT_NO,ACTUAL_WEIGHT,IO_TYPE  ";
                            strSQL += "FROM CMD_MST ";
                            strSQL += "WHERE CMD_SNO='" + strCommandID + "'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                strPltNo = dtCmdSno.Rows[0]["PLT_NO"].ToString();
                                string strIO_TYPE = dtCmdSno.Rows[0]["IO_TYPE"].ToString();

                                if (strIO_TYPE == IO_TYPE.StoreIn11)
                                {
                                    double inActualWeight = double.Parse(dtCmdSno.Rows[0]["ACTUAL_WEIGHT"].ToString());
                                    #region 入库
                                    strSQL = "SELECT SUM(I.WIDTH) AS WIDTH,COUNT(B.SUB_NO) AS BOX_COUNT ";
                                    strSQL += "FROM  PLT_MST P  JOIN BOX B  ON P.SUB_NO=B.SUB_NO JOIN ITEM_MST I  ON B.ITEM_NO=I.ITEM_NO ";
                                    strSQL += "WHERE P.PLT_NO='" + strPltNo + "' ";
                                    strSQL += "GROUP By B.SUB_NO ";

                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        if (funGetCode(ref Pack001, ref Pack002))
                                        {
                                            inSKU_GROUP3 = Convert.ToDouble(dtCmdSno.Rows[0]["WIDTH"].ToString());
                                            inBOX_COUNT = Convert.ToDouble(dtCmdSno.Rows[0]["BOX_COUNT"].ToString());
                                            inActualWeight = (Pack001 * inBOX_COUNT + Pack002 * inSKU_GROUP3) + inActualWeight;
                                            strSQL = "UPDATE CMD_MST SET WEIGH='" + strWeight + "' ,ACTUAL_WEIGHT='" + inActualWeight + "' WHERE CMD_SNO='" + strCommandID + "'";
                                            if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                            {

                                                #region 对比重量

                                                if ((((strWeight >= inActualWeight && strWeight - inActualWeight <= inWeightERRORValue) || (strWeight < inActualWeight && strWeight - inActualWeight >= -inWeightERRORValue)) && strWeight <= 1200 || chkAutoWeight.Checked == false))
                                                {
                                                    string[] strValues = new string[] { "1" };
                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues))
                                                    {
                                                        bufferData[intBufferIndex]._Discharged = true;
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strPltNo + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreIn 称重成功";
                                                        strMsg += "or WriteMPLC 写入放行信号成功！";
                                                        funWriteSysTraceLog(strMsg);

                                                        bCRData[intIndex].funClear();
                                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                                        strMsg += "2->0|";
                                                        strMsg += "ERROR|";
                                                        strMsg += "BCR Clear!";
                                                        funWriteSysTraceLog(strMsg);
                                                    }
                                                }
                                                else
                                                {
                                                    strMsg = strBufferName + "| 称重重量:";
                                                    strMsg += strWeight + "|";
                                                    strMsg += strCommandID + "|";
                                                    strMsg += "货物超重！";
                                                    funWriteSysTraceLog(strMsg);

                                                    bCRData[intIndex].funClear();
                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                    strMsg += "2->0|";
                                                    strMsg += "ERROR|";
                                                    strMsg += "BCR Clear!";
                                                    funWriteSysTraceLog(strMsg);
                                                    string[] strValues = new string[] { "3" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                    strValues = new string[] { "1" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                strMsg = strBufferName + "| 称重重量:";
                                                strMsg += strWeight + "|";
                                                strMsg += strCommandID + "|";
                                                strMsg += "称重时获取皮重失败！";
                                                funWriteSysTraceLog(strMsg);

                                                bCRData[intIndex].funClear();
                                                strMsg = bCRData[intIndex]._BufferName + "|";
                                                strMsg += "2->0|";
                                                strMsg += "ERROR|";
                                                strMsg += "BCR Clear!";
                                                funWriteSysTraceLog(strMsg);
                                                string[] strValues = new string[] { "1" };

                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        strMsg = strBufferName + "| 称重重量:";
                                        strMsg += strWeight + "|";
                                        strMsg += strCommandID + "|";
                                        strMsg += "称重时获取箱数、规格失败！";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += "ERROR|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);


                                        string[] strValues = new string[] { "1" };

                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                        strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 空托盘入库
                                    string[] strValues = new string[] { "1" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strPltNo + "|";
                                        strMsg += strCommandID + "|";
                                        strMsg += "|";
                                        strMsg += "StoreIn 空托盘 Success";
                                        strMsg += "or WriteMPLC Discharged Success";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += "ERROR|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                strMsg = strBufferName + "| 称重重量:";
                                strMsg += strWeight + "|";
                                strMsg += "称重时未查询到命令信息!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);
                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region A08站口高度检测
                    if (bufferData[7]._EQUStatus.AutoMode == Buffer.Signal.On && bufferData[7]._EQUStatus.Load == Buffer.Signal.On
                        && bufferData[7]._EQUStatus.FrontLocation == Buffer.Signal.On && bufferData[7]._ReturnRequest == "0" && !string.IsNullOrWhiteSpace(bufferData[7]._CommandID)
                        && !bufferData[7]._Discharged
                        && !string.IsNullOrWhiteSpace(bufferData[7]._Destination) && bufferData[7]._Mode == Buffer.StnMode.StoreIn
                        && bufferData[7]._BufferName == STN_NO.StoreInA08)
                    {
                        string strCommandID = bufferData[7]._CommandID.PadLeft(5, '0');
                        string strLocSiez = string.Empty;
                        string strLoc = string.Empty;
                        string strSubNo = string.Empty;
                        string strStnNo = string.Empty;
                        string strCrnNo = string.Empty;
                        string strPltNo = string.Empty;
                        int[] intarResultDataForD310 = new int[10];
                        ///读取D310站口是否有异常 
                        if (InitSys._MPLC.funReadMPLC("D310", 10, ref intarResultDataForD310))
                        {
                            if (intarResultDataForD310[0] == 0)
                            {
                                if (bufferData[7]._EQUStatus.Siez == Buffer.Signal.Off) strLocSiez = "L";
                                if (bufferData[7]._EQUStatus.Siez == Buffer.Signal.On) strLocSiez = "H";
                                if (funGetCraneNo(ref strCrnNo, ref strStnNo))
                                {
                                    //strSQL = "SELECT B.STATUS,COUNT(B.STATUS) FROM CMD_MST C JOIN PLT_MST P ON C.PLT_NO=P.PLT_NO JOIN BOX B ON P.SUB_NO=B.SUB_NO ";
                                    //strSQL += "WHERE C.CMD_STS='0' ";
                                    //strSQL += "AND Nvl(C.LOC,' ')=' '";
                                    //strSQL += "AND C.TRACE='" + Trace.Inital + "'";
                                    //strSQL += "AND C.CMD_SNO='" + strCommandID + "'";
                                    //strSQL += "GROUP BY B.STATUS";
                                    strSQL = "SELECT PLT_NO,ACTUAL_WEIGHT,IO_TYPE,WEIGH  ";
                                    strSQL += "FROM CMD_MST ";
                                    strSQL += "WHERE CMD_SNO='" + strCommandID + "'";
                                    strSQL += "AND CMD_STS='0' ";
                                    strSQL += "AND TRACE='" + Trace.Inital + "'";
                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        if (dtCmdSno.Rows.Count == 1)
                                        {
                                            int inWeght = 0;
                                            if (!string.IsNullOrEmpty(dtCmdSno.Rows[0]["WEIGH"].ToString())) inWeght = int.Parse(dtCmdSno.Rows[0]["WEIGH"].ToString());
                                            if (inWeght > inThresholdWegiht) strLocSiez = "H";
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                            {
                                                try
                                                {
                                                    #region 获取储位
                                                    if (funGetEmptyLocation(strLocSiez, strCrnNo, ref strLoc))
                                                    {
                                                        if (funUpdateCommandLoc(strCommandID, strStnNo, strLoc))
                                                        {
                                                            #region 更新储位
                                                            if (funUpdateLocationMaster(strLoc, LoactionState.IN, strPltNo))
                                                            {
                                                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                                {
                                                                    string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, strStnNo.Replace("A", "") };
                                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[7]._W_CmdSno, strValues))
                                                                    {
                                                                        strValues = new string[] { "1" };
                                                                        if (InitSys._MPLC.funWriteMPLC(bufferData[7]._W_Discharged, strValues))
                                                                        {
                                                                            strMsg = bufferData[7]._BufferName + "|";
                                                                            strMsg += strPltNo + "|";
                                                                            strMsg += strCommandID + "|";
                                                                            strMsg += strStnNo + "|";
                                                                            strMsg += strLoc + "|";
                                                                            strMsg += "StoreIn UpdateLocationMaster Success";
                                                                            strMsg += "or WriteMPLC Discharged Success";
                                                                            strMsg += "放行成功！";
                                                                            funWriteSysTraceLog(strMsg);
                                                                        }
                                                                        bufferData[7]._Discharged = true;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                                strMsg = bufferData[7]._BufferName + "|";
                                                                strMsg += strPltNo + "|";
                                                                strMsg += strCommandID + "|";
                                                                strMsg += "|";
                                                                strMsg += "StoreIn UpdateLocationMaster Fail";
                                                                funWriteSysTraceLog(strMsg);
                                                                string[] strValues = new string[] { "2" };
                                                                InitSys._MPLC.funWriteMPLC(bufferData[7]._W_ReturnRequest, strValues);
                                                                bufferData[7]._Discharged = true;
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        strMsg = bufferData[7]._BufferName + "|";
                                                        strMsg += strPltNo + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += strCrnNo + ":巷道|";
                                                        strMsg += "StoreIn GetEmptyLocation Fail";
                                                        funWriteSysTraceLog(strMsg);

                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);

                                                        bufferData[7]._Discharged = true;
                                                        //string[] strValues = new string[] { "2" };
                                                        //InitSys._MPLC.funWriteMPLC(bufferData[7]._W_ReturnRequest, strValues);
                                                    }
                                                    #endregion
                                                }
                                                catch (Exception Ex)
                                                {
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    bufferData[7]._Discharged = true;
                                                    funWriteSysTraceLog(Ex.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    #endregion

                    #region A117入库口入库&分配储位
                    //命令为空、目的站为空、模式0、自动、荷有、
                    // 
                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                    && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                    && bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On
                    && bufferData[intBufferIndex]._ReturnRequest == "0"
                    && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                    && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                    && bufferData[intBufferIndex]._Mode == Buffer.StnMode.None
                    && strBufferName == STN_NO.StoreInA117)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!---117";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!---117";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion Pallet On Station && BCR Trigger On
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR")
                        {
                            #region Read Error

                            #region BCR Read Fail
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail!";
                            funWriteSysTraceLog(strMsg);

                            bCRData[intIndex].funClear();
                            strMsg = bCRData[intIndex]._BufferName + "|";
                            strMsg += "2->0|";
                            strMsg += "ERROR|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);

                            string[] strValues = new string[] { "4" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear

                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += " AND CMD_STS='0'";
                            strSQL += " AND CMD_MODE='1'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                Cmd_Mst CmdMst = new Cmd_Mst();
                                CmdMst.Cmd_Sno = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                CmdMst.Cmd_Mode = dtCmdSno.Rows[0]["CMD_MODE"].ToString();
                                CmdMst.Stn_No = dtCmdSno.Rows[0]["STN_NO"].ToString();
                                CmdMst.Loc = dtCmdSno.Rows[0]["LOC"].ToString();
                                CmdMst.Plt_No = dtCmdSno.Rows[0]["PLT_NO"].ToString();
                                #region 使用原有命令

                                string strCrnNo = string.Empty;
                                string strStnNo = string.Empty;
                                string strLocSiez = string.Empty;
                                if (string.IsNullOrEmpty(CmdMst.Stn_No) && string.IsNullOrEmpty(CmdMst.Loc))
                                {
                                    if (bufferData[intBufferIndex]._EQUAlarmStatus.OverHigh == Buffer.Signal.Off
                                        || bufferData[intBufferIndex]._EQUAlarmStatus.OverLength == Buffer.Signal.Off
                                        || bufferData[intBufferIndex]._EQUAlarmStatus.OverWidth == Buffer.Signal.Off)
                                    {
                                        if (bufferData[intBufferIndex]._EQUStatus.Siez == Buffer.Signal.Off) strLocSiez = "L";
                                        if (bufferData[intBufferIndex]._EQUStatus.Siez == Buffer.Signal.On) strLocSiez = "H";
                                        if (funGetCraneNo(ref strCrnNo, ref strStnNo))
                                        {
                                            if (funGetCraneNo(ref strCrnNo, ref CmdMst.Stn_No))
                                            {
                                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                                {
                                                    try
                                                    {
                                                        #region 获取储位
                                                        if (funGetEmptyLocation(strLocSiez, strCrnNo, ref CmdMst.Loc))
                                                        {
                                                            if (funUpdateCommandLoc(CmdMst.Cmd_Sno, CmdMst.Stn_No, CmdMst.Loc))
                                                            {
                                                                #region 更新储位
                                                                if (funUpdateLocationMaster(CmdMst.Loc, LoactionState.IN, CmdMst.Plt_No))
                                                                {
                                                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                                    {
                                                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                                                        strMsg += strBCR + "|";
                                                                        strMsg += CmdMst.Cmd_Sno + "|";
                                                                        strMsg += CmdMst.Stn_No + "|";
                                                                        strMsg += CmdMst.Loc + "|";
                                                                        strMsg += "StoreIn UpdateLocationMaster Success";
                                                                        funWriteSysTraceLog(strMsg);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                                                    strMsg += CmdMst.Plt_No + "|";
                                                                    strMsg += CmdMst.Cmd_Sno + "|";
                                                                    strMsg += "|";
                                                                    strMsg += "StoreIn UpdateLocationMaster Fail";
                                                                    funWriteSysTraceLog(strMsg);

                                                                    bCRData[intIndex].funClear();
                                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                                    strMsg += "2->0|";
                                                                    strMsg += "ERROR|";
                                                                    strMsg += "BCR Clear!";
                                                                    funWriteSysTraceLog(strMsg);

                                                                    string[] strValues = new string[] { "2" };
                                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                                                }
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                                                strMsg += strBCR + "|";
                                                                strMsg += CmdMst.Cmd_Sno + "|";
                                                                strMsg += CmdMst.Stn_No + "|";
                                                                strMsg += CmdMst.Loc + "|";
                                                                strMsg += "StoreIn UpdateCommandMaster Fail";
                                                                funWriteSysTraceLog(strMsg);

                                                                bCRData[intIndex].funClear();
                                                                strMsg = bCRData[intIndex]._BufferName + "|";
                                                                strMsg += "2->0|";
                                                                strMsg += "ERROR|";
                                                                strMsg += "BCR Clear!";
                                                                funWriteSysTraceLog(strMsg);

                                                                string[] strValues = new string[] { "1" };
                                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                                            strMsg += CmdMst.Plt_No + "|";
                                                            strMsg += CmdMst.Cmd_Sno + "|";
                                                            strMsg += ":巷道|";
                                                            strMsg += "StoreIn GetEmptyLocation Fail";
                                                            funWriteSysTraceLog(strMsg);

                                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                            //string[] strValues = new string[] { "2" };
                                                            //InitSys._MPLC.funWriteMPLC(bufferData[7]._W_ReturnRequest, strValues);
                                                        }
                                                        #endregion
                                                    }
                                                    catch (Exception Ex)
                                                    {
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        bufferData[intBufferIndex]._Discharged = true;
                                                        funWriteSysTraceLog(Ex.ToString());
                                                        string[] strValues = new string[] { "1" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                                        bCRData[intIndex].funClear();
                                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                                        strMsg += "2->0|";
                                                        strMsg += "ERROR|";
                                                        strMsg += "BCR Clear!";
                                                        funWriteSysTraceLog(strMsg);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (dtCmdSno.Rows.Count == 1)
                                {
                                    #region  命令PLC写入
                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                    {
                                        try
                                        {
                                            if (funUpdateCommand(CmdMst.Cmd_Sno, CommandState.Inital, Trace.StoreIn_GetStoreInCommandAndWritePLC))
                                            {
                                                string[] strValues = new string[] { CmdMst.Cmd_Sno, CMDMode.StoreIn, CmdMst.Stn_No.Replace("A", "") };
                                                if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                {

                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);

                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += CmdMst.Cmd_Sno + "|";
                                                    strMsg += "|";
                                                    strMsg += "StoreIn Command And WritePLC Success";
                                                    funWriteSysTraceLog(strMsg);

                                                    bCRData[intIndex].funClear();
                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                    strMsg += "2->0|";
                                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                    strMsg += "BCR Clear!";
                                                    funWriteSysTraceLog(strMsg);

                                                }
                                                else
                                                {
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += CmdMst.Cmd_Sno + "|";
                                                    strMsg += "|";
                                                    strMsg += "StoreIn Command And WritePLC Faill";
                                                    funWriteSysTraceLog(strMsg);

                                                    bCRData[intIndex].funClear();
                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                    strMsg += "2->0|";
                                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                    strMsg += "BCR Clear!";
                                                    funWriteSysTraceLog(strMsg);
                                                }
                                            }
                                            else
                                            {
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = strBufferName + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += CmdMst.Cmd_Sno + "|";
                                                strMsg += "|";
                                                strMsg += "StoreIn Command And Update Faill";
                                                funWriteSysTraceLog(strMsg);

                                                bCRData[intIndex].funClear();
                                                strMsg = bCRData[intIndex]._BufferName + "|";
                                                strMsg += "2->0|";
                                                strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                strMsg += "BCR Clear!";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                        }
                                        catch (Exception re)
                                        {
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);

                                            bCRData[intIndex].funClear();
                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                            strMsg += "2->0|";
                                            strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                            strMsg += "BCR Clear!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    strMsg = strBufferName + "|";
                                    strMsg += strBCR + "|";
                                    strMsg += "存在多条未执行命令！";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += "ERROR|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);
                                }
                                #endregion
                            }
                            else
                            {
                                string strLoaction = string.Empty;
                                string strCrnNo = string.Empty;
                                string strStnNo = string.Empty;
                                string strLocSiez = string.Empty;


                                strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                                strSQL += " WHERE B.STATUS IN ('0','W') AND Nvl(LOC,' ')=' ' AND P.PLT_NO='" + strBCR + "'";
                                strSQL += " GROUP BY P.SUB_NO  ";

                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    #region 产生新的入库命令

                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                    {
                                        try
                                        {
                                            string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                            string strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                            string strCommandID = funGetCommandID();

                                            if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strBCR, strStnNo, strActualWeight, strLocSiez, " ", Trace.Inital))
                                            {

                                                if (funLockStoreInBox(strSubNo, LoactionState.IN))
                                                {
                                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                    {
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strBCR + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreIn Command And WritePLC Success";
                                                        funWriteSysTraceLog(strMsg);
                                                    }
                                                    else
                                                    {
                                                        #region StoreInAndTransactionCommit Fail
                                                        InitSys._MPLC.funClearMPLC(bufferData[intBufferIndex]._W_CmdSno);
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strBCR + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreIn And TransactionCommit Fail!";
                                                        funWriteSysTraceLog(strMsg);

                                                        bCRData[intIndex].funClear();
                                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                                        strMsg += "2->0|";
                                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                        strMsg += "BCR Clear!";
                                                        funWriteSysTraceLog(strMsg);

                                                        string[] strValues = new string[] { "1" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                                        #endregion StoreInAndTransactionCommit Fail
                                                    }
                                                }
                                                else
                                                {
                                                    #region Update StoreIn BOX_ID Fail
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += strCommandID + "|";
                                                    strMsg += "|";
                                                    strMsg += "Update StoreIn BOX_ID Fail!";
                                                    funWriteSysTraceLog(strMsg);

                                                    bCRData[intIndex].funClear();
                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                    strMsg += "2->0|";
                                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                    strMsg += "BCR Clear!";
                                                    funWriteSysTraceLog(strMsg);


                                                    string[] strValues = new string[] { "1" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                                    #endregion Update StoreIn PalletNo Fail
                                                }
                                            }
                                            else
                                            {
                                                #region Insert Command Fail

                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = strCommandID + "|";
                                                strMsg += strLoaction + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                                strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                                strMsg += "StroreIn Command Insert Fail!";
                                                funWriteSysTraceLog(strMsg);


                                                bCRData[intIndex].funClear();
                                                strMsg = bCRData[intIndex]._BufferName + "|";
                                                strMsg += "2->0|";
                                                strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                strMsg += "BCR Clear!";
                                                funWriteSysTraceLog(strMsg);

                                                string[] strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                                #endregion Insert Command Fail
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            strMsg = strBufferName + "|";
                                            strMsg += strBCR + "|";
                                            strMsg += "|";
                                            strMsg += ex.ToString();
                                            funWriteSysTraceLog(strMsg);

                                            bCRData[intIndex].funClear();
                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                            strMsg += "2->0|";
                                            strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                            strMsg += "BCR Clear!";
                                            funWriteSysTraceLog(strMsg);

                                            string[] strValues = new string[] { "1" };
                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Can't Find StroreIn PLT_MST&BOX
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bCRData[intIndex]._ResultID + "|";
                                    strMsg += "Can't Find StroreIn PLT_MST&BOX!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);

                                    string[] strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                    #endregion Can't Find StroreIn PLT_MST&BOX
                                }

                            }
                            #endregion Read OK
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }

        /// <summary>
        /// 创建EQU_CMD命令
        /// </summary>
        private void funStoreIn_CrateCraneCommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreIn)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    #region 下达主机命令
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                        && (strBufferName == STN_NO.StoreInA113 || strBufferName == STN_NO.StoreInA108 || strBufferName == STN_NO.StoreInA103 || strBufferName == STN_NO.StoreInA98 || strBufferName == STN_NO.StoreInA93))
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'1'";
                        strSQL += " AND Nvl(LOC,' ')!=' '";
                        strSQL += " AND STN_NO='" + strBufferName + "'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND ((CMD_MODE='1'";
                        strSQL += " AND TRACE in('" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "'))";
                        strSQL += " OR (CMD_MODE='3'";
                        strSQL += " AND TRACE='" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "'))";
                        strSQL += " ORDER BY LOC DESC";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                        {
                            CommandInfo commandInfo = new CommandInfo();
                            commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                            commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                            commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                            commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                            commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                            commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                            commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                            commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();
                            if (!funCheckCraneExistsCommand(CraneMode.StoreIn, stnDef.StationIndex.ToString()))
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
                                        if (funCrateCraneCommand(strCommandID, stnDef.StationIndex.ToString(), CraneMode.StoreIn, CraneMode.StoreIn, funGetCrnLoc(commandInfo.Loaction), commandInfo.Priority))
                                        {
                                            if (funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreIn_CrateCraneCommand))
                                            {
                                                #region Update Command & Create StoreIn Crane Command Success
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                                strMsg = strCommandID + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "StoreIn Crane Command Create Success!";
                                                funWriteSysTraceLog(strMsg);

                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "|";
                                                strMsg += Trace.StoreIn_GetStoreInCommandAndWritePLC + "->" + Trace.StoreIn_CrateCraneCommand + "|";
                                                strMsg += "StoreIn Command Update Success!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Update Command & Create StoreIn Crane Command Success
                                            }
                                            else
                                            {
                                                #region Update Command Fail
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "|";
                                                strMsg += Trace.StoreIn_GetStoreInCommandAndWritePLC + "->" + Trace.StoreIn_CrateCraneCommand + "|";
                                                strMsg += "StoreIn Command Update Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Update Command Fail
                                            }
                                        }
                                        else
                                        {
                                            #region Create StoreIn Crane Command Fail
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = strCommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += "StoreIn Crane Command Create Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Create StoreIn Crane Command Fail
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    }
                                }
                            }

                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }

        /// <summary>
        /// EQU_CMD命令完成
        /// </summary>
        private void funStoreIn_CraneCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtEquCmd = new DataTable();
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts<'3'";
                strSQL += " AND CMD_MODE IN ('1', '3')";
                strSQL += " AND TRACE='" + Trace.StoreIn_CrateCraneCommand + "'";
                strSQL += " order by  Crt_Dte";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    for (int i = 0; i < dtCmdSno.Rows.Count; i++)
                    {
                        CommandInfo commandInfo = new CommandInfo();
                        commandInfo.CommandID = dtCmdSno.Rows[i]["Cmd_Sno"].ToString();
                        commandInfo.CycleNo = dtCmdSno.Rows[i]["Cyc_No"].ToString();
                        commandInfo.PalletNo = dtCmdSno.Rows[i]["Plt_No"].ToString();
                        commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[i]["Cmd_Mode"].ToString());
                        commandInfo.IOType = dtCmdSno.Rows[i]["Io_Type"].ToString();
                        commandInfo.Loaction = dtCmdSno.Rows[i]["Loc"].ToString();
                        commandInfo.StationNo = dtCmdSno.Rows[i]["Stn_No"].ToString();
                        commandInfo.Priority = dtCmdSno.Rows[i]["Prty"].ToString();

                        strSQL = "SELECT * FROM EQUCMD";
                        strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                        strSQL += " AND RENEWFLAG<>'F'";
                        strSQL += " AND CMDMODE='1'";
                        strSQL += " AND CMDSTS in ('8','9')";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtEquCmd, ref strEM))
                        {
                            #region
                            string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                            string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                            if (strCmdSts == CommandState.EQUCompleted && strCompleteCode.Substring(0, 1) == "W")
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
                                        strSQL = "UPDATE CMD_MST";
                                        strSQL += " SET CMD_STS='0',TRACE='" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "'";
                                        strSQL += " WHERE CMD_STS='1' and CMD_SNO='" + commandInfo.CommandID + "'";
                                        if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                        {
                                            strSQL = "DELETE FROM EQUCMD where CMDSNO='" + commandInfo.CommandID + "'";
                                            if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                            {
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                                #region Retry StoreOut DELETE EQUCMD Success
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "|";
                                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "Retry StoreOut DELETE EQUCMD Success!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Retry StoreOut DELETE EQUCMD Success
                                            }
                                            else
                                            {
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                #region DELETE EQUCMD Fail
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "|";
                                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "DELETE EQUCMD Fail";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion DELETE EQUCMD Fail
                                            }
                                        }
                                        else
                                        {
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            #region Retry StoreOut Crane Command Fail
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "|";
                                            strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "StoreOut Crane Command Retry Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Retry StoreOut Crane Command Fail
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    }
                                }

                            }
                            else if (strCmdSts == CommandState.EQUCompleted && strCompleteCode == "EF")
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
                                        if (funUpdateCommand(commandInfo.CommandID, CommandState.CancelConstraint, Trace.StoreIn_CrateCraneCommand))
                                        {
                                            if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreIn).ToString()))
                                            {
                                                #region StoreOut Crane Command Finish & Update Command Success
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                                strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                                strMsg += "StoreOut Command Update Success!";
                                                funWriteSysTraceLog(strMsg);

                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "StoreOut Crane Command Delete Success!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion StoreOut Crane Command Finish & Update Command Success
                                            }
                                            else
                                            {
                                                #region Delete StoreOut Crane Command Fail
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "StoreOut Crane Command Delete Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Delete StoreOut Crane Command Fail
                                            }
                                        }
                                        else
                                        {
                                            #region Update Command Fail

                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                            strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "StoreIn Command Update Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command Fail
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    }
                                }

                            }
                            else if (strCmdSts == CommandState.EQUCancelWaitPost)
                            {
                                strSQL = "UPDATE EQUCMD set";
                                strSQL += " CMDSTS='0',";
                                strSQL += " CompleteCode=''";
                                strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                {
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += strSQL + "  Success";
                                    funWriteSysTraceLog(strMsg);
                                }
                                else
                                {
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += strSQL + "  Fail";
                                    funWriteSysTraceLog(strMsg);
                                }
                            }
                            else if (strCmdSts == CommandState.EQUCompleted && (strCompleteCode == "92" || strCompleteCode == "FF"))
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
                                        if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CraneCommandFinish))
                                        {
                                            if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreIn).ToString()))
                                            {
                                                #region StoreIn Crane Command Finish & Update Command Success
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                                strMsg += Trace.StoreIn_CrateCraneCommand + "->" + Trace.StoreIn_CraneCommandFinish + "|";
                                                strMsg += "StoreIn Command Update Success!";
                                                funWriteSysTraceLog(strMsg);

                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "StoreIn Crane Command Delete Success!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion StoreIn Crane Command Finish & Update Command Success
                                            }
                                            else
                                            {
                                                #region Delete StoreIn Crane Command Fail
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "StoreIn Crane Command Delete Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Delete StoreIn Crane Command Fail
                                            }
                                        }
                                        else
                                        {
                                            #region Update Command Fail
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                            strMsg += Trace.StoreIn_CrateCraneCommand + "->" + Trace.StoreIn_CraneCommandFinish + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "StoreIn Command Update Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command Fail
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if (dtEquCmd != null)
                {
                    dtEquCmd.Clear();
                    dtEquCmd.Dispose();
                    dtEquCmd = null;
                }
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();

                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }
    }
}
