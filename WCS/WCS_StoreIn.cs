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
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID)
                        && string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination)
                        && bufferData[intBufferIndex]._Mode == StnMode.None
                        && strBufferName == STN_NO.StoreInA53)
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrEmpty(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!---2";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!---2";
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
                            string[] strValues = new string[] { "1" };
                            if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                            {
                                #region BCR Read Fail & Write MPLC Success & BCR Clear
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bCRData[intIndex]._ResultID + "|";
                                strMsg += "BCR Read Fail!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Success!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail & Write MPLC Success & BCR Clear
                            }
                            else
                            {
                                #region BCR Read Fail But Write MPLC Fail
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail But Write MPLC Fail
                            }
                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
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
                            }
                            strSQL = " SELECT * FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO  ";
                            strSQL += " WHERE B.STATUS IN ('0','W') ";
                            strSQL += " AND LOC='' AND P.PLT_NO='" + strBCR + "'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                #region 产生新的入库命令
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                try
                                {
                                    string strLoaction = "";
                                    string strBOX_ID = dtCmdSno.Rows[0]["BOX_ID"].ToString();
                                    string strCommandID = funGetCommandID();
                                    if (funCreateStoreInCommand(strCommandID, strLoaction, strBCR, strBufferName))
                                    {
                                        if (funLockStoreInBox(strBOX_ID))
                                        {
                                            string[] strValues = new string[] { strCommandID, "1", CMDMode.StoreIn };
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
                            }

                            #endregion Read OK
                        }
                    }
                    #endregion

                    #region A90扫码主机站口分配

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
                            string[] strValues = new string[] { "1" };
                            if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                            {
                                #region BCR Read Fail & Write MPLC Success & BCR Clear
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bCRData[intIndex]._ResultID + "|";
                                strMsg += "BCR Read Fail!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Success!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail & Write MPLC Success & BCR Clear
                            }
                            else
                            {
                                #region BCR Read Fail But Write MPLC Fail
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail But Write MPLC Fail
                            }
                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            if (inStoreInStnNoIndex > CraneNo.Crane5 - 1) inStoreInStnNoIndex = CraneNo.Crane1 - 1;
                            strSQL = "SELECT * FROM CMD_MST ";
                            strSQL += "WHERE PLT_NO='" + strBCR + "' ";
                            strSQL += "AND LOC='' AND CMD_STS='0'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strCommandID = dtCmdSno.Rows[0]["CMD_MST"].ToString();
                                strSQL = "UPDATE CMD_MSY ";
                                strSQL += "SET STN_NO='"+sStoreInStnNo[inStoreInStnNoIndex] +"'";
                                strSQL += "WHERE CMD_MST='" + strCommandID + "' ";
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    
                                    string[] strValues = new string[] { strCommandID, (inStoreInStnNoIndex+1).ToString(), CMDMode.StoreIn };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strBCR+ "|";
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

                                        inStoreInStnNoIndex += 1;
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
                            #region Read Error
                            string[] strValues = new string[] { "1" };
                            if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                            {
                                #region BCR Read Fail & Write MPLC Success & BCR Clear
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bCRData[intIndex]._ResultID + "|";
                                strMsg += "BCR Read Fail!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Success!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += "ERROR|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail & Write MPLC Success & BCR Clear
                            }
                            else
                            {
                                #region BCR Read Fail But Write MPLC Fail
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "Write MPLC Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion BCR Read Fail But Write MPLC Fail
                            }
                            #endregion Read Error
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            if (inStoreInStnNoIndex > CraneNo.Crane5 - 1) inStoreInStnNoIndex = CraneNo.Crane1 - 1;
                            strSQL = "SELECT * FROM CMD_MST ";
                            strSQL += "WHERE PLT_NO='" + strBCR + "' ";
                            strSQL += "AND LOC='' AND CMD_STS='0'";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strCommandID = dtCmdSno.Rows[0]["CMD_MST"].ToString();
                                strSQL = "UPDATE CMD_MSY ";
                                strSQL += "SET STN_NO='" + sStoreInStnNo[inStoreInStnNoIndex] + "'";
                                strSQL += "WHERE CMD_MST='" + strCommandID + "' ";
                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {

                                    string[] strValues = new string[] { strCommandID, (inStoreInStnNoIndex + 1).ToString(), CMDMode.StoreIn };
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

                                        inStoreInStnNoIndex += 1;
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

                    #region 主机站口处理

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
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        (bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn || bufferData[intBufferIndex]._Mode == Buffer.StnMode.Picking) &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        intBufferIndex != 8 &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<='1'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND ((CMD_MODE='1'";
                        strSQL += " AND TRACE in('" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "','0'))";
                        strSQL += " OR (CMD_MODE='3'";
                        strSQL += " AND TRACE='" + Trace.StoreOut_CraneCommandFinish + "'))";
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


                            if (funQueryEquCmdCount(CraneMode.StoreIn))
                            {
                                if (!funCheckCraneExistsCommand(CraneMode.StoreIn, stnDef.StationIndex.ToString()))
                                {
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                    if (funCrateCraneCommand(strCommandID, CraneMode.StoreIn, stnDef.StationIndex.ToString(), commandInfo.Loaction, commandInfo.Priority))
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
                                            if (commandInfo.CommandMode.ToString() == CMDMode.Picking)
                                                strMsg += Trace.StoreOut_CraneCommandFinish + "->" + Trace.StoreIn_CrateCraneCommand + "|";
                                            else
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
                                            if (commandInfo.CommandMode.ToString() == CMDMode.Picking)
                                                strMsg += Trace.StoreOut_CraneCommandFinish + "->" + Trace.StoreIn_CrateCraneCommand + "|";
                                            else
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

                        if (strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
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
                        else if (strCmdSts == CommandState.Completed && strCompleteCode == "EF")
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
                        else if (strCmdSts == CommandState.CancelWaitPost)
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
                        else if (strCmdSts == CommandState.Completed && (strCompleteCode == "92" || strCompleteCode == "FF"))
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
