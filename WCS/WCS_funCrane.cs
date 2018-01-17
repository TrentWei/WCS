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
        private bool funCheckCraneExistsCommand(string craneNo, string craneMode, string stnIndex)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtEquCmd = new DataTable();
            try
            {
                switch(craneMode)
                {
                    case CraneMode.StoreOut:
                        strSQL = "SELECT COUNT (*) AS ICOUNT FROM EQUCMD";
                        strSQL += " WHERE EQUNO='" + craneNo + "'";
                        strSQL += " AND CmdSts IN ('0', '1')";
                        strSQL += " AND DESTINATION='" + stnIndex + "'";
                        break;
                }
                if(InitSys._DB.funGetDT(strSQL, ref dtEquCmd, ref strEM))
                    return int.Parse(dtEquCmd.Rows[0]["ICOUNT"].ToString()) > 0;
                else
                    return false;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
            finally
            {
                if(dtEquCmd != null)
                {
                    dtEquCmd.Clear();
                    dtEquCmd.Dispose();
                    dtEquCmd = null;
                }
            }
        }

        private bool funCrateCraneCommand(string craneNo, string commandID, string craneMode, string source, string destination, string priority)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                strSQL = "INSERT INTO EquCmd(CmdSno, EquNo, CmdMode, CmdSts, Source, Destination, LocSize, Priority, RCVDT, SpeedLevel) Values (";
                strSQL += "'" + commandID + "', ";
                strSQL += "'" + craneNo + "', ";
                strSQL += "'" + craneMode + "', ";
                strSQL += "'0', ";
                strSQL += "'" + source + "', ";
                strSQL += "'" + destination + "', ";
                strSQL += "'0', ";
                strSQL += "'" + priority + "', ";
                strSQL += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', ";
                strSQL += "'0')";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funDeleteEquCmd(string craneNo, string commandID, string craneMode)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
                strSQL = "UPDATE EQUCMD SET RENEWFLAG='F'";
                strSQL += " WHERE CMDSTS='9'";
                strSQL += " AND RENEWFLAG='Y'";
                strSQL += " AND EquNo='" + craneNo + "'";
                strSQL += " AND CMDSNO='" + commandID + "'";
                strSQL += " AND CmdMode='" + craneMode + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }
    }
}
