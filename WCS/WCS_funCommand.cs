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
                strSQL += " AND CMDSTS<='1'";
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
