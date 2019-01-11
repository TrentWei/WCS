using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private void funStoreOut()
        {
            funStoreOut_GetStoreOutCheckStnNo();
            funStoreOut_GetStoreOutCommandAndWritePLC();
            funStoreOut_CrateCraneCommand();
            funStoreOut_CraneCommandFinish();
            funStoreOut_CratePRODUCECommand();
        }

        private void funStoreOut_GetStoreOutCommandAndWritePLC()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    strSQL = "SELECT * FROM CMD_MST";
                    strSQL += " WHERE Cmd_Sts='0'";
                    strSQL += " AND TRACE='0'";
                    strSQL += " AND Cmd_Mode IN('2', '3')";
                    strSQL += " AND Stn_No='" + strBufferName + "'";
                    strSQL += " ORDER BY Prty, Crt_Dte, Cmd_Sno";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                    {
                        if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                            string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                            bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                            !bufferData[intBufferIndex]._ReturnRequest &&
                            intBufferIndex != 18 &&
                            intBufferIndex != 21 &&
                            bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                            bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                        {
                            #region StoreOut Command Initiated
                            CommandInfo commandInfo = new CommandInfo();
                            commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                            commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                            commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                            commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                            commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                            commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                            commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                            commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                            strMsg = commandInfo.CommandID + "|";
                            strMsg += commandInfo.CycleNo + "|";
                            strMsg += commandInfo.CommandMode + "|";
                            strMsg += commandInfo.Loaction + "|";
                            strMsg += commandInfo.StationNo + "|";
                            strMsg += commandInfo.PalletNo + "|";
                            strMsg += "StoreOut Command Initiated!";
                            funWriteSysTraceLog(strMsg);
                            #endregion StoreOut Command Initiated
                             InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_GetStoreOutCommandAndWritePLC))
                            {
                                string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                {
                                    #region Update Command & Write MPLC Success
                                     InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CycleNo + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                    strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                    strMsg += "StoreOut Command Update Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CycleNo + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Success!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command & Write MPLC Success
                                }
                                else
                                {
                                    #region Update Command Success But Write MPLC Fail
                                     InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CycleNo + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Success But Write MPLC Fail
                                }
                            }
                            else
                            {
                                #region Update Command Fail
                                 InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                strMsg += "StoreOut Command Update Fail!";
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
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }


        private void funStoreOut_GetStoreOutCheckStnNo()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        (intBufferIndex == 16||intBufferIndex == 19) &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts='0'";
                        strSQL += " AND TRACE='0'";
                        strSQL += " AND Cmd_Mode IN('2', '3')";
                        strSQL += " AND Stn_No=''";
                        strSQL += " ORDER BY Prty, Crt_Dte, Cmd_Sno";
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

                            strMsg = commandInfo.CommandID + "|";
                            strMsg += commandInfo.CycleNo + "|";
                            strMsg += commandInfo.CommandMode + "|";
                            strMsg += commandInfo.Loaction + "|";
                            strMsg += commandInfo.StationNo + "|";
                            strMsg += commandInfo.PalletNo + "|";
                            strMsg += "查询到为分配站口命令!";
                            funWriteSysTraceLog(strMsg);
                            if (funUpdateCommand(commandInfo.CommandID, strBufferName))
                            {
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += "StoreOut 模板出库站口分配成功!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += "StoreOut 模板出库站口分配失败!";
                                funWriteSysTraceLog(strMsg);
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



        private void funStoreOut_CrateCraneCommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        (bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreOut || bufferData[intBufferIndex]._Mode == Buffer.StnMode.Picking) &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'3'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND CMD_MODE IN ('2', '3')";
                        strSQL += " AND TRACE='" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "'";
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


                            if (funQueryEquCmdCount(CraneMode.StoreOut))
                            {
                                if (!funCheckCraneExistsCommand(CraneMode.StoreOut, stnDef.StationIndex.ToString()))
                                {
                                     InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                    if (funCrateCraneCommand(commandInfo.CommandID, CraneMode.StoreOut,
                                        commandInfo.Loaction, stnDef.StationIndex.ToString(), commandInfo.Priority))
                                    {
                                        if (funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreOut_CrateCraneCommand))
                                        {
                                            #region Update Command & Create StoreOut Crane Command Success
                                              InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                            strMsg = strCommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += "StoreOut Crane Command Create Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "|";
                                            strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                            strMsg += "StoreOut Command Update Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command & Create StoreOut Crane Command Success

                                        }
                                        else
                                        {
                                            #region Update Command Fail
                                             InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "|";
                                            strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                            strMsg += "StoreOut Command Update Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command Fail
                                        }
                                    }
                                    else
                                    {
                                        #region Create StoreOut Crane Command Fail
                                         InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += "StoreOut Crane Command Create Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Create StoreOut Crane Command Fail
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

        private void funStoreOut_CraneCommandFinish()
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
                strSQL += " AND CMD_MODE IN ('2', '3')";
                strSQL += " AND TRACE='" + Trace.StoreOut_CrateCraneCommand + "'";
                strSQL += " order by Crt_Dte";
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
                    strSQL += " AND CMDMODE='2'";
                    strSQL += " AND CMDSTS in ('8','9')";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if (strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
                        {
                             InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);

                            strSQL = "UPDATE CMD_MST";
                            strSQL += " SET CMD_STS='0',TRACE='" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "'";
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
                        else if (strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                        {
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CrateCraneCommand))
                            {
                                if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
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
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreOut Command Update Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command Fail
                            }
                        }
                        else if (strCmdSts == CommandState.Completed && (strCompleteCode == "92" || strCompleteCode == "FF"))
                        {
                            if (commandInfo.CommandMode.ToString() == CMDMode.Picking)
                            {
                                #region Picking
                                 InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_CraneCommandFinish))
                                {
                                    if (funUpdateCYCLE(commandInfo.CycleNo, commandInfo.PalletNo))
                                    {
                                        if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                        {
                                            #region StoreOut Crane Command Finish & Update Command Success & Update Cycle
                                              InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "|";
                                            strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                            strMsg += "StoreOut Command Update Success!";
                                            funWriteSysTraceLog(strMsg);

                                             InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Start + "|";
                                            strMsg += "0->1|";
                                            strMsg += "Cycle Update Success!";
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
                                            #endregion StoreOut Crane Command Finish & Update Command Success & Update Cycle
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
                                            strMsg += "StoreOut Crane Command Delete Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Delete StoreOut Crane Command Fail
                                        }
                                    }
                                    else
                                    {
                                        #region Cycle Update Fail
                                         InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += "0->1|";
                                        strMsg += "Cycle Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Cycle Update Fail
                                    }
                                }
                                else
                                {
                                    #region Update Command Fail
                                     InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CycleNo + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += CommandState.Start + "|";
                                    strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                    strMsg += "StoreOut Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                                #endregion Picking
                            }
                            else
                            {
                                //#region StoreOut
                                //if(lstStoreOutAGV.Exists(stnDef => stnDef.BufferName != commandInfo.StationNo))
                                //{
                                //    #region Normal
                                 InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CraneCommandFinish))
                                {
                                    if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
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
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "StoreOut Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                                //    #endregion Normal
                                //}
                                //else
                                //{
                                //    #region For AGV
                                //    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                //    if(funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_CraneCommandFinish))
                                //    {
                                //        if(funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                //        {
                                //            #region StoreOut Crane Command Finish & Update Command Success
                                //            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                //            strMsg = commandInfo.CommandID + "|";
                                //            strMsg += commandInfo.CycleNo + "|";
                                //            strMsg += commandInfo.CommandMode + "|";
                                //            strMsg += commandInfo.Loaction + "|";
                                //            strMsg += commandInfo.StationNo + "|";
                                //            strMsg += commandInfo.PalletNo + "|";
                                //            strMsg += CommandState.Start;
                                //            strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                //            strMsg += "StoreOut Command Update Success!";
                                //            funWriteSysTraceLog(strMsg);

                                //            strMsg = commandInfo.CommandID + "|";
                                //            strMsg += commandInfo.CycleNo + "|";
                                //            strMsg += commandInfo.CommandMode + "|";
                                //            strMsg += commandInfo.Loaction + "|";
                                //            strMsg += commandInfo.StationNo + "|";
                                //            strMsg += commandInfo.PalletNo + "|";
                                //            strMsg += strCompleteCode + "|";
                                //            strMsg += "StoreOut Crane Command Delete Success!";
                                //            funWriteSysTraceLog(strMsg);
                                //            #endregion StoreOut Crane Command Finish & Update Command Success
                                //        }
                                //        else
                                //        {
                                //            #region Delete StoreOut Crane Command Fail
                                //            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                //            strMsg = commandInfo.CommandID + "|";
                                //            strMsg += commandInfo.CycleNo + "|";
                                //            strMsg += commandInfo.CommandMode + "|";
                                //            strMsg += commandInfo.Loaction + "|";
                                //            strMsg += commandInfo.StationNo + "|";
                                //            strMsg += commandInfo.PalletNo + "|";
                                //            strMsg += strCompleteCode + "|";
                                //            strMsg += "StoreOut Crane Command Delete Fail!";
                                //            funWriteSysTraceLog(strMsg);
                                //            #endregion Delete StoreOut Crane Command Fail
                                //        }
                                //    }
                                //    else
                                //    {
                                //        #region Update Command Fail
                                //        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                //        strMsg = commandInfo.CommandID + "|";
                                //        strMsg += commandInfo.CycleNo + "|";
                                //        strMsg += commandInfo.CommandMode + "|";
                                //        strMsg += commandInfo.Loaction + "|";
                                //        strMsg += commandInfo.StationNo + "|";
                                //        strMsg += commandInfo.PalletNo + "|";
                                //        strMsg += CommandState.Start;
                                //        strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                //        strMsg += strCompleteCode + "|";
                                //        strMsg += "StoreOut Command Update Fail!";
                                //        funWriteSysTraceLog(strMsg);
                                //        #endregion Update Command Fail
                                //    }
                                //    #region For AGV
                                //}
                                //#endregion StoreOut
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



        private void funStoreOut_CratePRODUCECommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtProduce = new DataTable();
            try
            {

                strSQL = "SELECT * FROM PRODUCE ";
                strSQL += " WHERE STATUS='0'";
                strSQL += " AND ITEM_TYPE='M'";
                strSQL += " AND Wh_NO='A'";
                strSQL += " AND TIME <='" + DateTime.Now.ToString("HH:mm") + "'";
                strSQL += " ORDER BY PRODUCE_NO";
                if (InitSys._DB.GetDataTable(strSQL, ref dtProduce, ref strEM))
                {
                    Prodecu prodecu = new Prodecu();
                    prodecu.Prodecu_No = dtProduce.Rows[0]["PRODUCE_No"].ToString();
                    prodecu.Item_No = dtProduce.Rows[0]["ITEM_NO"].ToString();
                    prodecu.PStn_No = dtProduce.Rows[0]["PSTN_NO"].ToString();
                    prodecu.Prodecu_Qty = int.Parse(dtProduce.Rows[0]["PRODUCE_QTY"].ToString());
                    prodecu.Cmd_Sno = dtProduce.Rows[0]["Cmd_Sno"].ToString();
                    prodecu.Item_Type = dtProduce.Rows[0]["ITEM_TYPE"].ToString();

                    string strLocation = string.Empty;

                    if (funGetItemNoLocation(prodecu.Item_No, prodecu.Item_Type, ref strLocation))
                    {
                        string strCommandID = funGetCommandID();
                        string strStnNo = "";
                         InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                        if (bolChkStn)
                        {
                            strStnNo = "B07";
                            bolChkStn = false;
                        }
                        else
                        {
                            strStnNo = "B04";
                            bolChkStn = true;
                        }
                        if (funCreateAGVStoreOutCommand(strCommandID, strLocation, prodecu.Item_No, strStnNo))
                        {
                            if (funLockStoreOutLocation(strLocation))
                            {
                                if (funLockStoreOutPalletNo(prodecu.Item_No, prodecu.Prodecu_Qty, prodecu.PStn_No, prodecu.Item_Type))
                                {
                                    if (funUpdateProdecu(prodecu.Prodecu_No, "2", strCommandID))
                                    {
                                        #region Create AGV StroreOut Command & Lock StroreOut Location Success
                                          InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += "Create 模板 StroreOut Command Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = strCommandID + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += "Lock 模板 StroreOut Location Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = strCommandID + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += "|";
                                        strMsg += "Lock 模板 StroreOut PalletNo Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = prodecu.Prodecu_No + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += "0->1|";
                                        strMsg += strCommandID + "|";
                                        strMsg += "Update 模板 Prodecu Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Create AGV StroreOut Command & Lock StroreOut Location Success
                                    }
                                    else
                                    {
                                        #region Update Prodecu Fail
                                         InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = prodecu.Prodecu_No + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += "0->1|";
                                        strMsg += "Update 模板 Prodecu Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Prodecu Fail
                                    }
                                }
                                else
                                {
                                    #region Lock AGV StroreOut PalletNo Fail
                                     InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += strLocation + "|";
                                    strMsg += prodecu.Item_No + "|";
                                    strMsg += "|";
                                    strMsg += "Lock 模板 StroreOut PalletNo Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Lock AGV StroreOut PalletNo Fail
                                }
                            }
                            else
                            {
                                #region Lock  StroreOut Location Fail
                                 InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                strMsg = strCommandID + "|";
                                strMsg += strLocation + "|";
                                strMsg += prodecu.Item_No + "|";
                                strMsg += "Lock 模板 StroreOut Location Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Lock AGV StroreOut Location Fail
                            }
                        }
                        else
                        {
                            #region Create AGV StoreOut Command Fail
                             InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                            strMsg = prodecu.Prodecu_No + "|";
                            strMsg += prodecu.Item_No + "|";
                            strMsg += strLocation + "|";
                            strMsg += "Create 模板 StoreOut Command Fail!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Create AGV StoreOut Command Fail
                        }
                    }
                    else
                    {
                        if (funUpdateProdecu(prodecu.Prodecu_No, "3", "00000"))
                        {

                            strMsg += strLocation + "|";
                            strMsg += prodecu.Item_No + "|";
                            strMsg += "Create 模板 StroreOut Command Fail!";
                            funWriteSysTraceLog(strMsg);

                            strMsg = prodecu.Prodecu_No + "|";
                            strMsg += prodecu.Item_No + "|";
                            strMsg += strLocation + "|";
                            strMsg += "0->3|";

                            strMsg = prodecu.Prodecu_No + "|";
                            strMsg += prodecu.Item_No + "|";
                            strMsg += prodecu.Item_Type + "|";
                            strMsg += strLocation + "|";
                            strMsg += "Find PalletNo Location Error!";
                            funWriteSysTraceLog(strMsg);

                        }
                        else
                        {
                            #region Update Prodecu Fail
                            strMsg = prodecu.Prodecu_No + "|";
                            strMsg += prodecu.Item_No + "|";
                            strMsg += strLocation + "|";
                            strMsg += "0->3|";
                            strMsg += "Update 模板 Prodecu Fail!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Update Prodecu Fail
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
                if (dtProduce != null)
                {
                    dtProduce.Clear();
                    dtProduce.Dispose();
                    dtProduce = null;
                }
            }
        }


    }
}
