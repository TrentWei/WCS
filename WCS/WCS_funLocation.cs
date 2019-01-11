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
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funGetEmptyLocation(string Loc_Size,string Loc_Type,ref string strLoaction)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLocation = new DataTable();

            try
            {
                strSQL = "SELECT * FROM LOC_MST";
                strSQL += " WHERE Loc_Sts='N'";
                strSQL += " AND Loc_Size='"+ Loc_Size + "'";
                strSQL += " AND Loc_Type='"+ Loc_Type + "'";
                strSQL += " ORDER BY LVL_Z, BAY_Y, ROW_X";

                if (InitSys._DB.GetDataTable(strSQL, ref dtLocation, ref strEM))
                {
                    strLoaction=dtLocation.Rows[0]["LOC"].ToString();

                    strSQL = "UPDATE LOC_MST";
                    strSQL += " SET Loc_Sts='I',";
                    strSQL += " Loc_OSts=Loc_Sts,";
                    strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                    strSQL += " Trn_Tim='" + DateTime.Now.ToString("HH:mm:ss") + "'";
                    strSQL += " WHERE Loc='" + strLoaction + "'";
                    if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
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
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE LOC_MST";
                strSQL += " SET Loc_Sts='I',";
                strSQL += " Loc_OSts=Loc_Sts,";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                strSQL += " Trn_Tim='" + DateTime.Now.ToString("HH:mm:ss") + "'";
                strSQL += " WHERE Loc='" + strLoaction + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funLockStoreOutLocation(string strLoaction)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE LOC_MST";
                strSQL += " SET Loc_Sts='O',";
                strSQL += " Loc_OSts=Loc_Sts,";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                strSQL += " Trn_Tim='" + DateTime.Now.ToString("HH:mm:ss") + "'";
                strSQL += " WHERE Loc='" + strLoaction + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funQITEMTYPE(string loc, ref string Type)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLoc = new DataTable();
            try
            {
                strSQL = "SELECT * from LOC_MST where loc='"+loc+"' ";
                if (InitSys._DB.GetDataTable(strSQL, ref dtLoc, ref strEM))
                {
                    Type = dtLoc.Rows[0]["LOC_TYPE"].ToString();
                    return true;
                }
                else
                {
                    Type = string.Empty;
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
                if (dtLoc != null)
                {
                    dtLoc.Clear();
                    dtLoc.Dispose();
                    dtLoc = null;
                }
            }
        }

        private bool funGetItemNoLocation(string palletNo,string Item_Type, ref string location)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLoc = new DataTable();
            try
            {
                strSQL = "SELECT A.*,B.* ";
                strSQL += " FROM LOC_MST AS A, ITEM_MST AS B";
                strSQL += " WHERE A.Plt_No=B.Plt_No";
                strSQL += " AND A.LOC=B.loc";
                strSQL += " AND B.Item_Type='"+ Item_Type.Trim()+ "'";
                strSQL += " AND A.Plt_No='" + palletNo + "'";
                if(InitSys._DB.GetDataTable(strSQL, ref dtLoc, ref strEM))
                {
                    if(dtLoc.Rows.Count == 1)
                    {
                        location = dtLoc.Rows[0]["Loc"].ToString();
                        return true;
                    }
                    else
                    {
                        location = dtLoc.Rows[0]["Loc"].ToString();
                        return false;
                    }
                }
                else
                {
                    location = string.Empty;
                    return false;
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                location = string.Empty;
                return false;
            }
            finally
            {
                if(dtLoc != null)
                {
                    dtLoc.Clear();
                    dtLoc.Dispose();
                    dtLoc = null;
                }
            }
        }
    }
}
