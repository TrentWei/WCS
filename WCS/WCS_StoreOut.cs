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
            funStoreOut_GetStoreOutCommandAndWritePLC();
            funStoreOut_CrateCraneCommand();
            funStoreOut_CraneCommandFinish();
        }

        private void funStoreOut_GetStoreOutCommandAndWritePLC()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach(StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    strSQL = "SELECT * FROM CMD_MST";
                    strSQL += " WHERE Cmd_Sts='0'";
                    strSQL += " AND TRACE='0'";
                    strSQL += " AND Cmd_Mode IN('2', '3')";
                    strSQL += " AND Stn_No='" + stnDef.BufferName + "'";
                    strSQL += " ORDER BY Prty, Crt_Dte, Cmd_Sno";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        for(int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                        {
                            if(string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                                string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                                bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                                !bufferData[intBufferIndex]._ReturnRequest &&
                                bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                                bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                            {
                                #region StoreOut Command Initiated
                                CommandInfo commandInfo = new CommandInfo();
                                commandInfo.CommandID = dtCmdSno.Rows[intRow]["Cmd_Sno"].ToString();
                                commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[intRow]["Cmd_Mode"].ToString());
                                commandInfo.IO_Type = dtCmdSno.Rows[intRow]["Io_Type"].ToString();
                                commandInfo.Loaction = dtCmdSno.Rows[intRow]["Loc"].ToString();
                                commandInfo.StationNo = dtCmdSno.Rows[intRow]["Stn_No"].ToString();
                                commandInfo.Priority = dtCmdSno.Rows[intRow]["Prty"].ToString();

                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += "StoreOut Command Initiated!";
                                funWriteSysTraceLog(strMsg);
                                #endregion StoreOut Command Initiated

                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_GetStoreOutCommandAndWritePLC))
                                {
                                    string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                    if(InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        #region Update Command & Write MPLC Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StoreOut Command Update Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Write MPLC Success
                                    }
                                    else
                                    {
                                        #region Update Command Success But Write MPLC Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
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
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if(dtCmdSno != null)
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
                foreach(StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if(!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
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
                        if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                        {
                            CommandInfo commandInfo = new CommandInfo();
                            commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                            commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                            commandInfo.IO_Type = dtCmdSno.Rows[0]["Io_Type"].ToString();
                            commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                            commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                            commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                            if(!funCheckCraneExistsCommand("1", CraneMode.StoreOut, stnDef.StationIndex.ToString()))
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funCrateCraneCommand("1", commandInfo.CommandID, "2",
                                    commandInfo.Loaction, stnDef.StationIndex.ToString(), commandInfo.Priority))
                                {
                                    if(funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreOut_CrateCraneCommand))
                                    {
                                        #region Update Command & Create StoreOut Crane Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += "StoreOut Crane Command Create Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                        strMsg += "StoreOut Command Update Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Create StoreOut Crane Command Success

                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += "StoreOut Crane Command Create Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Create StoreOut Crane Command Fail
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if(dtCmdSno != null)
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
                strSQL += " ORDER BY LOC DESC";
                if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                {
                    CommandInfo commandInfo = new CommandInfo();
                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                    commandInfo.IO_Type = dtCmdSno.Rows[0]["Io_Type"].ToString();
                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                    strSQL = "SELECT * FROM EQUCMD";
                    strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                    strSQL += " AND RENEWFLAG<>'F'";
                    strSQL += " AND CMDMODE='2'";
                    strSQL += " AND CMDSTS='9'";
                    if(InitSys._DB.funGetDT(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if(strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
                        {
                            strSQL = "UPDATE EQUCMD SET CMDSTS='0' WHERE CMDSNO='" + commandInfo.CommandID + "'";
                            if(InitSys._DB.funExecSql(strSQL, ref strEM))
                            {
                                #region Retry StoreOut Crane Command Success
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += CommandState.Start + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreOut Crane Command Retry Success!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Retry StoreOut Crane Command Success
                            }
                            else
                            {
                                #region Retry StoreOut Crane Command Fail
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += CommandState.Start + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreOut Crane Command Retry Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Retry StoreOut Crane Command Fail
                            }
                        }
                        else if(strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                        {
                            if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CrateCraneCommand))
                            {
                                #region StoreOut Crane Command Finish & Update Command Success
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "Crane Command StoreOut Finish!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += "StoreOut Command Update Success!";
                                funWriteSysTraceLog(strMsg);
                                #endregion StoreOut Crane Command Finish & Update Command Success
                            }
                            else
                            {
                                #region Update Command Fail
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreOut Command Update Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command Fail
                            }
                        }
                        else if(strCmdSts == CommandState.Completed && strCompleteCode == "92")
                        {
                            if(commandInfo.CommandMode.ToString() == CMDMode.Picking)
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_CraneCommandFinish))
                                {
                                    if(funDeleteEquCmd("1", commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                    {
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                        strMsg += "StoreOut Command Update Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreOut Crane Command Delete Success!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    else
                                    {
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreOut Crane Command Delete Success!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                }
                                else
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += CommandState.Start + "|";
                                    strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                    strMsg += "StoreOut Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                }
                            }
                            else
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CraneCommandFinish))
                                {
                                    if(funDeleteEquCmd("1", commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                    {
                                        #region StoreOut Crane Command Finish & Update Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                        strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                        strMsg += "StoreOut Command Update Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreOut Crane Command Delete Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion StoreOut Crane Command Finish & Update Command Success
                                    }
                                    else
                                    {
                                        #region Delete StoreOut Crane Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreOut Crane Command Delete Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Delete StoreOut Crane Command Fail
                                    }
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "StoreOut Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if(dtEquCmd != null)
                {
                    dtEquCmd.Clear();
                    dtEquCmd.Dispose();
                    dtEquCmd = null;
                }
                if(dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }
    }
}
