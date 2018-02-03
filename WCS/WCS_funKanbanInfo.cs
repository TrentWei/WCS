using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private bool funSetKanbanInfo(string stationNo, string commandID, int commandMode, string palletNo, string location, string cycleNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;

            try
            {
                strSQL = "UPDATE KanbanInfo";
                strSQL += " SET Title1='" + commandID + "',";
                strSQL += " Title2='" + cycleNo + "',";
                strSQL += " Msg='" + location + "',";
                strSQL += " PartNo='" + palletNo + "',";
                strSQL += " Status='0'";
                strSQL += " WHERE STN_NO='" + stationNo + "'";
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
