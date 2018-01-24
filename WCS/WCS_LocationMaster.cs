using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private bool funUpdateLocationMaster(string location, string setLocationState, string setPalletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE LOC_MST";
                strSQL += " SET Loc_Sts='" + setLocationState + "',";
                if(setLocationState == "N")
                {
                    strSQL += " Plt_No='',";
                    strSQL += " ITEM_NO='',";
                }
                else
                {
                    strSQL += " Plt_No='" + setPalletNo + "',";
                    strSQL += " ITEM_NO='" + setPalletNo + "',";
                }
                strSQL += " Loc_OSts=Loc_Sts,";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                strSQL += " Trn_Tim='" + DateTime.Now.ToString("HH:mm:ss") + "'";
                strSQL += " WHERE Loc='" + location + "'";
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
