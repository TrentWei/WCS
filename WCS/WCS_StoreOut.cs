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
                    strSQL = "SELECT DISTINCT Prty, Crt_Dte, Cmd_Sno, Cmd_Mode, Loc, Io_Type FROM CMD_MST";
                    strSQL += " WHERE Cmd_Sts='0'";
                    strSQL += " AND TRACE='0'";
                    strSQL += " AND Cmd_Mode IN('2', '3')";
                    strSQL += " AND Stn_No='" + stnDef.StationName + "'";
                    strSQL += " ORDER BY Prty, Crt_Dte, Cmd_Sno";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        for(int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                        {
                            if(string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                                string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                                bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                                !bufferData[intBufferIndex]._ReturnRequest &&
                                bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
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
                                        strMsg += "Update Command Success!";
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
                                    strMsg += "Update Command Fail!";
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
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreOut &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID;
                        strSQL = "SELECT TOP 1 Loc, Prty FROM CMD_MST";
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
                                if(funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreOut_CrateCraneCommand))
                                {
                                    if(funCrateCraneCommand("1", commandInfo.CommandID, "2",
                                        commandInfo.Loaction, stnDef.StationIndex.ToString(), commandInfo.Priority))
                                    {
                                        #region Update Command & Create StoreOut Crane Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                        strMsg += "Update Command Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += "Create StoreOut Crane Command Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Create StoreOut Crane Command Success
                                    }
                                    else
                                    {
                                        #region Create StoreOut Crane Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += "Create StoreOut Crane Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Create StoreOut Crane Command Fail
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
                                    strMsg += CommandState.Start + "|";
                                    strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                    strMsg += "Update Command Fail!";
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

        private void funStoreOut_CraneCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtEquCmd = new DataTable();
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach(StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if(!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreOut &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID;
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'3'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
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
                            strSQL += " WHERE CMDSNO='" + strCommandID + "'";
                            strSQL += " AND RENEWFLAG<>'F'";
                            strSQL += " AND CMDMODE='2'";
                            strSQL += " AND CMDSTS='9'";
                            if(InitSys._DB.funGetDT(strSQL, ref dtEquCmd, ref strEM))
                            {
                                string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                                string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                                if(strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
                                {
                                    strSQL = "UPDATE EQUCMD SET CMDSTS='0' WHERE CMDSNO='" + strCommandID + "'";
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
                                        strMsg += "Retry StoreOut Crane Command Success!";
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
                                        strMsg += "Retry StoreOut Crane Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Retry StoreOut Crane Command Fail
                                    }
                                }
                                else if(strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                                {
                                    if(funUpdateCommand(strCommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CrateCraneCommand))
                                    {
                                        #region StoreOut Crane Command Finish & Update Command Success
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreOut Crane Command Finish!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                        strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                        strMsg += "Update Command Success!";
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
                                        strMsg += "Update Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                }
                                else if(strCmdSts == CommandState.Completed && strCompleteCode == "92")
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if(funUpdateCommand(strCommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CraneCommandFinish))
                                    {
                                        if(funDeleteEquCmd("1", strCommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                        {
                                            #region StoreOut Crane Command Finish & Update Command Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                            strMsg += Trace.StoreOut_CrateCraneCommand + "->" + Trace.StoreOut_CraneCommandFinish + "|";
                                            strMsg += "Update Command Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "Delete StoreOut Crane Command Success!";
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
                                            strMsg += "Delete StoreOut Crane Command Fail!";
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
                                        strMsg += "Update Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
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
