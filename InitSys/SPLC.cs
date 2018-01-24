using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using S7.Net;

namespace Mirle.ASRS
{
    public class SPLC
    {
        public class Tag
        {
            private string strItemName;
            private object objitemValue;

            public string _ItemName
            {
                get { return strItemName; }
                set { strItemName = value; }
            }

            public object _ItemValue
            {
                get { return objitemValue; }
                set { objitemValue = value; }
            }

            public Tag()
            {

            }

            public Tag(string itemName)
            {
                strItemName = itemName;
            }

            public Tag(string itemName, object itemValue)
            {
                strItemName = itemName;
                objitemValue = itemValue;
            }
        }

        #region 變數
        private Plc sPLC;
        private string strIPAddress = "127.0.0.1";
        private CpuType cpu = CpuType.S7200;
        private bool bolIsConnection = false;
        private short intRack = 0;
        private short intSolt = 2;
        private int intDb = 0;
        #endregion 變數

        public bool _IsConnection
        {
            get
            {
                if(sPLC != null)
                    return bolIsConnection;
                else
                    return bolIsConnection = false;
            }
        }

        public SPLC()
        {
        }

        public SPLC(CpuType cpuType, string iPAddress, short rack, short slot, int db)
        {
            cpu = cpuType;
            strIPAddress = iPAddress;
            intRack = rack;
            intSolt = slot;
            intDb = db;
        }

        #region Function

        #region Opend/Close
        public bool funOpenSPLC(ref string errMsg)
        {
            try
            {
                sPLC = new Plc(cpu, strIPAddress, intRack, intSolt);
                ErrorCode error = sPLC.Open();
                if(error == ErrorCode.NoError)
                {
                    InitSys.funWriteLog("SPLC_Trace", "Open Success!");
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("SPLC_Trace", "Open Fail!|" + errMsg);
                    return bolIsConnection = false;
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return bolIsConnection = false;
            }
        }

        public void funClose()
        {
            try
            {
                if(sPLC != null)
                {
                    sPLC.Close();
                    sPLC.Dispose();
                    sPLC = null;
                    bolIsConnection = false;
                    InitSys.funWriteLog("SPLC_Trace", "Close!");
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                bolIsConnection = false;
            }
        }
        #endregion Opend/Close

        #region Read/Write
        public bool funReadSPLC(object sourceClass)
        {
            try
            {
                sPLC.ReadClass(sourceClass, intDb);
                return true;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return bolIsConnection = false;
            }
        }

        public bool funWriteSPLC(Tag tag)
        {
            try
            {
                object value = tag._ItemValue;
                if(tag._ItemValue is double)
                {
                    var bytes = S7.Net.Types.Double.ToByteArray((double)tag._ItemValue);
                    value = S7.Net.Types.DWord.FromByteArray(bytes);
                }
                else if(tag._ItemValue is bool)
                    value = (bool)tag._ItemValue ? 1 : 0;

                var result = sPLC.Write(tag._ItemName, value);
                if(result is ErrorCode && (ErrorCode)result != ErrorCode.NoError)
                {
                    InitSys.funWriteLog("SPLC_Trace", tag._ItemName + "|" + tag._ItemValue + "|WriteDeviceBlock Success!");
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("SPLC_Trace", tag._ItemName + "|" + tag._ItemValue + "|WriteDeviceBlock Fail!");
                    return bolIsConnection = false;
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return bolIsConnection = false;
            }
        }
        #endregion Read/Write

        #endregion Function
    }
}
