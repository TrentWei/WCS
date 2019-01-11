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
        private void funKanbanInfo()
        {
            string strMsg = string.Empty;
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtkanbaninfo = new DataTable();
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstStoreIn)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;

                    //读取站口值，Set Kanbaninfo
                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                    !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                    !bufferData[intBufferIndex]._Clearnotice &&
                    intBufferIndex != 12 &&
                    intBufferIndex != 15 &&
                    bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                    bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCmdsno = bufferData[intBufferIndex]._CommandID;
                        strSQL = "select * from CMD_MST ";
                        strSQL += " where CMD_SNO='" + strCmdsno.PadLeft(5, '0') + "'";
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


                            string strPStnNo = funGetPStnNo(commandInfo.PalletNo, commandInfo.StationNo); ;
                            #region Set KanbanInfo
                            if (funSetKanbanInfo(commandInfo.StationNo, commandInfo.CommandID, commandInfo.CommandMode,
                                commandInfo.PalletNo, commandInfo.Loaction, commandInfo.CycleNo, strPStnNo))
                            {
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += "Set KanbanInfo Success!";
                                funWriteSysTraceLog(strMsg);
                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
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
                    }

                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                   string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                   intBufferIndex != 12 &&
                   intBufferIndex != 15 &&
                   bufferData[intBufferIndex]._Clearnotice &&
                   bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                   bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        funClearKanbanInfo(strBufferName);
                        string[] strValues = new string[] { "0" };
                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
                    }
                }
                foreach (StationInfo stnDef in lstStoreOut)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;
                    //读取站口值，Set Kanbaninfo
                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                    string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                    intBufferIndex != 16 &&
                    intBufferIndex != 19 &&
                     !bufferData[intBufferIndex]._Clearnotice &&
                    bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.On &&
                    bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        string strCmdsno = bufferData[intBufferIndex]._CommandID;
                        strSQL = "select * from CMD_MST ";
                        strSQL += " where CMD_SNO='" + strCmdsno.PadLeft(5, '0') + "'";
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


                            string strPStnNo = funGetPStnNo(commandInfo.PalletNo, commandInfo.StationNo); ;
                            #region Set KanbanInfo
                            if (funSetKanbanInfo(commandInfo.StationNo, commandInfo.CommandID, commandInfo.CommandMode,
                                commandInfo.PalletNo, commandInfo.Loaction, commandInfo.CycleNo, strPStnNo))
                            {
                                strMsg = commandInfo.CommandID + "|";
                                strMsg += commandInfo.CycleNo + "|";
                                strMsg += commandInfo.CommandMode + "|";
                                strMsg += commandInfo.StationNo + "|";
                                strMsg += commandInfo.Loaction + "|";
                                strMsg += commandInfo.PalletNo + "|";
                                strMsg += "Set KanbanInfo Success!";
                                funWriteSysTraceLog(strMsg);
                                string[] strValues = new string[] { "1" };
                                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
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
                    }

                    if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                   !string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                   intBufferIndex != 16 &&
                   intBufferIndex != 19 &&
                   bufferData[intBufferIndex]._Clearnotice &&
                   bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                   bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        funClearKanbanInfo(strBufferName);
                        string[] strValues = new string[] { "0" };
                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
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
                if(dtkanbaninfo != null)
                {
                    dtkanbaninfo.Clear();
                    dtkanbaninfo.Dispose();
                    dtkanbaninfo = null;
                }
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }

        private bool funSetKanbanInfo(
            string stationNo, string commandID, int commandMode, string palletNo,
            string location, string cycleNo, string message)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            if (stationNo == "A13")
            {
                stationNo = "A01";
            }
            if (stationNo == "A03")
            {
                stationNo = "A02";
            }
            if (stationNo == "A09")
            {
                stationNo = "A01";
            }
            try
            {
                strSQL = "INSERT INTO KanbanInfo_Log";
                strSQL += " SELECT * FROM kanbaninfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "DELETE FROM KanbanInfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "INSERT INTO KanbanInfo (InfoType, KanbanType, Msg, DetailMsg, AllocateQty, PartNo, Qty,";
                strSQL += " RemainQty, Title1, Title2, UpdatedDate, UpdatePerson, CreatedDate, CreatedPerson,";
                strSQL += " Status, STN_NO, Line1, Line2, Customer, OutQty, TotalQty) VALUES (";
                if(commandMode == 1)
                {
                    strSQL += " '入库',";
                    strSQL += " 'StoreIn1',";
                }
                else if(commandMode == 2)
                {
                    strSQL += " '出库',";
                    strSQL += " 'StoreOut1',";
                }
                else
                {
                    strSQL += " '盘点',";
                    strSQL += " 'StoreIn1',";
                }
                strSQL += " '" + location + "',";
                strSQL += " '" + message + "',";
                strSQL += " '',";
                strSQL += " '" + palletNo + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + commandID + "',";
                strSQL += " '" + cycleNo + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                strSQL += " 'WCS',";
                strSQL += " '0',";
                strSQL += " '" + stationNo + "',";
                strSQL += " ' ',";
                strSQL += " ' ',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '')";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funSetKanbanInfo(string stationNo, string commandID, int commandMode, string palletNo, string location, string cycleNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            if (stationNo=="A13")
            {
                stationNo = "A01";
            }
            if (stationNo == "A03")
            {
                stationNo = "A02";
            }
            if (stationNo == "A09")
            {
                stationNo = "A01";
            }
            try
            {
                strSQL = "INSERT INTO KanbanInfo_Log";
                strSQL += " SELECT * FROM kanbaninfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "DELETE FROM KanbanInfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "INSERT INTO KanbanInfo (InfoType, KanbanType, Msg, DetailMsg, AllocateQty, PartNo, Qty,";
                strSQL += " RemainQty, Title1, Title2, UpdatedDate, UpdatePerson, CreatedDate, CreatedPerson,";
                strSQL += " Status, STN_NO, Line1, Line2, Customer, OutQty, TotalQty) VALUES (";
                if(commandMode == 1)
                {
                    strSQL += " '入库',";
                    strSQL += " 'StoreIn1',";
                }
                else if(commandMode == 2)
                {
                    strSQL += " '出库',";
                    strSQL += " 'StoreOut1',";
                }
                else
                {
                    strSQL += " '盘点',";
                    strSQL += " 'StoreIn1',";
                }
                strSQL += " '" + location + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + palletNo + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + commandID + "',";
                strSQL += " '" + cycleNo + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                strSQL += " 'WCS',";
                strSQL += " '0',";
                strSQL += " '" + stationNo + "',";
                strSQL += " ' ',";
                strSQL += " ' ',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '')";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funSetKanbanInfo(string stationNo, string palletNo, string message)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            if (stationNo == "A13")
            {
                stationNo = "A01";
            }
            if (stationNo == "A03")
            {
                stationNo = "A02";
            }
            if (stationNo == "A09")
            {
                stationNo = "A01";
            }
            try
            {
                strSQL = "INSERT INTO KanbanInfo_Log";
                strSQL += " SELECT * FROM kanbaninfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "DELETE FROM KanbanInfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "INSERT INTO KanbanInfo (InfoType, KanbanType, Msg, DetailMsg, AllocateQty, PartNo, Qty,";
                strSQL += " RemainQty, Title1, Title2, UpdatedDate, UpdatePerson, CreatedDate, CreatedPerson,";
                strSQL += " Status, STN_NO, Line1, Line2, Customer, OutQty, TotalQty) VALUES (";
                strSQL += " '异常',";
                strSQL += " '00001',";
                strSQL += " '',";
                strSQL += " '" + message + "',";
                strSQL += " '',";
                strSQL += " '" + palletNo + "',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                strSQL += " 'WCS',";
                strSQL += " '0',";
                strSQL += " '" + stationNo + "',";
                strSQL += " ' ',";
                strSQL += " ' ',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '')";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }


        private bool funClearKanbanInfo(string stationNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            if (stationNo == "A13")
            {
                stationNo = "A01";
            }
            if (stationNo == "A03")
            {
                stationNo = "A02";
            }
            if (stationNo == "A09")
            {
                stationNo = "A01";
            }
            try
            {
                strSQL = "INSERT INTO KanbanInfo_Log";
                strSQL += " SELECT * FROM kanbaninfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "DELETE FROM KanbanInfo";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
                InitSys._DB.ExecuteSQL(strSQL, ref strEM);

                strSQL = "INSERT INTO KanbanInfo (InfoType, KanbanType, Msg, DetailMsg, AllocateQty, PartNo, Qty,";
                strSQL += " RemainQty, Title1, Title2, UpdatedDate, UpdatePerson, CreatedDate, CreatedPerson,";
                strSQL += " Status, STN_NO, Line1, Line2, Customer, OutQty, TotalQty) VALUES (";
                strSQL += " '清除',";
                strSQL += " '00000',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                strSQL += " 'WCS',";
                strSQL += " '0',";
                strSQL += " '" + stationNo + "',";
                strSQL += " ' ',";
                strSQL += " ' ',";
                strSQL += " '',";
                strSQL += " '',";
                strSQL += " '')";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private string funGetPStnNo(string Item_No,string Stn_NO)
        {
            DataTable dtGetPStnNo = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
               
                strSQL = "SELECT * FROM ITEM_MST ";
                strSQL += " WHERE ITEM_NO='"+Item_No+"' ";
                if (Stn_NO.Substring(0, 1) == "A")
                {
                    strSQL += " and ITEM_TYPE='P'";
                }
                else if (Stn_NO.Substring(0, 1) == "B")
                {
                    strSQL += " and ITEM_TYPE='M'";
                }
                if (InitSys._DB.GetDataTable(strSQL, ref dtGetPStnNo, ref strEM))
                {
                    return dtGetPStnNo.Rows[0]["PStn_No"].ToString();
                }
                else
                {
                    return "目的位置获取失败!";
                }
                    
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return "目的位置获取失败!";
            }
            finally
            {
                if (dtGetPStnNo != null)
                {
                    dtGetPStnNo.Clear();
                    dtGetPStnNo.Dispose();
                    dtGetPStnNo = null;
                }
            }
        }
    }
}
