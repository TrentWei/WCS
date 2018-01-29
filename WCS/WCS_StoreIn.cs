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
                            string[] strValues = new string[] { "1" };
                            if(InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
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

                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += bCRData[intIndex]._BCRSts + "|";
                                strMsg += bCRData[intIndex]._ResultID + "|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);

                                bCRData[intIndex].funClear();
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
                        }
                        else if(bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += " AND Cmd_Sts='0'";
                            strSQL += " AND CMD_MODE='1'";
                            strSQL += " AND TRACE='0'";
                            strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
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
                                if(funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreIn_GetStoreInCommandAndWritePLC))
                                {
                                    string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                    if(InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        #region Update Command & Write MPLC Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StroreIn Command Update Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._BCRSts + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        #endregion Update Command & Write MPLC Success
                                    }
                                    else
                                    {
                                        #region Update Command Success But Write MPLC Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
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
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                    strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                    strMsg += "StroreIn Command Update Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Update Command Fail
                                }
                            }
                            else
                            {
                                string[] strValues = new string[] { "2" };
                                if(InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                {
                                    #region Can't Find StroreIn Command & Write MPLC Success & BCR Clear
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += "Can't Find StroreIn Command!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += bCRData[intIndex]._BCRSts + "|";
                                    strMsg += bCRData[intIndex]._ResultID + "|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    #endregion Can't Find StroreIn Command & Write MPLC Success & BCR Clear
                                }
                                else
                                {
                                    #region Can't Find Command But Write MPLC Fail
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Can't Find Command But Write MPLC Fail
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
                        (bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn || bufferData[intBufferIndex]._Mode == Buffer.StnMode.Picking) &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts='1'";
                        strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
                        strSQL += " AND ((CMD_MODE='1'";
                        strSQL += " AND TRACE='" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "')";
                        strSQL += " OR (CMD_MODE='3'";
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

                            if(!funCheckCraneExistsCommand(InitSys._CraneNo, CraneMode.StoreIn, stnDef.StationIndex.ToString()))
                            {
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                if(funCrateCraneCommand(InitSys._CraneNo, commandInfo.CommandID, CraneMode.StoreIn,
                                    stnDef.StationIndex.ToString(), commandInfo.Loaction, commandInfo.Priority))
                                {
                                    if(funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreIn_CrateCraneCommand))
                                    {
                                        #region Update Command & Create StoreIn Crane Command Success
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += "StoreIn Crane Command Create Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += CommandState.Start + "|";
                                        if(commandInfo.CommandMode.ToString() == CMDMode.Picking)
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
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += CommandState.Start + "|";
                                        if(commandInfo.CommandMode.ToString() == CMDMode.Picking)
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += "StoreIn Crane Command Create Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Create StoreIn Crane Command Fail
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
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts<'3'";
                strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
                strSQL += " AND CMD_MODE IN ('1', '3')";
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
                    strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                    strSQL += " AND EquNo='" + InitSys._CraneNo + "'";
                    strSQL += " AND RENEWFLAG<>'F'";
                    strSQL += " AND CMDMODE='1'";
                    strSQL += " AND CMDSTS='9'";
                    if(InitSys._DB.funGetDT(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if(strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
                        {
                            strSQL = "UPDATE EQUCMD";
                            strSQL += " SET CMDSTS='0'";
                            strSQL += " WHERE CMDSNO='" + commandInfo.CommandID + "'";
                            strSQL += " AND EquNo='" + InitSys._CraneNo + "'";
                            if(InitSys._DB.funExecSql(strSQL, ref strEM))
                            {
                                #region Retry StoreIn Crane Command Success
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += CommandState.Start + "|";
                                strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "Retry StoreIn Crane Command Success!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Retry StoreIn Crane Command Success
                            }
                            else
                            {
                                #region Retry StoreIn Crane Command Fail
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += CommandState.Start + "|";
                                strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "Retry StoreIn Crane Command Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Retry StoreIn Crane Command Fail
                            }
                        }
                        else if(strCmdSts == CommandState.Completed && strCompleteCode == "EF")
                        {
                            if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CrateCraneCommand))
                            {
                                #region StoreIn Crane Command Finish & Update Command Success
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "Crane Command StoreIn Finish!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                strMsg += "StoreIn Command Update Success!";
                                funWriteSysTraceLog(strMsg);
                                #endregion StoreIn Crane Command Finish & Update Command Success
                            }
                            else
                            {
                                #region Update Command Fail
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreIn_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreIn Command Update Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command Fail
                            }
                        }
                        else if(strCmdSts == CommandState.Completed && strCompleteCode == "92")
                        {
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                            if(funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CraneCommandFinish))
                            {
                                if(funDeleteEquCmd(InitSys._CraneNo, commandInfo.CommandID, ((int)Buffer.StnMode.StoreIn).ToString()))
                                {
                                    #region StoreIn Crane Command Finish & Update Command Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                    strMsg += Trace.StoreIn_CrateCraneCommand + "->" + Trace.StoreIn_CraneCommandFinish + "|";
                                    strMsg += "StoreIn Command Update Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "StoreIn Crane Command Delete Success!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion StoreIn Crane Command Finish & Update Command Success
                                }
                                else
                                {
                                    #region Delete StoreIn Crane Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = commandInfo.CommandID + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += strCompleteCode + "|";
                                    strMsg += "StoreIn Crane Command Delete Fail!";
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
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
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
