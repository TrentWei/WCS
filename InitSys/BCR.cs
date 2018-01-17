using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Mirle.ASRS
{
    public class BCR
    {
        public enum BCRSts
        {
            None = 0,
            Reading = 1,
            ReadFinish = 2,
        }

        #region 變數
        private System.Timers.Timer timBCRReadTimeOut = new System.Timers.Timer();
        private DateTime dtBCRReadTime = new DateTime();
        private SerialPort serialPort = new SerialPort();
        private byte[] bytarTriggerOn = new byte[0];
        private byte[] bytarTriggerOff = new byte[0];
        private string strPort = string.Empty;
        private string strResultID = string.Empty;
        private string strBCRName = string.Empty;
        private string strBufferName = string.Empty;
        private BCRSts enuBCRSts = BCRSts.None;
        private int intBCRReadTimeOut = 1000;
        private int intBufferIndex = 0;

        private const string strError = "ERROR";
        #endregion 變數

        #region  屬性
        public BCRSts _BCRSts
        {
            get { return enuBCRSts; }
        }

        public string _ResultID
        {
            get { return strResultID; }
        }

        public string _BCRName
        {
            get { return strBCRName; }
        }

        public string _BufferName
        {
            get { return strBufferName; }
        }

        public int _BufferIndex
        {
            get { return intBufferIndex; }
        }

        public bool _IsOpen
        {
            get
            {
                if(serialPort != null)
                    return serialPort.IsOpen;
                else
                    return false;
            }
        }
        #endregion  屬性

        public BCR()
        {
        }

        public BCR(string bCRName, string port, int bCRReadTimeOut, string bufferName, int bufferIndex)
        {
            strBCRName = bCRName;
            intBCRReadTimeOut = bCRReadTimeOut;
            strPort = port;
            strResultID = string.Empty;
            enuBCRSts = BCRSts.None;
            strBufferName = bufferName;
            intBufferIndex = bufferIndex;

            bytarTriggerOn = new byte[3];
            bytarTriggerOn[0] = 0x16;
            bytarTriggerOn[1] = 0x54;
            bytarTriggerOn[2] = 0x0D;

            bytarTriggerOff = new byte[3];
            bytarTriggerOff[0] = 0x16;
            bytarTriggerOff[1] = 0x55;
            bytarTriggerOff[2] = 0x0D;
        }

        #region Event Function
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                timBCRReadTimeOut.Stop();
                switch(enuBCRSts)
                {
                    case BCRSts.Reading:
                        strResultID = serialPort.ReadExisting().Replace("\r", "");
                        if(!string.IsNullOrWhiteSpace(strResultID))
                        {
                            enuBCRSts = BCRSts.ReadFinish;
                            serialPort.Write(bytarTriggerOff, 0, bytarTriggerOff.Length);
                            InitSys.funWriteLog("BCR_Trace", strBCRName + "|DataReceived1|" + strResultID + "|" + enuBCRSts);
                        }
                        else
                        {
                            strResultID = strError;
                            enuBCRSts = BCRSts.ReadFinish;
                            serialPort.Write(bytarTriggerOff, 0, bytarTriggerOff.Length);
                            InitSys.funWriteLog("BCR_Trace", strBCRName + "|DataReceived2|" + strResultID + "|" + enuBCRSts);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void timBCRReadTimeOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timBCRReadTimeOut.Stop();
            if(DateTime.Now > dtBCRReadTime.AddMilliseconds(intBCRReadTimeOut))
            {
                serialPort.Write(bytarTriggerOff, 0, bytarTriggerOff.Length);
                strResultID = serialPort.ReadExisting().Replace("\r", "");
                switch(enuBCRSts)
                {
                    case BCRSts.Reading:
                        strResultID = serialPort.ReadExisting().Replace("\r", "");
                        if(!string.IsNullOrWhiteSpace(strResultID))
                        {
                            enuBCRSts = BCRSts.ReadFinish;
                            serialPort.Write(bytarTriggerOff, 0, bytarTriggerOff.Length);
                            InitSys.funWriteLog("BCR_Trace", strBCRName + "|TimeOut1|" + strResultID + "|" + enuBCRSts);
                        }
                        else
                        {
                            strResultID = strError;
                            enuBCRSts = BCRSts.ReadFinish;
                            serialPort.Write(bytarTriggerOff, 0, bytarTriggerOff.Length);
                            InitSys.funWriteLog("BCR_Trace", strBCRName + "|TimeOut2|" + strResultID + "|" + enuBCRSts);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                timBCRReadTimeOut.Start();
            }
        }
        #endregion Event Function

        #region Opend/Close
        public bool funOpenBCR(ref string errMsg)
        {
            try
            {
                timBCRReadTimeOut = new System.Timers.Timer();
                timBCRReadTimeOut.Interval = 100;
                timBCRReadTimeOut.Elapsed += new System.Timers.ElapsedEventHandler(timBCRReadTimeOut_Elapsed);

                serialPort = new SerialPort(strPort, 115200, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                serialPort.Open();
                InitSys.funWriteLog("BCR_Trace", strPort + "|Open!");
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
                if(serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    serialPort = null;
                    InitSys.funWriteLog("BCR_Trace", strBCRName + "|Close!");
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }
        #endregion Opend/Close

        public void funClear()
        {
            strResultID = string.Empty;
            enuBCRSts = BCRSts.None;
            InitSys.funWriteLog("BCR_Trace", strBCRName + "|Clear!|" + strResultID + "|" + enuBCRSts);
        }

        public bool funTriggerBCROn(ref string errMsg)
        {
            try
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.Write(bytarTriggerOn, 0, bytarTriggerOn.Length);

                InitSys.funWriteLog("BCR_Trace", strBCRName + "|TriggerBCROn!");

                strResultID = string.Empty;
                enuBCRSts = BCRSts.Reading;

                dtBCRReadTime = DateTime.Now;
                timBCRReadTimeOut.Start();
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
