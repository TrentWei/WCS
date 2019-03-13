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
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
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
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funLockStoreInBox(string SubNO,string strMode)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE BOX";
                strSQL += " SET Status='"+ strMode + "',";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Sub_No='" + SubNO + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funLockStoreInBox(string SubNO, string strMode,string strLoaction)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE BOX";
                strSQL += " SET Status='" + strMode + "',";
                strSQL += " SET LOC='" + strLoaction + "',";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Sub_No='" + SubNO + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funGetCode(ref int Pack001, ref int Pack002)
        {
            DataTable dtCode = new DataTable();
            string strSQL = string.Empty;
            
            string strEM = string.Empty;
            Pack001 = 0;
            Pack002 = 0;
            try
            {
                strSQL = "SELECT * FROM CMD_MST  where CODE_TYPE='PACK' AND CODE_NO in ('001','002')  ";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCode, ref strEM))
                {
                    if (dtCode.Rows.Count>=2)
                    {
                        Pack001 = Convert.ToInt32(dtCode.Rows[0]["CODE_NAME"].ToString());
                        Pack002 = Convert.ToInt32(dtCode.Rows[1]["CODE_NAME"].ToString());
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
            finally
            {
                if (dtCode != null)
                {
                    dtCode.Clear();
                    dtCode.Dispose();
                    dtCode = null;
                }
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
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
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
