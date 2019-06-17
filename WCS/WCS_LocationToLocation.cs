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
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Mode='5'";
                strSQL += " AND Cmd_Sts='0'";
                strSQL += " AND TRACE='0'";
                strSQL += " ORDER BY Cmd_Sno, Loc, Prty, Crt_Dte DESC";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    CommandInfo commandInfo = new CommandInfo();
                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                    commandInfo.NewLoaction = dtCmdSno.Rows[0]["New_Loc"].ToString();
                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                    strMsg = commandInfo.CommandID + "|";
                    strMsg += commandInfo.CommandMode + "|";
                    strMsg += commandInfo.Loaction + "->" + commandInfo.NewLoaction + "|";
                    strMsg += "Transfer Command Initiated!";
                    funWriteSysTraceLog(strMsg);

                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                    {
                        try
                        {
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.LoactionToLoaction_CrateCraneCommand))
                            {
                                if (funCrateCraneCommand(commandInfo.CommandID, funGetEquFLoc(commandInfo.Loaction).ToString(), CraneMode.LoactionToLoaction, funGetCrnLoc(commandInfo.Loaction), funGetCrnLoc(commandInfo.NewLoaction), commandInfo.Priority))
                                {
                                    #region Update Command & Create Transfer Crane Command Success
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        catch (Exception)
                        {
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    for (int intCmdSno = 0; intCmdSno < dtCmdSno.Rows.Count; intCmdSno++)
                    {
                        CommandInfo commandInfo = new CommandInfo();
                        commandInfo.CommandID = dtCmdSno.Rows[intCmdSno]["Cmd_Sno"].ToString();
                        commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[intCmdSno]["Cmd_Mode"].ToString());
                        commandInfo.IOType = dtCmdSno.Rows[intCmdSno]["Io_Type"].ToString();
                        commandInfo.Loaction = dtCmdSno.Rows[intCmdSno]["Loc"].ToString();
                        commandInfo.NewLoaction = dtCmdSno.Rows[intCmdSno]["New_Loc"].ToString();
                        commandInfo.StationNo = dtCmdSno.Rows[intCmdSno]["Stn_No"].ToString();
                        commandInfo.Priority = dtCmdSno.Rows[intCmdSno]["Prty"].ToString();

                        strSQL = "SELECT * FROM EQUCMD";
                        strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                        strSQL += " AND RENEWFLAG<>'F'";
                        strSQL += " AND CMDMODE='5'";
                        strSQL += " AND CMDSTS='9'";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtEquCmd, ref strEM))
                        {
                            string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                            string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                            if (strCmdSts == CommandState.EQUCompleted && strCompleteCode.Substring(0, 1) == "W")
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
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
                            else if (strCmdSts == CommandState.EQUCompleted && strCompleteCode == "EF")
                            {
                                if (funUpdateCommand(commandInfo.CommandID, CommandState.CancelConstraint, Trace.StoreOut_CrateCraneCommand))
                                {
                                    if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.L2L).ToString()))
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
                                        strMsg += "库对库 Command Update Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "库对库 Crane Command Delete Success!";
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
                                        strMsg += "库对库 Crane Command Delete Fail!";
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
                                    strMsg += "库对库 Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                            }
                            else if (strCmdSts == CommandState.EQUCompleted && (strCompleteCode == "92" || strCompleteCode == "FF"))
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                                {
                                    try
                                    {
                                        if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CraneCommandFinish))
                                        {
                                            if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.L2L).ToString()))
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
                                                strMsg += "库对库 Command Update Success!";
                                                funWriteSysTraceLog(strMsg);

                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += strCompleteCode + "|";
                                                strMsg += "库对库 Crane Command Delete Success!";
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
                                                strMsg += "库对库 Crane Command Delete Fail!";
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
                                            strMsg += "库对库 Command Update Fail!";
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
