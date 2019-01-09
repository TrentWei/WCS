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
        private void funAGVSchedule(bool load)
        {
            funAGVSchedule_GetScheduleAndCreateCommand(load);
            funAGVSchedule_ScheduleComandFinish();
        }

        private void funAGVSchedule_GetScheduleAndCreateCommand(bool load)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtProduce = new DataTable();
            try
            {
                if (!load && !funCheckExistsAGVSchedule())
                {
                    strSQL = "SELECT * FROM PRODUCE ";
                    strSQL += " WHERE STATUS='0'";
                    strSQL += " AND Wh_NO='A'";
                    strSQL += " AND ITEM_TYPE='P'";
                    strSQL += " AND TIME <='" + DateTime.Now.ToString("HH:mm") + "'";
                    strSQL += " ORDER BY PRODUCE_NO";
                    if (InitSys._DB.funGetDT(strSQL, ref dtProduce, ref strEM))
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
                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Begin);
                            if (funCreateAGVStoreOutCommand(strCommandID, strLocation, prodecu.Item_No, "A03"))
                            {
                                if (funLockStoreOutLocation(strLocation))
                                {
                                    if (funLockStoreOutPalletNo(prodecu.Item_No, prodecu.Prodecu_Qty, prodecu.PStn_No, prodecu.Item_Type))
                                    {
                                        if (funUpdateProdecu(prodecu.Prodecu_No, "1", strCommandID))
                                        {
                                            #region Create AGV StroreOut Command & Lock StroreOut Location Success
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Commit);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLocation + "|";
                                            strMsg += prodecu.Item_No + "|";
                                            strMsg += "Create AGV StroreOut Command Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = strCommandID + "|";
                                            strMsg += strLocation + "|";
                                            strMsg += prodecu.Item_No + "|";
                                            strMsg += "Lock AGV StroreOut Location Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = strCommandID + "|";
                                            strMsg += strLocation + "|";
                                            strMsg += prodecu.Item_No + "|";
                                            strMsg += "|";
                                            strMsg += "Lock AGV StroreOut PalletNo Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = prodecu.Prodecu_No + "|";
                                            strMsg += prodecu.Item_No + "|";
                                            strMsg += strLocation + "|";
                                            strMsg += "0->1|";
                                            strMsg += strCommandID + "|";
                                            strMsg += "Update Prodecu Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Create AGV StroreOut Command & Lock StroreOut Location Success
                                        }
                                        else
                                        {
                                            #region Update Prodecu Fail
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                            strMsg = prodecu.Prodecu_No + "|";
                                            strMsg += prodecu.Item_No + "|";
                                            strMsg += strLocation + "|";
                                            strMsg += "0->1|";
                                            strMsg += "Update Prodecu Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Prodecu Fail
                                        }
                                    }
                                    else
                                    {
                                        #region Lock AGV StroreOut PalletNo Fail
                                        InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLocation + "|";
                                        strMsg += prodecu.Item_No + "|";
                                        strMsg += "|";
                                        strMsg += "Lock AGV StroreOut PalletNo Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Lock AGV StroreOut PalletNo Fail
                                    }
                                }
                                else
                                {
                                    #region Lock AGV StroreOut Location Fail
                                    InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += strLocation + "|";
                                    strMsg += prodecu.Item_No + "|";
                                    strMsg += "Lock AGV StroreOut Location Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Lock AGV StroreOut Location Fail
                                }
                            }
                            else
                            {
                                #region Create AGV StoreOut Command Fail
                                InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                strMsg = prodecu.Prodecu_No + "|";
                                strMsg += prodecu.Item_No + "|";
                                strMsg += strLocation + "|";
                                strMsg += "Create AGV StoreOut Command Fail!";
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
                                strMsg += "Create 芯盒 StroreOut Command Fail!";
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
                                strMsg += "Update 芯盒 Prodecu Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Prodecu Fail
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
                if (dtProduce != null)
                {
                    dtProduce.Clear();
                    dtProduce.Dispose();
                    dtProduce = null;
                }
            }
        }

        private void funAGVSchedule_ScheduleComandFinish()
        {
            string strMsg = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(bufferData[InitSys._AGV_StoreOut_MPLCBufferIndex]._CommandID) &&
                    bufferData[InitSys._AGV_StoreOut_MPLCBufferIndex]._Mode == Buffer.StnMode.StoreOut &&
                    bufferData[InitSys._AGV_StoreOut_MPLCBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On &&
                    bufferData[InitSys._AGV_StoreOut_MPLCBufferIndex]._EQUStatus.Load == Buffer.Signal.On)
                {
                    string strCommandID = bufferData[InitSys._AGV_StoreOut_MPLCBufferIndex]._CommandID;
                    if (funCheckExistsAGVSchedule(strCommandID))
                    {
                        if (funUpdateProdecu(strCommandID, "2"))
                        {
                            #region Update Prodecu Finish Success
                            strMsg = strCommandID + "|";
                            strMsg += "1->2|";
                            strMsg += "Update Prodecu Finish Success!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Update Prodecu Finish Success
                        }
                        else
                        {
                            #region Update Prodecu Finish Fail
                            strMsg = strCommandID + "|";
                            strMsg += "1->2|";
                            strMsg += "Update Prodecu Finish Fail!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Update Prodecu Finish Fail
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funStoreInRequestFromAGV(string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                if ((!string.IsNullOrWhiteSpace(palletNo) && palletNo != "ERROR"))
                {
                    strSQL = "SELECT * FROM ITEM_MST";
                    strSQL += " WHERE Plt_No='" + palletNo + "'";
                    strSQL += " and Item_Type='P'";
                    if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        if (dtCmdSno.Rows[0]["Wh_No"].ToString().Substring(0, 1) == "A")
                        {
                            string strCommandID = string.Empty;
                            string strLoaction = string.Empty;
                            int intCommandCount = funCheckExistsAGVStoreInCommand(palletNo, ref strCommandID, ref strLoaction);

                            if (intCommandCount == 0)
                            {
                                strMsg = palletNo + "|";
                                strMsg += "StoreIn Request From AGV!";
                                funWriteSysTraceLog(strMsg);

                                InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Begin);

                                #region Create StroreIn Command
                                strCommandID = funGetCommandID();
                                if (funGetEmptyLocation("L", "P", ref strLoaction))
                                {
                                    if (funCreateAGVStoreInCommand(strCommandID, strLoaction, palletNo, "A13"))
                                    {
                                        if (funLockStoreInPalletNo(palletNo, "P"))
                                        {
                                            #region Create AGV StroreIn Command & Lock StroreIn Location Success
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Commit);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "Create AGV StroreIn Command Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "Lock AGV StroreIn Location Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "|";
                                            strMsg += "Lock AGV StroreIn PalletNo Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Create AGV StroreIn Command & Lock StroreIn Location Success
                                        }
                                        else
                                        {
                                            #region Lock AGV StroreIn PalletNo Fail
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "|";
                                            strMsg += "Lock AGV StroreIn PalletNo Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Lock AGV StroreIn PalletNo Fail
                                        }
                                    }
                                    else
                                    {
                                        #region Create AGV StoreIn Command Fail
                                        InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += palletNo + "|";
                                        strMsg += "Create AGV StoreIn Command Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Create AGV StoreIn Command Fail
                                    }
                                }
                                else
                                {
                                    #region Lock AGV StroreIn Location Fail
                                    InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += strLoaction + "|";
                                    strMsg += palletNo + "|";
                                    strMsg += "Lock AGV StroreIn Location Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Lock AGV StroreIn Location Fail
                                }
                                #endregion Create StroreIn Command

                            }
                            else if (intCommandCount == 1)
                            {
                                #region AGV StoreIn Command = 1
                                if (string.IsNullOrWhiteSpace(bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._CommandID) &&
                                    string.IsNullOrWhiteSpace(bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._Destination) &&
                                    bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._Mode == Buffer.StnMode.None &&
                                    bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                                {
                                    InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Begin);
                                    if (funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreIn_GetStoreInCommandAndWritePLC))
                                    {
                                        #region Write MPLC
                                        string[] strValues = new string[] { strCommandID, "1", "1" };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._W_CmdSno, strValues))
                                        {
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Commit);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                            strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                            strMsg += "StroreIn Command Update Success!";
                                            funWriteSysTraceLog(strMsg);

                                            #region Write MPLC Success
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "Write MPLC Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Write MPLC Success
                                        }
                                        else
                                        {
                                            #region Write MPLC Fail
                                            InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += bufferData[InitSys._AGV_StoreIn_MPLCBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "Write MPLC Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Write MPLC Fail
                                        }
                                        #endregion Write MPLC
                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += palletNo + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StroreIn Command Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                }
                                else if (sMPLCData_2.StoreIn == 0)
                                {
                                    #region Write SPLC
                                    SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "1");
                                    if (InitSys._SPLC.funWriteSPLC(tag))
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += tag._ItemName + "|";
                                        strMsg += tag._ItemValue + "|";
                                        strMsg += "Write SPLC Success!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    else
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += tag._ItemName + "|";
                                        strMsg += tag._ItemValue + "|";
                                        strMsg += "Write SPLC Fail!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    #endregion Write SPLC
                                }
                                #endregion AGV StoreIn Command = 1
                            }
                            else
                            {
                                #region AGV StoreIn Command > 1
                                if (sMPLCData_2.StoreIn == 0)
                                {
                                    SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                                    if (InitSys._SPLC.funWriteSPLC(tag))
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += tag._ItemName + "|";
                                        strMsg += tag._ItemValue + "|";
                                        strMsg += "Write SPLC Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = palletNo + "|";
                                        strMsg += "Exists Multiple AGV StoreIn Command!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    else
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += tag._ItemName + "|";
                                        strMsg += tag._ItemValue + "|";
                                        strMsg += "Write SPLC Fail!";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                }
                                #endregion AGV StoreIn Command > 1
                            }
                        }
                        else
                        {
                            #region Can't StoreIn
                            if (sMPLCData_2.StoreIn == 0)
                            {
                                strMsg = palletNo + "|";
                                strMsg += "StoreIn Request From AGV!";
                                funWriteSysTraceLog(strMsg);

                                SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                                if (InitSys._SPLC.funWriteSPLC(tag))
                                {
                                    strMsg = palletNo + "|";
                                    strMsg += tag._ItemName + "|";
                                    strMsg += tag._ItemValue + "|";
                                    strMsg += "Write SPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = palletNo + "|";
                                    strMsg += "Can't StoreIn!";
                                    funWriteSysTraceLog(strMsg);
                                }
                                else
                                {
                                    strMsg = palletNo + "|";
                                    strMsg += tag._ItemName + "|";
                                    strMsg += tag._ItemValue + "|";
                                    strMsg += "Write SPLC Fail!";
                                    funWriteSysTraceLog(strMsg);
                                }
                            }
                            #endregion Can't StoreIn
                        }
                    }
                    else
                    {
                        #region Can't Find PalletNo
                        if (sMPLCData_2.StoreIn == 0)
                        {
                            strMsg = palletNo + "|";
                            strMsg += "StoreIn Request From AGV!";
                            funWriteSysTraceLog(strMsg);

                            SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                            if (InitSys._SPLC.funWriteSPLC(tag))
                            {
                                strMsg = palletNo + "|";
                                strMsg += tag._ItemName + "|";
                                strMsg += tag._ItemValue + "|";
                                strMsg += "Write SPLC Success!";
                                funWriteSysTraceLog(strMsg);

                                strMsg = palletNo + "|";
                                strMsg += "Can't Find PalletNo!";
                                funWriteSysTraceLog(strMsg);
                            }
                            else
                            {
                                strMsg = palletNo + "|";
                                strMsg += tag._ItemName + "|";
                                strMsg += tag._ItemValue + "|";
                                strMsg += "Write SPLC Fail!";
                                funWriteSysTraceLog(strMsg);
                            }
                        }
                        #endregion Can't Find PalletNo
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

        private void funAGVNeedStationRequest(string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                if (!string.IsNullOrWhiteSpace(palletNo) && sMPLCData_2.StoreOut == 0 && strLastAGVBCR1 != palletNo)
                {
                    strMsg = palletNo + "|";
                    strMsg += "AGV Need Station Request!";
                    funWriteSysTraceLog(strMsg);

                    strSQL = "SELECT * FROM ITEM_MST";
                    strSQL += " WHERE Plt_No='" + palletNo + "'";
                    strSQL += " and Item_Type='P'";
                    if (InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        string strPStation = dtCmdSno.Rows[0]["PStn_No"].ToString();
                        if (!string.IsNullOrWhiteSpace(strPStation))
                        {
                            if (strPStation != "0")
                            {
                                #region Write SPLC
                                SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreOut_SPLCAddress, strPStation);
                                if (InitSys._SPLC.funWriteSPLC(tag))
                                {
                                    #region Pallet StoreOut To AGV Finish
                                    strMsg = palletNo + "|";
                                    strMsg += tag._ItemName + "|";
                                    strMsg += tag._ItemValue + "|";
                                    strMsg += "Write SPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = palletNo + "|";
                                    strMsg += strPStation + "|";
                                    strMsg += "Reply AGV Station Request!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Pallet StoreOut To AGV Finish
                                }
                                else
                                {
                                    #region Write SPLC Fail
                                    InitSys._DB.funCommitCtrl(DBSQL.TransactionType.Begin);
                                    strMsg = palletNo + "|";
                                    strMsg += tag._ItemName + "|";
                                    strMsg += tag._ItemValue + "|";
                                    strMsg += "Write SPLC Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Write SPLC Fail
                                }
                                #endregion Write SPLC
                            }
                            else
                            {
                                #region PStation Error, Please Check
                                strMsg = palletNo + "|";
                                strMsg = strPStation + "|";
                                strMsg += "AGV Station Error, Please Check!";
                                funWriteSysTraceLog(strMsg);
                                #endregion PStation Error, Please Check
                            }
                        }
                        else
                        {
                            #region Can't Find PStation, Please Check
                            strMsg = palletNo + "|";
                            strMsg += "Can't Find AGV Station, Please Check!";
                            funWriteSysTraceLog(strMsg);
                            #endregion Can't Find PStation, Please Check
                        }
                    }
                    else
                    {
                        #region Can't Find PalletNo, Please Check
                        strMsg = palletNo + "|";
                        strMsg += "Can't Find PalletNo, Please Check!";
                        funWriteSysTraceLog(strMsg);
                        #endregion Can't Find PalletNo, Please Check
                    }

                    strLastAGVBCR1 = palletNo;
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
    }
}
