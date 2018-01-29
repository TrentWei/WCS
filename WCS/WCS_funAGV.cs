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
        private void funStoreInRequestFromAGV(string palletNo)
        {

            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                if(!string.IsNullOrWhiteSpace(palletNo))
                {
                    strSQL = "SELECT * FROM ITEM_MST";
                    strSQL += " WHERE Plt_No='" + palletNo + "'";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        if(dtCmdSno.Rows[0]["Wh_No"].ToString().Substring(0, 1) != "A")
                        {
                            string strCommandID = string.Empty;
                            string strLoaction = string.Empty;
                            int intCommandCount = funCheckExistsAGVStoreInCommand(palletNo, ref strCommandID, ref strLoaction);

                            if(intCommandCount == 0)
                            {
                                strMsg = palletNo + "|";
                                strMsg += "StoreIn Request From AGV!";
                                funWriteSysTraceLog(strMsg);

                                bool bolCreateCommandFlag = false;
                                strLoaction = dtCmdSno.Rows[0]["Loc"].ToString();

                                #region StoreIn Loaction
                                if(string.IsNullOrWhiteSpace(strLoaction))
                                {
                                    strLoaction = funGetEmptyLocation();
                                    if(string.IsNullOrWhiteSpace(strLoaction))
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += "Try Get New Empty Location Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        bolCreateCommandFlag = false;

                                    }
                                    else
                                    {
                                        strMsg = palletNo + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += "Try Get New Empty Location Success!";
                                        funWriteSysTraceLog(strMsg);
                                        bolCreateCommandFlag = true;
                                    }
                                }
                                else
                                    bolCreateCommandFlag = true;
                                #endregion StoreIn Loaction

                                if(bolCreateCommandFlag)
                                {
                                    #region Create StroreIn Command
                                    strCommandID = funGetCommandID();
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                                    if(funCreateAGVStoreInCommand(strCommandID, strLoaction, palletNo))
                                    {
                                        if(funLockStoreInLocation(strLoaction))
                                        {
                                            #region Create StroreIn Command & Lock StroreIn Location Success
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "Create StroreIn Command Success!";

                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "Lock StroreIn Location Success!";
                                            #endregion Create StroreIn Command & Lock StroreIn Location Success
                                        }
                                        else
                                        {
                                            #region Lock StroreIn Location Fail
                                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                            strMsg = strCommandID + "|";
                                            strMsg += strLoaction + "|";
                                            strMsg += palletNo + "|";
                                            strMsg += "Lock StroreIn Location Fail!";
                                            #endregion Lock StroreIn Location Fail
                                        }
                                    }
                                    else
                                    {
                                        #region Create AGV StoreIn Command Fail
                                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += palletNo + "|";
                                        strMsg += "Create AGV StoreIn Command Fail!";
                                        #endregion Create AGV StoreIn Command Fail
                                    }
                                    #endregion Create StroreIn Command
                                }
                            }
                            else if(intCommandCount == 1)
                            {
                                #region AGV StoreIn Command = 1
                                if(string.IsNullOrWhiteSpace(bufferData[0]._CommandID) &&
                                    string.IsNullOrWhiteSpace(bufferData[0]._Destination) &&
                                    bufferData[0]._Mode == Buffer.StnMode.None &&
                                    bufferData[0]._EQUStatus.AutoMode == Buffer.Signal.On)
                                {
                                    #region Write MPLC
                                    string[] strValues = new string[] { strCommandID, "1", "1" };
                                    if(InitSys._MPLC.funWriteMPLC(bufferData[0]._W_CmdSno, strValues))
                                    {
                                        #region Write MPLC Success
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += bufferData[0]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Write MPLC Success
                                    }
                                    else
                                    {
                                        #region Write MPLC Fail
                                        strMsg = strCommandID + "|";
                                        strMsg += strLoaction + "|";
                                        strMsg += bufferData[0]._W_CmdSno + "|";
                                        strMsg += string.Join(",", strValues) + "|";
                                        strMsg += "Write MPLC Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Write MPLC Fail
                                    }
                                    #endregion Write MPLC
                                }
                                else if(sMPLCData_2.StoreOut == 0)
                                {
                                    #region Write SPLC
                                    SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "1");
                                    if(InitSys._SPLC.funWriteSPLC(tag))
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
                                if(sMPLCData_2.StoreOut == 0)
                                {
                                    SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                                    if(InitSys._SPLC.funWriteSPLC(tag))
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
                            if(sMPLCData_2.StoreOut == 0)
                            {
                                strMsg = palletNo + "|";
                                strMsg += "StoreIn Request From AGV!";
                                funWriteSysTraceLog(strMsg);

                                SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                                if(InitSys._SPLC.funWriteSPLC(tag))
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
                        if(sMPLCData_2.StoreOut == 0)
                        {
                            strMsg = palletNo + "|";
                            strMsg += "StoreIn Request From AGV!";
                            funWriteSysTraceLog(strMsg);

                            SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, "2");
                            if(InitSys._SPLC.funWriteSPLC(tag))
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

        private void funAGVNeedStationRequest(string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                if(!string.IsNullOrWhiteSpace(palletNo) && sMPLCData_2.StoreOut == 0)
                {
                    strMsg = palletNo + "|";
                    strMsg += "AGV Need Station Request!";
                    funWriteSysTraceLog(strMsg);

                    strSQL = "SELECT * FROM ITEM_MST";
                    strSQL += " WHERE Plt_No='" + palletNo + "'";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                    {
                        string strPStation = dtCmdSno.Rows[0]["PStn_No"].ToString();
                        if(!string.IsNullOrWhiteSpace(strPStation))
                        {
                            if(strPStation != "0")
                            {
                                #region Write SPLC
                                SPLC.Tag tag = new SPLC.Tag(InitSys._AGV_StoreIn_SPLCAddress, strPStation);
                                if(InitSys._SPLC.funWriteSPLC(tag))
                                {
                                    #region Pallet StoreOut To AGV Finish
                                    strMsg = palletNo + "|";
                                    strMsg += tag._ItemName + "|";
                                    strMsg += tag._ItemValue + "|";
                                    strMsg += "Write SPLC Success!";
                                    funWriteSysTraceLog(strMsg);

                                    strMsg = palletNo + "|";
                                    strMsg = strPStation + "|";
                                    strMsg += "Reply AGV Station Request!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Pallet StoreOut To AGV Finish
                                }
                                else
                                {
                                    #region Write SPLC Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
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
    }
}
