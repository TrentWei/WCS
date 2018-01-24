using System;
using System.Collections.Generic;
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
