using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Mirle.ASRS
{
    public class InitSys
    {
        #region 變數
        private static string strAPName = string.Empty;
        private static string strStartAddress = string.Empty;
        private static int intTotalAddress = 0;

        private static string strDBName = string.Empty;
        private static string strDBServer = string.Empty;
        private static string strDBPort = string.Empty;
        private static string strFODBerver = string.Empty;
        private static string strDBUser = string.Empty;
        private static string strDBPassword = string.Empty;
        private static string strDBConnTimeOut = string.Empty;
        private static int intDBCommandTimeOut = 0;

        private static string strAGV_StoreIn_SPLCAddress = string.Empty;
        private static string strAGV_StoreOut_SPLCAddress = string.Empty;
        private static int intAGV_StoreIn_MPLCBufferIndex = 0;
        private static int intAGV_StoreOut_MPLCBufferIndex = 0;
        private static int intAGV_GetWirteSPLCStartIndex = 0;


        private static DBOracle dBServer = new DBOracle();

        private static MPLC mPLC = new MPLC();
        private static SPLC sPLC = new SPLC();
        private static Archive archive = new Archive();
        #endregion 變數

        #region 属性
        public static string _APName
        {
            get { return strAPName; }
        }

        public static string _StartAddress
        {
            get { return strStartAddress; }
        }

        public static string _AGV_StoreIn_SPLCAddress
        {
            get { return strAGV_StoreIn_SPLCAddress; }
        }

        public static string _AGV_StoreOut_SPLCAddress
        {
            get { return strAGV_StoreOut_SPLCAddress; }
        }

        public static int _AGV_StoreIn_MPLCBufferIndex
        {
            get { return intAGV_StoreIn_MPLCBufferIndex; }
        }

        public static int _AGV_StoreOut_MPLCBufferIndex
        {
            get { return intAGV_StoreOut_MPLCBufferIndex; }
        }

        public static int _AGV_GetWirteSPLCStartIndex
        {
            get { return intAGV_GetWirteSPLCStartIndex; }
        }

        public static int _TotalAddress
        {
            get { return intTotalAddress; }
        }

        public static DBOracle _DB
        {
            get { return dBServer; }
        }

        public static MPLC _MPLC
        {
            get { return mPLC; }
        }

        public static SPLC _SPLC
        {
            get { return sPLC; }
        }
        #endregion 属性

        #region Function
        public static void funItialSys()
        {
            try
            {
                strAPName = ConfigurationManager.AppSettings["APName"];
                strStartAddress = ConfigurationManager.AppSettings["StartAddress"];
                intTotalAddress = int.Parse(ConfigurationManager.AppSettings["TotalAddress"]);


                strDBName = ConfigurationManager.AppSettings["DBName"];
                strDBServer = ConfigurationManager.AppSettings["DBServer"];
                strDBPort = ConfigurationManager.AppSettings["DBPort"];
                strFODBerver = ConfigurationManager.AppSettings["FODBerver"];
                strDBUser = ConfigurationManager.AppSettings["DBUser"];
                strDBPassword = ConfigurationManager.AppSettings["DBPassword"];
                strDBConnTimeOut = ConfigurationManager.AppSettings["DBConnTimeOut"];
                intDBCommandTimeOut = int.Parse(ConfigurationManager.AppSettings["DBCommandTimeOut"]);


                #region 鲁达AGV部分
                //strAGV_StoreIn_SPLCAddress = ConfigurationManager.AppSettings["AGV_StoreIn_SPLCAddress"];
                //strAGV_StoreOut_SPLCAddress = ConfigurationManager.AppSettings["AGV_StoreOut_SPLCAddress"];
                //intAGV_StoreIn_MPLCBufferIndex = int.Parse(ConfigurationManager.AppSettings["AGV_StoreIn_MPLCBufferIndex"]);
                //intAGV_StoreOut_MPLCBufferIndex = int.Parse(ConfigurationManager.AppSettings["AGV_StoreOut_MPLCBufferIndex"]);
                //intAGV_GetWirteSPLCStartIndex = int.Parse(ConfigurationManager.AppSettings["AGV_GetWirteSPLCStartIndex"]);
                #endregion


                archive = new Archive(Application.StartupPath + @"\LOG\", 10000, Application.StartupPath + @"\LOG\", 7, 90);
                archive.funStart();
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.StackTrace);
            }
        }

        public static bool funOpendDB()
        {
            try
            {
                string strEM = string.Empty;
                dBServer = new DBOracle(DBOracle.DatabaseType.Oracle_OracleClient
                    , strDBName
                    , strDBServer
                    , strFODBerver
                    , strDBUser
                    , strDBPassword
                    , strDBConnTimeOut
                    , strDBPort
                    , intDBCommandTimeOut);
                if (dBServer.Open(ref strEM))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        public static bool funOpendMPLC()
        {
            try
            {
                string strEM = string.Empty;
                mPLC = new MPLC(int.Parse(ConfigurationManager.AppSettings["StationNumber"]));
                if (mPLC.funOpenMPLC(ref strEM))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        public static bool funOpendSPLC()
        {
            try
            {
                string strEM = string.Empty;
                string strIP = ConfigurationManager.AppSettings["AGV_Connection_IP"];
                short intRack = short.Parse(ConfigurationManager.AppSettings["AGV_Connection_Rack"]);
                short intSlot = short.Parse(ConfigurationManager.AppSettings["AGV_Connection_Slot"]);
                int intDB = int.Parse(ConfigurationManager.AppSettings["AGV_Connection_DB"]);
                sPLC = new SPLC(S7.Net.CpuType.S7300, strIP, intRack, intSlot, intDB);
                if (sPLC.funOpenSPLC(ref strEM))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }

        public static void funWriteLog(string fileName, string logMsg)
        {
            try
            {
                string strFile = fileName + ".log";
                string strFilePath = Application.StartupPath + @"\LOG\" + DateTime.Now.ToString("yyyy-MM-dd");

                if (!Directory.Exists(strFilePath))
                    Directory.CreateDirectory(strFilePath);

                logMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logMsg;
                if (!File.Exists(strFilePath + @"\" + strFile))
                {
                    using (StreamWriter swrWriter = File.CreateText(strFilePath + @"\" + strFile))
                    {
                        swrWriter.WriteLine(logMsg);
                        swrWriter.Flush();
                        swrWriter.Close();
                    }
                }
                else
                {
                    using (StreamWriter swrWriter = File.AppendText(strFilePath + @"\" + strFile))
                    {
                        swrWriter.WriteLine(logMsg);
                        swrWriter.Flush();
                        swrWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrMsg = ex.Message;
            }
        }

        public static void funWriteHtml(string sSTN_NO, string sHTML)
        {
            try
            {
                string sFileName = null;
                sFileName = @"D:\TV_WS";
                if (System.IO.Directory.Exists(sFileName) == false)
                {
                    System.IO.Directory.CreateDirectory(sFileName);
                }
                string sFile = "";
                sFile = sSTN_NO + ".html";
                sFileName = sFileName + "\\" + sFile;
                if (System.IO.File.Exists(sFileName) == false)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(sFileName))
                    {
                        sw.WriteLine(sHTML);
                        sw.Close();
                    }
                }
                else
                {
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(sFileName))
                    {
                        sw.WriteLine(sHTML);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch
            {

            }
        }
        #endregion Function
    }
}
