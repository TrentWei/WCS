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
        private void funStroreIn()
        {
            funStoreIn_GetStoreInCommandAndWritePLC();
            funStoreIn_CrateCraneCommand();
            funStoreIn_CraneCommandFinish();
        }

        private void funStoreIn_GetStoreInCommandAndWritePLC()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                for(int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                {
                    string strBufferName = bCRData[intIndex]._BufferName;
                    int intBufferIndex = bCRData[intIndex]._BufferIndex;
                    if(string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        if(bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrWhiteSpace(bCRData[intIndex]._ResultID)) 
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!";
                            funWriteSysTraceLog(strMsg);

                            if(bCRData[intIndex].funTriggerBCROn(ref strEM))
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
                        else if(bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID == "ERROR")
                        {
                            #region BCR Read Fail && BCR Retrigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Read Fail!";
                            funWriteSysTraceLog(strMsg);

                            if(bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Retrigger On Success!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Retrigger On Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                            #endregion BCR Read Fail && BCR Retrigger On
                        }
                        else if(bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += " AND ((Cmd_Sts='0'";
                            strSQL += " AND CMDMODE='1'";
                            strSQL += " AND TRACE='0')";
                            strSQL += " OR (CMDMODE='3'";
                            strSQL += " AND TRACE='" + Trace.StoreOut_CraneCommandFinish + "'))";
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

                                        bCRData[intIndex].funClear();
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._BCRSts + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
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
                            else
                            {
                                string[] strValues = new string[] { "1" };
                                if(InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                {
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += "Can't Find Command!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bCRData[intIndex]._BCRSts + "|";
                                    strMsg += bCRData[intIndex]._ResultID + "|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);
                                }
                                else
                                {
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Fail!";
                                    funWriteSysTraceLog(strMsg);
                                }
                            }
                        }
                        else
                        {
                            bCRData[intIndex].funClear();
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += bCRData[intIndex]._BCRSts + "|";
                            strMsg += bCRData[intIndex]._ResultID + "|";
                            strMsg += "BCR Clear!";
                            funWriteSysTraceLog(strMsg);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funStoreIn_CrateCraneCommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach(StationInfo stnDef in lstStoreIn)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if(!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID;
                        strSQL = "SELECT TOP 1 Loc, Prty FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'3'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND CMDMODE IN ('1', '3')";
                        strSQL += " AND TRACE='" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "'";
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

                            if(!funCheckCraneExistsCommand("1", CraneMode.StoreIn, stnDef.StationIndex.ToString()))
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreIn_CrateCraneCommand))
                                {
                                    if(funCrateCraneCommand("1", strCommandID, "1",
                                        commandInfo.Loaction, stnDef.StationIndex.ToString(), commandInfo.Priority))
                                    {
                                        #region Update Command & Create StoreIn Crane Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreIn_GetStoreInCommandAndWritePLC + "->" + Trace.StoreIn_CrateCraneCommand + "|";
                                        strMsg += "Update Command Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += "Create StoreIn Crane Command Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Create StoreIn Crane Command Success
                                    }
                                    else
                                    {
                                        #region Create StoreIn Crane Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += "Create StoreIn Crane Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Create StoreIn Crane Command Fail
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
                                    strMsg += Trace.StoreIn_GetStoreInCommandAndWritePLC + "->" + Trace.StoreIn_CrateCraneCommand + "|";
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

        private void funStoreIn_CraneCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtEquCmd = new DataTable();
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach(StationInfo stnDef in lstStoreIn)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if(!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID;
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'3'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND CMDMODE IN ('1', '3')";
                        strSQL += " AND TRACE='" + Trace.StoreIn_CrateCraneCommand + "'";
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
                            strSQL += " AND CMDMODE='1'";
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
                                        #region Retry Crane Command Success
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "Retry StoreIn Crane Command Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Retry Crane Command Success
                                    }
                                    else
                                    {
                                        #region Retry Crane Command Fail
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "Retry StoreIn Crane Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Retry Crane Command Fail
                                    }
                                }
                                else if(strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                                {
                                    if(funUpdateCommand(strCommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CrateCraneCommand))
                                    {
                                        #region StoreIn Crane Command Finish & Update Command Success
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "StoreIn Crane Command Finish!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                        strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                        strMsg += "Update Command Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion StoreIn Crane Command Finish & Update Command Success
                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                        strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                        strMsg += strCompleteCode + "|";
                                        strMsg += "Update Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                }
                                else if(strCmdSts == CommandState.Completed && strCompleteCode == "92")
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if(funUpdateCommand(strCommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CraneCommandFinish))
                                    {
                                        if(funDeleteEquCmd("1", strCommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
                                        {
                                            #region StoreIn Crane Command Finish & Update Command Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                            strMsg += Trace.StoreIn_CrateCraneCommand + "->" + Trace.StoreIn_CraneCommandFinish + "|";
                                            strMsg += "Update Command Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "Delete StoreIn Crane Command Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion StoreIn Crane Command Finish & Update Command Success
                                        }
                                        else
                                        {
                                            #region Delete StoreIn Crane Command Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += strCompleteCode + "|";
                                            strMsg += "Delete StoreIn Crane Command Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Delete StoreIn Crane Command Fail
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
                                        strMsg += Trace.StoreIn_CrateCraneCommand + "->" + Trace.StoreIn_CraneCommandFinish + "|";
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
