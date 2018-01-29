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
        private static string strAGV_StoreIn_SPLCAddress = string.Empty;
        private static string strAGV_StoreOut_SPLCAddress = string.Empty;
        private static int intTotalAddress = 0;
        private static int intCraneNo = 1;
        private static DB dBServer = new DB();
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

        public static int _TotalAddress
        {
            get { return intTotalAddress; }
        }

        public static int _CraneNo
        {
            get { return intCraneNo; }
        }

        public static DB _DB
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
                strAGV_StoreIn_SPLCAddress = ConfigurationManager.AppSettings["AGV_StoreIn_SPLCAddress"];
                strAGV_StoreOut_SPLCAddress = ConfigurationManager.AppSettings["AGV_StoreOut_SPLCAddress"];
                intTotalAddress = int.Parse(ConfigurationManager.AppSettings["TotalAddress"]);
                intCraneNo = int.Parse(ConfigurationManager.AppSettings["CraneNo"]);
                archive = new Archive(Application.StartupPath + @"\LOG\", 10000, Application.StartupPath + @"\LOG\", 7, 90);
                archive.funStart();
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        public static bool funOpendDB()
        {
            string strEM = string.Empty;
            dBServer = new DB(ConfigurationManager.ConnectionStrings["ASRS"].ConnectionString);
            if(dBServer.funOpenDB(ref strEM))
                return true;
            else
                return false;
        }

        public static bool funOpendMPLC()
        {
            string strEM = string.Empty;
            mPLC = new MPLC(int.Parse(ConfigurationManager.AppSettings["StationNumber"]));
            if(mPLC.funOpenMPLC(ref strEM))
                return true;
            else
                return false;
        }

        public static bool funOpendSPLC()
        {
            string strEM = string.Empty;
            sPLC = new SPLC(S7.Net.CpuType.S7300, "192.168.1.21", 0, 2, 6);
            if(sPLC.funOpenSPLC(ref strEM))
                return true;
            else
                return false;
        }

        public static void funWriteLog(string fileName, string logMsg)
        {
            try
            {
                string strFile = fileName + ".log";
                string strFilePath = Application.StartupPath + @"\LOG\" + DateTime.Now.ToString("yyyy-MM-dd");

                if(!Directory.Exists(strFilePath))
                    Directory.CreateDirectory(strFilePath);

                logMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logMsg;
                if(!File.Exists(strFilePath + @"\" + strFile))
                {
                    using(StreamWriter swrWriter = File.CreateText(strFilePath + @"\" + strFile))
                    {
                        swrWriter.WriteLine(logMsg);
                        swrWriter.Flush();
                        swrWriter.Close();
                    }
                }
                else
                {
                    using(StreamWriter swrWriter = File.AppendText(strFilePath + @"\" + strFile))
                    {
                        swrWriter.WriteLine(logMsg);
                        swrWriter.Flush();
                        swrWriter.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                string strErrMsg = ex.Message;
            }
        }
        #endregion Function
    }
}
