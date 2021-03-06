﻿using System;
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
            funUpdateWeight();
            funCommandFinish();
            funCommandCancel();
            funDeleteCommandToHistory();
        }

        private void funUpdateWeight()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            string strCmdSno = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "select * from code where code_type='WeightERRORValue' or code_type='ThresholdWegiht' order by CODE_NAME desc";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    inWeightERRORValue = int.Parse(dtCmdSno.Rows[0]["CODE_NAME"].ToString());
                    inThresholdWegiht = int.Parse(dtCmdSno.Rows[1]["CODE_NAME"].ToString());
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

        private void funCommandFinish()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            string strSubNo = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts in('3','4','5')";
                strSQL += " AND CMD_MODE='5' ";
                strSQL += " ORDER BY Cmd_Sno";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    for (int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                    {
                        Cmd_Mst cmd_Mst = new Cmd_Mst();
                        cmd_Mst.Cmd_Sno = dtCmdSno.Rows[intRow]["Cmd_Sno"].ToString();
                        cmd_Mst.Cmd_Mode = dtCmdSno.Rows[intRow]["Cmd_Mode"].ToString();
                        cmd_Mst.Cmd_Sts = dtCmdSno.Rows[intRow]["Cmd_Sts"].ToString();
                        cmd_Mst.Io_Type = dtCmdSno.Rows[intRow]["Io_Type"].ToString();
                        cmd_Mst.Loc = dtCmdSno.Rows[intRow]["Loc"].ToString();
                        cmd_Mst.New_Loc = dtCmdSno.Rows[intRow]["New_Loc"].ToString();
                        cmd_Mst.Loc_Size = dtCmdSno.Rows[intRow]["LOC_SIZE"].ToString();
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


                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                        {
                            try
                            {
                                if (!funUpdateCommand(cmd_Mst.Cmd_Sno, "8", cmd_Mst.Trace, cmd_Mst.Plt_No)) throw new Exception("命令完成失败！");
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Cyc_No + "|";
                                strMsg += cmd_Mst.Cmd_Mode + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += cmd_Mst.New_Loc + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += "8：命令更新成功";
                                funWriteSysTraceLog(strMsg);
                                if (cmd_Mst.Io_Type == IO_TYPE.LoactionToLoaction)
                                {
                                    strSQL = " SELECT * FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO";
                                    strSQL += " WHERE P.PLT_NO='" + cmd_Mst.Plt_No + "'";
                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        strSubNo = dtCmdSno.Rows[intRow]["SUB_NO"].ToString();
                                    }
                                    if (!funUpdateLocationMaster(cmd_Mst.New_Loc, "S", cmd_Mst.Plt_No)) throw new Exception("新储位跟新失败！");

                                }
                                else
                                {
                                    if (!funUpdateLocationMaster(cmd_Mst.New_Loc, "E", cmd_Mst.Plt_No)) throw new Exception("新储位跟新失败！");
                                }
                                if (!funUpdateLocationMaster(cmd_Mst.Loc, "N", string.Empty)) throw new Exception("原储位跟新失败！");
                                if (cmd_Mst.Io_Type == IO_TYPE.LoactionToLoaction)
                                {
                                    if (!funLockStoreInBox(strSubNo, LoactionState.S, cmd_Mst.New_Loc)) throw new Exception("子托盘跟新失败！");
                                    if (!funUpdateLocationDtl(cmd_Mst.Loc, cmd_Mst.New_Loc)) throw new Exception("储位明细跟新失败！");
                                }
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                            }
                            catch (Exception ex)
                            {
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Cyc_No + "|";
                                strMsg += cmd_Mst.Cmd_Mode + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += cmd_Mst.New_Loc + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += ex.ToString();
                                funWriteSysTraceLog(strMsg);
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

        private void funCommandCancel()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            string strSubNo = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Cmd_Sts in ('6','7')";
                strSQL += " AND CMD_MODE='5'";
                strSQL += " ORDER BY Cmd_Sno";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    for (int intRow = 0; intRow < dtCmdSno.Rows.Count; intRow++)
                    {
                        Cmd_Mst cmd_Mst = new Cmd_Mst();
                        cmd_Mst.Cmd_Sno = dtCmdSno.Rows[intRow]["Cmd_Sno"].ToString();
                        cmd_Mst.Cmd_Mode = dtCmdSno.Rows[intRow]["Cmd_Mode"].ToString();
                        cmd_Mst.Cmd_Sts = dtCmdSno.Rows[intRow]["Cmd_Sts"].ToString();
                        cmd_Mst.Io_Type = dtCmdSno.Rows[intRow]["Io_Type"].ToString();
                        cmd_Mst.Loc = dtCmdSno.Rows[intRow]["Loc"].ToString();
                        cmd_Mst.New_Loc = dtCmdSno.Rows[intRow]["New_Loc"].ToString();
                        cmd_Mst.Loc_Size = dtCmdSno.Rows[intRow]["LOC_SIZE"].ToString();
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

                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                        {

                            try
                            {
                                if (!funUpdateCommand(cmd_Mst.Cmd_Sno, "B", cmd_Mst.Trace, cmd_Mst.Plt_No)) throw new Exception("命令取消失败！");
                                if (cmd_Mst.Io_Type == IO_TYPE.LoactionToLoaction)
                                {
                                    strSQL = " SELECT * FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO";
                                    strSQL = " WHERE P.PLT_NO='" + cmd_Mst.Plt_No + "'";
                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        strSubNo = dtCmdSno.Rows[intRow]["SUB_NO"].ToString();
                                    }
                                    if (!funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No)) throw new Exception("原储位还原失败！");
                                }
                                else
                                {
                                    if (!funUpdateLocationMaster(cmd_Mst.Loc, "E", cmd_Mst.Plt_No)) throw new Exception("原储位还原失败！");
                                }

                                if (!funUpdateLocationMaster(cmd_Mst.New_Loc, "N", string.Empty)) throw new Exception("新储位还原失败！");
                                if (cmd_Mst.Io_Type == IO_TYPE.LoactionToLoaction)
                                {
                                    if (!funLockStoreInBox(strSubNo, LoactionState.S)) throw new Exception("子托盘还原失败！");
                                }
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                            }
                            catch (Exception ex)
                            {
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                strMsg = cmd_Mst.Cmd_Sno + "|";
                                strMsg += cmd_Mst.Cyc_No + "|";
                                strMsg += cmd_Mst.Cmd_Mode + "|";
                                strMsg += cmd_Mst.Loc + "|";
                                strMsg += cmd_Mst.New_Loc + "|";
                                strMsg += cmd_Mst.Plt_No + "|";
                                strMsg += ex.ToString();
                                funWriteSysTraceLog(strMsg);
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
        private void funUpdateCMD(Cmd_Mst cmd_Mst, bool cancel)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            string strType = string.Empty;
            try
            {
                if (funQITEMTYPE(cmd_Mst.Loc, ref strType))
                {
                    if (cancel)
                    {
                        #region Cancel
                        if (cmd_Mst.Cmd_Mode == CMDMode.StoreIn)
                        {
                            #region StoreIn
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "N", string.Empty, true, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "N", string.Empty))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "D", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
                                        strMsg += cmd_Mst.Plt_No + "|";
                                        strMsg += "I->N|";
                                        strMsg += "Update StoreIn Command Cancel Success!";
                                        funWriteUpdateLog(strMsg);
                                        #endregion Update Success
                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Stn_No + "->" + cmd_Mst.Loc + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.StoreOut)
                        {
                            #region StoreOut
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "D", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
                                        strMsg += cmd_Mst.Plt_No + "|";
                                        strMsg += "O->N|";
                                        strMsg += "Update StoreOut Command Cancel Success!";
                                        funWriteUpdateLog(strMsg);
                                        #endregion Update Success
                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Loc + "->" + cmd_Mst.Stn_No + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.Picking)
                        {
                            #region Picking
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "D", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.LoactionToLoaction)
                        {
                            #region LoactionToLoaction
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, false, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No) &&
                                    funUpdateLocationMaster(cmd_Mst.New_Loc, "N", string.Empty))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "D", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
                                        strMsg += cmd_Mst.Plt_No + "|";
                                        strMsg += "O->S,I->N|";
                                        strMsg += "Update LoactionToLoaction Command Cancel Success!";
                                        funWriteUpdateLog(strMsg);
                                        #endregion Update Success
                                    }
                                    else
                                    {
                                        #region Update Command Fail
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = cmd_Mst.Cmd_Sno + "|";
                                        strMsg += cmd_Mst.Loc + "->" + cmd_Mst.New_Loc + "|";
                                        strMsg += cmd_Mst.Trace + "|";
                                        strMsg += "6->D|";
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                            strMsg += "6->D|";
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
                        if (cmd_Mst.Cmd_Mode == CMDMode.StoreIn)
                        {
                            #region StoreIn
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.StoreOut)
                        {
                            #region StoreOut
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "N", string.Empty, false, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "N", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.Picking)
                        {
                            #region Picking
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.Loc, true, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "S", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                        else if (cmd_Mst.Cmd_Mode == CMDMode.LoactionToLoaction)
                        {
                            #region LoactionToLoaction
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateItemMaster(cmd_Mst.Plt_No, "S", cmd_Mst.New_Loc, false, strType))
                            {
                                if (funUpdateLocationMaster(cmd_Mst.Loc, "N", string.Empty) && funUpdateLocationMaster(cmd_Mst.New_Loc, "S", cmd_Mst.Plt_No))
                                {
                                    if (funUpdateCommand(cmd_Mst.Cmd_Sno, "9", cmd_Mst.Trace, cmd_Mst.Plt_No))
                                    {
                                        #region Update Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
            }
            catch (Exception ex)
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
            string strCmdSno = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                strSQL = "SELECT * FROM CMD_MST WHERE (CMD_MODE in ('2','3') AND ((CMD_STS like 'P%') OR (CMD_STS in ('B','C','E')) ) OR (CMD_MODE in ('1','5') AND CMD_STS in ('8','9','A','B','C','E'))) OR (CMD_MODE in ('2','1','3') and CMD_STS in ('8','9','A','B','C','E') and LOC like '00%')";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    for (int i = 0; i < dtCmdSno.Rows.Count; i++)
                    {
                        strCmdSno = dtCmdSno.Rows[i]["CMD_SNO"].ToString();
                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin))
                        {
                            try
                            {
                                strSQL = "INSERT INTO CMD_MST_log";
                                strSQL += " SELECT * FROM CMD_MST";
                                strSQL += " WHERE CMD_SNO='" + strCmdSno + "'";
                                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                {
                                    strSQL = " DELETE CMD_MST";
                                    strSQL += " WHERE CMD_SNO='" + strCmdSno + "'";
                                    if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                    {
                                        strSQL = "SELECT * FROM CMD_DTL WHERE CMD_SNO='" + strCmdSno + "'";
                                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                        {
                                            strSQL = "INSERT INTO CMD_DTL_log";
                                            strSQL += " SELECT * FROM CMD_DTL";
                                            strSQL += " WHERE CMD_SNO='" + strCmdSno + "'";
                                            if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                            {
                                                strSQL = " DELETE CMD_DTL";
                                                strSQL += " WHERE CMD_SNO='" + strCmdSno + "'";
                                                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                                {
                                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                    {
                                                        strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                                        strMsg += "Delete Command To Log Success!";
                                                        strMsg += "Delete CMD_DTL To Log Success!";
                                                        funWriteUpdateLog(strMsg);
                                                    }
                                                }
                                                else
                                                {
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                                    strMsg += "Delete CMD_DTL To Log Fail!";
                                                    funWriteUpdateLog(strMsg);
                                                }
                                            }
                                            else
                                            {
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                                strMsg += "Insert CMD_DTL To CMD_DTL_LOG Fail!";
                                                funWriteUpdateLog(strMsg);
                                            }
                                        }
                                        else
                                        {
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                            {
                                                strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                                strMsg += "Delete Command To History Success!";
                                                funWriteUpdateLog(strMsg);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                        strMsg += "DELETE CMD_MST Fail!";
                                        funWriteUpdateLog(strMsg);
                                    }
                                }
                                else
                                {
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = DateTime.Now.ToString("yyyy-MM-dd") + "|";
                                    strMsg += "Insert CMD_MST To CMD_MST_HIS Fail!";
                                    funWriteUpdateLog(strMsg);
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
