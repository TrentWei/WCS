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
        private bool funCheckCraneExistsCommand(string craneMode, string stnIndex)
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
                        strSQL += " WHERE CmdSts IN ('0', '1')";
                        strSQL += " AND DESTINATION='" + stnIndex + "'";
                        break;
                    case CraneMode.StoreIn:
                        strSQL = "SELECT COUNT (*) AS ICOUNT FROM EQUCMD";
                        strSQL += " WHERE CmdSts IN ('0', '1')";
                        strSQL += " AND SOURCE='" + stnIndex + "'";
                        break;
                    case CraneMode.StationToStation:
                    case CraneMode.LoactionToLoaction:
                        strSQL = "SELECT COUNT (*) AS ICOUNT FROM EQUCMD";
                        strSQL += " WHERE CMDMODE='" + craneMode + "'";
                        strSQL += " AND CmdSts IN ('0', '1')";
                        break;
                    default:
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

        private bool funCrateCraneCommand(string commandID, string craneMode, string source, string destination, string priority)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                if(source.Length == 6)
                    source = source.Insert(2, "0");
                else
                    source = source.PadLeft(7, '0');

                if(destination.Length == 6)
                    destination = destination.Insert(2, "0");
                else
                    destination = destination.PadLeft(7, '0');

                strSQL = "INSERT INTO EquCmd(CmdSno, EquNo, CmdMode, CmdSts, Source, Destination, LocSize, Priority, RCVDT) Values (";
                strSQL += "'" + commandID + "', ";
                strSQL += "'1', ";
                strSQL += "'" + craneMode + "', ";
                strSQL += "'0', ";
                strSQL += "'" + source + "', ";
                strSQL += "'" + destination + "', ";
                strSQL += "'0', ";
                strSQL += "'" + priority + "', ";
                strSQL += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funDeleteEquCmd(string commandID, string craneMode)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
                strSQL = "UPDATE EQUCMD SET RENEWFLAG='F'";
                strSQL += " WHERE CMDSTS='9'";
                strSQL += " AND RENEWFLAG='Y'";
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
