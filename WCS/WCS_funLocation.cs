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
                strSQL += setLocationState == "N" ? " Plt_No=''," : "Plt_No='" + setPalletNo + "',";
                strSQL += " Loc_OSts=Loc_Sts,";
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Loc='" + location + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }
        private bool funUpdateLocationDtl(string location, string Newlocation)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                strSQL = "UPDATE LOC_DTL";
                strSQL += " SET LOC='" + Newlocation + "',";;
                strSQL += " Trn_Dte='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                strSQL += " WHERE Loc='" + location + "'";
                return InitSys._DB.ExecuteSQL(strSQL, ref strEM);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 站口分配
        /// </summary>
        /// <returns></returns>
        private bool funGetCraneNo(ref string strCrnNo, ref string strStnNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                if (inStoreInStnNoIndex < 0) inStoreInStnNoIndex = 4;
                foreach (StationInfo stnDef in lstAllotCrane)
                {
                    
                    int intBufferIndex = stnDef.BufferIndex;
                    int intStnIndex = stnDef.StationIndex;
                    string strBufferName = stnDef.BufferName;

                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                   string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                   bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                   bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On &&
                   intStnIndex == inStoreInStnNoIndex)
                    {
                        strCrnNo = (inStoreInStnNoIndex + 1).ToString();
                        strStnNo = strBufferName;
                        inStoreInStnNoIndex -= 1;
                        if (strStnNo == "A07") strStnNo = STN_NO.StoreInA113;
                        if (strStnNo == "A15") strStnNo = STN_NO.StoreInA108;
                        if (strStnNo == "A23") strStnNo = STN_NO.StoreInA103;
                        if (strStnNo == "A31") strStnNo = STN_NO.StoreInA98;
                        if (strStnNo == "A39") strStnNo = STN_NO.StoreInA93;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funGetEmptyLocation(string Loc_Size, string strSnoNo, ref string strLoaction)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLocation = new DataTable();

            int inCraneNo = Convert.ToInt32(strSnoNo);
            try
            {
                //if (!funGetCraneNo(strSnoNo, ref inCraneNo))
                //{
                //    return false;
                //}
                strSQL = "select x.LOC,x.LOC_SIZE,case x.LOC_SIZE when 'L' then 1 when 'H' then 2  end as GAODU from LOC_MST x where x.LOC_STS='N' ";
                strSQL += Loc_Size == "L" ? "and x.LOC_SIZE in('L','H') " : "and x.LOC_SIZE in('H') ";
                strSQL += "and x.ROW_X >=('" + inCraneNo + "'-1)*4+1 and x.ROW_X <= '" + inCraneNo + "'*4 ";
                strSQL += "and exists (select y.LOC,y.LOC_SIZE from LOC_MST y where y.LOC_STS ='N' ";
                strSQL += "and (x.ROW_X=y.ROW_X + 2  or x.ROW_X = ('" + inCraneNo + "'-1)*4+1 or x.ROW_X= ('" + inCraneNo + "'-1)*4 +2 )and x.BAY_Y =y.BAY_Y and x.LVL_Z=y.LVL_Z) ";
                strSQL += "order by  ROW_X desc, GAODU asc, LVL_Z asc,BAY_Y asc  ";
                if (InitSys._DB.GetDataTable(strSQL, ref dtLocation, ref strEM))
                {
                    foreach (DataRow drLoc in dtLocation.Rows)
                    {
                        strLoaction = drLoc["LOC"].ToString();
                        if (funIsInnerLoc(strLoaction))
                        {
                            string strOutLoc = funGetOutLoc(strLoaction);
                            DataTable dt = new DataTable();
                            strSQL = "SELECT * FROM LOC_MST WHERE LOC_STS IN ('N','O','I','C') AND LOC='" + strOutLoc + "' ";
                            if (InitSys._DB.GetDataTable(strSQL, ref dt, ref strEM))
                            {
                                strLoaction = string.Empty;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            string strInLoc = funGetInnerLoc(strLoaction);
                            DataTable dt = new DataTable();
                            strSQL = "SELECT * FROM LOC_MST WHERE LOC_STS IN ('S','E','O','I','C','X') AND LOC='" + strInLoc + "' ";
                            if (InitSys._DB.GetDataTable(strSQL, ref dt, ref strEM))
                            {
                                strLoaction = string.Empty;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return (!string.IsNullOrEmpty(strLoaction));
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
                if (dtLocation != null)
                {
                    dtLocation.Clear();
                    dtLocation.Dispose();
                    dtLocation = null;
                }
            }
        }


        private bool funGetEmptyLocationToLocation(string Loc_Size, string strOldLoaction, ref string strLoaction)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtLocation = new DataTable();
            int inCraneNo = funGetEquFLoc(strOldLoaction);
            int inROW_X = Convert.ToInt32(strOldLoaction.Substring(0,2));
            try
            {
             
                strSQL = "select x.LOC,x.LOC_SIZE,case x.LOC_SIZE when 'L' then 1 when 'H' then 2  end as GAODU from LOC_MST x where x.LOC_STS='N' ";
                strSQL += Loc_Size == "L" ? "and x.LOC_SIZE in('L','H') " : "and x.LOC_SIZE in('H') ";
                strSQL += "and x.ROW_X >=('" + inCraneNo + "'-1)*4+1 and x.ROW_X <= '" + inCraneNo + "'*4 ";
                strSQL += "and exists (select y.LOC,y.LOC_SIZE from LOC_MST y where y.LOC_STS ='N' ";
                strSQL += "and (x.ROW_X=y.ROW_X + 2  or x.ROW_X = ('" + inCraneNo + "'-1)*4+1 or x.ROW_X= ('" + inCraneNo + "'-1)*4 +2 )and x.BAY_Y =y.BAY_Y and x.LVL_Z=y.LVL_Z) ";
                
                if (inROW_X % 2 == 0)
                {
                    strSQL += "order by  case when ROW_X-(ROW_X/2)*2 then 0 else 1 end asc,ROW_X desc, GAODU asc, LVL_Z asc,BAY_Y asc  ";
                }
                else
                {
                    strSQL += "order by   case when ROW_X-(ROW_X/2)*2 then 0 else 1 end desc,ROW_X desc, GAODU asc, LVL_Z asc,BAY_Y asc  ";
                }
                if (InitSys._DB.GetDataTable(strSQL, ref dtLocation, ref strEM))
                {
                    foreach (DataRow drLoc in dtLocation.Rows)
                    {
                        strLoaction = drLoc["LOC"].ToString();
                        if (funIsInnerLoc(strLoaction))
                        {
                            string strOutLoc = funGetOutLoc(strLoaction);
                            DataTable dt = new DataTable();
                            strSQL = "SELECT * FROM LOC_MST WHERE LOC_STS IN ('N','O','I','C') AND LOC='" + strOutLoc + "' ";
                            if (InitSys._DB.GetDataTable(strSQL, ref dt, ref strEM))
                            {
                                strLoaction = string.Empty;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            string strInLoc = funGetInnerLoc(strLoaction);
                            DataTable dt = new DataTable();
                            strSQL = "SELECT * FROM LOC_MST WHERE LOC_STS IN ('S','E','O','I','C','X') AND LOC='" + strInLoc + "' ";
                            if (InitSys._DB.GetDataTable(strSQL, ref dt, ref strEM))
                            {
                                strLoaction = string.Empty;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return (!string.IsNullOrEmpty(strLoaction));
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
                if (dtLocation != null)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                strSQL = "SELECT * from LOC_MST where loc='" + loc + "' ";
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

        /// <summary>
        /// 获取出站口号
        /// </summary>
        /// <param name="strLoc"></param>
        /// <returns></returns>
        private string funGetStationNo(string strLoc)
        {
            string strStationNo = string.Empty;
            try
            {
                int nLoc = int.Parse(strLoc.Substring(0, 2));
                nLoc = (nLoc % 4 > 0 ? 1 : 0) + nLoc / 4;
                switch (nLoc)
                {
                    case CRANE_NO.Crane5:
                        strStationNo = STN_NO.StoreOutA01;
                        break;
                    case CRANE_NO.Crane4:
                        strStationNo = STN_NO.StoreOutA10;
                        break;
                    case CRANE_NO.Crane3:
                        strStationNo = STN_NO.StoreOutA18;
                        break;
                    case CRANE_NO.Crane2:
                        strStationNo = STN_NO.StoreOutA26;
                        break;
                    case CRANE_NO.Crane1:
                        strStationNo = STN_NO.StoreOutA34;
                        break;
                    default:
                        break;
                }
                return strStationNo;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return strStationNo;
            }
        }
        private bool funGetCraneNo(string StnNo, ref int CraneNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            try
            {
                switch (StnNo)
                {
                    case STN_NO.StoreInA113:
                        CraneNo = CRANE_NO.Crane5;
                        break;
                    case STN_NO.StoreOutA01:
                        CraneNo = CRANE_NO.Crane5;
                        break;
                    case STN_NO.StoreInA108:
                        CraneNo = CRANE_NO.Crane4;
                        break;
                    case STN_NO.StoreOutA10:
                        CraneNo = CRANE_NO.Crane4;
                        break;
                    case STN_NO.StoreInA103:
                        CraneNo = CRANE_NO.Crane3;
                        break;
                    case STN_NO.StoreOutA18:
                        CraneNo = CRANE_NO.Crane3;
                        break;
                    case STN_NO.StoreInA98:
                        CraneNo = CRANE_NO.Crane2;
                        break;
                    case STN_NO.StoreOutA26:
                        CraneNo = CRANE_NO.Crane2;
                        break;
                    case STN_NO.StoreInA93:
                        CraneNo = CRANE_NO.Crane1;
                        break;
                    case STN_NO.StoreOutA34:
                        CraneNo = CRANE_NO.Crane1;
                        break;
                    default:
                        break;
                }
                if (CraneNo == 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取主机号
        /// </summary>
        /// <param name="strLoc">LOC储位</param>
        /// <returns></returns>
        private int funGetEquFLoc(string strLoc)
        {
            int nLoc = int.Parse(strLoc.Substring(0, 2));
            nLoc = (nLoc % 4 > 0 ? 1 : 0) + nLoc / 4;
            return nLoc;
        }

        /// <summary>
        /// 获取主机储位号
        /// </summary>
        /// <param name="sLoc">LOC储位</param>
        /// <returns></returns>
        private string funGetCrnLoc(object sLoc)
        {
            string strLoc = sLoc.ToString();
            int nRow = 0;
            nRow = int.Parse(strLoc.Substring(0, 2));
            nRow = nRow % 4;
            if (nRow == 0) nRow = 4;
            strLoc = nRow.ToString("00") + strLoc.Substring(2, 4);
            return strLoc;
        }
        /// <summary>
        /// 判断是否是内储位
        /// </summary>
        /// <param name="sLoc"></param>
        /// <returns>内储位true,外储位false</returns>
        private bool funIsInnerLoc(string sLoc)
        {
            int nRow = int.Parse(sLoc.Substring(0, 2));
            nRow = nRow % 4;
            if (nRow == 1 || nRow == 2)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取内储位对应的外储位
        /// </summary>
        /// <param name="sLoc">内储位</param>
        /// <returns>外储位</returns>
        private string funGetOutLoc(string sLoc)
        {
            int nRow = int.Parse(sLoc.Substring(0, 2));
            nRow = int.Parse(sLoc.Substring(0, 2)) + 2;
            return nRow.ToString("00") + sLoc.Substring(2, 5);

        }
        /// <summary>
        /// 获取外储位对应的内储位
        /// </summary>
        /// <param name="sLoc">外储位，内储位</param>
        /// <returns>如果是内储位，则返回原储位，如果是外储位则寻找内储位</returns>
        private string funGetInnerLoc(string sLoc)
        {
            int nRow = int.Parse(sLoc.Substring(0, 2));
            nRow = nRow % 4;
            if (nRow == 0) nRow = 4;
            if (nRow == 1 || nRow == 2)
            {
                return sLoc;
            }
            else
            {
                nRow = int.Parse(sLoc.Substring(0, 2)) - 2;
                return nRow.ToString("00") + sLoc.Substring(2, 4);
            }
        }
        private bool funGetItemNoLocation(string palletNo, string Item_Type, ref string location)
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
                strSQL += " AND B.Item_Type='" + Item_Type.Trim() + "'";
                strSQL += " AND A.Plt_No='" + palletNo + "'";
                if (InitSys._DB.GetDataTable(strSQL, ref dtLoc, ref strEM))
                {
                    if (dtLoc.Rows.Count == 1)
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
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                location = string.Empty;
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
    }
}
