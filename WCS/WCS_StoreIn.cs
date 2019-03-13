using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static Mirle.ASRS.Buffer;

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
                        && bufferData[intBufferIndex]._EQUStatus.RearLocation == Buffer.Signal.On
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._Mode == StnMode.None
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
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                            funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "扫码失败！", intBufferIndex);

                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL = "SELECT * FROM CMD_MST WHERE PLT_NO='" + strBCR + "'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strIoType = dtCmdSno.Rows[0]["IO_TYPE"].ToString();
                                string strCmdSno = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                if (strIoType == IO_TYPE.StoreIn11)
                                {
                                    #region 删除已存在命令
                                    strMsg = strBufferName + "|";
                                    strMsg += strBCR + "|";
                                    strMsg += "存在未执行命令！";
                                    funWriteSysTraceLog(strMsg);
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                    try
                                    {
                                        strSQL = "INSERT INTO CMD_MST_HIS SELECT * FROM CMD_MST WHERE PLT_NO='" + strBCR + "'";
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
                                    #endregion

                                    strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                                    strSQL += " WHERE B.STATUS IN ('0','W') AND LOC='' AND P.PLT_NO='" + strBCR + "'";
                                    strSQL += " GROUP BY P.SUB_NO  ";
                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        #region 产生新的入库命令
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                        try
                                        {
                                            string strLoaction = "";
                                            string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                            string strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                            string strCommandID = funGetCommandID();

                                            if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strBCR, strBufferName, strActualWeight, " ", " "))
                                            {
                                                if (funLockStoreInBox(strSubNo, LoactionState.IN))
                                                {
                                                    string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, "83" };
                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                    {
                                                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                        {
                                                            strMsg = strBufferName + "|";
                                                            strMsg += strBCR + "|";
                                                            strMsg += strCommandID + "|";
                                                            strMsg += "|";
                                                            strMsg += "StoreIn Command And WritePLC Success";
                                                            funWriteSysTraceLog(strMsg);

                                                            funSetKanbanInfo(KanbanModel.IN, strBufferName, strCommandID, strSubNo, string.Empty, intBufferIndex);

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
                                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                                            funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "事务提交失败！", intBufferIndex);

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
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                                        funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "命令写入PLC失败！", intBufferIndex);

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
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                                    funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "子托盘跟新失败！", intBufferIndex);

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
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "命令产生失败！", intBufferIndex);

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
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                        funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "条码无资料！", intBufferIndex);
                                    }
                                }
                                else
                                {
                                    string[] strValues = new string[] { strCmdSno, CMDMode.StoreIn, "83" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                        funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "扫码失败！", intBufferIndex);
                                    }
                                }

                            }
                            #endregion Read OK
                        }
                    }
                    #endregion

                    #region A90扫码确认

                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                        && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._Mode == StnMode.StoreIn
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
                            strSQL += "AND LOC='' AND CMD_STS='0'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strCommandID = dtCmdSno.Rows[0]["CMD_MST"].ToString();
                                string strStnNo = dtCmdSno.Rows[0]["STN_NO"].ToString();
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {

                                    string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, strStnNo.Remove(0, 1) };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strBCR + "|";
                                        strMsg += strCommandID + "|";
                                        strMsg += sStoreInStnNo[inStoreInStnNoIndex] + "|";
                                        strMsg += (inStoreInStnNoIndex + 1).ToString() + "|";
                                        strMsg += "命令站口分配成功!";
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
                                }
                            }
                            else
                            {
                                strMsg = strBufferName + "|";
                                strMsg += strBCR + "|";
                                strMsg += "命令站口分配时托盘未查询到对应命令信息!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region A83称重机称重
                    if (bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On
                      && bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On
                      && bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On
                      && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                      && !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                      && bufferData[intBufferIndex]._Mode == StnMode.StoreIn
                      && strBufferName == STN_NO.StoreInA83)
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
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                            #endregion BCR Read Fail & Write MPLC Success & BCR Clear
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            int strWeight = int.Parse(bCRData[intIndex]._ResultID);
                            string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                            string strPltNo = string.Empty;
                            int Pack001 = 0;
                            int Pack002 = 0;
                            int inSKU_GROUP3 = 0;
                            int inBOX_COUNT = 0;



                            strSQL = "SELECT PLT_NO,ACTUAL_WEIGHT,IO_TYPE  ";
                            strSQL += "FROM CMD_MST ";
                            strSQL += "WHERE CMD_MST='" + strCommandID + "'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                int inActualWeight = int.Parse(dtCmdSno.Rows[0]["ACTUAL_WEIGHT"].ToString());
                                strPltNo = dtCmdSno.Rows[0]["PLT_NO"].ToString();
                                string strIO_TYPE = dtCmdSno.Rows[0]["IO_TYPE"].ToString();

                                if (strIO_TYPE == IO_TYPE.StoreIn11)
                                {
                                    #region 入库
                                    strSQL = "SELECT SUM(I.SKU_GROUP3) AS SKU_GROUP3,COUNT(B.SUB_NO) AS BOX_COUNT ";
                                    strSQL += "FROM  PLT_MST P  JOIN BOX B  ON P.SUB_NO=B.SUB_NO JOIN ITEM_MST I  ON B.ITEM_NO=I.ITEM_NO ";
                                    strSQL += "WHERE P.PLT_NO='" + strPltNo + "' ";
                                    strSQL += "GROUP By B.SUB_NO ";

                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        inSKU_GROUP3 = Convert.ToInt32(dtCmdSno.Rows[0]["SKU_GROUP3"].ToString());
                                        inBOX_COUNT = Convert.ToInt32(dtCmdSno.Rows[0]["BOX_COUNT"].ToString());
                                        inActualWeight = (Pack001 * inBOX_COUNT + Pack002 * inSKU_GROUP3) + inActualWeight;
                                        strSQL = "UPDATE CMD_MST SET WEIGHT='" + strWeight + "' ,ACTUAL_WEIGHT='" + inActualWeight + "'";
                                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                        {
                                            if (funGetCode(ref Pack001, ref Pack002))
                                            {
                                                #region 对比重量
                                                if (strWeight - inActualWeight >= 20)
                                                {
                                                    string[] strValues = new string[] { "1" };
                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues))
                                                    {
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strPltNo + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreIn UpdateLocationMaster Success";
                                                        strMsg += "or WriteMPLC Discharged Success";
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
                                                    string[] strValues = new string[] { "3" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
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
                                                string[] strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
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
                                        string[] strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
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
                                        strMsg += "StoreIn UpdateLocationMaster Success";
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
                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region A08站口高度检测
                    if (bufferData[7]._EQUStatus.AutoMode == Buffer.Signal.On && bufferData[7]._EQUStatus.Load == Buffer.Signal.On
                        && bufferData[7]._EQUStatus.FrontLocation == Buffer.Signal.On && bufferData[7]._ReturnRequest == "0" && !string.IsNullOrWhiteSpace(bufferData[7]._CommandID)
                        && !string.IsNullOrWhiteSpace(bufferData[7]._Destination)&& bufferData[7]._Mode == StnMode.StoreIn
                        && strBufferName == STN_NO.StoreInA08)
                    {
                        string strCommandID = bufferData[7]._CommandID.PadLeft(5, '0');
                        string strLocSiez = string.Empty;
                        string strLoc = string.Empty;
                        string strSubNo = string.Empty;
                        string strStnNo = string.Empty;
                        string strCrnNo = string.Empty;
                        string strPltNo = string.Empty;

                        if (bufferData[7]._EQUAlarmStatus.OverHigh == Signal.Off|| bufferData[7]._EQUAlarmStatus.OverLength == Signal.Off|| bufferData[7]._EQUAlarmStatus.OverWidth == Signal.Off)
                        {
                            if (bufferData[7]._EQUStatus.Siez == Signal.Off) strLocSiez = "L";
                            if (bufferData[7]._EQUStatus.Siez == Signal.On) strLocSiez = "H";
                            if (funGetCraneNo(ref strCrnNo, ref strStnNo))
                            {
                                strSQL = "SELECT B.STATUS,COUNT(B.STATUS) FROM CMD_MST C JOIN PLT_MST P ON C.PLT_NO=P.PLT_NO JOIN BOX B ON P.SUB_NO=B.SUB_NO ";
                                strSQL += "WHERE C.CMD_STS='0' ";
                                strSQL += "AND C.LOC=' '";
                                strSQL += "AND C.TRACE='" + Trace.Inital + "'";
                                strSQL += "AND C.CMD_SNO='" + strCommandID + "'";
                                strSQL += "GROUP BY B.STATUS";
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    if (dtCmdSno.Rows.Count == 1)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
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
                                                            string[] strValues = new string[] { "1" };
                                                            if (InitSys._MPLC.funWriteMPLC(bufferData[7]._W_Discharged, strValues))
                                                            {
                                                                strMsg = strBufferName + "|";
                                                                strMsg += strPltNo + "|";
                                                                strMsg += strCommandID + "|";
                                                                strMsg += "|";
                                                                strMsg += "StoreIn UpdateLocationMaster Success";
                                                                strMsg += "or WriteMPLC Discharged Success";
                                                                funWriteSysTraceLog(strMsg);

                                                                bCRData[intIndex].funClear();
                                                                strMsg = bCRData[intIndex]._BufferName + "|";
                                                                strMsg += "2->0|";
                                                                strMsg += "ERROR|";
                                                                strMsg += "BCR Clear!";
                                                                funWriteSysTraceLog(strMsg);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strPltNo + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "StoreIn UpdateLocationMaster Fail";
                                                        funWriteSysTraceLog(strMsg);
                                                        string[] strValues = new string[] { "2" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[7]._ReturnRequest, strValues);
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                strMsg = strBufferName + "|";
                                                strMsg += strPltNo + "|";
                                                strMsg += strCommandID + "|";
                                                strMsg += "|";
                                                strMsg += "StoreIn GetEmptyLocation Fail";
                                                funWriteSysTraceLog(strMsg);
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                string[] strValues = new string[] { "2" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[7]._ReturnRequest, strValues);
                                            }
                                            #endregion
                                        }
                                        catch (Exception Ex)
                                        {
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            funWriteSysTraceLog(Ex.ToString());
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
                    && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                    && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                    && bufferData[intBufferIndex]._Mode == StnMode.None
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
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

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
                                #region 使用原有命令
                                if (dtCmdSno.Rows.Count == 1)
                                {
                                    Cmd_Mst CmdMst = new Cmd_Mst();
                                    CmdMst.Cmd_Sno = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                    CmdMst.Cmd_Mode = dtCmdSno.Rows[0]["CMD_MODE"].ToString();
                                    CmdMst.Stn_No = dtCmdSno.Rows[0]["STN_NO"].ToString();

                                    string[] strValues = new string[] { CmdMst.Cmd_Sno, CMDMode.StoreIn, CmdMst.Stn_No.Remove(0, 1) };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strBCR + "|";
                                        strMsg += CmdMst.Cmd_Sno + "|";
                                        strMsg += "|";
                                        strMsg += "StoreIn Command And WritePLC Success";
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
                                if (bufferData[intBufferIndex]._EQUAlarmStatus.OverHigh == Signal.Off
                             || bufferData[intBufferIndex]._EQUAlarmStatus.OverLength == Signal.Off
                             || bufferData[intBufferIndex]._EQUAlarmStatus.OverWidth == Signal.Off)
                                {
                                    if (bufferData[intBufferIndex]._EQUStatus.Siez == Signal.Off) strLocSiez = "L";
                                    if (bufferData[intBufferIndex]._EQUStatus.Siez == Signal.On) strLocSiez = "H";
                                    if (funGetCraneNo(ref strCrnNo, ref strStnNo))
                                    {

                                        strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                                        strSQL += " WHERE B.STATUS IN ('0','W') AND LOC=' ' AND P.PLT_NO='" + strBCR + "'";
                                        strSQL += " GROUP BY P.SUB_NO  ";

                                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                        {
                                            #region 产生新的入库命令
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                            try
                                            {

                                                string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                                string strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                                string strCommandID = funGetCommandID();
                                                if (funGetEmptyLocation(strLocSiez, strCrnNo, ref strLoaction))
                                                {
                                                    if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strBCR, strStnNo, strActualWeight, strLocSiez, " "))
                                                    {
                                                        if (funUpdateLocationMaster(strLoaction, LoactionState.IN, strBCR))
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

                                                                    //string[] strValues = new string[] { strCommandID, strStnNo, CMDMode.StoreIn };
                                                                    //if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                                    //{
                                                                    //    strMsg = strBufferName + "|";
                                                                    //    strMsg += strBCR + "|";
                                                                    //    strMsg += strCommandID + "|";
                                                                    //    strMsg += "|";
                                                                    //    strMsg += "StoreIn Command And WritePLC Success";
                                                                    //    funWriteSysTraceLog(strMsg);

                                                                    //    bCRData[intIndex].funClear();
                                                                    //    strMsg = bCRData[intIndex]._BufferName + "|";
                                                                    //    strMsg += "2->0|";
                                                                    //    strMsg += "ERROR|";
                                                                    //    strMsg += "BCR Clear!";
                                                                    //    funWriteSysTraceLog(strMsg);

                                                                    //    strValues = new string[] { "1" };
                                                                    //    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);
                                                                    //}
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

                                                                    string[] strValues = new string[] { "1" };
                                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

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

                                                                string[] strValues = new string[] { "1" };
                                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

                                                                #endregion Update StoreIn PalletNo Fail
                                                            }
                                                        }
                                                        else
                                                        {
                                                            strMsg = strBufferName + "|";
                                                            strMsg += strBCR + "|";
                                                            strMsg += "|";
                                                            strMsg += "储位更新失败！";
                                                            funWriteSysTraceLog(strMsg);

                                                            bCRData[intIndex].funClear();
                                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                                            strMsg += "2->0|";
                                                            strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                            strMsg += "BCR Clear!";
                                                            funWriteSysTraceLog(strMsg);

                                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                            string[] strValues = new string[] { "2" };
                                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

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

                                                        string[] strValues = new string[] { "1" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

                                                        #endregion Insert Command Fail
                                                    }
                                                }
                                                else
                                                {
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strBCR + "|";
                                                    strMsg += "|";
                                                    strMsg += "储位获取失败！";
                                                    funWriteSysTraceLog(strMsg);

                                                    bCRData[intIndex].funClear();
                                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                                    strMsg += "2->0|";
                                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                                    strMsg += "BCR Clear!";
                                                    funWriteSysTraceLog(strMsg);

                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    string[] strValues = new string[] { "2" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

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
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);

                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._ReturnRequest, strValues);
                                            #endregion Can't Find StroreIn PLT_MST&BOX
                                        }
                                    }
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
                        strSQL += " AND LOC!=' '";
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
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
                    CommandInfo commandInfo = new CommandInfo();
                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                    commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                    commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                    strSQL = "SELECT * FROM EQUCMD";
                    strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                    strSQL += " AND RENEWFLAG<>'F'";
                    strSQL += " AND CMDMODE='1'";
                    strSQL += " AND CMDSTS in ('8','9')";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if (strCmdSts == CommandState.EQUCompleted && strCompleteCode.Substring(0, 1) == "W")
                        {
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);

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
                        else if (strCmdSts == CommandState.EQUCompleted && strCompleteCode == "EF")
                        {
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CrateCraneCommand))
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
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
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
