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
        private bool funCheckExistsAGVSchedule()
        {
            DataTable dtProduce = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
                strSQL = "SELECT * FROM PRODUCE ";
                strSQL += " WHERE STATUS='1'";
                strSQL += " AND Wh_NO='A'";
                strSQL += " ORDER BY PRODUCE_NO";
                if(InitSys._DB.GetDataTable(strSQL, ref dtProduce, ref strEM))
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
            finally
            {
                if(dtProduce != null)
                {
                    dtProduce.Clear();
                    dtProduce.Dispose();
                    dtProduce = null;
                }
            }
        }

        private bool funCheckExistsAGVSchedule(string strCmdsno)
        {
            DataTable dtProduce = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;
            int intCmdSno = int.Parse(strCmdsno);
            try
            {
                strSQL = "SELECT * FROM PRODUCE ";
                strSQL += " WHERE STATUS='1'";
                strSQL += " AND Cmd_Sno='"+ intCmdSno.ToString("00000") + "'";
                strSQL += " ORDER BY PRODUCE_NO";
                if (InitSys._DB.GetDataTable(strSQL, ref dtProduce, ref strEM))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
            finally
            {
                if (dtProduce != null)
                {
                    dtProduce.Clear();
                    dtProduce.Dispose();
                    dtProduce = null;
                }
            }
        }

        private bool funUpdateProdecu(string commandID, string status)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            int comsno = int.Parse(commandID);
            try
            {
                strSQL = "UPDATE PRODUCE ";
                strSQL += " SET STATUS='" + status + "'";
                strSQL += " WHERE Cmd_Sno='" + comsno.ToString("00000") + "'";
                if(InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        private bool funUpdateProdecu(string prodecuNo, string status, string commandID)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
                strSQL = "UPDATE PRODUCE ";
                strSQL += " SET STATUS='" + status + "',";
                strSQL += " Cmd_Sno='" + commandID + "'";
                strSQL += " WHERE PRODUCE_NO='" + prodecuNo + "'";
                if(InitSys._DB.ExecuteSQL(strSQL, ref strEM))
                    return true;
                else
                    return false;
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
