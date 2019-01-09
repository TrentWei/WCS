using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Mirle.ASRS;

namespace Mirle
{
    public class DBOracle : IDisposable
    {
        public DBOracle()
        {
        }

        public enum TransactionType
        {
            Begin,
            Commit,
            Rollback
        }

        public enum DatabaseType
        {
            Oracle_OracleClient,
        }

        public struct DataBaseConfig
        {
            public DatabaseType DBType;
            public string DBServer;
            public string FODBServer;
            public string DBUser;
            public string DBPassword;
            public string DBName;
            public string ConnTimeOut;
            public string DBPort;
            public int CommandTimeOut;

            public void setParameters(DatabaseType dbType, string dbName, string dbServer, string FOdbServer, string dbUser, string dbPassword, string dbConnTimeOut, string dbPort, int dbCommandTimeOut)
            {
                this.DBType = dbType;
                this.DBName = dbName;
                this.DBServer = dbServer;
                this.FODBServer = FOdbServer;
                this.DBUser = dbUser;
                this.DBPassword = dbPassword;
                this.ConnTimeOut = dbConnTimeOut;
                this.DBPort = dbPort;
                this.CommandTimeOut = dbCommandTimeOut;
            }
        }

        private OracleConnection _dbConn_Oracle = new OracleConnection();
        private OracleTransaction _dbTran_Oracle;

        private DataBaseConfig _DBConfig;
        private Exception _ExceptMsg;


        private bool disposedValue = false;

        private static object _Lock = new object();
        private string _DisconnectExMsg = "ExecuteReader 必須有開啟與可用的 Connection。連接目前的狀態已關閉。|ExecuteScalar 必須有開啟與可用的 Connection。連接目前的狀態已關閉。|ExecuteNonQuery 必須有開啟與可用的 Connection。連接目前的狀態已關閉。|作業無效。已經關閉連接。";

        public DatabaseType DBType
        {
            get { return _DBConfig.DBType; }
            set { _DBConfig.DBType = value; }
        }

        public string DBName
        {
            get { return _DBConfig.DBName; }
            set { _DBConfig.DBName = value; }
        }

        public string DBServer
        {
            get { return _DBConfig.DBServer; }
            set { _DBConfig.DBServer = value; }
        }

        public string FODBerver
        {
            get { return _DBConfig.FODBServer; }
            set { _DBConfig.FODBServer = value; }
        }

        public string DBUser
        {
            get { return _DBConfig.DBUser; }
            set { _DBConfig.DBUser = value; }
        }

        public string DBPassword
        {
            get { return _DBConfig.DBPassword; }
            set { _DBConfig.DBPassword = value; }
        }

        public string DBConnTimeOut
        {
            get { return _DBConfig.ConnTimeOut; }
            set { _DBConfig.ConnTimeOut = value; }
        }

        public string DBPort
        {
            get { return _DBConfig.DBPort; }
            set { _DBConfig.DBPort = value; }
        }

        public int DBCommandTimeOut
        {
            get { return _DBConfig.CommandTimeOut; }
            set { _DBConfig.CommandTimeOut = value; }
        }

        public bool ConnFlag
        {
            get
            {
                bool bolTemp = false;
                string sSql = string.Empty;

                if (!(_dbConn_Oracle==null))
                {
                    if (_dbConn_Oracle.State == ConnectionState.Open)
                        bolTemp = true;
                }

                if (bolTemp)
                {
                    sSql = "SELECT * FROM DUAL";
                    DataTable dataBase = null;
                    GetDataTable(sSql, ref dataBase);
                    if (_ExceptMsg != null)
                        bolTemp = false;
                    dataBase.Clear();
                    dataBase.Dispose();
                    dataBase = null;
                }
                return bolTemp;
            }
        }

        public bool Open(ref string errorMsg)
        {
            string strConnectString = string.Empty;
            bool RetCode = false;
            _ExceptMsg = null;

            try
            {
                strConnectString = @"Data Source=(DESCRIPTION";
                strConnectString += " = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = " + _DBConfig.DBServer + ")";
                strConnectString += "(PORT = " + _DBConfig.DBPort + ")))(CONNECT_DATA = (SERVICE_NAME = " + _DBConfig.DBName + ")));";
                strConnectString += "Persist Security Info=True;User ID=" + _DBConfig.DBUser + ";";
                strConnectString += "Password = " + _DBConfig.DBPassword + ";";
                _dbConn_Oracle = new OracleConnection(strConnectString);
                _dbConn_Oracle.Open();
                if (_dbConn_Oracle.State == ConnectionState.Open)
                    RetCode = true;
                else
                {
                    errorMsg = "Initial Fail";
                    RetCode = false;
                    InitSys.funWriteLog("Exception:"+errorMsg, strConnectString);
                }
            }
            catch (OracleException ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message + "|" + strConnectString);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);

            }
            return RetCode;
        }

        public bool Open()
        {
            string strEM = string.Empty;
            return Open(ref strEM);
        }

        public bool Close()
        {
            bool RetCode = false;
            _ExceptMsg = null;

            try
            {
                if (!(_dbConn_Oracle ==null))
                    _dbConn_Oracle.Close();

                RetCode = true;
            }
            catch (OracleException ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message);
            }
            catch (Exception ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);
            }
            return RetCode;
        }

        public bool Reconnect()
        {
            bool RetCode = false;
            try
            {
                _dbConn_Oracle.Open();
                RetCode=true;
            }
            catch (OracleException ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message );
            }
            catch (Exception ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);
            }
            return RetCode;
        }

        public bool CommitCtrl(TransactionType objType, ref string errorMsg)
        {
            bool RetCode = false;

            errorMsg = "Initial Fail";
            _ExceptMsg = null;

            try
            {
                switch (objType)
                {
                    case TransactionType.Begin:
                        _dbTran_Oracle = _dbConn_Oracle.BeginTransaction();
                        break;
                    case TransactionType.Commit:
                        _dbTran_Oracle.Commit();
                        _dbTran_Oracle.Dispose();
                        _dbTran_Oracle = null;

                        break;
                    case TransactionType.Rollback:
                        _dbTran_Oracle.Rollback();
                        _dbTran_Oracle.Dispose();
                        _dbTran_Oracle = null;
                        break;
                }
                RetCode=true; errorMsg = string.Empty;
            }
            catch (OracleException ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);

                if (_DisconnectExMsg.Contains(ex.Message))
                {
                    if (Reconnect() ==true)
                        RetCode = CommitCtrl(objType, ref errorMsg);
                }
            }
            return RetCode;
        }

        public bool CommitCtrl(TransactionType transactionType)
        {
            string strEM = string.Empty;
            return CommitCtrl(transactionType, ref strEM);
        }

        public bool ExecuteSQL(string SQL, ref string errorMsg)
        {
            bool RetCode = false;
            _ExceptMsg = null;

            try
            {
                lock (_Lock)
                {
                    OracleCommand dbCommand_Oracle = new OracleCommand(SQL, _dbConn_Oracle);
                    dbCommand_Oracle.Transaction = _dbTran_Oracle;
                    dbCommand_Oracle.CommandTimeout = _DBConfig.CommandTimeOut;

                    int executeCount = dbCommand_Oracle.ExecuteNonQuery();
                    if (executeCount <= 0)
                    {
                        RetCode = false;
                        errorMsg = "No Data Update";
                    }
                    else
                    {
                        RetCode=true;
                        errorMsg = "";
                    }
                    dbCommand_Oracle.Dispose();
                    dbCommand_Oracle = null;
                }
            }
            catch (OracleException ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);

                if (_DisconnectExMsg.Contains(ex.Message))
                {
                    if (Reconnect() == true)
                        RetCode = ExecuteSQL(SQL, ref errorMsg);
                }
            }
            return RetCode;
        }

        public bool ExecuteSQL(string SQL)
        {
            string strEM = string.Empty;
            return ExecuteSQL(SQL, ref strEM);
        }

        public bool ExecuteSQL(string SQL, ref int executeCount, ref string errorMsg)
        {
            bool RetCode = false;
            executeCount = 0;
            _ExceptMsg = null;
            try
            {
                lock (_Lock)
                {
                    OracleCommand dbCommand_Oracle = new OracleCommand(SQL, _dbConn_Oracle);
                    dbCommand_Oracle.Transaction = _dbTran_Oracle;
                    dbCommand_Oracle.CommandTimeout = _DBConfig.CommandTimeOut;

                    executeCount = dbCommand_Oracle.ExecuteNonQuery();
                    if (executeCount <= 0)
                    {
                        RetCode = false;
                        errorMsg = "No Data Update";
                    }
                    else
                    {
                        RetCode=true;
                        errorMsg = "";
                    }
                    dbCommand_Oracle.Dispose();
                    dbCommand_Oracle = null;
                }
            }
            catch (OracleException ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message );
            }
            catch (Exception ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);

                if (_DisconnectExMsg.Contains(ex.Message))
                {
                    if (Reconnect() == false)
                        RetCode = ExecuteSQL(SQL, ref executeCount, ref errorMsg);
                }
            }
            return RetCode;
        }

        public bool ExecuteSQL(string SQL, ref int executeCount)
        {
            string strEM = string.Empty;
            return ExecuteSQL(SQL, ref executeCount, ref strEM);
        }

        public bool GetDataTable(string SQL, ref DataTable dataBase, ref string errorMsg)
        {
            bool RetCode = false;
            errorMsg = "Initial Fail";
            _ExceptMsg = null;

            dataBase = (dataBase == null) ? new DataTable() : dataBase;

            try
            {
                lock (_Lock)
                {
                    OracleDataAdapter dbDataAdapter_Oracle = new OracleDataAdapter();
                    dbDataAdapter_Oracle.SelectCommand = new OracleCommand(SQL, _dbConn_Oracle);
                    dbDataAdapter_Oracle.SelectCommand.CommandTimeout = _DBConfig.CommandTimeOut;
                    dbDataAdapter_Oracle.SelectCommand.Transaction = _dbTran_Oracle;
                    DataSet dtDataSet = new DataSet();
                    dbDataAdapter_Oracle.Fill(dtDataSet, "Data");
                    dataBase = dtDataSet.Tables["Data"];
                    if (dataBase.Rows.Count > 0)
                    {
                        RetCode=true;
                        errorMsg = string.Empty;
                    }
                    else
                    {
                        RetCode = false;
                        errorMsg = "No Data Selected";
                    }
                }
            }
            catch (OracleException ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception:" + methodBase.DeclaringType.FullName + "|" + methodBase.Name, "|Code:" + ex.ErrorCode + "|Message:" + ex.Message);
            }
            catch (Exception ex)
            {
                _ExceptMsg = ex;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog(methodBase.DeclaringType.FullName, ex.Message);

                if (_DisconnectExMsg.Contains(ex.Message))
                {
                    if (Reconnect() == true)
                        RetCode = GetDataTable(SQL, ref dataBase, ref errorMsg);
                }
            }
            return RetCode;
        }

        public bool GetDataTable(string SQL, ref DataTable dataBase)
        {
            string errorMsg = string.Empty;
            return GetDataTable(SQL, ref dataBase, ref errorMsg);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._dbConn_Oracle.Dispose();
                    ((IDisposable)this._dbTran_Oracle).Dispose();
                }
                disposedValue = true;
            }
        }

        ~DBOracle()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
