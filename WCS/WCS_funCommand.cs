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
        private bool funUpdateCommand(string commandID, string setCommandState, string setTrace)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                if(string.IsNullOrWhiteSpace(commandID) || string.IsNullOrWhiteSpace(setCommandState) || string.IsNullOrWhiteSpace(setTrace))
                    return false;

                strSQL = "UPDATE CMD_MST SET";
                strSQL += " Cmd_Sts='" + setCommandState + "',";
                strSQL += " TRACE='" + setTrace + "'";
                strSQL += " WHERE Cmd_Sno='" + commandID + "'";
                strSQL += " AND CMD_STS<='1'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funUpdateCommandLoc(string commandID, string setStn_No,string setLoc)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(commandID) || string.IsNullOrWhiteSpace(setStn_No) || string.IsNullOrWhiteSpace(setLoc))
                    return false;
                strSQL = "UPDATE CMD_MST SET";
                strSQL += " STN_NO='" + setStn_No + "'";
                strSQL += " LOC='" + setLoc + "'";
                strSQL += " TRACE='" + Trace.StoreIn_GetStoreInCommandAndSetLoc + "'";
                strSQL += " WHERE Cmd_Sno='" + commandID + "'";
                strSQL += " AND CMD_STS<'1'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funUpdateCommand(string commandID, string setCommandState, string trace, string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE CMD_MST";
                strSQL += " SET Cmd_Sts='" + setCommandState + "',";
                strSQL += " End_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Cmd_Sno='" + commandID + "'";
                strSQL += " AND Plt_No='" + palletNo + "'";
                strSQL += " AND TRACE='" + trace + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 下达入库指令时判断是否存在 相同料号 状态0，1的命令存在
        /// </summary>
        /// <param name="palletNo">料号：ITEM_NO</param>
        /// <returns></returns>
        private bool funChekAGVStoreInCommand(string palletNo)
        {
            DataTable dtProduce = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                strSQL = "SELECT * FROM CMD_MST  where PLT_NO='"+palletNo+"' and cmd_STS in ('0','1') and cmd_Mode='1' ";
                if (InitSys._DB.GetDataTable(strSQL, ref dtProduce, ref strEM))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
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

      


        private bool funCreateStoreInCommand(string commandID,string commandStatus,string commandIoType, string location, string palletNo,string Stn_No,string ActualWeight,string Loc_Size,string Newlocation)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {

                strSQL = "INSERT INTO CMD_MST(CMD_DTE,Cmd_Sno, Cmd_Mode, Cmd_Sts, Io_Type, Plt_No,";
                strSQL += " Stn_No, Loc, Prty, Prog_Id, User_Id, TRACE, Crt_Dte,Actual_Weight,LOC_SIZE) Values (";
                strSQL += "'" + DateTime.Now.ToString("yyyyMMdd") + "',";
                strSQL += "'" + commandID + "', ";
                strSQL += "'"+ commandStatus + "', ";
                strSQL += "'"+ CommandState.Inital+ "', ";
                strSQL += "'"+ commandIoType + "', ";
                strSQL += "'" + palletNo + "', ";
                strSQL += "'" + Stn_No + "', ";
                strSQL += "'" + location + "', ";
                strSQL += "'5', ";
                strSQL += "'WCS', ";
                strSQL += "'WCS', ";
                strSQL += "'0', ";
                strSQL += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                strSQL += "'"+ ActualWeight + "', ";
                strSQL += "'" + Loc_Size + "', ";
                strSQL += "'" + Newlocation + "') ";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funCreateAGVStoreOutCommand(string commandID, string location, string palletNo,string Stn_No)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {

                strSQL = "INSERT INTO CMD_MST(Cmd_Sno, Cmd_Mode, Cmd_Sts, Io_Type, Plt_No,";
                strSQL += " Stn_No, Loc, Prty, Prog_Id, User_Id, TRACE, Crt_Dte) Values (";
                strSQL += "'" + commandID + "', ";
                strSQL += "'2', ";
                strSQL += "'0', ";
                strSQL += "'12', ";
                strSQL += "'" + palletNo + "', ";
                strSQL += "'" + Stn_No + "', ";
                strSQL += "'" + location + "', ";
                strSQL += "'5', ";
                strSQL += "'WCS', ";
                strSQL += "'WCS', ";
                strSQL += "'0', ";
                strSQL += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private string funGetCommandID()
        {
            DataTable dtCommandID = new DataTable();
            int intTimes = 0;

            try
            {
                do
                {
                    string strSQL = string.Empty;
                    string strEM = string.Empty;

                    intTimes++;
                    strSQL = "SELECT * FROM SNO_CTL";
                    strSQL += " WHERE Sno_Type='CmdSno'";
                    if(InitSys._DB.GetDataTable(strSQL, ref dtCommandID, ref strEM))
                    {
                        string strCmdSno = dtCommandID.Rows[0]["Sno"].ToString();
                        string strCmdSnoNew = "";
                        int intCmdSno = int.Parse(strCmdSno);

                        if (intCmdSno > 29999)
                        {
                            intCmdSno = 1;
                        }
                        strCmdSnoNew = (intCmdSno + 1).ToString();
                        strSQL = "UPDATE SNO_CTL";
                        strSQL += " SET Trn_Dte='" + DateTime.Now.ToString("yyyyMMdd") + "',";
                        strSQL += " SNO='" + strCmdSnoNew + "'";
                        strSQL += " WHERE Sno_Type='CmdSno'";
                        if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                            return strCmdSnoNew.PadLeft(5, '0');
                    }
                    else
                    {
                        strSQL = "INSERT INTO SNO_CTL (Trn_Dte, Sno_Type, SNO) VALUES (";
                        strSQL += "'" + DateTime.Now.ToString("yyyyMMdd") + "',";
                        strSQL += "'CmdSno',";
                        strSQL += "'1')";
                        if(InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                            return "1".PadLeft(5, '0');
                    }
                }
                while(intTimes < 10);

                return string.Empty;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return string.Empty;
            }
            finally
            {
                if(dtCommandID != null)
                {
                    dtCommandID.Clear();
                    dtCommandID.Dispose();
                    dtCommandID = null;
                }
            }
        }

        private int funCheckExistsAGVStoreInCommand(string palletNo, ref string commandID, ref string loaction)
        {
            DataTable dtCommand = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
                strSQL = "SELECT * FROM CMD_MST C join LOC_MST L on c.Loc=l.Loc";
                strSQL += " WHERE C.Loc!='' and L.Loc_Type='P' and C.Plt_No='" + palletNo + "'";
                strSQL += " AND Cmd_Sts in ('0','1')";
                strSQL += " AND l.LOC_STS in ('I')";
                strSQL += " AND Cmd_Mode='1'";
                strSQL += " AND Io_Type='12'";
                if(InitSys._DB.GetDataTable(strSQL, ref dtCommand, ref strEM))
                {
                    if(dtCommand.Rows.Count == 1)
                    {
                        commandID = dtCommand.Rows[0]["Cmd_Sno"].ToString();
                        loaction = dtCommand.Rows[0]["Loc"].ToString();
                        return 1;
                    }
                    else
                        return dtCommand.Rows.Count;
                }
                else
                    return 0;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return 0;
            }
            finally
            {
                if(dtCommand != null)
                {
                    dtCommand.Clear();
                    dtCommand.Dispose();
                    dtCommand = null;
                }
            }
        }
    }
}
