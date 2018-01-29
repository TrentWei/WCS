using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Mirle.ASRS
{
    public partial class WCS : Form
    {
        private bool bolMPLC = false;
        private bool bolSPLC = false;
        private bool bolDB = false;
        private bool bolBCR = false;
        private bool bolAuto = true;
        private bool bolClose = false;
        private int[] intarResultData = new int[0];
        private BufferData bufferData = new BufferData();
        private BCRData bCRData = new BCRData();
        private Thread thdReconnection = null;
        private System.Windows.Forms.Timer timRefresh = new System.Windows.Forms.Timer();
        private System.Timers.Timer timProgram = new System.Timers.Timer();
        private System.Timers.Timer timSPLCProgram = new System.Timers.Timer();
        private System.Timers.Timer timUpdate = new System.Timers.Timer();
        private Dictionary<int, Control> dicBufferMap = new Dictionary<int, Control>();
        private List<StationInfo> lstStoreIn = new List<StationInfo>();
        private List<StationInfo> lstStoreOut = new List<StationInfo>();
        private SMPLCData_1 sMPLCData_1 = new SMPLCData_1();
        private SMPLCData_2 sMPLCData_2 = new SMPLCData_2();

        private delegate void ShowMessage_EventHandler(string Message);
        private delegate void ButtonEnable_EventHandler(Button button, bool enable);

        public WCS()
        {
            InitializeComponent();
        }

        #region Event Function
        private void WCS_Load(object sender, EventArgs e)
        {
            InitSys.funItialSys();

            if(string.IsNullOrWhiteSpace(InitSys._APName))
                this.Text = "盟立自动仓控制系统" + " (V." + Application.ProductVersion + ")";
            else
                this.Text = InitSys._APName + " (V." + Application.ProductVersion + ")";

            funWriteSysTraceLog(this.Text + " Program Start!");

            funInital();
        }

        private void btnAutoPause_Click(object sender, EventArgs e)
        {
            funWriteSysTraceLog(bolAuto ? "WCS Set Pause!" : "WCS Set Auto!");
            btnAutoPause.Text = bolAuto ? "自动模式" : "维护模式";
            bolAuto = !bolAuto;
            btnReconnectMPLC.Enabled = !bolAuto;
            btnReconnectSPLC.Enabled = !bolAuto;
            btnReconnectDB.Enabled = !bolAuto;
            btnReconnectBCR.Enabled = !bolAuto;
        }

        #region Close
        private void WCS_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !bolClose;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            funWriteSysTraceLog(this.Text + " Program Stop!");
            funClose();
        }
        #endregion Close

        #region Reconnect
        private void btnReconnectDB_Click(object sender, EventArgs e)
        {
            btnReconnectDB.Enabled = false;
            thdReconnection = new Thread(funReconnectionDB);
            thdReconnection.IsBackground = true;
            thdReconnection.Start();
        }

        private void btnReconnectBCR_Click(object sender, EventArgs e)
        {
            btnReconnectBCR.Enabled = false;
            thdReconnection = new Thread(funReconnectionBCR);
            thdReconnection.IsBackground = true;
            thdReconnection.Start();
        }

        private void btnReconnectMPLC_Click(object sender, EventArgs e)
        {
            btnReconnectMPLC.Enabled = false;
            thdReconnection = new Thread(funReconnectionMPLC);
            thdReconnection.IsBackground = true;
            thdReconnection.Start();
        }

        private void btnReconnectSPLC_Click(object sender, EventArgs e)
        {
            btnReconnectSPLC.Enabled = false;
            thdReconnection = new Thread(funReconnectionSPLC);
            thdReconnection.IsBackground = true;
            thdReconnection.Start();
        }
        #endregion Reconnect

        #region Time
        private void timRefresh_Tick(object sender, EventArgs e)
        {
            timRefresh.Stop();

            try
            {
                lblDataTime.Text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff");

                InitSys._MPLC.funWriteMPLC("D11", "1");

                funShowAutoPause(bolAuto);
                funShowConnect(InitSys._MPLC._IsConnection, lblMPLCSts);
                funShowConnect(InitSys._SPLC._IsConnection, lblSPLCSts);
                funShowConnect(InitSys._DB._IsConnection, lblDBSts);

                #region Auto Reconnect
                if(chkAutoReconnect.Checked && !bolAuto)
                {
                    if(!InitSys._MPLC._IsConnection && !bolMPLC)
                    {
                        bolMPLC = true;
                        thdReconnection = new Thread(funReconnectionMPLC);
                        thdReconnection.IsBackground = true;
                        thdReconnection.Start();
                    }
                    if(!InitSys._SPLC._IsConnection && !bolSPLC)
                    {
                        bolSPLC = true;
                        thdReconnection = new Thread(funReconnectionSPLC);
                        thdReconnection.IsBackground = true;
                        thdReconnection.Start();
                    }
                    for(int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                    {
                        if(!bCRData[intIndex]._IsOpen && !bolBCR)
                        {
                            bolBCR = true;
                            thdReconnection = new Thread(funReconnectionBCR);
                            thdReconnection.IsBackground = true;
                            thdReconnection.Start();
                        }
                    }
                    if(!InitSys._DB._IsConnection && !bolDB)
                    {
                        bolDB = true;
                        thdReconnection = new Thread(funReconnectionDB);
                        thdReconnection.IsBackground = true;
                        thdReconnection.Start();
                    }
                }
                #endregion Auto Reconnect
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                timRefresh.Start();
            }
        }

        private void timProgram_Elapsed(object sender, ElapsedEventArgs e)
        {
            timProgram.Stop();

            try
            {
                if(InitSys._MPLC._IsConnection)
                {
                    if(InitSys._MPLC.funReadMPLC(InitSys._StartAddress, InitSys._TotalAddress, ref intarResultData))
                    {
                        for(int intIndex = 0; intIndex < bufferData._BufferCount; intIndex++)
                        {
                            bufferData[intIndex]._CommandID = intarResultData[(intIndex * 10)].ToString() == "0" ?
                                string.Empty : intarResultData[(intIndex * 10)].ToString();
                            bufferData[intIndex]._Destination = intarResultData[(intIndex * 10) + 1].ToString() == "0" ?
                                string.Empty : intarResultData[(intIndex * 10) + 1].ToString();
                            bufferData[intIndex]._Mode = (Buffer.StnMode)intarResultData[(intIndex * 10) + 2];
                            bufferData[intIndex]._ReturnRequest = intarResultData[(intIndex * 10) + 3] == 1;

                            #region EQUStatus
                            string strTmp = Convert.ToString(intarResultData[(intIndex * 10) + 6], 2).PadLeft(16, '0');
                            bufferData[intIndex]._EQUStatus.AutoMode =
                                strTmp.Substring(15, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUStatus.Load =
                                strTmp.Substring(14, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            #endregion EQUStatus

                            #region EQUAlarmStatus
                            strTmp = Convert.ToString(intarResultData[(intIndex * 10) + 7], 2).PadLeft(16, '0');
                            bufferData[intIndex]._EQUAlarmStatus.EMO =
                                strTmp.Substring(15, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.TransportMotorOverLoad =
                                strTmp.Substring(14, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.TransportTimeout =
                                strTmp.Substring(13, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.LiftMotorOverLoad =
                                strTmp.Substring(12, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.LiftTimeout =
                                strTmp.Substring(11, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.OverHigh =
                                strTmp.Substring(10, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.LoadNoData =
                                strTmp.Substring(9, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            bufferData[intIndex]._EQUAlarmStatus.DataNoLoad =
                                strTmp.Substring(8, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                            #endregion EQUAlarmStatus
                        }
                    }

                    if(bolAuto)
                    {
                        funStoreOut();
                        funStroreIn();
                        funLocationToLocation();
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                timProgram.Start();
            }
        }

        private void timSPLCProgram_Elapsed(object sender, ElapsedEventArgs e)
        {
            timSPLCProgram.Stop();

            try
            {
                if(InitSys._SPLC._IsConnection)
                {
                    if(InitSys._SPLC.funReadSPLC(sMPLCData_1))
                    {
                        int[] intBCRArray1 = new int[]
                        {
                            sMPLCData_1.BCR1_1, sMPLCData_1.BCR1_2, sMPLCData_1.BCR1_3, sMPLCData_1.BCR1_4, sMPLCData_1.BCR1_5
                        };
                        int[] intBCRArray2 = new int[]
                        {
                            sMPLCData_1.BCR2_1, sMPLCData_1.BCR2_2, sMPLCData_1.BCR2_3, sMPLCData_1.BCR2_4, sMPLCData_1.BCR2_5
                        };
                        string strBCR1 = funIntArrayConvertASCII(intBCRArray1);
                        string strBCR2 = funIntArrayConvertASCII(intBCRArray2);

                        if(InitSys._SPLC.funReadSPLC(sMPLCData_2, 50))
                        {
                            funAGVNeedStationRequest(strBCR2);
                            funStoreInRequestFromAGV(strBCR1);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                timSPLCProgram.Start();
            }
        }

        private void timUpdate_Elapsed(object sender, ElapsedEventArgs e)
        {
            timUpdate.Stop();
            try
            {
                funUpdatePosted();
            }
            finally
            {
                timUpdate.Start();
            }
        }
        #endregion Time

        #endregion Event Function

        #region Inital Function
        private void funInital()
        {
            btnReconnectMPLC.Enabled = !bolAuto;
            btnReconnectSPLC.Enabled = !bolAuto;
            btnReconnectDB.Enabled = !bolAuto;
            btnReconnectBCR.Enabled = !bolAuto;

            funInitalTimer();
            funInitalBuffer();
            funInitalStoreIn();
            funInitalStoreOut();
            funinitalBCR();
            funStart();
        }

        private void funInitalBuffer()
        {
            List<string> lstBuffer = new List<string>();
            Dictionary<string, string> dicPLCMap = new Dictionary<string, string>();

            try
            {
                string[] strarBufferMAPLines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\Buffer.txt");
                foreach(string strValues in strarBufferMAPLines)
                {
                    if(strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');

                    try
                    {
                        dicPLCMap.Add(strTmp[0], strTmp[1]);
                        lstBuffer.Add(strTmp[0]);
                        if(tbpFlowControl.Controls.ContainsKey(strTmp[0]))
                            dicBufferMap.Add(int.Parse(strTmp[2]), tbpFlowControl.Controls[strTmp[0]]);
                    }
                    catch(Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
                bufferData = new BufferData(lstBuffer, dicPLCMap);

                intarResultData = new int[InitSys._TotalAddress];
                for(int i = 0; i < intarResultData.Length; i++)
                    intarResultData[i] = 0;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funInitalStoreIn()
        {
            try
            {
                lstStoreIn.Clear();
                lstStoreIn = new List<StationInfo>();
                string[] strarStroreInines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\StoreIn.txt");
                foreach(string strValues in strarStroreInines)
                {
                    if(strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');
                    StationInfo stnDef = new StationInfo();
                    try
                    {
                        stnDef.BufferName = strTmp[0];
                        stnDef.BufferIndex = int.Parse(strTmp[1]);
                        stnDef.StationName = strTmp[2];
                        stnDef.StationIndex = int.Parse(strTmp[3]);
                        lstStoreIn.Add(stnDef);
                    }
                    catch(Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funInitalStoreOut()
        {
            try
            {
                lstStoreOut.Clear();
                lstStoreOut = new List<StationInfo>();
                string[] strarStroreInines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\StoreOut.txt");
                foreach(string strValues in strarStroreInines)
                {
                    if(strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');
                    StationInfo stnDef = new StationInfo();
                    try
                    {
                        stnDef.BufferName = strTmp[0];
                        stnDef.BufferIndex = int.Parse(strTmp[1]);
                        stnDef.StationName = strTmp[2];
                        stnDef.StationIndex = int.Parse(strTmp[3]);
                        lstStoreOut.Add(stnDef);
                    }
                    catch(Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funinitalBCR()
        {
            List<BCR> lstBCR = new List<BCR>();
            string strEM = string.Empty;
            try
            {
                string[] strarBCRPLines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\BCR.txt");
                foreach(string strValues in strarBCRPLines)
                {
                    if(strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');
                    try
                    {
                        BCR bCR = new BCR(strTmp[0], strTmp[1], int.Parse(strTmp[2]), strTmp[3], int.Parse(strTmp[4]));
                        lstBCR.Add(bCR);
                    }
                    catch(Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }

                bCRData = new BCRData(lstBCR);
                for(int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                {
                    if(bCRData[intIndex].funOpenBCR(ref strEM))
                        funWriteSysTraceLog("Connection BCR:" + bCRData[intIndex]._BCRName + " Success!");
                    else
                        funWriteSysTraceLog("Connection BCR:" + bCRData[intIndex]._BCRName + " Fail!");
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funInitalTimer()
        {
            timRefresh.Stop();
            timRefresh.Tick += new EventHandler(timRefresh_Tick);
            timRefresh.Interval = 100;

            timProgram.Stop();
            timProgram.Elapsed += new ElapsedEventHandler(timProgram_Elapsed);
            timProgram.Interval = 200;

            timSPLCProgram.Stop();
            timSPLCProgram.Elapsed += new ElapsedEventHandler(timSPLCProgram_Elapsed);
            timSPLCProgram.Interval = 200;

            timUpdate.Stop();
            timUpdate.Elapsed += new ElapsedEventHandler(timUpdate_Elapsed);
            timUpdate.Interval = 2000;
        }

        private void funStart()
        {
            if(InitSys.funOpendMPLC())
                funWriteSysTraceLog("Connection MPLC Success!");
            else
                funWriteSysTraceLog("Connection MPLC Fail!");

            if(InitSys.funOpendSPLC())
                funWriteSysTraceLog("Connection SPLC Success!");
            else
                funWriteSysTraceLog("Connection SPLC Fail!");

            if(InitSys.funOpendDB())
                funWriteSysTraceLog("Connection DB Success!");
            else
                funWriteSysTraceLog("Connection DB Fail!");

            timRefresh.Start();
            timProgram.Start();
            timSPLCProgram.Start();
            timUpdate.Start();
        }
        #endregion Inital Function

        #region Reconnection Function
        private void funReconnectionDB()
        {
            string strEM = string.Empty;
            if(InitSys._DB != null)
            {
                InitSys._DB.funClose();

                if(InitSys._DB.funOpenDB(ref strEM))
                    funWriteSysTraceLog("Try Reconnection DB Success!");
                else
                    funWriteSysTraceLog("Try Reconnection DB Fail!");
            }
            bolDB = false;
            funEnableButton(btnReconnectDB, true);
        }

        private void funReconnectionBCR()
        {
            string strEM = string.Empty;
            for(int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
            {
                if(bCRData[intIndex] != null)
                {
                    bCRData[intIndex].funClose();
                    if(bCRData[intIndex].funOpenBCR(ref strEM))
                        funWriteSysTraceLog("Try Reconnection BCR:" + bCRData[intIndex]._BCRName + " Success!");
                    else
                        funWriteSysTraceLog("Try Reconnection BCR:" + bCRData[intIndex]._BCRName + " Fail!");
                }
            }
            bolBCR = false;
            funEnableButton(btnReconnectBCR, true);
        }

        private void funReconnectionMPLC()
        {
            string strEM = string.Empty;
            if(InitSys._MPLC != null)
            {
                InitSys._MPLC.funClose();
                if(InitSys._MPLC.funOpenMPLC(ref strEM))
                    funWriteSysTraceLog("Try Reconnection MPLC Success!");
                else
                    funWriteSysTraceLog("Try Reconnection MPLC Fail!");
            }
            bolMPLC = false;
            funEnableButton(btnReconnectMPLC, true);
        }

        private void funReconnectionSPLC()
        {
            string strEM = string.Empty;
            if(InitSys._SPLC != null)
            {
                InitSys._SPLC.funClose();
                if(InitSys._SPLC.funOpenSPLC(ref strEM))
                    funWriteSysTraceLog("Try Reconnection SPLC Success!");
                else
                    funWriteSysTraceLog("Try Reconnection SPLC Fail!");
            }
            bolSPLC = false;
            funEnableButton(btnReconnectSPLC, true);
        }
        #endregion Reconnection Function

        #region Write Log Function
        private void funWriteSysTraceLog(string message)
        {
            try
            {
                if(lsbSysTrace.InvokeRequired)
                {
                    ShowMessage_EventHandler ShowMessage = new ShowMessage_EventHandler(funWriteSysTraceLog);
                    lsbSysTrace.Invoke(ShowMessage, message);
                }
                else
                {
                    if(lsbSysTrace.Items.Count >= 200)
                        lsbSysTrace.Items.RemoveAt(0);

                    lsbSysTrace.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message);
                    lsbSysTrace.SelectedIndex = lsbSysTrace.Items.Count - 1;

                    InitSys.funWriteLog("SysTrace", message);
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funWriteUpdateLog(string message)
        {
            try
            {
                if(lsbUpdate.InvokeRequired)
                {
                    ShowMessage_EventHandler ShowMessage = new ShowMessage_EventHandler(funWriteUpdateLog);
                    lsbUpdate.Invoke(ShowMessage, message);
                }
                else
                {
                    if(lsbUpdate.Items.Count >= 200)
                        lsbUpdate.Items.RemoveAt(0);

                    lsbUpdate.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message);
                    lsbUpdate.SelectedIndex = lsbUpdate.Items.Count - 1;

                    InitSys.funWriteLog("Update", message);
                }
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }
        #endregion Write Log Function

        #region Other Function
        private void funShowAutoPause(bool auto)
        {
            switch(auto)
            {
                case true:
                    lblCmuSts.Text = "自动模式";
                    lblCmuSts.BackColor = Color.Lime;
                    break;
                case false:
                default:
                    lblCmuSts.Text = "维护模式";
                    lblCmuSts.BackColor = Color.Red;
                    break;
            }
        }

        private void funShowConnect(bool connect, Label label)
        {
            switch(connect)
            {
                case true:
                    label.Text = "已连线";
                    label.BackColor = Color.Lime;
                    break;
                case false:
                default:
                    label.Text = "未连线";
                    label.BackColor = Color.Red;
                    break;
            }
        }

        private void funEnableButton(Button button, bool enable)
        {
            try
            {
                if(button.InvokeRequired)
                {
                    ButtonEnable_EventHandler ShowMessage = new ButtonEnable_EventHandler(funEnableButton);
                    button.Invoke(ShowMessage, button, enable);
                }
                else
                    button.Enabled = enable;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private string funIntArrayConvertASCII(int[] intArray)
        {
            string strResults = string.Empty;
            try
            {
                foreach(int intData in intArray)
                {
                    string strTemp_0007 = intData.ToString("X").PadLeft(4, "0"[0]).Substring(2, 2);
                    string strTemp_0815 = intData.ToString("X").PadLeft(4, "0"[0]).Substring(0, 2);
                    strResults += Convert.ToChar(Convert.ToInt32(strTemp_0007, 16)).ToString();
                    strResults += Convert.ToChar(Convert.ToInt32(strTemp_0815, 16)).ToString();
                }

                string strTemp = string.Empty;
                if(strResults.IndexOf("\r"[0]) >= 0)
                {
                    strTemp = strResults.Remove(strResults.IndexOf("\r"[0]));
                    strTemp = strTemp.Trim("\0"[0]).Trim();
                }
                else
                {
                    if(strResults.IndexOf("\0"[0]) >= 0)
                    {
                        strTemp = strResults.Remove(strResults.IndexOf("\0"[0]));
                        strTemp = strTemp.Trim();
                    }
                    else
                        strTemp = strResults.Trim();
                }
                return strTemp;
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return string.Empty;
            }
        }

        private void funClose()
        {
            bolAuto = false;
            timRefresh.Stop();
            timProgram.Stop();
            timUpdate.Stop();
            InitSys._DB.funClose();
            InitSys._MPLC.funClose();
            InitSys._SPLC.funClose();
            for(int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
            {
                if(bCRData[intIndex] != null)
                {
                    string strEM = string.Empty;
                    bCRData[intIndex].funClose();
                }
            }
            bolClose = true;
            this.Close();
        }
        #endregion Other Function
    }
}
