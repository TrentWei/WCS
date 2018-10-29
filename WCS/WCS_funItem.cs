using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private bool funUpdateItemMaster(string palletNo, string setState, string setNewLocation, bool clearPStation,string Item_Type)
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
                strSQL += " Trn_Dte='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +"'";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
                strSQL += " and Item_Type='" + Item_Type + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        
        private bool funUpdateCYCLE(string cycleNo, string palletNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE CYCLE";
                strSQL += " SET Status='1'";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
                strSQL += " AND Cyc_No='" + cycleNo + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funLockStoreInPalletNo(string palletNo,string Item_type)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE ITEM_MST";
                strSQL += " SET Status='I',";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
                strSQL += " and Item_Type='" + Item_type + "'";
                return InitSys._DB.funExecSql(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funLockStoreOutPalletNo(string palletNo, int Prodecu_Qty,string Pstn_no,string Item_Type)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE ITEM_MST";
                strSQL += " SET Status='O',";
                strSQL += " PRODUCE_Qty=PRODUCE_Qty+" + Prodecu_Qty + ",";
                strSQL += " PSTN_NO='" + Pstn_no + "',";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Plt_No='" + palletNo + "'";
                strSQL += " and ITEM_Type='"+ Item_Type + "'";
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
