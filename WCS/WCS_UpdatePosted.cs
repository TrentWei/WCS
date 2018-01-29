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
        private void funUpdatePosted()
        {
            funCommandFinish();
            funCommandCancel();
            funDeleteCommandToHistory();
        }

        private void funCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts='7'";
                strSQL += " ORDER BY Cmd_Sno";
                if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                {
                    for(int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                    {
                        Cmd_Mst cmd_Mst = new Cmd_Mst();
                        cmd_Mst.Cmd_Sno = dtCmdSno.Rows[intRow]["Cmd_Sno"].ToString();
                        cmd_Mst.Cmd_Mode = dtCmdSno.Rows[intRow]["Cmd_Mode"].ToString();
                        cmd_Mst.Cmd_Sts = dtCmdSno.Rows[intRow]["Cmd_Sts"].ToString();
                        cmd_Mst.Io_Type = dtCmdSno.Rows[intRow]["Io_Type"].ToString();
                        cmd_Mst.Loc = dtCmdSno.Rows[intRow]["Loc"].ToString();
                        cmd_Mst.New_Loc = dtCmdSno.Rows[intRow]["New_Loc"].ToString();
                        cmd_Mst.Height = dtCmdSno.Rows[intRow]["Height"].ToString();
                        cmd_Mst.Stn_No = dtCmdSno.Rows[intRow]["Stn_No"].ToString();
                        cmd_Mst.Plt_No = dtCmdSno.Rows[intRow]["Plt_No"].ToString();
                        cmd_Mst.Cyc_No = dtCmdSno.Rows[intRow]["Cyc_No"].ToString();
                        cmd_Mst.Crt_Dte = dtCmdSno.Rows[intRow]["Crt_Dte"].ToString();
                        cmd_Mst.Exp_Dte = dtCmdSno.Rows[intRow]["Exp_Dte"].ToString();
                        cmd_Mst.End_Dte = dtCmdSno.Rows[intRow]["End_Dte"].ToString();
                        cmd_Mst.Prty = dtCmdSno.Rows[intRow]["Prty"].ToString();
                        cmd_Mst.Trace = dtCmdSno.Rows[intRow]["Trace"].ToString();
                        cmd_Mst.User_Id = dtCmdSno.Rows[intRow]["User_Id"].ToString();
                        cmd_Mst.Remark = dtCmdSno.Rows[intRow]["Remark"].ToString();

                        funUpdateCMD(cmd_Mst, false);
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

        private void funCommandCancel()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts='6'";
                strSQL += " ORDER BY Cmd_Sno";
                if(InitSys._DB.funGetDT(strSQL, ref dtCmdSno, ref strEM))
                {
                    for(int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                    {
                        Cmd_Mst cmd_Mst = new Cmd_Mst();
                        cmd_Mst.Cmd_Sno = dtCmdSno.Rows[intRow]["Cmd_Sno"].ToString();
                        cmd_Mst.Cmd_Mode = dtCmdSno.Rows[intRow]["Cmd_Mode"].ToString();
                        cmd_Mst.Cmd_Sts = dtCmdSno.Rows[intRow]["Cmd_Sts"].ToString();
                        cmd_Mst.Io_Type = dtCmdSno.Rows[intRow]["Io_Type"].ToString();
                        cmd_Mst.Loc = dtCmdSno.Rows[intRow]["Loc"].ToString();
                        cmd_Mst.New_Loc = dtCmdSno.Rows[intRow]["New_Loc"].ToString();
                        cmd_Mst.Height = dtCmdSno.Rows[intRow]["Height"].ToString();
                        cmd_Mst.Stn_No = dtCmdSno.Rows[intRow]["Stn_No"].ToString();
                        cmd_Mst.Plt_No = dtCmdSno.Rows[intRow]["Plt_No"].ToString();
                        cmd_Mst.Cyc_No = dtCmdSno.Rows[intRow]["Cyc_No"].ToString();
                        cmd_Mst.Crt_Dte = dtCmdSno.Rows[intRow]["Crt_Dte"].ToString();
                        cmd_Mst.Exp_Dte = dtCmdSno.Rows[intRow]["Exp_Dte"].ToString();
                        cmd_Mst.End_Dte = dtCmdSno.Rows[intRow]["End_Dte"].ToString();
                        cmd_Mst.Prty = dtCmdSno.Rows[intRow]["Prty"].ToString();
                        cmd_Mst.Trace = dtCmdSno.Rows[intRow]["Trace"].ToString();
                        cmd_Mst.User_Id = dtCmdSno.Rows[intRow]["User_Id"].ToString();
                        cmd_Mst.Remark = dtCmdSno.Rows[intRow]["Remark"].ToString();

                        funUpdateCMD(cmd_Mst, true);
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

        private void funUpdateCMD(Cmd_Mst cmd_Mst, bool cancel)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                if(cancel)
                {
                    #region Cancel
                    if(cmd_Mst.Cmd_Mode == CMDMode.StoreIn)
                    {
                        #region StoreIn
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "N", string.Empty, true))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "N", string.Empty))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "I->N|";
                                    strMsg += "Update StoreIn Command Cancel Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update StoreIn Command Cancel Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "I->N|";
                                strMsg += "Update Location Cancel Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "I->N|";
                            strMsg += "Update Item Cancel Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion StoreIn
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.StoreOut)
                    {
                        #region StoreOut
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "O->N|";
                                    strMsg += "Update StoreOut Command Cancel Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update StoreOut Command Cancel Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "O->S|";
                                strMsg += "Update Location Cancel Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "O->S|";
                            strMsg += "Update Item Cancel Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion StoreOut
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.Picking)
                    {
                        #region Picking
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "<->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "C->S|";
                                    strMsg += "Update Picking Command Cancel Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "<->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update Picking Command Cancel Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "C->S|";
                                strMsg += "Update Location Cancel Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "C->S|";
                            strMsg += "Update Item Cancel Cancel Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion Picking
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.LoactionToLoaction)
                    {
                        #region LoactionToLoaction
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, false))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No) &&
                                funUpdateLocationMaster(cmd_Mst.New_Loc, "N", string.Empty))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "O->S,I->N|";
                                    strMsg += "Update LoactionToLoaction Command Cancel Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "6->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update LoactionToLoaction Command Cancel Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                strMsg += "O->S,I->N|";
                                strMsg += "Update Location Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                            strMsg += "Update Item Cancel Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion LoactionToLoaction
                    }
                    else
                    {
                        strMsg = cmd_Mst.Cmd_Sno + "|";
                        strMsg = cmd_Mst.Cmd_Mode + "|";
                        strMsg += "6->E|";
                        strMsg += cmd_Mst.Plt_No + "|";
                        strMsg += "Cancel Command Posted Error!";
                        funWriteUpdateLog(strMsg);

                        funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                    }
                    #endregion Cancel
                }
                else
                {
                    #region Finish
                    if(cmd_Mst.Cmd_Mode == CMDMode.StoreIn)
                    {
                        #region StoreIn
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "I->S|";
                                    strMsg += "Update StoreIn Command Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update StoreIn Command Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "I->S|";
                                strMsg += "Update Location Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "I->S|";
                            strMsg += "Update Item Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion StoreIn
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.StoreOut)
                    {
                        #region StoreOut
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "N", string.Empty, false))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "N", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "O->N|";
                                    strMsg += "Update StoreOut Command Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update StoreOut Command Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "O->N|";
                                strMsg += "Update Location Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "O->N|";
                            strMsg += "Update Item Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion StoreOut
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.Picking)
                    {
                        #region Picking
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "<->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "C->S|";
                                    strMsg += "Update Picking Command Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "<->" + cmd_Mst.Stn_No + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update Picking Command Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += "C->S|";
                                strMsg += "Update Location Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "|";
                            strMsg += "C->S|";
                            strMsg += "Update Item Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion Picking
                    }
                    else if(cmd_Mst.Cmd_Mode == CMDMode.LoactionToLoaction)
                    {
                        #region LoactionToLoaction
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                        if(funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.New_Loc, false))
                        {
                            if(funUpdateLocationMaster(cmd_Mst.Loc, "N", string.Empty) && funUpdateLocationMaster(cmd_Mst.New_Loc, "S", cmd_Mst.Plt_No))
                            {
                                if(funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                {
                                    #region Update Success
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "O->N,I->S|";
                                    strMsg += "Update StoreIn Command Success!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update Success
                                }
                                else
                                {
                                    #region Update Command Fail
                                    InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                    strMsg = cmd_Mst.Cmd_Sno + "|";
                                    strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                    strMsg += cmd_Mst.Trace + "|";
                                    strMsg += "7->9|";
                                    strMsg += cmd_Mst.Plt_No + "|";
                                    strMsg += "Update Command Fail!";
                                    funWriteUpdateLog(strMsg);
                                    #endregion Update LoactionToLoaction Command Fail

                                    funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                                }
                            }
                            else
                            {
                                #region Update Location Master Fail
                                InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                strMsg += "O->N,I->S|";
                                strMsg += "Update Location Fail!";
                                funWriteUpdateLog(strMsg);
                                #endregion Update Location Master Fail

                                funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                            }
                        }
                        else
                        {
                            #region Update Item Master Fail
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = cmd_Mst.Cmd_Sno + "|";
                            strMsg += cmd_Mst.Plt_No + "|";
                            strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                            strMsg += "Update Item Fail!";
                            funWriteUpdateLog(strMsg);
                            #endregion Update Item Master Fail

                            funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                        }
                        #endregion LoactionToLoaction
                    }
                    else
                    {
                        strMsg = cmd_Mst.Cmd_Sno + "|";
                        strMsg = cmd_Mst.Cmd_Mode + "|";
                        strMsg += "7->E|";
                        strMsg += cmd_Mst.Plt_No + "|";
                        strMsg += "Command Posted Error!";
                        funWriteUpdateLog(strMsg);

                        funUpdateCommand(cmd_Mst.Cmd_Sno, "E", cmd_Mst.Trace, cmd_Mst.Plt_No);
                    }
                    #endregion Finish
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funDeleteCommandToHistory()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                if(DateTime.Now.ToString("HH:mm") == "00:00")
                {
                    strSQL = "INSERT INTO CMD_MST_HIS";
                    strSQL += " SELECT * FROM CMD_MST";
                    strSQL += " WHERE Cmd_Sts='9'";
                    strSQL += " AND SUBSTRING(End_Dte,1,10)='" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + "'";
                    InitSys._DB.funCommitCtrl(DB.TransactionType.Begin);
                    if(InitSys._DB.funExecSql(strSQL, ref strEM))
                    {
                        strSQL = " DELETE CMD_MST";
                        strSQL += " WHERE Cmd_Sts='9'";
                        strSQL += " AND SUBSTRING(End_Dte,1,10)='" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + "'";
                        if(InitSys._DB.funExecSql(strSQL, ref strEM))
                        {
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Commit);
                            strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                            strMsg += "Delete Command To History Success!";
                            funWriteUpdateLog(strMsg);
                        }
                        else
                        {
                            InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                            strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                            strMsg += "DELETE CMD_MST Fail!";
                            funWriteUpdateLog(strMsg);
                        }
                    }
                    else
                    {
                        InitSys._DB.funCommitCtrl(DB.TransactionType.Rollback);
                        strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                        strMsg += "Insert CMD_MST To CMD_MST_HIS Fail!";
                        funWriteUpdateLog(strMsg);
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
