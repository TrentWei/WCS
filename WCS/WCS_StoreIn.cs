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
                    //命令不为空、目的站不为空、自动、无退回请求、站口模式；
                    #region 入库1
                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                        bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                        !bufferData[intBufferIndex]._ReturnRequest &&
                        intBufferIndex != 13 &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On &&
                        (strBufferName != "A12" || strBufferName != "B01"))
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrWhiteSpace(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!---1";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!---1";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Fail!---1";
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

                                //#region Set KanbanInfo
                                //if (funSetKanbanInfo("B01", strCommandID, 1, strBCR, strLoaction, " "))
                                //{
                                //    strMsg = strCommandID + "|";
                                //    strMsg += strLoaction + "|";
                                //    strMsg += strBCR + "|";
                                //    strMsg += "Set KanbanInfo Success!";
                                //    funWriteSysTraceLog(strMsg);
                                //}
                                //else
                                //{
                                //    strMsg = strCommandID + "|";
                                //    strMsg += strLoaction + "|";
                                //    strMsg += strBCR + "|";
                                //    strMsg += "Set KanbanInfo Fail!";
                                //    funWriteSysTraceLog(strMsg);
                                //}
                                //#endregion Set KanbanInfo
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
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR" && strBufferName == "A02")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += " AND Cmd_Sts='0'";
                            strSQL += " AND CMD_MODE='1'";
                            strSQL += " AND TRACE='0'";
                            strSQL += " ORDER BY LOC DESC";
                            if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                            {
                                if (dtCmdSno.Rows.Count == 1)
                                {
                                    #region StoreIn Command = 1
                                    CommandInfo commandInfo = new CommandInfo();
                                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                                    commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                                    commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreIn_GetStoreInCommandAndWritePLC))
                                    {
                                        string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                        {
                                            #region Update Command & Write MPLC Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                            strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                            strMsg += "StroreIn Command Update Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "Write MPLC Success!";
                                            funWriteSysTraceLog(strMsg);

                                            bCRData[intIndex].funClear();
                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                            strMsg += "2->0|";
                                            strMsg += strBCR + "|";
                                            strMsg += "BCR Clear!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command & Write MPLC Success

                                            #region Set KanbanInfo
                                            if (funSetKanbanInfo(commandInfo.StationNo, commandInfo.CommandID, commandInfo.CommandMode,
                                                commandInfo.PalletNo, commandInfo.Loaction, commandInfo.CycleNo))
                                            {
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "Set KanbanInfo Success!";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                            else
                                            {
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "Set KanbanInfo Fail!";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                            #endregion Set KanbanInfo
                                        }
                                        else
                                        {
                                            #region Update Command Success But Write MPLC Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
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
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StroreIn Command Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                    #endregion StoreIn Command = 1
                                }
                                else
                                {
                                    #region StoreIn Command > 1
                                    string[] strValues = new string[] { "2" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                    {
                                        #region Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "Exists Multiple StroreIn Command!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear

                                        #region Set KanbanInfo
                                        if (funSetKanbanInfo(bCRData[intIndex]._BufferName, bCRData[intIndex]._ResultID, "找到多笔入库命令"))
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Success!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        else
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        #endregion Set KanbanInfo
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
                                    #endregion StoreIn Command > 1
                                }
                            }
                            else
                            {
                                #region 查询不到命令,根据条码值,产生新的入库命令
                                strSQL = "SELECT * FROM ITEM_MST";
                                strSQL += " WHERE Plt_No='" + strBCR + "'";
                                if (strBufferName == "B01")
                                {
                                    strSQL += " AND ITEM_TYPE='M'";
                                }
                                else
                                {
                                    strSQL += " AND ITEM_TYPE='P'";
                                }
                                strSQL += " AND ITEM_NO='" + strBCR + "'";
                                strSQL += " AND WH_NO='A'";
                                strSQL += " AND ITEM_STS='Q'";
                                strSQL += " AND STATUS='N'";
                                if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    #region StoreIn ITEM_NO
                                    string strLoaction = "";
                                    string Item_type = "P";
                                    #region
                                    string strCommandID = funGetCommandID();
                                    if (funGetEmptyLocation("L", "P", ref strLoaction))
                                    {
                                        if (funCreateAGVStoreInCommand(strCommandID, strLoaction, strBCR, strBufferName))
                                        {

                                            if (funLockStoreInPalletNo(strBCR, Item_type))
                                            {
                                                #region 201803021 没有命令产生新命令之后不清除条码 下次循环直接使用该条码
                                                //string[] strValues = new string[] { strCommandID, "1", "1" };
                                                //if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                //{
                                                //    #region InSert Command & Write MPLC Success
                                                InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                                //    strMsg = strCommandID + "|";
                                                //    strMsg += strLoaction + "|";
                                                //    strMsg += strBCR + "|";
                                                //    strMsg += "StroreIn Command Insert Success!";
                                                //    funWriteSysTraceLog(strMsg);

                                                //    strMsg = strCommandID + "|";
                                                //    strMsg += strLoaction + "|";
                                                //    strMsg += strBCR + "|";
                                                //    strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                                //    strMsg += string.Join(",", strValues) + "|";
                                                //    strMsg += "Write MPLC Success!";
                                                //    funWriteSysTraceLog(strMsg);

                                                //    bCRData[intIndex].funClear();
                                                //    strMsg = bCRData[intIndex]._BufferName + "|";
                                                //    strMsg += "2->0|";
                                                //    strMsg += strBCR + "|";
                                                //    strMsg += "BCR Clear!";
                                                //    funWriteSysTraceLog(strMsg);
                                                //    #endregion Insert Command & Write MPLC Success

                                                #region Set KanbanInfo
                                                //    if (funSetKanbanInfo("B01", strCommandID, 1, strBCR, strLoaction, " "))
                                                //    {
                                                //        strMsg = strCommandID + "|";
                                                //        strMsg += strLoaction + "|";
                                                //        strMsg += strBCR + "|";
                                                //        strMsg += "Set KanbanInfo Success!";
                                                //        funWriteSysTraceLog(strMsg);
                                                //    }
                                                //    else
                                                //    {
                                                //        strMsg = strCommandID + "|";
                                                //        strMsg += strLoaction + "|";
                                                //        strMsg += strBCR + "|";
                                                //        strMsg += "Set KanbanInfo Fail!";
                                                //        funWriteSysTraceLog(strMsg);
                                                //    }
                                                #endregion Set KanbanInfo
                                                //}
                                                //else
                                                //{
                                                #region Insert Command Success But Write MPLC Fail
                                                //    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                                //    strMsg = strCommandID + "|";
                                                //    strMsg += strLoaction + "|";
                                                //    strMsg += strBCR + "|";
                                                //    strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                                //    strMsg += string.Join(",", strValues) + "|";
                                                //    strMsg += "Write MPLC Fail!";
                                                //    funWriteSysTraceLog(strMsg);
                                                #endregion Insert Command Success But Write MPLC Fail
                                                //}
                                                #endregion
                                            }
                                            else
                                            {
                                                #region Update StoreIn PalletNo Fail

                                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                                strMsg = strCommandID + "|";
                                                strMsg += strLoaction + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += "|";
                                                strMsg += "Update StoreIn PalletNo Fail!";
                                                funWriteSysTraceLog(strMsg);

                                                #endregion Update StoreIn PalletNo Fail
                                            }

                                        }
                                        else
                                        {
                                            #region Insert Command Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                    else
                                    {
                                        #region Update StoreInLocation Fail

                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += strBCR + "|";
                                        strMsg += "Update StoreInLocation Fail!";
                                        funWriteSysTraceLog(strMsg);

                                        #endregion StoreInLocation
                                    }
                                    #endregion
                                    #endregion StoreIn Command = 1
                                }
                                else
                                {
                                    string[] strValues = new string[] { "1" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                    {
                                        #region Can't Find StroreIn ITEM_NO & Write MPLC Success & BCR Clear
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "Can't Find StroreIn ITEM_NO!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Can't Find StroreIn Command & Write MPLC Success & BCR Clear

                                        #region Set KanbanInfo
                                        if (funSetKanbanInfo(bCRData[intIndex]._BufferName, bCRData[intIndex]._ResultID, "找不到该物料基本信息"))
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Success!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        else
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        #endregion Set KanbanInfo
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
                                #endregion 查询不到命令,根据条码值,产生新的入库命令
                            }
                            #endregion Read OK
                        }
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += "AND STN_NO='" + strBufferName + "'";
                            strSQL += " AND Cmd_Sts='0'";
                            strSQL += " AND CMD_MODE='1'";
                            strSQL += " AND TRACE='0'";
                            strSQL += " ORDER BY LOC DESC";
                            if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                            {
                                if (dtCmdSno.Rows.Count == 1)
                                {
                                    #region StoreIn Command = 1
                                    CommandInfo commandInfo = new CommandInfo();
                                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                                    commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                                    commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreIn_GetStoreInCommandAndWritePLC))
                                    {
                                        string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                        {
                                            #region Update Command & Write MPLC Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                            strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                            strMsg += "StroreIn Command Update Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "Write MPLC Success!";
                                            funWriteSysTraceLog(strMsg);

                                            bCRData[intIndex].funClear();
                                            strMsg = bCRData[intIndex]._BufferName + "|";
                                            strMsg += "2->0|";
                                            strMsg += strBCR + "|";
                                            strMsg += "BCR Clear!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command & Write MPLC Success

                                            #region Set KanbanInfo
                                            if (funSetKanbanInfo(commandInfo.StationNo, commandInfo.CommandID, commandInfo.CommandMode,
                                                commandInfo.PalletNo, commandInfo.Loaction, commandInfo.CycleNo))
                                            {
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "Set KanbanInfo Success!";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                            else
                                            {
                                                strMsg = commandInfo.CommandID + "|";
                                                strMsg += commandInfo.CycleNo + "|";
                                                strMsg += commandInfo.CommandMode + "|";
                                                strMsg += commandInfo.StationNo + "|";
                                                strMsg += commandInfo.Loaction + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "Set KanbanInfo Fail!";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                            #endregion Set KanbanInfo
                                        }
                                        else
                                        {
                                            #region Update Command Success But Write MPLC Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
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
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StroreIn Command Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                    #endregion StoreIn Command = 1
                                }
                                else
                                {
                                    #region StoreIn Command > 1
                                    string[] strValues = new string[] { "2" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                    {
                                        #region Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "Exists Multiple StroreIn Command!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear

                                        #region Set KanbanInfo
                                        if (funSetKanbanInfo(bCRData[intIndex]._BufferName, bCRData[intIndex]._ResultID, "找到多笔入库命令"))
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Success!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        else
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        #endregion Set KanbanInfo
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
                                    #endregion StoreIn Command > 1
                                }
                            }
                            else
                            {
                                string[] strValues = new string[] { "2" };
                                if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                {
                                    #region Can't Find StroreIn Command & Write MPLC Success & BCR Clear
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bCRData[intIndex]._ResultID + "|";
                                    strMsg += "Can't Find StroreIn Command!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                    strMsg += string.Join(",", strValues) + "|";
                                    strMsg += "Write MPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    bCRData[intIndex].funClear();
                                    strMsg = bCRData[intIndex]._BufferName + "|";
                                    strMsg += "2->0|";
                                    strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                    strMsg += "BCR Clear!";
                                    funWriteSysTraceLog(strMsg);
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
                            #endregion Read OK
                        }
                    }
                    #endregion

                    #region 入库2

                    if (!bufferData[intBufferIndex]._ReturnRequest &&
                    bufferData[intBufferIndex]._APositioning &&
                    intBufferIndex == 13 &&
                    bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On &&
                    strBufferName == "B01")
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
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR" && strBufferName == "B01")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += "AND STN_NO='" + strBufferName + "'";
                            strSQL += " AND Cmd_Sts='0'";
                            strSQL += " AND CMD_MODE='1'";
                            strSQL += " AND TRACE='0'";
                            strSQL += " ORDER BY LOC DESC";
                            if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                            {
                                if (dtCmdSno.Rows.Count == 1)
                                {
                                    #region StoreIn Command = 1
                                    CommandInfo commandInfo = new CommandInfo();
                                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                                    commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                                    commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                                    commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                                    commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                                    commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();

                                    string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        #region  Write MPLC Success

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Write MPLC Success

                                        strValues = new string[] { "2" };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_APositioning, strValues))
                                        {
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "B01站口已识别条码，PLC修改触发条件1~2 成功";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                    }
                                    else
                                    {
                                        #region  Write MPLC Fail
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Success But Write MPLC Fail
                                    }

                                    #endregion StoreIn Command = 1
                                }
                                else
                                {
                                    #region StoreIn Command > 1
                                    string[] strValues = new string[] { "2" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                    {
                                        #region Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "Exists Multiple StroreIn Command!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Exists Multiple StroreIn Command & Write MPLC Success & BCR Clear

                                        #region Set KanbanInfo
                                        if (funSetKanbanInfo(bCRData[intIndex]._BufferName, bCRData[intIndex]._ResultID, "找到多笔入库命令"))
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Success!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        else
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        #endregion Set KanbanInfo
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
                                    #endregion StoreIn Command > 1
                                }
                            }
                            else
                            {
                                #region 查询不到命令,根据条码值,产生新的入库命令
                                strSQL = "SELECT * FROM ITEM_MST";
                                strSQL += " WHERE Plt_No='" + strBCR + "'";
                                if (strBufferName == "B01")
                                {
                                    strSQL += " AND ITEM_TYPE='M'";
                                }
                                else
                                {
                                    strSQL += " AND ITEM_TYPE='P'";
                                }
                                strSQL += " AND ITEM_NO='" + strBCR + "'";
                                strSQL += " AND WH_NO='A'";
                                strSQL += " AND ITEM_STS='Q'";
                                strSQL += " AND STATUS='N'";
                                if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    #region StoreIn ITEM_NO
                                    string strLoaction = "";
                                    string Item_type = "M";
                                    #region
                                    string strCommandID = funGetCommandID();
                                    if (funGetEmptyLocation("H", "M", ref strLoaction))
                                    {
                                        if (funCreateAGVStoreInCommand(strCommandID, strLoaction, strBCR, strBufferName))
                                        {
                                            if (funLockStoreInPalletNo(strBCR, Item_type))
                                            {
                                                InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            }
                                            else
                                            {
                                                #region Update StoreIn PalletNo Fail
                                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                                strMsg = strCommandID + "|";
                                                strMsg += strLoaction + "|";
                                                strMsg += strBCR + "|";
                                                strMsg += "|";
                                                strMsg += "Update StoreIn PalletNo Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion Update StoreIn PalletNo Fail
                                            }
                                        }
                                        else
                                        {
                                            #region Insert Command Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                    else
                                    {
                                        #region Update StoreInLocation Fail

                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += strBCR + "|";
                                        strMsg += "Update StoreInLocation Fail!";
                                        funWriteSysTraceLog(strMsg);

                                        #endregion StoreInLocation
                                    }
                                    #endregion
                                    #endregion StoreIn Command = 1
                                }
                                else
                                {
                                    string[] strValues = new string[] { "1" };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues))
                                    {
                                        #region Can't Find StroreIn ITEM_NO & Write MPLC Success & BCR Clear
                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bCRData[intIndex]._ResultID + "|";
                                        strMsg += "Can't Find StroreIn ITEM_NO!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                        strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "->" + bCRData[intIndex]._ResultID + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Can't Find StroreIn Command & Write MPLC Success & BCR Clear

                                        #region Set KanbanInfo
                                        if (funSetKanbanInfo(bCRData[intIndex]._BufferName, bCRData[intIndex]._ResultID, "找不到该物料基本信息"))
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Success!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        else
                                        {
                                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                            strMsg += bCRData[intIndex]._ResultID + "|";
                                            strMsg += "Set KanbanInfo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                        #endregion Set KanbanInfo
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
                                #endregion 查询不到命令,根据条码值,产生新的入库命令
                            }
                            #endregion Read OK
                        }
                    }

                    #endregion

                    #region 入库3

                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                    !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                    !bufferData[intBufferIndex]._ReturnRequest &&
                    !bufferData[intBufferIndex].W_Discharged &&
                    bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                    bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On &&
                    strBufferName == "A12")
                    {
                        if (bCRData[intIndex]._BCRSts == BCR.BCRSts.None && string.IsNullOrWhiteSpace(bCRData[intIndex]._ResultID))
                        {
                            #region Pallet On Station && BCR Trigger On
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Pallet On Station!---3";
                            funWriteSysTraceLog(strMsg);

                            if (bCRData[intIndex].funTriggerBCROn(ref strEM))
                            {
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += "BCR Trigger On Success!---3";
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

                            string[] strValues = new string[] { "1" };
                            if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues))
                            {
                                #region BCR Read Fail & Write MPLC Success & BCR Clear
                                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                strMsg += bufferData[intBufferIndex]._W_ReturnRequest + "|";
                                strMsg += string.Join(",", strValues) + "|";
                                strMsg += "扫码失败PC放行!";
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
                        else if (bCRData[intIndex]._BCRSts == BCR.BCRSts.ReadFinish && bCRData[intIndex]._ResultID != "ERROR" && strBufferName == "A12")
                        {
                            #region Read OK
                            string strBCR = bCRData[intIndex]._ResultID;
                            strSQL = "SELECT * FROM CMD_MST";
                            strSQL += " WHERE Plt_No='" + strBCR + "'";
                            strSQL += "AND STN_NO='" + strBufferName + "'";
                            strSQL += " AND Cmd_Sts<='1'";
                            strSQL += " AND CMD_MODE='1'";
                            strSQL += " AND TRACE in ('0','11')";
                            strSQL += " ORDER BY LOC DESC";
                            if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                            {
                                if (dtCmdSno.Rows.Count == 1)
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
                                    commandInfo.Cmd_Sts = dtCmdSno.Rows[0]["Cmd_Sts"].ToString();
                                    string[] strValues = new string[] { commandInfo.CommandID, "1", commandInfo.CommandMode.ToString() };
                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                    {
                                        #region  Write MPLC Success

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        bCRData[intIndex].funClear();
                                        strMsg = bCRData[intIndex]._BufferName + "|";
                                        strMsg += "2->0|";
                                        strMsg += strBCR + "|";
                                        strMsg += "BCR Clear!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Write MPLC Success

                                        strValues = new string[] { "1" };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues))
                                        {
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "A012站口已识别条码，PLC写入成功";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                    }
                                    else
                                    {
                                        #region  Write MPLC Fail
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Success But Write MPLC Fail
                                    }
                                }
                            }
                            else
                            {
                                bCRData[intIndex].funClear();
                                strMsg = bCRData[intIndex]._BufferName + "|";
                                strMsg += "2->0|";
                                strMsg += strBCR + "|";
                                strMsg += "BCR Clear!";
                                funWriteSysTraceLog(strMsg);
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, "1");
                            }
                            #endregion 入库3
                        }
                    #endregion
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
        /// 创建equ命令
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
                        if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if (funCrateCraneCommand(strCommandID, CraneMode.StoreIn, stnDef.StationIndex.ToString(), commandInfo.Loaction, commandInfo.Priority))
                                    {
                                        if (funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreIn_CrateCraneCommand))
                                        {
                                            #region Update Command & Create StoreIn Crane Command Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
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
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
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
                    if (InitSys._DB.funGetDT(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if (strCmdSts == CommandState.Completed && strCompleteCode.Substring(0, 1) == "W")
                        {
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);

                            strSQL = "UPDATE CMD_MST";
                            strSQL += " SET CMD_STS='0',TRACE='" + Trace.StoreIn_GetStoreInCommandAndWritePLC + "'";
                            strSQL += " WHERE CMD_STS='1' and CMD_SNO='" + commandInfo.CommandID + "'";
                            if (InitSys._DB.funExecSql(strSQL, ref strEM))
                            {
                                strSQL = "DELETE FROM EQUCMD where CMDSNO='" + commandInfo.CommandID + "'";
                                if (InitSys._DB.funExecSql(strSQL, ref strEM))
                                {
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                            if (InitSys._DB.funExecSql(strSQL, ref strEM))
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
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.CompletedWaitPost, Trace.StoreIn_CraneCommandFinish))
                            {
                                if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreIn).ToString()))
                                {
                                    #region StoreIn Crane Command Finish & Update Command Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
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
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
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
