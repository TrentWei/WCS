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
        private void funLocationToLocation()
        {
            funLocationToLocation_CrateCraneCommand();
            funLocationToLocation_CraneCommandFinish();
        }

        private void funLocationToLocation_CrateCraneCommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();

            try
            {
                if(funCheckCraneExistsCommand("1", CraneMode.LoactionToLoaction, "0"))
                {
                    strSQL = "SELECT * FROM CMD_MST";
                    strSQL += " WHERE Cmd_Mode='5'";
                    strSQL += " AND Cmd_Sts='0'";
                    strSQL += " AND TRACE='0'";
                    strSQL += " ORDER BY Cmd_Sno, Loc, Prty, Crt_Dte DESC";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        CommandInfo commandInfo = new CommandInfo();
                        commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                        commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                        commandInfo.IO_Type = dtCmdSno.Rows[0]["Io_Type"].ToString();
                        commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                        commandInfo.NewLoaction = dtCmdSno.Rows[0]["New_Loc"].ToString();
                        commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                        commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                        strMsg = commandInfo.CommandID + "|";
                        strMsg += commandInfo.CommandMode + "|";
                        strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                        strMsg += "Transfer Command Initiated!";
                        funWriteSysTraceLog(strMsg);

                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.LoactionToLoaction_CrateCraneCommand))
                        {
                            if(funCrateCraneCommand("1", commandInfo.CommandID, "5",
                                commandInfo.Loaction, commandInfo.NewLoaction, commandInfo.Priority))
                            {
                                #region Update Command & Create Transfer Crane Command Success
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                strMsg += Trace.Inital + "->" + Trace.LoactionToLoaction_CrateCraneCommand + "|";
                                strMsg += "Update Command Success!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                strMsg += "Create Transfer Crane Command Success!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command & Create Transfer Crane Command Success
                            }
                            else
                            {
                                #region Create Transfer Crane Command Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                strMsg += "Create Transfer Crane Command Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Create Transfer Crane Command Fail 
                            }
                        }
                        else
                        {
                            #region Update Command Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = commandInfo.CommandID + "|";
                            strMsg += commandInfo.CommandMode + "|";
                            strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                            strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                            strMsg += Trace.Inital + "->" + Trace.LoactionToLoaction_CrateCraneCommand + "|";
                            strMsg += "Update Command Fail!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Update Command Fail
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

        private void funLocationToLocation_CraneCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            DataTable dtEquCmd = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts='1'";
                strSQL += " AND Cmd_Mode='5'";
                strSQL += " AND TRACE='" + Trace.LoactionToLoaction_CrateCraneCommand + "'";
                strSQL += " ORDER BY LOC DESC";
                if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                {
                    for(int intCmdSno = 0; intCmdSno < dtCmdSno.Rows.Count; intCmdSno++)
                    {
                        CommandInfo commandInfo = new CommandInfo();
                        commandInfo.CommandID = dtCmdSno.Rows[intCmdSno]["Cmd_Sno"].ToString();
                        commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[intCmdSno]["Cmd_Mode"].ToString());
                        commandInfo.IO_Type = dtCmdSno.Rows[intCmdSno]["Io_Type"].ToString();
                        commandInfo.Loaction = dtCmdSno.Rows[intCmdSno]["Loc"].ToString();
                        commandInfo.NewLoaction = dtCmdSno.Rows[intCmdSno]["New_Loc"].ToString();
                        commandInfo.StationNo = dtCmdSno.Rows[intCmdSno]["Stn_No"].ToString();
                        commandInfo.Priority = dtCmdSno.Rows[intCmdSno]["Prty"].ToString();

                        strSQL = "SELECT * FROM EQUCMD";
                        strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                        strSQL += " AND RENEWFLAG<>'F'";
                        strSQL += " AND CMDMODE='1'";
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
                                    #region Retry Transfer Crane Command Success
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += CommandState.Start + "|";
                                    strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "Retry Transfer Crane Command Success!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Retry Transfer Crane Command Success
                                }
                                else
                                {
                                    #region Retry Transfer Crane Command Fail
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += CommandState.Start + "|";
                                    strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "Retry Transfer Crane Command Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Retry Transfer Crane Command Fail
                                }
                            }
                            else if(strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                            {
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.LoactionToLoaction_CrateCraneCommand))
                                {
                                    #region Transfer Crane Command Finish & Update Command Success
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "Transfer Crane Command Finish!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "|";
                                    strMsg += "Update Command Success!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Transfer Crane Command Finish & Update Command Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "Update Command Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                            }
                            else if(strCmdSts == CommandState.Completed && strCompleteCode == "92")
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.LoactionToLoaction_CraneCommandFinish))
                                {
                                    if(funDeleteEquCmd("1", commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                    {
                                        #region Transfer Crane Command Finish & Update Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                        strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                        strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "->" + Trace.LoactionToLoaction_CraneCommandFinish + "|";
                                        strMsg += "Update Command Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "Delete Transfer Crane Command Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Transfer Crane Command Finish & Update Command Success
                                    }
                                    else
                                    {
                                        #region Delete Transfer Crane Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "Delete Transfer Crane Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Delete Transfer Crane Command Fail
                                    }
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.LoactionToLoaction_CrateCraneCommand + "->" + Trace.LoactionToLoaction_CraneCommandFinish + "|";
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
