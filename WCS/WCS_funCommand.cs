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
                strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
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
                strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funCreateAGVStoreInCommand(string commandID, string location, string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {

                strSQL = "INSERT INTO CMD_MST(Cmd_Sno, Equ_No, Cmd_Mode, Cmd_Sts, Io_Type,";
                strSQL += " Plt_No, Stn_No, Loc, Prty, Prog_Id, User_Id, TRACE, Crt_Dte) Values (";
                strSQL += "'" + commandID + "', ";
                strSQL += "'" + InitSys._CraneNo + "', ";
                strSQL += "'1', ";
                strSQL += "'0', ";
                strSQL += "'12', ";
                strSQL += "'" + palletNo + "', ";
                strSQL += "'" + "A13" + "', ";
                strSQL += "'" + location + "', ";
                strSQL += "'5', ";
                strSQL += "'WCS', ";
                strSQL += "'WCS', ";
                strSQL += "'0', ";
                strSQL += "GETDATE())";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
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
                    strSQL += " WHERE Sno_Type='CF'";
                    if(InitSys._DB.funGetDT(strSQL, ref dtCommandID, ref strEM))
                    {
                        string strCmdSno = dtCommandID.Rows[0]["Sno"].ToString();
                        int intCmdSno = int.Parse(strCmdSno) + 1;

                        if(intCmdSno > 29999)
                            intCmdSno = 1;

                        strCmdSno = intCmdSno.ToString();
                        strSQL = "UPDATE SNO_CTL";
                        strSQL += " SET TRNDATE='" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                        strSQL += " SNO='" + strCmdSno + "'";
                        strSQL += " WHERE SNOTYP='CF'";
                        if(InitSys._DB.funExecSql(strSQL, ref strEM))
                            return strCmdSno.PadLeft(5, '0');
                    }
                    else
                    {
                        strSQL = "INSERT INTO SNO_CTL (TRNDATE, SNOTYP, SNO) VALUES (";
                        strSQL += "'" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                        strSQL += "'CF',";
                        strSQL += "'1')";
                        if(InitSys._DB.funExecSql(strSQL, ref strEM))
                            return "1";
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
                strSQL = "SELECT * FROM CMD_MST";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
                strSQL += " AND Cmd_Sts='0'";
                strSQL += " AND Cmd_Mode='1'";
                strSQL += " AND Io_Type='12'";
                strSQL += " AND Equ_No='" + InitSys._CraneNo + "'";
                if(InitSys._DB.funGetDT(strSQL, ref dtCommand, ref strEM))
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
