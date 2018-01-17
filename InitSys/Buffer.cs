using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class Buffer
    {
        #region 列舉
        public enum Signal
        {
            Off = 0,
            On = 1,
        }

        public enum StnMode
        {
            None = 0,
            StoreIn = 1,
            StoreOut = 2,
            Picking = 3,
        }
        #endregion 列舉

        private string strAddress = string.Empty;
        private string strBufferName = string.Empty;

        #region 屬性
        public string _BufferName
        {
            get { return strBufferName; }
        }

        public string _CommandID { get; set; }
        public string _Destination { get; set; }
        public StnMode _Mode { get; set; }
        public bool _ReturnRequest { get; set; }
        public EQU_Status_Signal _EQUStatus { get; set; }
        public EQU_Alarm_Status_Signal _EQUAlarmStatus { get; set; }
        public EQU_Status_Signal_Address _EQUStatusAddress { get; set; }
        public EQU_Alarm_Status_Signal_Address _EQUAlarmStatusAddress { get; set; }

        #region PLC位址
        public string _W_CmdSno
        {
            get { return strAddress; }
        }
        public string _W_Destination
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 1).ToString(); }
        }
        public string _W_Mode
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 2).ToString(); }
        }
        public string _W_ReturnRequest
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 3).ToString(); }
        }
        public string _W_LocSize
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 5).ToString(); }
        }
        #endregion PLC位址

        #endregion 屬性

        #region 結構
        public class EQU_Status_Signal
        {
            public Signal AutoMode { get; set; }
            public Signal Load { get; set; }

            public EQU_Status_Signal()
            {
                AutoMode = Signal.Off;
                Load = Signal.Off;
            }
        }

        public class EQU_Alarm_Status_Signal
        {
            public Signal EMO { get; set; }
            public Signal TransportMotorOverLoad { get; set; }
            public Signal TransportTimeout { get; set; }
            public Signal LiftMotorOverLoad { get; set; }
            public Signal LiftTimeout { get; set; }
            public Signal OverHigh { get; set; }
            public Signal DataNoLoad { get; set; }
            public Signal LoadNoData { get; set; }

            public EQU_Alarm_Status_Signal()
            {
                EMO = Signal.Off;
                TransportMotorOverLoad = Signal.Off;
                TransportTimeout = Signal.Off;
                LiftMotorOverLoad = Signal.Off;
                LiftTimeout = Signal.Off;
                OverHigh = Signal.Off;
                DataNoLoad = Signal.Off;
                LoadNoData = Signal.Off;
            }
        }

        #region PLC位址
        public class EQU_Status_Signal_Address
        {
            private string strAutoMode = string.Empty;
            private string strLoad = string.Empty;

            public string B_AutoMode { get { return strAutoMode; } }
            public string B_Load { get { return strLoad; } }

            public EQU_Status_Signal_Address(string address)
            {
                int intAddress = int.Parse(address.Remove(0, 1)) + 7;
                strAutoMode = "D" + intAddress.ToString() + ".0";
                strLoad = "D" + intAddress.ToString() + ".1";
            }
        }

        public class EQU_Alarm_Status_Signal_Address
        {
            private string strEMO = string.Empty;
            private string strTransportMotorOverLoad = string.Empty;
            private string strTransportTimeout = string.Empty;
            private string strLiftMotorOverLoad = string.Empty;
            private string strLiftTimeout = string.Empty;
            private string strOverHigh = string.Empty;
            private string strDataNoLoad = string.Empty;
            private string strLoadNoData = string.Empty;

            public string B_EMO { get { return strEMO; } }
            public string B_TransportMotorOverLoad { get { return strTransportMotorOverLoad; } }
            public string B_TransportTimeout { get { return strTransportTimeout; } }
            public string B_LiftMotorOverLoad { get { return strLiftMotorOverLoad; } }
            public string B_LiftTimeout { get { return strLiftTimeout; } }
            public string B_OverHigh { get { return strOverHigh; } }
            public string B_DataNoLoad { get { return strDataNoLoad; } }
            public string B_LoadNoData { get { return strLoadNoData; } }

            public EQU_Alarm_Status_Signal_Address(string address)
            {
                int intAddress = int.Parse(address.Remove(0, 1)) + 8;
                strEMO = "D" + intAddress.ToString() + ".0";
                strTransportMotorOverLoad = "D" + intAddress.ToString() + ".1";
                strTransportTimeout = "D" + intAddress.ToString() + ".2";
                strLiftMotorOverLoad = "D" + intAddress.ToString() + ".3";
                strLiftTimeout = "D" + intAddress.ToString() + ".4";
                strOverHigh = "D" + intAddress.ToString() + ".5";
                strDataNoLoad = "D" + intAddress.ToString() + ".6";
                strLoadNoData = "D" + intAddress.ToString() + ".7";
            }
        }
        #endregion PLC位址

        #endregion 結構

        public Buffer(string bufferName, string address)
        {
            strBufferName = bufferName;
            strAddress = address;

            _EQUStatusAddress = new EQU_Status_Signal_Address(address);
            _EQUAlarmStatusAddress = new EQU_Alarm_Status_Signal_Address(address);
            _EQUStatus = new EQU_Status_Signal();
            _EQUAlarmStatus = new EQU_Alarm_Status_Signal();
        }
    }
}
