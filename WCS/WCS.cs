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
        private string strLastAGVBCR1 = string.Empty;
        private bool bolMPLC = false;
        private bool bolSPLC = false;
        private bool bolDB = false;
        private bool bolBCR = false;
        private bool bolAuto = true;
        private bool bolClose = false;
        private bool bolChkStn = false;
        private int[] intarResultData = new int[0];
        private BufferData bufferData = new BufferData();
        private BCRData bCRData = new BCRData();
        private Thread thdReconnection = null;
        private System.Windows.Forms.Timer timRefresh = new System.Windows.Forms.Timer();
        private System.Timers.Timer timProgram = new System.Timers.Timer();
        //private System.Timers.Timer timSPLCProgram = new System.Timers.Timer();
        //private System.Timers.Timer timUpdate = new System.Timers.Timer();
        private Dictionary<int, BufferMonitor> dicBufferMap = new Dictionary<int, BufferMonitor>();
        private Dictionary<int, CraneMonitor> dicCraneMap = new Dictionary<int, CraneMonitor>();
        private List<StationInfo> lstStoreIn = new List<StationInfo>();
        private List<StationInfo> lstStoreOut = new List<StationInfo>();
        private SMPLCData_1 sMPLCData_1 = new SMPLCData_1();
        private SMPLCData_2 sMPLCData_2 = new SMPLCData_2();
        private List<StationInfo> lstClearKanbanInfo = new List<StationInfo>();

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

            if (string.IsNullOrWhiteSpace(InitSys._APName))
                this.Text = "盟立自动仓控制系统,厦门长塑实业" + " (V." + Application.ProductVersion + ")";
            else
                this.Text = InitSys._APName + " (V." + Application.ProductVersion + ")";

            funWriteSysTraceLog(this.Text + " Program Start!");

            funInital();
            if (!funLoda())
            {
                MessageBox.Show("网络内已有程式运行！");
                funRef();
                funClose();
            }
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
            funRef();
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
                lblDataTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");

                funShowAutoPause(bolAuto);
                funShowConnect(InitSys._MPLC._IsConnection, lblMPLCSts);
                funShowConnect(InitSys._SPLC._IsConnection, lblSPLCSts);
                funShowConnect(InitSys._DB.ConnFlag, lblDBSts);

                #region Auto Reconnect
                if (chkAutoReconnect.Checked && !bolAuto)
                {
                    if (!InitSys._MPLC._IsConnection && !bolMPLC)
                    {
                        bolMPLC = true;
                        thdReconnection = new Thread(funReconnectionMPLC);
                        thdReconnection.IsBackground = true;
                        thdReconnection.Start();
                    }
                    //if (!InitSys._SPLC._IsConnection && !bolSPLC)
                    //{
                    //    bolSPLC = true;
                    //    thdReconnection = new Thread(funReconnectionSPLC);
                    //    thdReconnection.IsBackground = true;
                    //    thdReconnection.Start();
                    //}
                    for (int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                    {
                        if (!bCRData[intIndex]._IsOpen && !bolBCR)
                        {
                            bolBCR = true;
                            thdReconnection = new Thread(funReconnectionBCR);
                            thdReconnection.IsBackground = true;
                            thdReconnection.Start();
                        }
                    }
                    if (!InitSys._DB.ConnFlag && !bolDB)
                    {
                        bolDB = true;
                        thdReconnection = new Thread(funReconnectionDB);
                        thdReconnection.IsBackground = true;
                        thdReconnection.Start();
                    }
                }
                #endregion Auto Reconnect

                if (InitSys._MPLC._IsConnection)
                {
                    InitSys._MPLC.funWriteMPLC("D11", "1");

                    for (int intIndex = 0; intIndex < bufferData._BufferCount; intIndex++)
                    {
                        if (dicBufferMap.ContainsKey(intIndex))
                        {
                            //界面控件赋值
                            BufferMonitor bufferMonitor = dicBufferMap[intIndex];
                            bufferMonitor._CommandID = bufferData[intIndex]._CommandID;
                            bufferMonitor._Destination = bufferData[intIndex]._Destination;
                            bufferMonitor._Mode = bufferData[intIndex]._Mode;
                            bufferMonitor._ReturnRequest = bufferData[intIndex]._ReturnRequest;
                            bufferMonitor._Auto = bufferData[intIndex]._EQUStatus.AutoMode;
                            bufferMonitor._Load = bufferData[intIndex]._EQUStatus.Load;
                            bufferMonitor._Error = bufferData[intIndex]._EQUAlarmStatus.Error ? Buffer.Signal.On : Buffer.Signal.Off;
                        }
                    }
                }

                if (InitSys._DB.ConnFlag)
                    funRefreshCrane();
            }
            catch (Exception ex)
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
                if (InitSys._DB.ConnFlag)
                {
                    if (InitSys._MPLC._IsConnection)
                    {
                        if (InitSys._MPLC.funReadMPLC(InitSys._StartAddress, InitSys._TotalAddress, ref intarResultData))
                        {
                            for (int intIndex = 0; intIndex < bufferData._BufferCount; intIndex++)
                            {
                                bufferData[intIndex]._CommandID = intarResultData[(intIndex * 10)].ToString() == "0" ?
                                    string.Empty : intarResultData[(intIndex * 10)].ToString();

                                bufferData[intIndex]._Mode = (Buffer.StnMode)intarResultData[(intIndex * 10) + 1];

                                bufferData[intIndex]._Destination = intarResultData[(intIndex * 10) + 2].ToString() == "0" ?
                                    string.Empty : intarResultData[(intIndex * 10) + 2].ToString();

                                #region EQUStatus
                                string strTmp = Convert.ToString(intarResultData[(intIndex * 10) + 3], 2).PadLeft(16, '0');
                                bufferData[intIndex]._EQUStatus.AutoMode =
                                    strTmp.Substring(15, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                bufferData[intIndex]._EQUStatus.Load =
                                    strTmp.Substring(14, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                bufferData[intIndex]._EQUStatus.FrontLocation =
                                   strTmp.Substring(13, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                bufferData[intIndex]._EQUStatus.RearLocation =
                                   strTmp.Substring(12, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                bufferData[intIndex]._EQUStatus.BelowLocation =
                                 strTmp.Substring(11, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                bufferData[intIndex]._EQUStatus.Completion =
                               strTmp.Substring(11, 1) == "1" ? Buffer.Signal.On : Buffer.Signal.Off;
                                #endregion EQUStatus

                                #region EQUAlarmStatus
                                strTmp = Convert.ToString(intarResultData[(intIndex * 10) + 5], 2).PadLeft(16, '0');
                                bufferData[intIndex]._EQUAlarmStatus.Error = (intarResultData[(intIndex * 10) + 5] > 0);
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


                                //bufferData[intIndex]._ReturnRequest = intarResultData[(intIndex * 10) + 3] == 1;

                                //bufferData[intIndex].W_Discharged = intarResultData[(intIndex * 10) + 4] == 1;

                                //bufferData[intIndex]._APositioning = intarResultData[(intIndex * 10) + 5] == 1;

                                //bufferData[intIndex]._Clearnotice = intarResultData[(intIndex * 10) + 8] == 1;

                            }
                        }

                        if (bolAuto)
                        {
                            funKanbanInfo();
                            funStoreOut();
                            funStroreIn();
                            funLocationToLocation();
                        }
                    }
                    #region 烟台鲁达西门子PLC部分 //注释
                    //if (InitSys._SPLC._IsConnection)
                    //{
                    //    if (InitSys._SPLC.funReadSPLC(sMPLCData_1))
                    //    {
                    //        int[] intBCRArray1 = new int[]
                    //        {
                    //                sMPLCData_1.BCR1_1, sMPLCData_1.BCR1_2, sMPLCData_1.BCR1_3, sMPLCData_1.BCR1_4, sMPLCData_1.BCR1_5
                    //        };
                    //        int[] intBCRArray2 = new int[]
                    //        {
                    //                sMPLCData_1.BCR2_1, sMPLCData_1.BCR2_2, sMPLCData_1.BCR2_3, sMPLCData_1.BCR2_4, sMPLCData_1.BCR2_5
                    //        };
                    //        string strBCR1 = funIntArrayConvertASCII(intBCRArray1);
                    //        string strBCR2 = funIntArrayConvertASCII(intBCRArray2);
                    //        bool bolLoad = Convert.ToString(sMPLCData_1.Load, 2).PadLeft(16, '0').Substring(15, 1) == "1";
                    //        if (InitSys._SPLC.funReadSPLC(sMPLCData_2, InitSys._AGV_GetWirteSPLCStartIndex))
                    //        {
                    //            if (bolAuto)
                    //            {
                    //                funAGVSchedule(bolLoad);
                    //                funAGVNeedStationRequest(strBCR1);
                    //                funStoreInRequestFromAGV(strBCR2);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    funUpdatePosted();
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                timProgram.Start();
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
            funinitalKanbanInfo();
            funinitalCrane();
            funinitalBCR();
            funStart();
        }

        private void funInitalTimer()
        {
            timRefresh.Stop();
            timRefresh.Tick += new EventHandler(timRefresh_Tick);
            timRefresh.Interval = 100;

            timProgram.Stop();
            timProgram.Elapsed += new ElapsedEventHandler(timProgram_Elapsed);
            timProgram.Interval = 200;

            //timSPLCProgram.Stop();
            //timSPLCProgram.Elapsed += new ElapsedEventHandler(timSPLCProgram_Elapsed);
            //timSPLCProgram.Interval = 200;

            //timUpdate.Stop();
            //timUpdate.Elapsed += new ElapsedEventHandler(timUpdate_Elapsed);
            //timUpdate.Interval = 2000;
        }

        private void funInitalBuffer()
        {
            List<string> lstBuffer = new List<string>();
            Dictionary<string, string> dicPLCMap = new Dictionary<string, string>();

            try
            {
                string[] strarBufferMAPLines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\Buffer.txt");
                foreach (string strValues in strarBufferMAPLines)
                {
                    if (strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');

                    try
                    {
                        dicPLCMap.Add(strTmp[0], strTmp[1]);
                        lstBuffer.Add(strTmp[0]);
                        if (sctMain1.Panel2.Controls.ContainsKey(strTmp[0]))
                        {
                            if (sctMain1.Panel1.Controls[strTmp[0]] is BufferMonitor)
                                dicBufferMap.Add(int.Parse(strTmp[2]), (BufferMonitor)sctMain1.Panel2.Controls[strTmp[0]]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
                bufferData = new BufferData(lstBuffer, dicPLCMap);

                intarResultData = new int[InitSys._TotalAddress];
                for (int i = 0; i < intarResultData.Length; i++)
                    intarResultData[i] = 0;
            }
            catch (Exception ex)
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
                foreach (string strValues in strarStroreInines)
                {
                    if (strValues.Contains("#"))
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
                    catch (Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
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
                foreach (string strValues in strarStroreInines)
                {
                    if (strValues.Contains("#"))
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
                    catch (Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funinitalKanbanInfo()
        {
            try
            {
                lstClearKanbanInfo.Clear();
                lstClearKanbanInfo = new List<StationInfo>();
                string[] strarStroreInines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\KanbanInfo.txt");
                foreach (string strValues in strarStroreInines)
                {
                    if (strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');
                    StationInfo stnKanbanInfo = new StationInfo();
                    try
                    {
                        stnKanbanInfo.BufferName = strTmp[0];
                        stnKanbanInfo.BufferIndex = int.Parse(strTmp[1]);
                        lstClearKanbanInfo.Add(stnKanbanInfo);
                    }
                    catch (Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funinitalCrane()
        {
            try
            {
                dicCraneMap.Clear();
                for (int intIndex = 0; intIndex < sctMain1.Panel2.Controls.Count; intIndex++)
                {
                    if (sctMain1.Panel2.Controls[intIndex] is CraneMonitor)
                    {
                        CraneMonitor craneMonitor = (CraneMonitor)sctMain1.Panel2.Controls[intIndex];
                        dicCraneMap.Add(craneMonitor._CraneNo, craneMonitor);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception",
                    methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funinitalBCR()
        {
            List<BCR> lstBCR = new List<BCR>();
            string strEM = string.Empty;
            try
            {
                string[] strarBCRPLines = System.IO.File.ReadAllLines(Application.StartupPath + @"\Config\BCR.txt");
                foreach (string strValues in strarBCRPLines)
                {
                    if (strValues.Contains("#"))
                        continue;

                    string[] strTmp = strValues.Split(',');
                    try
                    {
                        BCR bCR = new BCR(strTmp[0], strTmp[1], int.Parse(strTmp[2]), strTmp[3], int.Parse(strTmp[4]));
                        lstBCR.Add(bCR);
                    }
                    catch (Exception ex)
                    {
                        MethodBase methodBase = MethodBase.GetCurrentMethod();
                        InitSys.funWriteLog("Exception",
                            methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + strTmp[0] + "|" + ex.Message);
                    }
                }

                bCRData = new BCRData(lstBCR);
                for (int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                {
                    if (bCRData[intIndex].funOpenBCR(ref strEM))
                        funWriteSysTraceLog("Connection BCR:" + bCRData[intIndex]._BCRName + " Success!");
                    else
                        funWriteSysTraceLog("Connection BCR:" + bCRData[intIndex]._BCRName + " Fail!");
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funStart()
        {
            if (InitSys.funOpendMPLC())
                funWriteSysTraceLog("Connection MPLC Success!");
            else
                funWriteSysTraceLog("Connection MPLC Fail!");

            //if (InitSys.funOpendSPLC())
            //    funWriteSysTraceLog("Connection SPLC Success!");
            //else
            //    funWriteSysTraceLog("Connection SPLC Fail!");

            if (InitSys.funOpendDB())
                funWriteSysTraceLog("Connection DB Success!");
            else
                funWriteSysTraceLog("Connection DB Fail!");

            timRefresh.Start();
            timProgram.Start();
            //timSPLCProgram.Start();
            //timUpdate.Start();
        }
        #endregion Inital Function

        #region Reconnection Function
        private void funReconnectionDB()
        {
            try
            {
                string strEM = string.Empty;
                if (InitSys._DB != null)
                {
                    InitSys._DB.Close();

                    if (InitSys._DB.Open(ref strEM))
                        funWriteSysTraceLog("Try Reconnection DB Success!");
                    else
                        funWriteSysTraceLog("Try Reconnection DB Fail!");
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                bolDB = false;
                funEnableButton(btnReconnectDB, true);
            }
        }

        private void funReconnectionBCR()
        {
            try
            {
                string strEM = string.Empty;
                for (int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
                {
                    if (bCRData[intIndex] != null)
                    {
                        bCRData[intIndex].funClose();
                        if (bCRData[intIndex].funOpenBCR(ref strEM))
                            funWriteSysTraceLog("Try Reconnection BCR:" + bCRData[intIndex]._BCRName + " Success!");
                        else
                            funWriteSysTraceLog("Try Reconnection BCR:" + bCRData[intIndex]._BCRName + " Fail!");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                bolBCR = false;
                funEnableButton(btnReconnectBCR, true);
            }
        }

        private void funReconnectionMPLC()
        {
            try
            {
                string strEM = string.Empty;
                if (InitSys._MPLC != null)
                {
                    InitSys._MPLC.funClose();
                    if (InitSys._MPLC.funOpenMPLC(ref strEM))
                        funWriteSysTraceLog("Try Reconnection MPLC Success!");
                    else
                        funWriteSysTraceLog("Try Reconnection MPLC Fail!");
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                bolMPLC = false;
                funEnableButton(btnReconnectMPLC, true);
            }
        }

        private void funReconnectionSPLC()
        {
            try
            {
                string strEM = string.Empty;
                if (InitSys._SPLC != null)
                {
                    InitSys._SPLC.funClose();
                    if (InitSys._SPLC.funOpenSPLC(ref strEM))
                        funWriteSysTraceLog("Try Reconnection SPLC Success!");
                    else
                        funWriteSysTraceLog("Try Reconnection SPLC Fail!");
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                bolSPLC = false;
                funEnableButton(btnReconnectSPLC, true);
            }
        }
        #endregion Reconnection Function

        #region Write Log Function
        private void funWriteSysTraceLog(string message)
        {
            try
            {
                if (lsbSysTrace.InvokeRequired)
                {
                    ShowMessage_EventHandler ShowMessage = new ShowMessage_EventHandler(funWriteSysTraceLog);
                    lsbSysTrace.Invoke(ShowMessage, message);
                }
                else
                {
                    if (lsbSysTrace.Items.Count >= 200)
                        lsbSysTrace.Items.RemoveAt(0);

                    lsbSysTrace.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message);
                    lsbSysTrace.SelectedIndex = lsbSysTrace.Items.Count - 1;

                    InitSys.funWriteLog("SysTrace", message);
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funWriteUpdateLog(string message)
        {
            try
            {
                if (lsbUpdate.InvokeRequired)
                {
                    ShowMessage_EventHandler ShowMessage = new ShowMessage_EventHandler(funWriteUpdateLog);
                    lsbUpdate.Invoke(ShowMessage, message);
                }
                else
                {
                    if (lsbUpdate.Items.Count >= 200)
                        lsbUpdate.Items.RemoveAt(0);

                    lsbUpdate.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message);
                    lsbUpdate.SelectedIndex = lsbUpdate.Items.Count - 1;

                    InitSys.funWriteLog("Update", message);
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }
        #endregion Write Log Function

        #region Other Function
        private void funShowAutoPause(bool auto)
        {
            switch (auto)
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
            switch (connect)
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
                if (button.InvokeRequired)
                {
                    ButtonEnable_EventHandler ShowMessage = new ButtonEnable_EventHandler(funEnableButton);
                    button.Invoke(ShowMessage, button, enable);
                }
                else
                    button.Enabled = enable;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
        }

        private void funRefreshCrane()
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCraneMode = new DataTable();
            DataTable dtCraneState = new DataTable();
            DataTable dtCmdSno = new DataTable();

            try
            {
                strSQL = "SELECT * FROM EQUMODELOG";
                strSQL += " WHERE ENDDT IN ('', ' ')";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCraneMode, ref strEM))
                {
                    for (int intCount = 0; intCount < dtCraneMode.Rows.Count; intCount++)
                    {
                        int intCraneNo = int.Parse(dtCraneMode.Rows[intCount]["EQUNO"].ToString());
                        if (dicCraneMap.ContainsKey(intCraneNo))
                        {
                            string strCurrentCraneMode = dtCraneMode.Rows[intCount]["EQUMODE"].ToString();
                            string strLastCraneMode = dicCraneMap[intCraneNo]._CraneMode;
                            dicCraneMap[intCraneNo]._CraneMode = strCurrentCraneMode;
                            strMsg = "Crane" + intCraneNo + "|";
                            strMsg += strLastCraneMode + ">" + strCurrentCraneMode + "|";
                            strMsg += "Crane Mode Change!";
                        }
                    }
                }
                strSQL = "SELECT * FROM EQUSTSLOG";
                strSQL += " WHERE ENDDT IN ('', ' ')";
                if (InitSys._DB.GetDataTable(strSQL, ref dtCraneState, ref strEM))
                {
                    for (int intCount = 0; intCount < dtCraneState.Rows.Count; intCount++)
                    {
                        int intCraneNo = int.Parse(dtCraneState.Rows[intCount]["EQUNO"].ToString());
                        if (dicCraneMap.ContainsKey(intCraneNo))
                        {
                            string strCurrentCraneState = dtCraneState.Rows[intCount]["EQUSTS"].ToString();
                            string strLastCraneState = dicCraneMap[intCraneNo]._CraneState;
                            dicCraneMap[intCraneNo]._CraneState = strCurrentCraneState;
                            strMsg = "Crane" + intCraneNo + "|";
                            strMsg += strLastCraneState + ">" + strCurrentCraneState + "|";
                            strMsg += "Crane State Change!";
                        }
                    }
                }
                foreach (int intCraneNo in dicCraneMap.Keys)
                {
                    strSQL = "SELECT * FROM EQUCMD";
                    strSQL += " WHERE CMDSTS ='1'";
                    strSQL += " AND EQUNO='" + intCraneNo + "'";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                        dicCraneMap[intCraneNo]._CommandID = dtCmdSno.Rows[0]["CmdSno"].ToString();
                    else
                        dicCraneMap[intCraneNo]._CommandID = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if (dtCraneMode != null)
                {
                    dtCraneMode.Clear();
                    dtCraneMode.Dispose();
                    dtCraneMode = null;
                }
                if (dtCraneState != null)
                {
                    dtCraneState.Clear();
                    dtCraneState.Dispose();
                    dtCraneState = null;
                }
                if (dtCraneMode != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }

        private string funIntArrayConvertASCII(int[] intArray)
        {
            string strResults = string.Empty;
            try
            {
                foreach (int intData in intArray)
                {
                    string strTemp_0007 = intData.ToString("X").PadLeft(4, "0"[0]).Substring(0, 2);
                    string strTemp_0815 = intData.ToString("X").PadLeft(4, "0"[0]).Substring(2, 2);
                    strResults += Convert.ToChar(Convert.ToInt32(strTemp_0007, 16)).ToString();
                    strResults += Convert.ToChar(Convert.ToInt32(strTemp_0815, 16)).ToString();
                }

                string strTemp = string.Empty;
                if (strResults.IndexOf("\n"[0]) >= 0)
                {
                    strTemp = strResults.Remove(strResults.IndexOf("\n"[0]));
                    strTemp = strTemp.Trim("\0"[0]).Trim();
                }
                else
                {
                    if (strResults.IndexOf("\0"[0]) >= 0)
                    {
                        strTemp = strResults.Remove(strResults.IndexOf("\0"[0]));
                        strTemp = strTemp.Trim();
                    }
                    else
                        strTemp = strResults.Trim();
                }
                return strTemp;
            }
            catch (Exception ex)
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
            //timUpdate.Stop();
            InitSys._DB.Close();
            InitSys._MPLC.funClose();
            InitSys._SPLC.funClose();
            for (int intIndex = 0; intIndex < bCRData._BCRCount; intIndex++)
            {
                if (bCRData[intIndex] != null)
                    bCRData[intIndex].funClose();
            }
            bolClose = true;
            this.Close();
        }
        #endregion Other Function


        #region Maintain Function
        private void btn_Query_Click(object sender, EventArgs e)
        {
            Query();
        }
        private void Query()
        {
            string strEM = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            string strSql = string.Format("select * from equcmd");
            if (InitSys._DB.GetDataTable(strSql, ref dtCmdSno, ref strEM))
            {
                dgvCmdMst.DataSource = dtCmdSno;
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string strEM = string.Empty;
            if (dgvCmdMst.CurrentCell.RowIndex < 0)
            {
                return;
            }
            int row = dgvCmdMst.CurrentCell.RowIndex;
            string strCmdSno = dgvCmdMst.Rows[row].Cells[1].Value.ToString();
            string strSql = string.Format("insert into EquCmdHis select '{1}' as HISDT,* from EquCmd where EquNo = 'R1' and CmdSno = '{0}'", strCmdSno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (InitSys._DB.ExecuteSQL(strSql, ref strEM))
            {
                strSql = string.Format("delete from equcmd where cmdsno='{0}'", strCmdSno);
                if (InitSys._DB.ExecuteSQL(strSql, ref strEM))
                {
                    Query();
                    funWriteSysTraceLog(strCmdSno + "清除成功!");
                }
                else
                {
                    funWriteSysTraceLog(strCmdSno + "清除失败!");
                }
            }
            else
            {
                funWriteSysTraceLog(strCmdSno + "清除失败!");
            }

        }
        #endregion

    }
}
