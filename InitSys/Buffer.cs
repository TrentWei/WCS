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
            L2L = 5
        }
        public enum ReturnMode
        {
            None = 0,
            NotItem = 1,
            GetLocFail = 2,
            OverWeght = 3,
            BCRError = 4
        }
        #endregion 列舉

        private string strAddress = string.Empty;
        private string strBufferName = string.Empty;

        #region 屬性
        public string _BufferName
        {
            get { return strBufferName; }
        }
        /// <summary>
        /// 命令号
        /// </summary>
        public string _CommandID { get; set; }
        /// <summary>
        /// 目的站
        /// </summary>
        public string _Destination { get; set; }
        /// <summary>
        /// 命令模式
        /// </summary>
        public StnMode _Mode { get; set; }
        /// <summary>
        /// 退回请求
        /// </summary>
        public string _ReturnRequest { get; set; }
        /// <summary>
        /// 放行信号
        /// </summary>
        public bool _Discharged { get; set; }
        /// <summary>
        /// 读码通知
        /// </summary>
        public bool _APositioning { get; set; }
        /// <summary>
        /// 站口看板清除
        /// </summary>
        public bool _Clearnotice { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string _PalletNo { get; set; }

        /// <summary>
        /// 设备信号状态
        /// </summary>
        public EQU_Status_Signal _EQUStatus { get; set; }
        /// <summary>
        /// 设备报警信号状态
        /// </summary>
        public EQU_Alarm_Status_Signal _EQUAlarmStatus { get; set; }
        /// <summary>
        /// 设备信号状态地址
        /// </summary>
        public EQU_Status_Signal_Address _EQUStatusAddress { get; set; }
        /// <summary>
        /// 设备报警信号状态地址
        /// </summary>
        public EQU_Alarm_Status_Signal_Address _EQUAlarmStatusAddress { get; set; }

        #region PLC位址

        /// <summary>
        /// 命令序号
        /// </summary>
        public string _W_CmdSno
        {
            get { return strAddress; }
        }

        /// <summary>
        /// 模式
        /// </summary>
        public string _W_Mode
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 1).ToString(); }
        }
        /// <summary>
        /// 目的站
        /// </summary>
        public string _W_Destination
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 2).ToString(); }
        }
        /// <summary>
        /// 放行
        /// </summary>
        public string _W_Discharged
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 4).ToString(); }
        }
        /// <summary>
        /// 退回请求
        /// </summary>
        public string _W_ReturnRequest
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 6).ToString(); }
        }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string _W_PalletNo
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 7).ToString(); }
        }

        /// <summary>
        /// 通知读码
        /// </summary>
        public string _W_APositioning
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 5).ToString(); }
        }

        public string _W_Clearnotice
        {
            get { return "D" + (int.Parse(strAddress.Remove(0, 1)) + 9).ToString(); }
        }
        #endregion PLC位址

        #endregion 屬性

        #region 結構
        public class EQU_Status_Signal
        {
            /// <summary>
            /// 自动/手动
            /// </summary>
            public Signal AutoMode { get; set; }
            /// <summary>
            /// 荷有/荷无
            /// </summary>
            public Signal Load { get; set; }

            /// <summary>
            /// 前定位
            /// </summary>
            public Signal FrontLocation { get; set; }

            /// <summary>
            /// 后定位
            /// </summary>
            public Signal RearLocation { get; set; }

            /// <summary>
            /// 上定位
            /// </summary>
            public Signal UpLocation { get; set; }
            /// <summary>
            /// 下定位
            /// </summary>
            public Signal BelowLocation { get; set; }

            /// <summary>
            /// 作业完了
            /// </summary>
            public Signal Completion { get; set; }

            /// <summary>
            /// 大小储位 0：L,1：H
            /// </summary>
            public Signal Siez { get; set; }

            /// <summary>
            /// 余料回库 0：空托盘回流,1：盘点回库
            /// </summary>
            public Signal CycIn { get; set; }

            public EQU_Status_Signal()
            {
                AutoMode = Signal.Off;
                Load = Signal.Off;
            }
        }

        public class EQU_Alarm_Status_Signal
        {
            /// <summary>
            /// 总异常
            /// </summary>
            public bool Error { get; set; }
            /// <summary>
            /// 急停
            /// </summary>
            public Signal EMO { get; set; }
            /// <summary>
            /// 输送马达过载
            /// </summary>
            public Signal TransportMotorOverLoad { get; set; }
            /// <summary>
            /// 输送超时
            /// </summary>
            public Signal TransportTimeout { get; set; }
            /// <summary>
            /// 输送变频器异常
            /// </summary>
            public Signal TransportTransducerERROR { get; set; }
            /// <summary>
            /// 升降马达过载
            /// </summary>
            public Signal LiftMotorOverLoad { get; set; }
            /// <summary>
            /// 升降超时
            /// </summary>
            public Signal LiftTimeout { get; set; }
            /// <summary>
            /// 升降变频器异常
            /// </summary>
            public Signal LiftTransducerERROR{ get; set; }
            /// <summary>
            /// 升降上极限异常
            /// </summary>
            public Signal LiftUpERROR { get; set; }
            /// <summary>
            /// 升降下极限异常
            /// </summary>
            public Signal LiftBelowERROR { get; set; }
            /// <summary>
            /// 突出异常
            /// </summary>
            public Signal ExtrudeERROR { get; set; }
            /// <summary>
            /// 超长异常
            /// </summary>
            public Signal OverLength { get; set; }
            /// <summary>
            /// 超宽异常
            /// </summary>
            public Signal OverWidth { get; set; }
            /// <summary>
            /// 超高异常
            /// </summary>
            public Signal OverHigh { get; set; }
            /// <summary>
            /// 超重异常
            /// </summary>
            public Signal OverWeight { get; set; }
            /// <summary>
            /// 通讯异常
            /// </summary>
            public Signal CommunicationERROR { get; set; }
            /// <summary>
            /// 有值无物
            /// </summary>
            public Signal DataNoLoad { get; set; }
            /// <summary>
            /// 有物无值
            /// </summary>
            public Signal LoadNoData { get; set; }

            public EQU_Alarm_Status_Signal()
            {
                EMO = Signal.Off;
                TransportMotorOverLoad = Signal.Off;
                TransportTimeout = Signal.Off;
                TransportTransducerERROR = Signal.Off;

                LiftMotorOverLoad = Signal.Off;
                LiftTimeout = Signal.Off;
                LiftTransducerERROR = Signal.Off;
                LiftUpERROR = Signal.Off;
                LiftBelowERROR = Signal.Off;
                ExtrudeERROR = Signal.Off;
                OverLength = Signal.Off;
                OverWidth = Signal.Off;
                OverHigh = Signal.Off;
                OverWeight = Signal.Off;
                CommunicationERROR = Signal.Off;

                //DataNoLoad = Signal.Off;
                //LoadNoData = Signal.Off;
            }
        }

        #region PLC位址
        public class EQU_Status_Signal_Address
        {
            private string strAutoMode = string.Empty;
            private string strLoad = string.Empty;
            private string strFrontLocation = string.Empty;
            private string strRearLocation = string.Empty;
            private string strUpLocation = string.Empty;
            private string strBelowLocation = string.Empty;
            private string strCompletion = string.Empty;
            private string strSize = string.Empty;
            private string strCycIn = string.Empty;
            //private string strAPositioning = string.Empty;

            public string B_AutoMode { get { return strAutoMode; } }
            public string B_Load { get { return strLoad; } }


            public EQU_Status_Signal_Address(string address)
            {
                int intAddress = int.Parse(address.Remove(0, 1)) + 3;
                strAutoMode = "D" + intAddress.ToString() + ".0";
                strLoad = "D" + intAddress.ToString() + ".1";
                strFrontLocation = "D" + intAddress.ToString() + ".2";
                strRearLocation = "D" + intAddress.ToString() + ".3";
                strUpLocation = "D" + intAddress.ToString() + ".4";
                strBelowLocation = "D" + intAddress.ToString() + ".5";
                strCompletion = "D" + intAddress.ToString() + ".6";
                strSize = "D" + intAddress.ToString() + ".7";
                strCycIn = "D" + intAddress.ToString() + ".8";
            }
        }

        public class EQU_Alarm_Status_Signal_Address
        {
            private string strEMO = string.Empty;
            private string strTransportMotorOverLoad = string.Empty;
            private string strTransportTimeout = string.Empty;
            private string strTransportTransducerERROR = string.Empty;
            private string strLiftMotorOverLoad = string.Empty;
            private string strLiftTimeout = string.Empty;
            private string strLiftTransducerERROR = string.Empty;
            private string strLiftUpERROR = string.Empty;
            private string strLiftBelowERROR = string.Empty;
            private string strExtrudeERROR = string.Empty;
            private string strOverLength = string.Empty;
            private string strOverWidth = string.Empty;
            private string strOverHigh = string.Empty;
            private string strOverWeight = string.Empty;
            private string strCommunicationERROR = string.Empty;

            public string B_EMO { get { return strEMO; } }
            public string B_TransportMotorOverLoad { get { return strTransportMotorOverLoad; } }
            public string B_TransportTimeout { get { return strTransportTimeout; } }
            public string B_LiftMotorOverLoad { get { return strLiftMotorOverLoad; } }
            public string B_LiftTimeout { get { return strLiftTimeout; } }
            public string B_OverHigh { get { return strOverHigh; } }
            //public string B_DataNoLoad { get { return strDataNoLoad; } }
            //public string B_LoadNoData { get { return strLoadNoData; } }

            public EQU_Alarm_Status_Signal_Address(string address)
            {
                int intAddress = int.Parse(address.Remove(0, 1)) + 5;
                strEMO = "D" + intAddress.ToString() + ".1";
                strTransportMotorOverLoad = "D" + intAddress.ToString() + ".2";
                strTransportTimeout = "D" + intAddress.ToString() + ".3";
                strTransportTransducerERROR = "D" + intAddress.ToString() + ".4";
                strLiftMotorOverLoad = "D" + intAddress.ToString() + ".5";
                strLiftTimeout = "D" + intAddress.ToString() + ".6";
                strLiftTransducerERROR= "D" + intAddress.ToString() + ".7";
                strLiftUpERROR = "D" + intAddress.ToString() + ".8";
                strLiftBelowERROR = "D" + intAddress.ToString() + ".9";
                strExtrudeERROR = "D" + intAddress.ToString() + ".A";
                strOverLength = "D" + intAddress.ToString() + ".B";
                strOverWidth = "D" + intAddress.ToString() + ".C";
                strOverHigh = "D" + intAddress.ToString() + ".D";
                strOverWeight = "D" + intAddress.ToString() + ".E";
                strCommunicationERROR = "D" + intAddress.ToString() + ".F";
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
