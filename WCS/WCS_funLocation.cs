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

        private string funGetEmptyLocation()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLocation = new DataTable();

            try
            {
                strSQL = "SELECT * FROM LOC_MST";
                strSQL += " WHERE LOCSTS='N'";
                strSQL += " AND Loc_Size='L'";
                strSQL += " AND Loc_Type='P'";
                strSQL += " ORDER BY LVL_Z, BAY_Y, ROW_X";

                if(InitSys._DB.funGetDT(strSQL, ref dtLocation, ref strEM))
                    return dtLocation.Rows[0]["LOC"].ToString();
                else
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
                if(dtLocation != null)
                {
                    dtLocation.Clear();
                    dtLocation.Dispose();
                    dtLocation = null;
                }
            }
        }

        private bool funLockStoreInLocation(string strLoaction)
        {
            throw new NotImplementedException();
        }
    }
}
