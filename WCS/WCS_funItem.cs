using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private bool funUpdateItemMaster(string palletNo, string setState, string setNewLocation, bool clearPStation)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE ITEM_MST";
                strSQL += " SET Status='" + setState + "',";
                strSQL += " Loc='" + setNewLocation + "',";
                if(clearPStation)
                    strSQL += " PStn_No='0',";
                strSQL += " Trn_Dte=GETDATE()";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
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
