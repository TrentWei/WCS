using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ACTMULTILib;

namespace Mirle.ASRS
{
    public class MPLC
    {
        #region 變數
        private int intStationNumber = 1;
        private bool bolIsConnection = false;
        private ActEasyIF aeiPLC;
        #endregion 變數

        #region  屬性
        public bool _IsConnection
        {
            get
            {
                if(aeiPLC != null)
                    return bolIsConnection;
                else
                    return bolIsConnection = false;
            }
        }
        #endregion  屬性

        public MPLC()
        {
        }

        public MPLC(int StationNumber)
        {
            intStationNumber = StationNumber;
        }

        #region Function

        #region Opend/Close
        public bool funOpenMPLC(ref string errMsg)
        {
            try
            {
                aeiPLC = new ActEasyIF();
                aeiPLC.ActLogicalStationNumber = intStationNumber;
                if(aeiPLC.Open() == 0)
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", "Open Success!");
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", "Open Fail!|" + errMsg);
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
                if(aeiPLC != null)
                {
                    aeiPLC.Close();
                    aeiPLC = null;
                    bolIsConnection = false;
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", "Close!");
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

        #region Read
        public bool funReadMPLC(string address, int length, ref int[] retData)
        {
            string strData = string.Empty;
            try
            {
                if(aeiPLC.ReadDeviceBlock(address, length, out retData[0]) == 0)
                {
                    for(int intLength = 0; intLength < retData.Length; intLength++)
                    {
                        if(string.IsNullOrWhiteSpace(strData))
                            strData = retData[intLength].ToString();
                        else
                            strData += "," + retData[intLength].ToString();
                    }
                    InitSys.funWriteLog("MPLCR_" + intStationNumber + "_" + DateTime.Now.ToString("yyyyMMddHH"), strData);
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", address + "|" + length + "|ReadDeviceBlock Fail!");
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
        #endregion Read

        #region Write
        public bool funWriteMPLC(string deviceBlockAddress, params string[] setData)
        {
            string strData = string.Empty;

            try
            {
                int[] intData = new int[setData.Length];
                for(int intLength = 0; intLength < setData.Length; intLength++)
                {
                    if(string.IsNullOrWhiteSpace(setData[intLength]))
                        setData[intLength] = "0";

                    intData[intLength] = int.Parse(setData[intLength]);

                    if(string.IsNullOrWhiteSpace(strData))
                        strData = setData[intLength];
                    else
                        strData += "," + setData[intLength];
                }

                if(aeiPLC.WriteDeviceBlock(deviceBlockAddress, setData.Length, ref intData[0]) == 0)
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", deviceBlockAddress + "|" + strData + "|WriteDeviceBlock Success!");
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", deviceBlockAddress + "|" + strData + "|WriteDeviceBlock Fail!");
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

        public bool funWriteMPLC(string deviceAddress, bool setBit)
        {
            try
            {
                string strAdrress = string.Empty;

                if(aeiPLC.SetDevice(strAdrress, setBit ? 1 : 0) == 0)
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", deviceAddress + "|" + (setBit ? 1 : 0) + "|SetDevice Success!");
                    return bolIsConnection = true;
                }
                else
                {
                    InitSys.funWriteLog("MPLC_" + intStationNumber + "_Trace", deviceAddress + "|" + (setBit ? 1 : 0) + "|SetDevice Fail!");
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

        public void funClearMPLC(string deviceBlockAddress)
        {
            string[] strValues = new string[] { "0", "0", "0" };
            funWriteMPLC(deviceBlockAddress, strValues);
        }
        #endregion Write

        #endregion Function
    }
}
