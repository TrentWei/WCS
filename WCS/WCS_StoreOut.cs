using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static Mirle.ASRS.Buffer;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private void funStoreOut()
        {
            funStoreOutOrCycIn();
            funStoreOut_GetStoreOutCommandAndWritePLC();
            funStoreOut_CrateCraneCommand();
            funStoreOut_CraneCommandFinish();
        }


        private void funStoreOutOrCycIn()
        {
            string strSQL = string.Empty;
            string strOutStntionName = string.Empty;
            string strSubNo = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOutIn)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    #region 出库待过账
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                          (bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreOut || bufferData[intBufferIndex]._Mode == Buffer.StnMode.Picking) &&
                          bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                          bufferData[intBufferIndex]._EQUStatus.RearLocation == Buffer.Signal.On &&
                          bufferData[intBufferIndex]._BufferName != STN_NO.StoreInA58 &&
                          bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID;
                        strSQL = "SELECT C.*,L.EQU_NO，P.SUB_NO FROM CMD_MST C JOIN LOC_MST L ON C.LOC=L.LOC JOIN PLT_MST P ON C.PLT_NO=P.PLT_NO";
                        strSQL += " WHERE C.Cmd_Sts='2'";
                        strSQL += " AND C.TRACE='0'";
                        strSQL += " AND C.Cmd_Mode IN('2', '3')";
                        strSQL += " AND L.LOC_STS='O'";
                        strSQL += " AND C.CMD_SNO='" + strCommandID + "'";
                        strSQL += " ORDER BY Prty, Crt_Dte,Cmd_Sno,C.LOC";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
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
                            commandInfo.LOC_SIZE = dtCmdSno.Rows[0]["LOC_SIZE"].ToString();
                            strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                            if (funUpdateCommand(strCommandID, CommandState.CompletedWaitPost, Trace.StoreOut_CraneCommandFinish))
                            {
                                string[] strValues = new string[] { commandInfo.PalletNo.Remove(0, 1) };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_PalletNo, strValues);

                                if (commandInfo.LOC_SIZE == "L") strValues = new string[] { "0" };
                                if (commandInfo.LOC_SIZE == "H") strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_LoactionSize, strValues);

                                funSetKanbanInfo(KanbanModel.OUT, strBufferName, strCommandID, strSubNo, string.Empty, intBufferIndex);
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
                                strMsg += CommandState.CompletedWaitPost + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreOut_CraneCommandFinish + "|";
                                strMsg += "StoreOut Command Update Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command Fail
                            }
                        }
                    }
                    #endregion
                    #region 出库回库

                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                      !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._PalletNo) &&
                      bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._EQUStatus.CycIn == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._BufferName != STN_NO.StoreInA58 &&
                      bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strLoactionSize = bufferData[intBufferIndex]._LoactionSize ? "L" : "H";
                        string strPltNo = "X" + bufferData[intBufferIndex]._PalletNo;
                        #region Read OK
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Plt_No='" + strPltNo + "'";
                        strSQL += " AND CMD_STS='0'";
                        strSQL += " AND CMD_MODE='1'";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                        {
                            #region 使用原有命令
                            if (dtCmdSno.Rows.Count == 1)
                            {
                                Cmd_Mst CmdMst = new Cmd_Mst();
                                CmdMst.Cmd_Sno = dtCmdSno.Rows[0]["CMD_SNO"].ToString();
                                CmdMst.Cmd_Mode = dtCmdSno.Rows[0]["CMD_MODE"].ToString();
                                CmdMst.Stn_No = dtCmdSno.Rows[0]["STN_NO"].ToString();

                                string[] strValues = new string[] { CmdMst.Cmd_Sno, CMDMode.StoreIn, CmdMst.Stn_No.Remove(0, 1) };
                                if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                {
                                    strMsg = strBufferName + "|";
                                    strMsg += strPltNo + "|";
                                    strMsg += CmdMst.Cmd_Sno + "|";
                                    strMsg += "|";
                                    strMsg += "StoreIn Command And WritePLC Success";
                                    funWriteSysTraceLog(strMsg);

                                    strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Discharged, strValues);

                                    funSetKanbanInfo(KanbanModel.IN, strBufferName, CmdMst.Cmd_Sno, strSubNo, string.Empty, intBufferIndex);
                                }
                            }
                            else
                            {
                                strMsg = strBufferName + "|";
                                strMsg += strPltNo + "|";
                                strMsg += "存在多条未执行命令！";
                                funWriteSysTraceLog(strMsg);

                                string[] strValues = new string[] { "4" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "存在多条未执行命令！", intBufferIndex);
                            }
                            #endregion
                        }
                        else
                        {
                            string strLoaction = string.Empty;
                            string strCrnNo = string.Empty;
                            string strStnNo = string.Empty;
                            if (funGetCraneNo(ref strCrnNo, ref strStnNo))
                            {

                                strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                                strSQL += " WHERE B.STATUS IN ('0','W') AND LOC=' ' AND P.PLT_NO='" + strPltNo + "'";
                                strSQL += " GROUP BY P.SUB_NO  ";

                                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                {
                                    #region 产生新的入库命令
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                    try
                                    {

                                        string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                        strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                        string strCommandID = funGetCommandID();
                                        if (funGetEmptyLocation(strLoactionSize, strCrnNo, ref strLoaction))
                                        {
                                            if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strPltNo, strStnNo, strActualWeight, strLoactionSize, " "))
                                            {
                                                if (funUpdateLocationMaster(strLoaction, LoactionState.IN, strPltNo))
                                                {
                                                    if (funLockStoreInBox(strSubNo, LoactionState.IN))
                                                    {
                                                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                                        {
                                                            strMsg = strBufferName + "|";
                                                            strMsg += strPltNo + "|";
                                                            strMsg += strCommandID + "|";
                                                            strMsg += "|";
                                                            strMsg += "StoreIn Command And WritePLC Success";
                                                            funWriteSysTraceLog(strMsg);


                                                        }
                                                        else
                                                        {
                                                            #region StoreInAndTransactionCommit Fail
                                                            InitSys._MPLC.funClearMPLC(bufferData[intBufferIndex]._W_CmdSno);
                                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                            strMsg = strBufferName + "|";
                                                            strMsg += strPltNo + "|";
                                                            strMsg += strCommandID + "|";
                                                            strMsg += "|";
                                                            strMsg += "StoreIn And TransactionCommit Fail!";
                                                            funWriteSysTraceLog(strMsg);

                                                            string[] strValues = new string[] { "1" };
                                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                            funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "命令提交失败！", intBufferIndex);

                                                            #endregion StoreInAndTransactionCommit Fail
                                                        }
                                                    }
                                                    else
                                                    {
                                                        #region Update StoreIn BOX_ID Fail
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        strMsg = strBufferName + "|";
                                                        strMsg += strPltNo + "|";
                                                        strMsg += strCommandID + "|";
                                                        strMsg += "|";
                                                        strMsg += "Update StoreIn BOX_ID Fail!";
                                                        funWriteSysTraceLog(strMsg);

                                                        string[] strValues = new string[] { "1" };
                                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                        funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "更新子托盘状态失败！", intBufferIndex);

                                                        #endregion Update StoreIn PalletNo Fail
                                                    }
                                                }
                                                else
                                                {
                                                    strMsg = strBufferName + "|";
                                                    strMsg += strPltNo + "|";
                                                    strMsg += "|";
                                                    strMsg += "储位更新失败！";
                                                    funWriteSysTraceLog(strMsg);


                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    string[] strValues = new string[] { "2" };
                                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                    funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "获取储位失败！", intBufferIndex);

                                                }
                                            }
                                            else
                                            {
                                                #region Insert Command Fail

                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = strCommandID + "|";
                                                strMsg += strLoaction + "|";
                                                strMsg += strPltNo + "|";
                                                strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                                strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                                strMsg += "StroreIn Command Insert Fail!";
                                                funWriteSysTraceLog(strMsg);

                                                string[] strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "新增命令失败！", intBufferIndex);

                                                #endregion Insert Command Fail
                                            }
                                        }
                                        else
                                        {
                                            strMsg = strBufferName + "|";
                                            strMsg += strPltNo + "|";
                                            strMsg += "|";
                                            strMsg += "储位获取失败！";
                                            funWriteSysTraceLog(strMsg);
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            string[] strValues = new string[] { "2" };
                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += strPltNo + "|";
                                        strMsg += "|";
                                        strMsg += ex.ToString();
                                        funWriteSysTraceLog(strMsg);

                                        string[] strValues = new string[] { "1" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);

                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Can't Find StroreIn PLT_MST&BOX
                                    strMsg = bufferData[intBufferIndex]._BufferName + "|";
                                    strMsg += bufferData[intBufferIndex]._PalletNo + "|";
                                    strMsg += "Can't Find StroreIn PLT_MST&BOX!";
                                    funWriteSysTraceLog(strMsg);

                                    string[] strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                    funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "未查询到托盘讯息！", intBufferIndex);
                                    #endregion Can't Find StroreIn PLT_MST&BOX
                                }
                            }

                        }
                        #endregion Read OK
                    }

                    #endregion

                    #region 异常口排出过账
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                          bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreIn &&
                          bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                          bufferData[intBufferIndex]._EQUStatus.RearLocation == Buffer.Signal.On &&
                          bufferData[intBufferIndex]._BufferName == STN_NO.StoreInA58 &&
                          bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        strSQL = " SELECT C.*,P.PLT_NO,B.SUB_NO FROM CMD_MST C JOIN PLT_MST P ON C.PLT_NO=P.PLT_NO JOIN BOX B ON P.SUB_NO=B.SUB_NO ";
                        strSQL += " WHERE C.CMD_SNO='" + bufferData[intBufferIndex]._CommandID.PadLeft(5, '0') + "'";
                        strSQL += " AND B.STATUS='I'";
                        strSQL += " AND C.CMD_STS='0'";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                        {
                            CommandInfo commandInfo = new CommandInfo();
                            commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                            commandInfo.PalletNo = dtCmdSno.Rows[0]["PLT_NO"].ToString();
                            commandInfo.LOC_SIZE = dtCmdSno.Rows[0]["LOC_SIZE"].ToString();
                            strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                            if (bufferData[intBufferIndex]._ReturnRequest == "1")
                            {
                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "条码资料异常！", intBufferIndex);
                            }
                            if (bufferData[intBufferIndex]._ReturnRequest == "2")
                            {
                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "储位相关异常！", intBufferIndex);
                            }
                            if (bufferData[intBufferIndex]._ReturnRequest == "3")
                            {
                                funSetKanbanInfo(KanbanModel.OVERWEIGHT, strBufferName, commandInfo.CommandID, strSubNo, string.Empty, intBufferIndex);
                            }
                            #region 删除已存在命令
                            strMsg = strBufferName + "|";
                            strMsg += commandInfo.PalletNo + "|";
                            strMsg += "存在未执行命令！";
                            funWriteSysTraceLog(strMsg);
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            try
                            {
                                strSQL = "INSERT INTO CMD_MST_HIS SELECT * FROM CMD_MST WHERE CMD_SNO='" + commandInfo.CommandID + "'";
                                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                {

                                    strMsg = strBufferName + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += "成功备份未执行命令！";
                                    funWriteSysTraceLog(strMsg);
                                    strSQL = "DELETE FROM CMD_MST WHERE  PLT_NO='" + commandInfo.PalletNo + "'";
                                    if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                                    {
                                        if (funLockStoreInBox(strSubNo, LoactionState.W))
                                        {
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                            {

                                                string[] strValues = new string[] { commandInfo.PalletNo.Remove(0, 1) };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_PalletNo, strValues);

                                                if (commandInfo.LOC_SIZE == "L") strValues = new string[] { "0" };
                                                if (commandInfo.LOC_SIZE == "H") strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_LoactionSize, strValues);

                                                strMsg = strBufferName + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "成功删除未完成命令！";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                        }
                                        else
                                        {
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                            {
                                                strMsg = strBufferName + "|";
                                                strMsg += commandInfo.PalletNo + "|";
                                                strMsg += "还原产品状态失败！";
                                                funWriteSysTraceLog(strMsg);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                        {
                                            strMsg = strBufferName + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += "删除未完成命令失败！";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                    }
                                }
                                else
                                {
                                    if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                    {
                                        strMsg = strBufferName + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += "备份未执行命令失败！|";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback))
                                {
                                    strMsg = strBufferName + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += "备份删除未执行命令时发生异常！|";
                                    strMsg += ex.ToString();
                                    funWriteSysTraceLog(strMsg);
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region  异常口回库
                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                      !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._PalletNo) &&
                      bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._EQUStatus.CycIn == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._EQUStatus.FrontLocation == Buffer.Signal.On &&
                      bufferData[intBufferIndex]._BufferName == STN_NO.StoreInA58 &&
                      bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strPltNo = "X" + bufferData[intBufferIndex]._PalletNo;
                        strSQL = " SELECT P.SUB_NO,SUM(LOTATT12)AS INWEIGHT  FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO   ";
                        strSQL += " WHERE B.STATUS IN ('0','W') AND Nvl(LOC,' ')=' ' AND P.PLT_NO='" + strPltNo + "'";
                        strSQL += " GROUP BY P.SUB_NO  ";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                        {
                            #region 产生新的入库命令
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            try
                            {
                                string strLoaction = "";

                                string strActualWeight = dtCmdSno.Rows[0]["INWEIGHT"].ToString();
                                strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                                string strCommandID = funGetCommandID();

                                if (funCreateStoreInCommand(strCommandID, CMDMode.StoreIn, IO_TYPE.StoreIn11, strLoaction, strPltNo, strBufferName, strActualWeight, " ", " "))
                                {
                                    if (funLockStoreInBox(strSubNo, LoactionState.IN))
                                    {
                                        string[] strValues = new string[] { strCommandID, CMDMode.StoreIn, "83" };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                        {
                                            if (InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit))
                                            {
                                                strMsg = strBufferName + "|";
                                                strMsg += strPltNo + "|";
                                                strMsg += strCommandID + "|";
                                                strMsg += "|";
                                                strMsg += "StoreIn Command And WritePLC Success";
                                                funWriteSysTraceLog(strMsg);

                                                funSetKanbanInfo(KanbanModel.IN, strBufferName, strCommandID, strSubNo, string.Empty, intBufferIndex);

                                            }
                                            else
                                            {
                                                #region StoreInAndTransactionCommit Fail
                                                InitSys._MPLC.funClearMPLC(bufferData[intBufferIndex]._W_CmdSno);
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                strMsg = strBufferName + "|";
                                                strMsg += strPltNo + "|";
                                                strMsg += strCommandID + "|";
                                                strMsg += "|";
                                                strMsg += "StoreIn And TransactionCommit Fail!";
                                                funWriteSysTraceLog(strMsg);
                                                #endregion StoreInAndTransactionCommit Fail

                                                strValues = new string[] { "1" };
                                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                                funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "事务提交失败！", intBufferIndex);

                                            }
                                        }
                                        else
                                        {
                                            #region StoreInAndWriteMPLC Fail
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = strBufferName + "|";
                                            strMsg += strPltNo + "|";
                                            strMsg += strCommandID + "|";
                                            strMsg += "|";
                                            strMsg += "StoreInAndWriteMPLC Fail!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion StoreInAndWriteMPLC Fail

                                            strValues = new string[] { "1" };
                                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                            funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "命令写入PLC失败！", intBufferIndex);

                                        }
                                    }
                                    else
                                    {
                                        #region Update StoreIn BOX_ID Fail
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = strBufferName + "|";
                                        strMsg += strPltNo + "|";
                                        strMsg += strCommandID + "|";
                                        strMsg += "|";
                                        strMsg += "Update StoreIn BOX_ID Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update StoreIn PalletNo Fail

                                        string[] strValues = new string[] { "2" };
                                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                        funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "子托盘跟新失败！", intBufferIndex);

                                    }
                                }
                                else
                                {
                                    #region Insert Command Fail
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += strLoaction + "|";
                                    strMsg += strPltNo + "|";
                                    strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                    strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                    strMsg += "StroreIn Command Insert Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Insert Command Fail

                                    string[] strValues = new string[] { "1" };
                                    InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                                    funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "命令产生失败！", intBufferIndex);

                                }
                            }
                            catch (Exception ex)
                            {
                                strMsg = strBufferName + "|";
                                strMsg += strPltNo + "|";
                                strMsg += "|";
                                strMsg += "产生命令入库出现异常！";
                                funWriteSysTraceLog(strMsg);
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                            }
                            #endregion
                        }
                        else
                        {
                            #region Can't Find StroreIn PLT_MST&BOX
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += strPltNo + "|";
                            strMsg += "Can't Find StroreIn PLT_MST&BOX!";
                            funWriteSysTraceLog(strMsg);
                            #endregion

                            string[] strValues = new string[] { "1" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_ReturnRequest, strValues);
                            funSetKanbanInfo(KanbanModel.ERROR, strBufferName, string.Empty, string.Empty, "条码无资料！", intBufferIndex);
                        }
                    }
                    #endregion
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
        private void funStoreOut_GetStoreOutCommandAndWritePLC()
        {
            string strSQL = string.Empty;
            string strOutStntionName = string.Empty;
            string strNewLoc = string.Empty;
            int strCrn_No = 0;
            string strSubNo = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    funGetCraneNo(strBufferName, ref strCrn_No);
                    strSQL = "SELECT C.*,L.EQU_NO，P.SUB_NO FROM CMD_MST C JOIN LOC_MST L ON C.LOC=L.LOC JOIN PLT_MST P ON C.PLT_NO=P.PLT_NO";
                    strSQL += " WHERE C.Cmd_Sts='0'";
                    strSQL += " AND C.TRACE='0'";
                    strSQL += " AND C.Cmd_Mode IN('2', '3')";
                    strSQL += " AND L.LOC_STS='O'";
                    strSQL += " AND L.EQU_NO='" + strCrn_No + "'";
                    strSQL += " ORDER BY Prty, Crt_Dte,Cmd_Sno,C.LOC";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
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
                        commandInfo.LOC_SIZE = dtCmdSno.Rows[0]["LOC_SIZE"].ToString();
                        strSubNo = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                        strOutStntionName = funGetStationNo(commandInfo.Loaction);
                        if (!funIsInnerLoc(commandInfo.Loaction))
                        {
                            string strInLoc = funGetInnerLoc(commandInfo.Loaction);
                            strSQL = "SELECT * FROM LOC_MST WHERE LOC_STS IN ('S','E','O') AND LOC='" + strInLoc + "' ";
                            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                            {
                                string strLocSts = dtCmdSno.Rows[0]["LOC_STS"].ToString();
                                if (strLocSts == "O")
                                {
                                    #region 如果外储位对应的内储位,状态为出库预约,且命令并未执行则先出内储位货物
                                    strSQL = " SELECT * FROM CMD_MST WHERE LOC='" + strInLoc + "' ";
                                    strSQL += " AND CMD_MODE='2' AND CMD_STS='0'";
                                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                                    {
                                        commandInfo = new CommandInfo();
                                        commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                                        commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                                        commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                                        commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                                        commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                                        commandInfo.Loaction = dtCmdSno.Rows[0]["Loc"].ToString();
                                        commandInfo.StationNo = dtCmdSno.Rows[0]["Stn_No"].ToString();
                                        commandInfo.Priority = dtCmdSno.Rows[0]["Prty"].ToString();
                                        #region 内储位直接出货
                                        if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                                            string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                                            strBufferName == strOutStntionName &&
                                            bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                                            bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                                            bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                                        {
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += "StoreOut Command Initiated!";
                                            funWriteSysTraceLog(strMsg);


                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                            try
                                            {
                                                if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_GetStoreOutCommandAndWritePLC))
                                                {
                                                    string[] strValues = new string[] { commandInfo.CommandID, commandInfo.CommandMode.ToString(), commandInfo.StationNo.Remove(0, 1) };
                                                    if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                                    {
                                                        #region Update Command & Write MPLC Success
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                                        strMsg = commandInfo.CommandID + "|";
                                                        strMsg += commandInfo.CycleNo + "|";
                                                        strMsg += commandInfo.CommandMode + "|";
                                                        strMsg += commandInfo.Loaction + "|";
                                                        strMsg += commandInfo.StationNo + "|";
                                                        strMsg += commandInfo.PalletNo + "|";
                                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                                        strMsg += "StoreOut Command Update Success!";
                                                        funWriteSysTraceLog(strMsg);

                                                        strMsg = commandInfo.CommandID + "|";
                                                        strMsg += commandInfo.CycleNo + "|";
                                                        strMsg += commandInfo.CommandMode + "|";
                                                        strMsg += commandInfo.Loaction + "|";
                                                        strMsg += commandInfo.StationNo + "|";
                                                        strMsg += commandInfo.PalletNo + "|";
                                                        strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                                        strMsg += string.Join(",", strValues) + "|";
                                                        strMsg += "Write MPLC Success!";
                                                        funWriteSysTraceLog(strMsg);
                                                        #endregion Update Command & Write MPLC Success
                                                    }
                                                    else
                                                    {
                                                        #region Update Command Success But Write MPLC Fail
                                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                        strMsg = commandInfo.CommandID + "|";
                                                        strMsg += commandInfo.CycleNo + "|";
                                                        strMsg += commandInfo.CommandMode + "|";
                                                        strMsg += commandInfo.Loaction + "|";
                                                        strMsg += commandInfo.StationNo + "|";
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
                                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                                    strMsg = commandInfo.CommandID + "|";
                                                    strMsg += commandInfo.CycleNo + "|";
                                                    strMsg += commandInfo.CommandMode + "|";
                                                    strMsg += commandInfo.Loaction + "|";
                                                    strMsg += commandInfo.StationNo + "|";
                                                    strMsg += commandInfo.PalletNo + "|";
                                                    strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                                    strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                                    strMsg += "StoreOut Command Update Fail!";
                                                    funWriteSysTraceLog(strMsg);
                                                    #endregion Update Command Fail
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += "外储位对应的内储位,状态为出库预约,且命令并未执行则先出内储位货物时,未获取到命令.";
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 如果外储位对应的内储位,状态为库存储位,则下达该储位库对库命令
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                    try
                                    {
                                        string strCommandID = funGetCommandID();
                                        string strIotype = string.Empty;
                                        if (funGetEmptyLocationToLocation(commandInfo.LOC_SIZE, commandInfo.Loaction, ref strNewLoc))
                                        {
                                            if (strLocSts == "S") strIotype = IO_TYPE.LoactionToLoaction;
                                            if (strLocSts == "E") strIotype = IO_TYPE.LoactionToLoaction2;
                                            if (!funCreateStoreInCommand(strCommandID, CMDMode.LoactionToLoaction, IO_TYPE.LoactionToLoaction, commandInfo.Loaction, commandInfo.PalletNo, "", "", commandInfo.LOC_SIZE, strNewLoc))
                                            {
                                                throw new Exception("库对库命令产生失败！");
                                            }
                                            if (!funUpdateLocationMaster(strNewLoc, LoactionState.IN, commandInfo.PalletNo))
                                            {
                                                throw new Exception("库对库新储位预约失败！");
                                            }
                                            if (!funUpdateLocationMaster(commandInfo.Loaction, LoactionState.OUT, " "))
                                            {
                                                throw new Exception("库对库旧储位预约失败！");
                                            }
                                            if (strLocSts == "S")
                                            {
                                                if (!funLockStoreInBox(strSubNo, LoactionState.OUT))
                                                {
                                                    throw new Exception("子托盘预约失败！");
                                                }
                                            }

                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        }
                                        else
                                        {
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += "库对库储位获取失败!";
                                            funWriteSysTraceLog(strMsg);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += ex.ToString();
                                        funWriteSysTraceLog(strMsg);
                                    }
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            #region 内储位直接出货
                            if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                                string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                                strBufferName == strOutStntionName &&
                                bufferData[intBufferIndex]._Mode == Buffer.StnMode.None &&
                                bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                                bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                            {
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += "StoreOut Command Initiated!";
                                funWriteSysTraceLog(strMsg);


                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                try
                                {
                                    if (funUpdateCommand(commandInfo.CommandID, CommandState.Start, Trace.StoreOut_GetStoreOutCommandAndWritePLC))
                                    {
                                        string[] strValues = new string[] { commandInfo.CommandID, commandInfo.CommandMode.ToString(), commandInfo.StationNo.Remove(0, 1) };
                                        if (InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_CmdSno, strValues))
                                        {
                                            #region Update Command & Write MPLC Success
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                            strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                            strMsg += "StoreOut Command Update Success!";
                                            funWriteSysTraceLog(strMsg);

                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
                                            strMsg += commandInfo.PalletNo + "|";
                                            strMsg += bufferData[intBufferIndex]._W_CmdSno + "|";
                                            strMsg += string.Join(",", strValues) + "|";
                                            strMsg += "Write MPLC Success!";
                                            funWriteSysTraceLog(strMsg);
                                            #endregion Update Command & Write MPLC Success
                                        }
                                        else
                                        {
                                            #region Update Command Success But Write MPLC Fail
                                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                            strMsg = commandInfo.CommandID + "|";
                                            strMsg += commandInfo.CycleNo + "|";
                                            strMsg += commandInfo.CommandMode + "|";
                                            strMsg += commandInfo.Loaction + "|";
                                            strMsg += commandInfo.StationNo + "|";
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
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += CommandState.Inital + "->" + CommandState.Start + "|";
                                        strMsg += Trace.Inital + "->" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "|";
                                        strMsg += "StoreOut Command Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                }
                                catch (Exception)
                                {
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                }
                            }
                            #endregion
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

        private void funStoreOut_CrateCraneCommand()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        (bufferData[intBufferIndex]._Mode == Buffer.StnMode.StoreOut || bufferData[intBufferIndex]._Mode == Buffer.StnMode.Picking) &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                        bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCommandID = bufferData[intBufferIndex]._CommandID.PadLeft(5, '0');
                        strSQL = "SELECT * FROM CMD_MST";
                        strSQL += " WHERE Cmd_Sts<'1'";
                        strSQL += " AND Cmd_Sno='" + strCommandID + "'";
                        strSQL += " AND CMD_MODE IN ('2', '3')";
                        strSQL += " AND TRACE='" + Trace.StoreOut_GetStoreOutCommandAndWritePLC + "'";
                        strSQL += " ORDER BY LOC DESC";
                        if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
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

                            if (!funCheckCraneExistsCommand(CraneMode.StoreOut, stnDef.StationIndex.ToString()))
                            {
                                InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                                if (funCrateCraneCommand(commandInfo.CommandID, stnDef.StationIndex.ToString(), CraneMode.StoreOut,
                                    funGetCrnLoc(commandInfo.Loaction), CraneMode.StoreOut, commandInfo.Priority))
                                {
                                    if (funUpdateCommand(strCommandID, CommandState.Start, Trace.StoreOut_CrateCraneCommand))
                                    {
                                        #region Update Command & Create StoreOut Crane Command Success
                                        InitSys._DB.CommitCtrl(DBOracle.TransactionType.Commit);
                                        strMsg = strCommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += "StoreOut Crane Command Create Success!";
                                        funWriteSysTraceLog(strMsg);

                                        strMsg = commandInfo.CommandID + "|";
                                        strMsg += commandInfo.CycleNo + "|";
                                        strMsg += commandInfo.CommandMode + "|";
                                        strMsg += commandInfo.Loaction + "|";
                                        strMsg += commandInfo.StationNo + "|";
                                        strMsg += commandInfo.PalletNo + "|";
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                        strMsg += "StoreOut Command Update Success!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command & Create StoreOut Crane Command Success

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
                                        strMsg += CommandState.Start + "|";
                                        strMsg += Trace.StoreOut_GetStoreOutCommandAndWritePLC + "->" + Trace.StoreOut_CrateCraneCommand + "|";
                                        strMsg += "StoreOut Command Update Fail!";
                                        funWriteSysTraceLog(strMsg);
                                        #endregion Update Command Fail
                                    }
                                }
                                else
                                {
                                    #region Create StoreOut Crane Command Fail
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
                                    strMsg = strCommandID + "|";
                                    strMsg += commandInfo.CycleNo + "|";
                                    strMsg += commandInfo.CommandMode + "|";
                                    strMsg += commandInfo.Loaction + "|";
                                    strMsg += commandInfo.StationNo + "|";
                                    strMsg += commandInfo.PalletNo + "|";
                                    strMsg += "StoreOut Crane Command Create Fail!";
                                    funWriteSysTraceLog(strMsg);
                                    #endregion Create StoreOut Crane Command Fail
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

        private void funStoreOut_CraneCommandFinish()
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
                strSQL += " AND CMD_MODE IN ('2', '3')";
                strSQL += " AND TRACE='" + Trace.StoreOut_CrateCraneCommand + "'";
                strSQL += " order by Crt_Dte";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
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
                    strSQL += " AND CMDMODE='2'";
                    strSQL += " AND CMDSTS in ('8','9')";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtEquCmd, ref strEM))
                    {
                        string strCmdSts = dtEquCmd.Rows[0]["CmdSts"].ToString();
                        string strCompleteCode = dtEquCmd.Rows[0]["CompleteCode"].ToString();

                        if (strCmdSts == CommandState.EQUCompleted && strCompleteCode.Substring(0, 1) == "W")
                        {
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);

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
                                if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += CommandState.Start + "->" + CommandState.CompletedWaitPost + "|";
                                strMsg += Trace.StoreOut_CrateCraneCommand + "|";
                                strMsg += strCompleteCode + "|";
                                strMsg += "StoreOut Command Update Fail!";
                                funWriteSysTraceLog(strMsg);
                                #endregion Update Command Fail
                            }
                        }
                        else if (strCmdSts == CommandState.EQUCompleted && (strCompleteCode == "92" || strCompleteCode == "FF"))
                        {
                            InitSys._DB.CommitCtrl(DBOracle.TransactionType.Begin);
                            if (funUpdateCommand(commandInfo.CommandID, CommandState.OnTransit, Trace.StoreOut_CraneCommandFinish))
                            {
                                if (funDeleteEquCmd(commandInfo.CommandID, ((int)Buffer.StnMode.StoreOut).ToString()))
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
                                    InitSys._DB.CommitCtrl(DBOracle.TransactionType.Rollback);
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
                                strMsg += "StoreOut Command Update Fail!";
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
