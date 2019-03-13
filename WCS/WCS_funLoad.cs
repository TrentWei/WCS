using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    partial  class WCS
    {

        private bool funRef()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;

            try
            {
              
                strSQL = string.Format("update CtrlHs set Hs='0' where EquNo='{0}'", "A1");
                
                if (InitSys._DB.ExecuteSQL(strSQL, ref strEM))
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
        }
        public bool funLoda()
        {
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            try
            {
                string strSql = string.Format("select * from CtrlHs where EquNo='{0}'", "A1");
                if (InitSys._DB.GetDataTable(strSql, ref dtCmdSno, ref strEM))
                {
                    if (dtCmdSno.Rows[0]["HS"].ToString() == "0")
                    {
                        strSql = string.Format("update CtrlHs set Hs='1' where EquNo='{0}'", "A1");
                        return InitSys._DB.ExecuteSQL(strSql, ref strEM);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    strSql = string.Format("insert into ctrlhs(HS,EquNo,trndt) values ('1','{0}','2015-10-10 12:00:00')", "A1");
                    return InitSys._DB.ExecuteSQL(strSql, ref strEM);
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
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }
    }
}
