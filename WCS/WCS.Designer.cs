namespace Mirle.ASRS
{
    partial class WCS
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WCS));
            this.gpbMainState = new System.Windows.Forms.GroupBox();
            this.tlpMainState = new System.Windows.Forms.TableLayoutPanel();
            this.lblSPLCSts = new System.Windows.Forms.Label();
            this.lblSPLCStsLabel = new System.Windows.Forms.Label();
            this.lblMPLCSts = new System.Windows.Forms.Label();
            this.lblMPLCStsLabel = new System.Windows.Forms.Label();
            this.lblDBSts = new System.Windows.Forms.Label();
            this.lblDBStsLabel = new System.Windows.Forms.Label();
            this.lblCmuSts = new System.Windows.Forms.Label();
            this.lblCmuStsLabel = new System.Windows.Forms.Label();
            this.btnAutoPause = new System.Windows.Forms.Button();
            this.tlpLogo = new System.Windows.Forms.TableLayoutPanel();
            this.ptbLogo = new System.Windows.Forms.PictureBox();
            this.lblDataTime = new System.Windows.Forms.Label();
            this.sctMain1 = new System.Windows.Forms.SplitContainer();
            this.tlpButton = new System.Windows.Forms.TableLayoutPanel();
            this.btnReconnectDB = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnReconnectMPLC = new System.Windows.Forms.Button();
            this.btnReconnectBCR = new System.Windows.Forms.Button();
            this.chkAutoReconnect = new System.Windows.Forms.CheckBox();
            this.btnReconnectSPLC = new System.Windows.Forms.Button();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tbpSysTrace = new System.Windows.Forms.TabPage();
            this.lsbSysTrace = new System.Windows.Forms.ListBox();
            this.tbpUpdate = new System.Windows.Forms.TabPage();
            this.lsbUpdate = new System.Windows.Forms.ListBox();
            this.tbpFlowControl = new System.Windows.Forms.TabPage();
            this.gpbMainState.SuspendLayout();
            this.tlpMainState.SuspendLayout();
            this.tlpLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sctMain1)).BeginInit();
            this.sctMain1.Panel1.SuspendLayout();
            this.sctMain1.Panel2.SuspendLayout();
            this.sctMain1.SuspendLayout();
            this.tlpButton.SuspendLayout();
            this.tbcMain.SuspendLayout();
            this.tbpSysTrace.SuspendLayout();
            this.tbpUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpbMainState
            // 
            this.gpbMainState.AutoSize = true;
            this.gpbMainState.Controls.Add(this.tlpMainState);
            this.gpbMainState.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpbMainState.Location = new System.Drawing.Point(0, 84);
            this.gpbMainState.Name = "gpbMainState";
            this.gpbMainState.Size = new System.Drawing.Size(177, 121);
            this.gpbMainState.TabIndex = 9;
            this.gpbMainState.TabStop = false;
            this.gpbMainState.Text = "Main State";
            // 
            // tlpMainState
            // 
            this.tlpMainState.AutoSize = true;
            this.tlpMainState.ColumnCount = 2;
            this.tlpMainState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpMainState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMainState.Controls.Add(this.lblSPLCSts, 1, 3);
            this.tlpMainState.Controls.Add(this.lblSPLCStsLabel, 0, 3);
            this.tlpMainState.Controls.Add(this.lblMPLCSts, 1, 2);
            this.tlpMainState.Controls.Add(this.lblMPLCStsLabel, 0, 2);
            this.tlpMainState.Controls.Add(this.lblDBSts, 1, 1);
            this.tlpMainState.Controls.Add(this.lblDBStsLabel, 0, 1);
            this.tlpMainState.Controls.Add(this.lblCmuSts, 1, 0);
            this.tlpMainState.Controls.Add(this.lblCmuStsLabel, 0, 0);
            this.tlpMainState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainState.Location = new System.Drawing.Point(3, 18);
            this.tlpMainState.Name = "tlpMainState";
            this.tlpMainState.RowCount = 4;
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.Size = new System.Drawing.Size(171, 100);
            this.tlpMainState.TabIndex = 4;
            // 
            // lblSPLCSts
            // 
            this.lblSPLCSts.BackColor = System.Drawing.Color.Red;
            this.lblSPLCSts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSPLCSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSPLCSts.ForeColor = System.Drawing.Color.Black;
            this.lblSPLCSts.Location = new System.Drawing.Point(75, 75);
            this.lblSPLCSts.Margin = new System.Windows.Forms.Padding(0);
            this.lblSPLCSts.Name = "lblSPLCSts";
            this.lblSPLCSts.Size = new System.Drawing.Size(96, 25);
            this.lblSPLCSts.TabIndex = 24;
            this.lblSPLCSts.Text = "未连线";
            this.lblSPLCSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSPLCStsLabel
            // 
            this.lblSPLCStsLabel.BackColor = System.Drawing.Color.Black;
            this.lblSPLCStsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSPLCStsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSPLCStsLabel.ForeColor = System.Drawing.Color.White;
            this.lblSPLCStsLabel.Location = new System.Drawing.Point(0, 75);
            this.lblSPLCStsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.lblSPLCStsLabel.Name = "lblSPLCStsLabel";
            this.lblSPLCStsLabel.Size = new System.Drawing.Size(75, 25);
            this.lblSPLCStsLabel.TabIndex = 23;
            this.lblSPLCStsLabel.Text = "SPLC状态";
            this.lblSPLCStsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMPLCSts
            // 
            this.lblMPLCSts.BackColor = System.Drawing.Color.Red;
            this.lblMPLCSts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMPLCSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMPLCSts.ForeColor = System.Drawing.Color.Black;
            this.lblMPLCSts.Location = new System.Drawing.Point(75, 50);
            this.lblMPLCSts.Margin = new System.Windows.Forms.Padding(0);
            this.lblMPLCSts.Name = "lblMPLCSts";
            this.lblMPLCSts.Size = new System.Drawing.Size(96, 25);
            this.lblMPLCSts.TabIndex = 22;
            this.lblMPLCSts.Text = "未连线";
            this.lblMPLCSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMPLCStsLabel
            // 
            this.lblMPLCStsLabel.BackColor = System.Drawing.Color.Black;
            this.lblMPLCStsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMPLCStsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMPLCStsLabel.ForeColor = System.Drawing.Color.White;
            this.lblMPLCStsLabel.Location = new System.Drawing.Point(0, 50);
            this.lblMPLCStsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.lblMPLCStsLabel.Name = "lblMPLCStsLabel";
            this.lblMPLCStsLabel.Size = new System.Drawing.Size(75, 25);
            this.lblMPLCStsLabel.TabIndex = 21;
            this.lblMPLCStsLabel.Text = "MPLC状态";
            this.lblMPLCStsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDBSts
            // 
            this.lblDBSts.BackColor = System.Drawing.Color.Red;
            this.lblDBSts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDBSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBSts.ForeColor = System.Drawing.Color.Black;
            this.lblDBSts.Location = new System.Drawing.Point(75, 25);
            this.lblDBSts.Margin = new System.Windows.Forms.Padding(0);
            this.lblDBSts.Name = "lblDBSts";
            this.lblDBSts.Size = new System.Drawing.Size(96, 25);
            this.lblDBSts.TabIndex = 20;
            this.lblDBSts.Text = "未连线";
            this.lblDBSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDBStsLabel
            // 
            this.lblDBStsLabel.BackColor = System.Drawing.Color.Black;
            this.lblDBStsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDBStsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBStsLabel.ForeColor = System.Drawing.Color.White;
            this.lblDBStsLabel.Location = new System.Drawing.Point(0, 25);
            this.lblDBStsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.lblDBStsLabel.Name = "lblDBStsLabel";
            this.lblDBStsLabel.Size = new System.Drawing.Size(75, 25);
            this.lblDBStsLabel.TabIndex = 19;
            this.lblDBStsLabel.Text = "数据库状态";
            this.lblDBStsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCmuSts
            // 
            this.lblCmuSts.BackColor = System.Drawing.Color.Lime;
            this.lblCmuSts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCmuSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCmuSts.ForeColor = System.Drawing.Color.Black;
            this.lblCmuSts.Location = new System.Drawing.Point(75, 0);
            this.lblCmuSts.Margin = new System.Windows.Forms.Padding(0);
            this.lblCmuSts.Name = "lblCmuSts";
            this.lblCmuSts.Size = new System.Drawing.Size(96, 25);
            this.lblCmuSts.TabIndex = 16;
            this.lblCmuSts.Text = "自动模式";
            this.lblCmuSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCmuStsLabel
            // 
            this.lblCmuStsLabel.BackColor = System.Drawing.Color.Black;
            this.lblCmuStsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCmuStsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCmuStsLabel.ForeColor = System.Drawing.Color.White;
            this.lblCmuStsLabel.Location = new System.Drawing.Point(0, 0);
            this.lblCmuStsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.lblCmuStsLabel.Name = "lblCmuStsLabel";
            this.lblCmuStsLabel.Size = new System.Drawing.Size(75, 25);
            this.lblCmuStsLabel.TabIndex = 15;
            this.lblCmuStsLabel.Text = "系统状态";
            this.lblCmuStsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAutoPause
            // 
            this.btnAutoPause.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAutoPause.Font = new System.Drawing.Font("新細明體", 9F);
            this.btnAutoPause.Location = new System.Drawing.Point(41, 43);
            this.btnAutoPause.Name = "btnAutoPause";
            this.btnAutoPause.Size = new System.Drawing.Size(94, 34);
            this.btnAutoPause.TabIndex = 13;
            this.btnAutoPause.Text = "维护模式";
            this.btnAutoPause.UseVisualStyleBackColor = true;
            this.btnAutoPause.Click += new System.EventHandler(this.btnAutoPause_Click);
            // 
            // tlpLogo
            // 
            this.tlpLogo.AutoSize = true;
            this.tlpLogo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpLogo.ColumnCount = 2;
            this.tlpLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tlpLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLogo.Controls.Add(this.ptbLogo, 0, 0);
            this.tlpLogo.Controls.Add(this.lblDataTime, 0, 1);
            this.tlpLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpLogo.Location = new System.Drawing.Point(0, 0);
            this.tlpLogo.Name = "tlpLogo";
            this.tlpLogo.RowCount = 3;
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLogo.Size = new System.Drawing.Size(177, 84);
            this.tlpLogo.TabIndex = 14;
            // 
            // ptbLogo
            // 
            this.ptbLogo.BackColor = System.Drawing.Color.White;
            this.ptbLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptbLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ptbLogo.Image = global::Mirle.ASRS.Properties.Resources.logo;
            this.ptbLogo.Location = new System.Drawing.Point(1, 1);
            this.ptbLogo.Margin = new System.Windows.Forms.Padding(0);
            this.ptbLogo.Name = "ptbLogo";
            this.ptbLogo.Size = new System.Drawing.Size(175, 50);
            this.ptbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ptbLogo.TabIndex = 0;
            this.ptbLogo.TabStop = false;
            // 
            // lblDataTime
            // 
            this.lblDataTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblDataTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDataTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataTime.Location = new System.Drawing.Point(1, 52);
            this.lblDataTime.Margin = new System.Windows.Forms.Padding(0);
            this.lblDataTime.Name = "lblDataTime";
            this.lblDataTime.Size = new System.Drawing.Size(175, 30);
            this.lblDataTime.TabIndex = 1;
            this.lblDataTime.Text = "yyyy-MM-dd HH:mm:ss";
            this.lblDataTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sctMain1
            // 
            this.sctMain1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sctMain1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sctMain1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sctMain1.IsSplitterFixed = true;
            this.sctMain1.Location = new System.Drawing.Point(0, 0);
            this.sctMain1.Name = "sctMain1";
            // 
            // sctMain1.Panel1
            // 
            this.sctMain1.Panel1.Controls.Add(this.tlpButton);
            this.sctMain1.Panel1.Controls.Add(this.gpbMainState);
            this.sctMain1.Panel1.Controls.Add(this.tlpLogo);
            // 
            // sctMain1.Panel2
            // 
            this.sctMain1.Panel2.Controls.Add(this.tbcMain);
            this.sctMain1.Size = new System.Drawing.Size(1084, 561);
            this.sctMain1.SplitterDistance = 181;
            this.sctMain1.TabIndex = 15;
            // 
            // tlpButton
            // 
            this.tlpButton.AutoSize = true;
            this.tlpButton.ColumnCount = 3;
            this.tlpButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButton.Controls.Add(this.btnReconnectDB, 1, 3);
            this.tlpButton.Controls.Add(this.btnExit, 1, 9);
            this.tlpButton.Controls.Add(this.btnAutoPause, 1, 1);
            this.tlpButton.Controls.Add(this.btnReconnectMPLC, 1, 4);
            this.tlpButton.Controls.Add(this.btnReconnectBCR, 1, 6);
            this.tlpButton.Controls.Add(this.chkAutoReconnect, 1, 7);
            this.tlpButton.Controls.Add(this.btnReconnectSPLC, 1, 5);
            this.tlpButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlpButton.Location = new System.Drawing.Point(0, 222);
            this.tlpButton.Name = "tlpButton";
            this.tlpButton.RowCount = 11;
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpButton.Size = new System.Drawing.Size(177, 335);
            this.tlpButton.TabIndex = 17;
            // 
            // btnReconnectDB
            // 
            this.btnReconnectDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectDB.Font = new System.Drawing.Font("新細明體", 9F);
            this.btnReconnectDB.Location = new System.Drawing.Point(41, 93);
            this.btnReconnectDB.Name = "btnReconnectDB";
            this.btnReconnectDB.Size = new System.Drawing.Size(94, 34);
            this.btnReconnectDB.TabIndex = 17;
            this.btnReconnectDB.Text = "重连数据库";
            this.btnReconnectDB.UseVisualStyleBackColor = true;
            this.btnReconnectDB.Click += new System.EventHandler(this.btnReconnectDB_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExit.Location = new System.Drawing.Point(41, 288);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(94, 34);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "结束退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnReconnectMPLC
            // 
            this.btnReconnectMPLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectMPLC.Font = new System.Drawing.Font("新細明體", 9F);
            this.btnReconnectMPLC.Location = new System.Drawing.Point(41, 133);
            this.btnReconnectMPLC.Name = "btnReconnectMPLC";
            this.btnReconnectMPLC.Size = new System.Drawing.Size(94, 34);
            this.btnReconnectMPLC.TabIndex = 6;
            this.btnReconnectMPLC.Text = "重连MPLC";
            this.btnReconnectMPLC.UseVisualStyleBackColor = true;
            this.btnReconnectMPLC.Click += new System.EventHandler(this.btnReconnectMPLC_Click);
            // 
            // btnReconnectBCR
            // 
            this.btnReconnectBCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectBCR.Font = new System.Drawing.Font("新細明體", 9F);
            this.btnReconnectBCR.Location = new System.Drawing.Point(41, 213);
            this.btnReconnectBCR.Name = "btnReconnectBCR";
            this.btnReconnectBCR.Size = new System.Drawing.Size(94, 34);
            this.btnReconnectBCR.TabIndex = 19;
            this.btnReconnectBCR.Text = "重连条码机";
            this.btnReconnectBCR.UseVisualStyleBackColor = true;
            this.btnReconnectBCR.Click += new System.EventHandler(this.btnReconnectBCR_Click);
            // 
            // chkAutoReconnect
            // 
            this.chkAutoReconnect.AutoSize = true;
            this.tlpButton.SetColumnSpan(this.chkAutoReconnect, 2);
            this.chkAutoReconnect.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkAutoReconnect.Location = new System.Drawing.Point(41, 253);
            this.chkAutoReconnect.Name = "chkAutoReconnect";
            this.chkAutoReconnect.Size = new System.Drawing.Size(72, 19);
            this.chkAutoReconnect.TabIndex = 20;
            this.chkAutoReconnect.Text = "自动重连";
            this.chkAutoReconnect.UseVisualStyleBackColor = true;
            // 
            // btnReconnectSPLC
            // 
            this.btnReconnectSPLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectSPLC.Font = new System.Drawing.Font("新細明體", 9F);
            this.btnReconnectSPLC.Location = new System.Drawing.Point(41, 173);
            this.btnReconnectSPLC.Name = "btnReconnectSPLC";
            this.btnReconnectSPLC.Size = new System.Drawing.Size(94, 34);
            this.btnReconnectSPLC.TabIndex = 21;
            this.btnReconnectSPLC.Text = "重连SPLC";
            this.btnReconnectSPLC.UseVisualStyleBackColor = true;
            this.btnReconnectSPLC.Click += new System.EventHandler(this.btnReconnectSPLC_Click);
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpSysTrace);
            this.tbcMain.Controls.Add(this.tbpUpdate);
            this.tbcMain.Controls.Add(this.tbpFlowControl);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(895, 557);
            this.tbcMain.TabIndex = 0;
            // 
            // tbpSysTrace
            // 
            this.tbpSysTrace.Controls.Add(this.lsbSysTrace);
            this.tbpSysTrace.Location = new System.Drawing.Point(4, 22);
            this.tbpSysTrace.Name = "tbpSysTrace";
            this.tbpSysTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSysTrace.Size = new System.Drawing.Size(887, 531);
            this.tbpSysTrace.TabIndex = 0;
            this.tbpSysTrace.Text = "Sys Trace";
            this.tbpSysTrace.UseVisualStyleBackColor = true;
            // 
            // lsbSysTrace
            // 
            this.lsbSysTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbSysTrace.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lsbSysTrace.FormattingEnabled = true;
            this.lsbSysTrace.ItemHeight = 16;
            this.lsbSysTrace.Location = new System.Drawing.Point(3, 3);
            this.lsbSysTrace.Name = "lsbSysTrace";
            this.lsbSysTrace.Size = new System.Drawing.Size(881, 525);
            this.lsbSysTrace.TabIndex = 0;
            // 
            // tbpUpdate
            // 
            this.tbpUpdate.Controls.Add(this.lsbUpdate);
            this.tbpUpdate.Location = new System.Drawing.Point(4, 22);
            this.tbpUpdate.Name = "tbpUpdate";
            this.tbpUpdate.Size = new System.Drawing.Size(887, 531);
            this.tbpUpdate.TabIndex = 2;
            this.tbpUpdate.Text = "Update";
            this.tbpUpdate.UseVisualStyleBackColor = true;
            // 
            // lsbUpdate
            // 
            this.lsbUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbUpdate.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lsbUpdate.FormattingEnabled = true;
            this.lsbUpdate.ItemHeight = 16;
            this.lsbUpdate.Location = new System.Drawing.Point(0, 0);
            this.lsbUpdate.Name = "lsbUpdate";
            this.lsbUpdate.Size = new System.Drawing.Size(887, 531);
            this.lsbUpdate.TabIndex = 1;
            // 
            // tbpFlowControl
            // 
            this.tbpFlowControl.AutoScroll = true;
            this.tbpFlowControl.Location = new System.Drawing.Point(4, 22);
            this.tbpFlowControl.Name = "tbpFlowControl";
            this.tbpFlowControl.Padding = new System.Windows.Forms.Padding(3);
            this.tbpFlowControl.Size = new System.Drawing.Size(887, 531);
            this.tbpFlowControl.TabIndex = 1;
            this.tbpFlowControl.Text = "Flow Control";
            this.tbpFlowControl.UseVisualStyleBackColor = true;
            // 
            // WCS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1084, 561);
            this.ControlBox = false;
            this.Controls.Add(this.sctMain1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1100, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1100, 600);
            this.Name = "WCS";
            this.Text = "WCS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WCS_FormClosing);
            this.Load += new System.EventHandler(this.WCS_Load);
            this.gpbMainState.ResumeLayout(false);
            this.gpbMainState.PerformLayout();
            this.tlpMainState.ResumeLayout(false);
            this.tlpLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ptbLogo)).EndInit();
            this.sctMain1.Panel1.ResumeLayout(false);
            this.sctMain1.Panel1.PerformLayout();
            this.sctMain1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sctMain1)).EndInit();
            this.sctMain1.ResumeLayout(false);
            this.tlpButton.ResumeLayout(false);
            this.tlpButton.PerformLayout();
            this.tbcMain.ResumeLayout(false);
            this.tbpSysTrace.ResumeLayout(false);
            this.tbpUpdate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbMainState;
        private System.Windows.Forms.TableLayoutPanel tlpMainState;
        private System.Windows.Forms.Label lblCmuStsLabel;
        private System.Windows.Forms.Label lblCmuSts;
        private System.Windows.Forms.Button btnAutoPause;
        private System.Windows.Forms.TableLayoutPanel tlpLogo;
        private System.Windows.Forms.PictureBox ptbLogo;
        private System.Windows.Forms.Label lblDataTime;
        private System.Windows.Forms.SplitContainer sctMain1;
        private System.Windows.Forms.TableLayoutPanel tlpButton;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblDBSts;
        private System.Windows.Forms.Label lblDBStsLabel;
        private System.Windows.Forms.Button btnReconnectDB;
        private System.Windows.Forms.Label lblMPLCSts;
        private System.Windows.Forms.Label lblMPLCStsLabel;
        private System.Windows.Forms.Label lblSPLCSts;
        private System.Windows.Forms.Label lblSPLCStsLabel;
        private System.Windows.Forms.Button btnReconnectMPLC;
        private System.Windows.Forms.Button btnReconnectBCR;
        private System.Windows.Forms.CheckBox chkAutoReconnect;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpSysTrace;
        private System.Windows.Forms.TabPage tbpFlowControl;
        private System.Windows.Forms.ListBox lsbSysTrace;
        private System.Windows.Forms.Button btnReconnectSPLC;
        private System.Windows.Forms.TabPage tbpUpdate;
        private System.Windows.Forms.ListBox lsbUpdate;
    }
}

