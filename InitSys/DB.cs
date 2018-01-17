using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public class DB
    {
        public enum TransactionType
        {
            Begin,
            Commit,
            Rollback
        }

        private static object objLock = new object();
        private string strConnectionString = string.Empty;
        private SqlConnection scnDBConnection = new SqlConnection();
        private SqlTransaction stnTransaction;

        public bool _IsConnection
        {
            get
            {
                if(scnDBConnection != null)
                {
                    if(scnDBConnection.State == ConnectionState.Open)
                        return true;
                }
                return false;
            }
        }

        public DB()
        {
        }

        public DB(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public bool funOpenDB(ref string errMsg)
        {
            try
            {
                scnDBConnection = new SqlConnection(strConnectionString);
                scnDBConnection.Open();
                return true;
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + errMsg);
                return false;
            }
        }

        public void funClose()
        {
            try
            {
                if(scnDBConnection != null)
                {
                    scnDBConnection.Close();
                    scnDBConnection = null;
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        public bool funGetDT(string sQL, ref DataTable dtData, ref string errMsg)
        {
            try
            {
                dtData = new DataTable();
                dtData.Clear();
                lock(objLock)
                {
                    SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(sQL, scnDBConnection);
                    sdaDataAdapter.SelectCommand.Transaction = stnTransaction;
                    sdaDataAdapter.Fill(dtData);
                    if(dtData.Rows.Count > 0)
                    {
                        errMsg = string.Empty;
                        return true;
                    }
                    else
                    {
                        errMsg = "No Data Selected";
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + errMsg);
                return false;
            }
        }

        public bool funExecSql(string sQL, ref string errMsg)
        {
            try
            {
                lock(objLock)
                {
                    SqlCommand scdCommand = new SqlCommand(sQL, scnDBConnection, stnTransaction);
                    if(scdCommand.ExecuteNonQuery() > 0)
                    {
                        errMsg = string.Empty;
                        return true;
                    }
                    else
                    {
                        errMsg = "No Data Execute";
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + errMsg);
                return false;
            }
        }

        public bool funCommitCtrl(TransactionType transaction)
        {
            try
            {
                switch(transaction)
                {
                    case TransactionType.Begin:
                        stnTransaction = scnDBConnection.BeginTransaction();
                        break;
                    case TransactionType.Commit:
                        stnTransaction.Commit();
                        stnTransaction.Dispose();
                        stnTransaction = null;
                        break;
                    case TransactionType.Rollback:
                        stnTransaction.Rollback();
                        stnTransaction.Dispose();
                        stnTransaction = null;
                        break;
                }
                return true;
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
