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
            this.sctMain1 = new System.Windows.Forms.SplitContainer();
            this.tlpButton = new System.Windows.Forms.TableLayoutPanel();
            this.btnReconnectDB = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnAutoPause = new System.Windows.Forms.Button();
            this.btnReconnectMPLC = new System.Windows.Forms.Button();
            this.btnReconnectBCR = new System.Windows.Forms.Button();
            this.chkAutoReconnect = new System.Windows.Forms.CheckBox();
            this.btnReconnectSPLC = new System.Windows.Forms.Button();
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
            this.tlpLogo = new System.Windows.Forms.TableLayoutPanel();
            this.ptbLogo = new System.Windows.Forms.PictureBox();
            this.lblDataTime = new System.Windows.Forms.Label();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tbpSystemTrace = new System.Windows.Forms.TabPage();
            this.lsbSysTrace = new System.Windows.Forms.ListBox();
            this.tbpUpdatePosted = new System.Windows.Forms.TabPage();
            this.lsbUpdate = new System.Windows.Forms.ListBox();
            this.tbpSingelMonitor = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvCmdMst = new System.Windows.Forms.DataGridView();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Query = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlCraneBottom = new System.Windows.Forms.Panel();
            this.lblCraneBottom = new System.Windows.Forms.Label();
            this.bufferMonitor50 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor49 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor39 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor40 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor41 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor47 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor48 = new Mirle.ASRS.BufferMonitor();
            this.craneMonitor5 = new Mirle.ASRS.CraneMonitor();
            this.bufferMonitor42 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor43 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor44 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor45 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor46 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor34 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor35 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor36 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor37 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor38 = new Mirle.ASRS.BufferMonitor();
            this.craneMonitor3 = new Mirle.ASRS.CraneMonitor();
            this.bufferMonitor29 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor30 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor31 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor32 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor33 = new Mirle.ASRS.BufferMonitor();
            this.craneMonitor4 = new Mirle.ASRS.CraneMonitor();
            this.bufferMonitor24 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor25 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor26 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor27 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor28 = new Mirle.ASRS.BufferMonitor();
            this.craneMonitor2 = new Mirle.ASRS.CraneMonitor();
            this.bufferMonitor22 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor23 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor21 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor20 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor5 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor12 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor13 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor14 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor15 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor16 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor17 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor18 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor19 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor8 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor9 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor10 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor11 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor7 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor6 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor4 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor3 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor2 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor1 = new Mirle.ASRS.BufferMonitor();
            this.craneMonitor1 = new Mirle.ASRS.CraneMonitor();
            this.bufferMonitor52 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor53 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor54 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor55 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor56 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor57 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor58 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor59 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor60 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor61 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor62 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor63 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor51 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor64 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor65 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor66 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor67 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor68 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor69 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor70 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor71 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor72 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor73 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor74 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor75 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor76 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor77 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor78 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor79 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor80 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor81 = new Mirle.ASRS.BufferMonitor();
            this.bufferMonitor82 = new Mirle.ASRS.BufferMonitor();
            ((System.ComponentModel.ISupportInitialize)(this.sctMain1)).BeginInit();
            this.sctMain1.Panel1.SuspendLayout();
            this.sctMain1.Panel2.SuspendLayout();
            this.sctMain1.SuspendLayout();
            this.tlpButton.SuspendLayout();
            this.gpbMainState.SuspendLayout();
            this.tlpMainState.SuspendLayout();
            this.tlpLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbLogo)).BeginInit();
            this.tbcMain.SuspendLayout();
            this.tbpSystemTrace.SuspendLayout();
            this.tbpUpdatePosted.SuspendLayout();
            this.tbpSingelMonitor.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCmdMst)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlCraneBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // sctMain1
            // 
            this.sctMain1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sctMain1.Dock = System.Windows.Forms.DockStyle.Left;
            this.sctMain1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
            this.sctMain1.Size = new System.Drawing.Size(2000, 709);
            this.sctMain1.SplitterDistance = 148;
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
            this.tlpButton.Location = new System.Drawing.Point(0, 370);
            this.tlpButton.Name = "tlpButton";
            this.tlpButton.RowCount = 11;
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpButton.Size = new System.Drawing.Size(144, 335);
            this.tlpButton.TabIndex = 17;
            // 
            // btnReconnectDB
            // 
            this.btnReconnectDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReconnectDB.Location = new System.Drawing.Point(25, 84);
            this.btnReconnectDB.Name = "btnReconnectDB";
            this.btnReconnectDB.Size = new System.Drawing.Size(94, 38);
            this.btnReconnectDB.TabIndex = 17;
            this.btnReconnectDB.Text = "重连数据库";
            this.btnReconnectDB.UseVisualStyleBackColor = true;
            this.btnReconnectDB.Click += new System.EventHandler(this.btnReconnectDB_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExit.Location = new System.Drawing.Point(25, 288);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(94, 34);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "结束退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnAutoPause
            // 
            this.btnAutoPause.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAutoPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAutoPause.Location = new System.Drawing.Point(25, 14);
            this.btnAutoPause.Name = "btnAutoPause";
            this.btnAutoPause.Size = new System.Drawing.Size(94, 38);
            this.btnAutoPause.TabIndex = 13;
            this.btnAutoPause.Text = "维护模式";
            this.btnAutoPause.UseVisualStyleBackColor = true;
            this.btnAutoPause.Click += new System.EventHandler(this.btnAutoPause_Click);
            // 
            // btnReconnectMPLC
            // 
            this.btnReconnectMPLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectMPLC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReconnectMPLC.Location = new System.Drawing.Point(25, 128);
            this.btnReconnectMPLC.Name = "btnReconnectMPLC";
            this.btnReconnectMPLC.Size = new System.Drawing.Size(94, 36);
            this.btnReconnectMPLC.TabIndex = 6;
            this.btnReconnectMPLC.Text = "重连MPLC";
            this.btnReconnectMPLC.UseVisualStyleBackColor = true;
            this.btnReconnectMPLC.Click += new System.EventHandler(this.btnReconnectMPLC_Click);
            // 
            // btnReconnectBCR
            // 
            this.btnReconnectBCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectBCR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReconnectBCR.Location = new System.Drawing.Point(25, 212);
            this.btnReconnectBCR.Name = "btnReconnectBCR";
            this.btnReconnectBCR.Size = new System.Drawing.Size(94, 35);
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
            this.chkAutoReconnect.Location = new System.Drawing.Point(25, 253);
            this.chkAutoReconnect.Name = "chkAutoReconnect";
            this.chkAutoReconnect.Size = new System.Drawing.Size(72, 19);
            this.chkAutoReconnect.TabIndex = 20;
            this.chkAutoReconnect.Text = "自动重连";
            this.chkAutoReconnect.UseVisualStyleBackColor = true;
            // 
            // btnReconnectSPLC
            // 
            this.btnReconnectSPLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReconnectSPLC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReconnectSPLC.Location = new System.Drawing.Point(25, 170);
            this.btnReconnectSPLC.Name = "btnReconnectSPLC";
            this.btnReconnectSPLC.Size = new System.Drawing.Size(94, 36);
            this.btnReconnectSPLC.TabIndex = 21;
            this.btnReconnectSPLC.Text = "重连SPLC";
            this.btnReconnectSPLC.UseVisualStyleBackColor = true;
            this.btnReconnectSPLC.Click += new System.EventHandler(this.btnReconnectSPLC_Click);
            // 
            // gpbMainState
            // 
            this.gpbMainState.AutoSize = true;
            this.gpbMainState.Controls.Add(this.tlpMainState);
            this.gpbMainState.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpbMainState.Location = new System.Drawing.Point(0, 84);
            this.gpbMainState.Name = "gpbMainState";
            this.gpbMainState.Size = new System.Drawing.Size(144, 120);
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
            this.tlpMainState.Location = new System.Drawing.Point(3, 17);
            this.tlpMainState.Name = "tlpMainState";
            this.tlpMainState.RowCount = 4;
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMainState.Size = new System.Drawing.Size(138, 100);
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
            this.lblSPLCSts.Size = new System.Drawing.Size(63, 25);
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
            this.lblMPLCSts.Size = new System.Drawing.Size(63, 25);
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
            this.lblDBSts.Size = new System.Drawing.Size(63, 25);
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
            this.lblCmuSts.Size = new System.Drawing.Size(63, 25);
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
            // tlpLogo
            // 
            this.tlpLogo.AutoSize = true;
            this.tlpLogo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpLogo.ColumnCount = 1;
            this.tlpLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpLogo.Controls.Add(this.ptbLogo, 0, 0);
            this.tlpLogo.Controls.Add(this.lblDataTime, 0, 1);
            this.tlpLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpLogo.Location = new System.Drawing.Point(0, 0);
            this.tlpLogo.Name = "tlpLogo";
            this.tlpLogo.RowCount = 3;
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpLogo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLogo.Size = new System.Drawing.Size(144, 84);
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
            this.ptbLogo.Size = new System.Drawing.Size(142, 50);
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
            this.lblDataTime.Size = new System.Drawing.Size(142, 30);
            this.lblDataTime.TabIndex = 1;
            this.lblDataTime.Text = "yyyy-MM-dd HH:mm:ss:fff";
            this.lblDataTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpSystemTrace);
            this.tbcMain.Controls.Add(this.tbpUpdatePosted);
            this.tbcMain.Controls.Add(this.tbpSingelMonitor);
            this.tbcMain.Controls.Add(this.tabPage1);
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1846, 702);
            this.tbcMain.TabIndex = 0;
            // 
            // tbpSystemTrace
            // 
            this.tbpSystemTrace.Controls.Add(this.lsbSysTrace);
            this.tbpSystemTrace.Location = new System.Drawing.Point(4, 22);
            this.tbpSystemTrace.Name = "tbpSystemTrace";
            this.tbpSystemTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSystemTrace.Size = new System.Drawing.Size(1358, 583);
            this.tbpSystemTrace.TabIndex = 0;
            this.tbpSystemTrace.Text = "System Trace";
            this.tbpSystemTrace.UseVisualStyleBackColor = true;
            // 
            // lsbSysTrace
            // 
            this.lsbSysTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbSysTrace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lsbSysTrace.FormattingEnabled = true;
            this.lsbSysTrace.ItemHeight = 20;
            this.lsbSysTrace.Location = new System.Drawing.Point(3, 3);
            this.lsbSysTrace.Name = "lsbSysTrace";
            this.lsbSysTrace.Size = new System.Drawing.Size(1352, 577);
            this.lsbSysTrace.TabIndex = 0;
            // 
            // tbpUpdatePosted
            // 
            this.tbpUpdatePosted.Controls.Add(this.lsbUpdate);
            this.tbpUpdatePosted.Location = new System.Drawing.Point(4, 22);
            this.tbpUpdatePosted.Name = "tbpUpdatePosted";
            this.tbpUpdatePosted.Size = new System.Drawing.Size(1358, 583);
            this.tbpUpdatePosted.TabIndex = 2;
            this.tbpUpdatePosted.Text = "Update Posted";
            this.tbpUpdatePosted.UseVisualStyleBackColor = true;
            // 
            // lsbUpdate
            // 
            this.lsbUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lsbUpdate.FormattingEnabled = true;
            this.lsbUpdate.ItemHeight = 20;
            this.lsbUpdate.Location = new System.Drawing.Point(0, 0);
            this.lsbUpdate.Name = "lsbUpdate";
            this.lsbUpdate.Size = new System.Drawing.Size(1358, 583);
            this.lsbUpdate.TabIndex = 1;
            // 
            // tbpSingelMonitor
            // 
            this.tbpSingelMonitor.AutoScroll = true;
            this.tbpSingelMonitor.Controls.Add(this.panel1);
            this.tbpSingelMonitor.Location = new System.Drawing.Point(4, 22);
            this.tbpSingelMonitor.Name = "tbpSingelMonitor";
            this.tbpSingelMonitor.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSingelMonitor.Size = new System.Drawing.Size(1838, 676);
            this.tbpSingelMonitor.TabIndex = 1;
            this.tbpSingelMonitor.Text = "Singel Monitor";
            this.tbpSingelMonitor.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1358, 583);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Maintain";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvCmdMst);
            this.groupBox2.Controls.Add(this.btn_Delete);
            this.groupBox2.Controls.Add(this.btn_Query);
            this.groupBox2.Location = new System.Drawing.Point(79, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(692, 222);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "命令查询";
            // 
            // dgvCmdMst
            // 
            this.dgvCmdMst.AllowUserToAddRows = false;
            this.dgvCmdMst.AllowUserToDeleteRows = false;
            this.dgvCmdMst.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvCmdMst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCmdMst.Location = new System.Drawing.Point(6, 19);
            this.dgvCmdMst.Name = "dgvCmdMst";
            this.dgvCmdMst.ReadOnly = true;
            this.dgvCmdMst.RowHeadersVisible = false;
            this.dgvCmdMst.RowTemplate.Height = 23;
            this.dgvCmdMst.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCmdMst.Size = new System.Drawing.Size(683, 154);
            this.dgvCmdMst.TabIndex = 3;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(559, 179);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(129, 37);
            this.btn_Delete.TabIndex = 2;
            this.btn_Delete.Text = "清理主机命令";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Query
            // 
            this.btn_Query.Location = new System.Drawing.Point(6, 179);
            this.btn_Query.Name = "btn_Query";
            this.btn_Query.Size = new System.Drawing.Size(129, 37);
            this.btn_Query.TabIndex = 0;
            this.btn_Query.Text = "查询主机命令";
            this.btn_Query.UseVisualStyleBackColor = true;
            this.btn_Query.Click += new System.EventHandler(this.btn_Query_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.bufferMonitor82);
            this.panel1.Controls.Add(this.bufferMonitor81);
            this.panel1.Controls.Add(this.bufferMonitor77);
            this.panel1.Controls.Add(this.bufferMonitor78);
            this.panel1.Controls.Add(this.bufferMonitor79);
            this.panel1.Controls.Add(this.bufferMonitor80);
            this.panel1.Controls.Add(this.bufferMonitor75);
            this.panel1.Controls.Add(this.bufferMonitor76);
            this.panel1.Controls.Add(this.bufferMonitor51);
            this.panel1.Controls.Add(this.bufferMonitor64);
            this.panel1.Controls.Add(this.bufferMonitor65);
            this.panel1.Controls.Add(this.bufferMonitor66);
            this.panel1.Controls.Add(this.bufferMonitor67);
            this.panel1.Controls.Add(this.bufferMonitor68);
            this.panel1.Controls.Add(this.bufferMonitor69);
            this.panel1.Controls.Add(this.bufferMonitor70);
            this.panel1.Controls.Add(this.bufferMonitor71);
            this.panel1.Controls.Add(this.bufferMonitor72);
            this.panel1.Controls.Add(this.bufferMonitor73);
            this.panel1.Controls.Add(this.bufferMonitor74);
            this.panel1.Controls.Add(this.bufferMonitor52);
            this.panel1.Controls.Add(this.bufferMonitor53);
            this.panel1.Controls.Add(this.bufferMonitor54);
            this.panel1.Controls.Add(this.bufferMonitor55);
            this.panel1.Controls.Add(this.bufferMonitor56);
            this.panel1.Controls.Add(this.bufferMonitor57);
            this.panel1.Controls.Add(this.bufferMonitor58);
            this.panel1.Controls.Add(this.bufferMonitor59);
            this.panel1.Controls.Add(this.bufferMonitor60);
            this.panel1.Controls.Add(this.bufferMonitor61);
            this.panel1.Controls.Add(this.bufferMonitor62);
            this.panel1.Controls.Add(this.bufferMonitor63);
            this.panel1.Controls.Add(this.bufferMonitor50);
            this.panel1.Controls.Add(this.bufferMonitor49);
            this.panel1.Controls.Add(this.bufferMonitor39);
            this.panel1.Controls.Add(this.bufferMonitor40);
            this.panel1.Controls.Add(this.bufferMonitor41);
            this.panel1.Controls.Add(this.bufferMonitor47);
            this.panel1.Controls.Add(this.bufferMonitor48);
            this.panel1.Controls.Add(this.craneMonitor5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.bufferMonitor42);
            this.panel1.Controls.Add(this.bufferMonitor43);
            this.panel1.Controls.Add(this.bufferMonitor44);
            this.panel1.Controls.Add(this.bufferMonitor45);
            this.panel1.Controls.Add(this.bufferMonitor46);
            this.panel1.Controls.Add(this.bufferMonitor34);
            this.panel1.Controls.Add(this.bufferMonitor35);
            this.panel1.Controls.Add(this.bufferMonitor36);
            this.panel1.Controls.Add(this.bufferMonitor37);
            this.panel1.Controls.Add(this.bufferMonitor38);
            this.panel1.Controls.Add(this.craneMonitor3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.bufferMonitor29);
            this.panel1.Controls.Add(this.bufferMonitor30);
            this.panel1.Controls.Add(this.bufferMonitor31);
            this.panel1.Controls.Add(this.bufferMonitor32);
            this.panel1.Controls.Add(this.bufferMonitor33);
            this.panel1.Controls.Add(this.craneMonitor4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.bufferMonitor24);
            this.panel1.Controls.Add(this.bufferMonitor25);
            this.panel1.Controls.Add(this.bufferMonitor26);
            this.panel1.Controls.Add(this.bufferMonitor27);
            this.panel1.Controls.Add(this.bufferMonitor28);
            this.panel1.Controls.Add(this.craneMonitor2);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.bufferMonitor22);
            this.panel1.Controls.Add(this.bufferMonitor23);
            this.panel1.Controls.Add(this.bufferMonitor21);
            this.panel1.Controls.Add(this.bufferMonitor20);
            this.panel1.Controls.Add(this.bufferMonitor5);
            this.panel1.Controls.Add(this.bufferMonitor12);
            this.panel1.Controls.Add(this.bufferMonitor13);
            this.panel1.Controls.Add(this.bufferMonitor14);
            this.panel1.Controls.Add(this.bufferMonitor15);
            this.panel1.Controls.Add(this.bufferMonitor16);
            this.panel1.Controls.Add(this.bufferMonitor17);
            this.panel1.Controls.Add(this.bufferMonitor18);
            this.panel1.Controls.Add(this.bufferMonitor19);
            this.panel1.Controls.Add(this.bufferMonitor8);
            this.panel1.Controls.Add(this.bufferMonitor9);
            this.panel1.Controls.Add(this.bufferMonitor10);
            this.panel1.Controls.Add(this.bufferMonitor11);
            this.panel1.Controls.Add(this.bufferMonitor7);
            this.panel1.Controls.Add(this.bufferMonitor6);
            this.panel1.Controls.Add(this.bufferMonitor4);
            this.panel1.Controls.Add(this.bufferMonitor3);
            this.panel1.Controls.Add(this.bufferMonitor2);
            this.panel1.Controls.Add(this.bufferMonitor1);
            this.panel1.Controls.Add(this.craneMonitor1);
            this.panel1.Controls.Add(this.pnlCraneBottom);
            this.panel1.Location = new System.Drawing.Point(7, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1828, 663);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Lime;
            this.panel4.Controls.Add(this.label4);
            this.panel4.Location = new System.Drawing.Point(1048, 296);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(103, 197);
            this.panel4.TabIndex = 218;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "下层";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Lime;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(839, 296);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(103, 197);
            this.panel2.TabIndex = 206;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "下层";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Lime;
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(630, 296);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(103, 197);
            this.panel3.TabIndex = 199;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "下层";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Lime;
            this.panel5.Controls.Add(this.label1);
            this.panel5.Location = new System.Drawing.Point(421, 296);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(103, 197);
            this.panel5.TabIndex = 192;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "下层";
            // 
            // pnlCraneBottom
            // 
            this.pnlCraneBottom.BackColor = System.Drawing.Color.Lime;
            this.pnlCraneBottom.Controls.Add(this.lblCraneBottom);
            this.pnlCraneBottom.Location = new System.Drawing.Point(212, 296);
            this.pnlCraneBottom.Name = "pnlCraneBottom";
            this.pnlCraneBottom.Size = new System.Drawing.Size(103, 197);
            this.pnlCraneBottom.TabIndex = 167;
            // 
            // lblCraneBottom
            // 
            this.lblCraneBottom.AutoSize = true;
            this.lblCraneBottom.Location = new System.Drawing.Point(39, 88);
            this.lblCraneBottom.Name = "lblCraneBottom";
            this.lblCraneBottom.Size = new System.Drawing.Size(29, 12);
            this.lblCraneBottom.TabIndex = 0;
            this.lblCraneBottom.Text = "下层";
            // 
            // bufferMonitor50
            // 
            this.bufferMonitor50._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor50._BufferName = "A01";
            this.bufferMonitor50._CommandID = "00000";
            this.bufferMonitor50._Destination = "0";
            this.bufferMonitor50._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor50._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor50._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor50._ReturnRequest = false;
            this.bufferMonitor50.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor50.Location = new System.Drawing.Point(3, 137);
            this.bufferMonitor50.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor50.Name = "bufferMonitor50";
            this.bufferMonitor50.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor50.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor50.TabIndex = 226;
            // 
            // bufferMonitor49
            // 
            this.bufferMonitor49._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor49._BufferName = "BufName";
            this.bufferMonitor49._CommandID = "00000";
            this.bufferMonitor49._Destination = "0";
            this.bufferMonitor49._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor49._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor49._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor49._ReturnRequest = false;
            this.bufferMonitor49.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor49.Location = new System.Drawing.Point(1100, 3);
            this.bufferMonitor49.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor49.Name = "bufferMonitor49";
            this.bufferMonitor49.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor49.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor49.TabIndex = 225;
            // 
            // bufferMonitor39
            // 
            this.bufferMonitor39._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor39._BufferName = "BufName";
            this.bufferMonitor39._CommandID = "00000";
            this.bufferMonitor39._Destination = "0";
            this.bufferMonitor39._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor39._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor39._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor39._ReturnRequest = false;
            this.bufferMonitor39.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor39.Location = new System.Drawing.Point(1100, 137);
            this.bufferMonitor39.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor39.Name = "bufferMonitor39";
            this.bufferMonitor39.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor39.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor39.TabIndex = 224;
            // 
            // bufferMonitor40
            // 
            this.bufferMonitor40._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor40._BufferName = "BufName";
            this.bufferMonitor40._CommandID = "00000";
            this.bufferMonitor40._Destination = "0";
            this.bufferMonitor40._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor40._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor40._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor40._ReturnRequest = false;
            this.bufferMonitor40.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor40.Location = new System.Drawing.Point(1048, 137);
            this.bufferMonitor40.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor40.Name = "bufferMonitor40";
            this.bufferMonitor40.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor40.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor40.TabIndex = 223;
            // 
            // bufferMonitor41
            // 
            this.bufferMonitor41._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor41._BufferName = "BufName";
            this.bufferMonitor41._CommandID = "00000";
            this.bufferMonitor41._Destination = "0";
            this.bufferMonitor41._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor41._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor41._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor41._ReturnRequest = false;
            this.bufferMonitor41.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor41.Location = new System.Drawing.Point(1100, 70);
            this.bufferMonitor41.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor41.Name = "bufferMonitor41";
            this.bufferMonitor41.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor41.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor41.TabIndex = 222;
            // 
            // bufferMonitor47
            // 
            this.bufferMonitor47._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor47._BufferName = "BufName";
            this.bufferMonitor47._CommandID = "00000";
            this.bufferMonitor47._Destination = "0";
            this.bufferMonitor47._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor47._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor47._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor47._ReturnRequest = false;
            this.bufferMonitor47.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor47.Location = new System.Drawing.Point(1048, 70);
            this.bufferMonitor47.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor47.Name = "bufferMonitor47";
            this.bufferMonitor47.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor47.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor47.TabIndex = 221;
            // 
            // bufferMonitor48
            // 
            this.bufferMonitor48._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor48._BufferName = "BufName";
            this.bufferMonitor48._CommandID = "00000";
            this.bufferMonitor48._Destination = "0";
            this.bufferMonitor48._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor48._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor48._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor48._ReturnRequest = false;
            this.bufferMonitor48.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor48.Location = new System.Drawing.Point(943, 70);
            this.bufferMonitor48.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor48.Name = "bufferMonitor48";
            this.bufferMonitor48.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor48.Size = new System.Drawing.Size(104, 66);
            this.bufferMonitor48.TabIndex = 220;
            // 
            // craneMonitor5
            // 
            this.craneMonitor5._CommandID = "";
            this.craneMonitor5._CraneMode = "X";
            this.craneMonitor5._CraneNo = 1;
            this.craneMonitor5._CraneState = "X";
            this.craneMonitor5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.craneMonitor5.Location = new System.Drawing.Point(1048, 205);
            this.craneMonitor5.Name = "craneMonitor5";
            this.craneMonitor5.Padding = new System.Windows.Forms.Padding(3);
            this.craneMonitor5.Size = new System.Drawing.Size(103, 90);
            this.craneMonitor5.TabIndex = 219;
            // 
            // bufferMonitor42
            // 
            this.bufferMonitor42._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor42._BufferName = "BufName";
            this.bufferMonitor42._CommandID = "00000";
            this.bufferMonitor42._Destination = "0";
            this.bufferMonitor42._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor42._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor42._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor42._ReturnRequest = false;
            this.bufferMonitor42.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor42.Location = new System.Drawing.Point(839, 3);
            this.bufferMonitor42.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor42.Name = "bufferMonitor42";
            this.bufferMonitor42.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor42.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor42.TabIndex = 217;
            // 
            // bufferMonitor43
            // 
            this.bufferMonitor43._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor43._BufferName = "BufName";
            this.bufferMonitor43._CommandID = "00000";
            this.bufferMonitor43._Destination = "0";
            this.bufferMonitor43._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor43._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor43._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor43._ReturnRequest = false;
            this.bufferMonitor43.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor43.Location = new System.Drawing.Point(891, 3);
            this.bufferMonitor43.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor43.Name = "bufferMonitor43";
            this.bufferMonitor43.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor43.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor43.TabIndex = 216;
            // 
            // bufferMonitor44
            // 
            this.bufferMonitor44._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor44._BufferName = "BufName";
            this.bufferMonitor44._CommandID = "00000";
            this.bufferMonitor44._Destination = "0";
            this.bufferMonitor44._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor44._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor44._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor44._ReturnRequest = false;
            this.bufferMonitor44.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor44.Location = new System.Drawing.Point(1048, 3);
            this.bufferMonitor44.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor44.Name = "bufferMonitor44";
            this.bufferMonitor44.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor44.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor44.TabIndex = 215;
            // 
            // bufferMonitor45
            // 
            this.bufferMonitor45._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor45._BufferName = "BufName";
            this.bufferMonitor45._CommandID = "00000";
            this.bufferMonitor45._Destination = "0";
            this.bufferMonitor45._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor45._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor45._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor45._ReturnRequest = false;
            this.bufferMonitor45.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor45.Location = new System.Drawing.Point(996, 3);
            this.bufferMonitor45.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor45.Name = "bufferMonitor45";
            this.bufferMonitor45.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor45.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor45.TabIndex = 214;
            // 
            // bufferMonitor46
            // 
            this.bufferMonitor46._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor46._BufferName = "BufName";
            this.bufferMonitor46._CommandID = "00000";
            this.bufferMonitor46._Destination = "0";
            this.bufferMonitor46._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor46._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor46._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor46._ReturnRequest = false;
            this.bufferMonitor46.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor46.Location = new System.Drawing.Point(944, 3);
            this.bufferMonitor46.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor46.Name = "bufferMonitor46";
            this.bufferMonitor46.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor46.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor46.TabIndex = 213;
            // 
            // bufferMonitor34
            // 
            this.bufferMonitor34._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor34._BufferName = "BufName";
            this.bufferMonitor34._CommandID = "00000";
            this.bufferMonitor34._Destination = "0";
            this.bufferMonitor34._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor34._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor34._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor34._ReturnRequest = false;
            this.bufferMonitor34.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor34.Location = new System.Drawing.Point(891, 137);
            this.bufferMonitor34.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor34.Name = "bufferMonitor34";
            this.bufferMonitor34.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor34.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor34.TabIndex = 212;
            // 
            // bufferMonitor35
            // 
            this.bufferMonitor35._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor35._BufferName = "BufName";
            this.bufferMonitor35._CommandID = "00000";
            this.bufferMonitor35._Destination = "0";
            this.bufferMonitor35._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor35._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor35._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor35._ReturnRequest = false;
            this.bufferMonitor35.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor35.Location = new System.Drawing.Point(839, 137);
            this.bufferMonitor35.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor35.Name = "bufferMonitor35";
            this.bufferMonitor35.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor35.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor35.TabIndex = 211;
            // 
            // bufferMonitor36
            // 
            this.bufferMonitor36._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor36._BufferName = "BufName";
            this.bufferMonitor36._CommandID = "00000";
            this.bufferMonitor36._Destination = "0";
            this.bufferMonitor36._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor36._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor36._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor36._ReturnRequest = false;
            this.bufferMonitor36.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor36.Location = new System.Drawing.Point(891, 70);
            this.bufferMonitor36.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor36.Name = "bufferMonitor36";
            this.bufferMonitor36.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor36.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor36.TabIndex = 210;
            // 
            // bufferMonitor37
            // 
            this.bufferMonitor37._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor37._BufferName = "BufName";
            this.bufferMonitor37._CommandID = "00000";
            this.bufferMonitor37._Destination = "0";
            this.bufferMonitor37._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor37._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor37._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor37._ReturnRequest = false;
            this.bufferMonitor37.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor37.Location = new System.Drawing.Point(839, 70);
            this.bufferMonitor37.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor37.Name = "bufferMonitor37";
            this.bufferMonitor37.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor37.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor37.TabIndex = 209;
            // 
            // bufferMonitor38
            // 
            this.bufferMonitor38._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor38._BufferName = "BufName";
            this.bufferMonitor38._CommandID = "00000";
            this.bufferMonitor38._Destination = "0";
            this.bufferMonitor38._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor38._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor38._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor38._ReturnRequest = false;
            this.bufferMonitor38.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor38.Location = new System.Drawing.Point(734, 70);
            this.bufferMonitor38.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor38.Name = "bufferMonitor38";
            this.bufferMonitor38.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor38.Size = new System.Drawing.Size(104, 66);
            this.bufferMonitor38.TabIndex = 208;
            // 
            // craneMonitor3
            // 
            this.craneMonitor3._CommandID = "";
            this.craneMonitor3._CraneMode = "X";
            this.craneMonitor3._CraneNo = 1;
            this.craneMonitor3._CraneState = "X";
            this.craneMonitor3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.craneMonitor3.Location = new System.Drawing.Point(839, 205);
            this.craneMonitor3.Name = "craneMonitor3";
            this.craneMonitor3.Padding = new System.Windows.Forms.Padding(3);
            this.craneMonitor3.Size = new System.Drawing.Size(103, 90);
            this.craneMonitor3.TabIndex = 207;
            // 
            // bufferMonitor29
            // 
            this.bufferMonitor29._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor29._BufferName = "BufName";
            this.bufferMonitor29._CommandID = "00000";
            this.bufferMonitor29._Destination = "0";
            this.bufferMonitor29._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor29._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor29._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor29._ReturnRequest = false;
            this.bufferMonitor29.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor29.Location = new System.Drawing.Point(682, 137);
            this.bufferMonitor29.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor29.Name = "bufferMonitor29";
            this.bufferMonitor29.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor29.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor29.TabIndex = 205;
            // 
            // bufferMonitor30
            // 
            this.bufferMonitor30._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor30._BufferName = "BufName";
            this.bufferMonitor30._CommandID = "00000";
            this.bufferMonitor30._Destination = "0";
            this.bufferMonitor30._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor30._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor30._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor30._ReturnRequest = false;
            this.bufferMonitor30.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor30.Location = new System.Drawing.Point(630, 137);
            this.bufferMonitor30.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor30.Name = "bufferMonitor30";
            this.bufferMonitor30.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor30.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor30.TabIndex = 204;
            // 
            // bufferMonitor31
            // 
            this.bufferMonitor31._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor31._BufferName = "BufName";
            this.bufferMonitor31._CommandID = "00000";
            this.bufferMonitor31._Destination = "0";
            this.bufferMonitor31._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor31._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor31._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor31._ReturnRequest = false;
            this.bufferMonitor31.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor31.Location = new System.Drawing.Point(682, 70);
            this.bufferMonitor31.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor31.Name = "bufferMonitor31";
            this.bufferMonitor31.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor31.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor31.TabIndex = 203;
            // 
            // bufferMonitor32
            // 
            this.bufferMonitor32._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor32._BufferName = "BufName";
            this.bufferMonitor32._CommandID = "00000";
            this.bufferMonitor32._Destination = "0";
            this.bufferMonitor32._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor32._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor32._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor32._ReturnRequest = false;
            this.bufferMonitor32.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor32.Location = new System.Drawing.Point(630, 70);
            this.bufferMonitor32.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor32.Name = "bufferMonitor32";
            this.bufferMonitor32.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor32.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor32.TabIndex = 202;
            // 
            // bufferMonitor33
            // 
            this.bufferMonitor33._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor33._BufferName = "BufName";
            this.bufferMonitor33._CommandID = "00000";
            this.bufferMonitor33._Destination = "0";
            this.bufferMonitor33._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor33._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor33._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor33._ReturnRequest = false;
            this.bufferMonitor33.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor33.Location = new System.Drawing.Point(525, 70);
            this.bufferMonitor33.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor33.Name = "bufferMonitor33";
            this.bufferMonitor33.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor33.Size = new System.Drawing.Size(104, 66);
            this.bufferMonitor33.TabIndex = 201;
            // 
            // craneMonitor4
            // 
            this.craneMonitor4._CommandID = "";
            this.craneMonitor4._CraneMode = "X";
            this.craneMonitor4._CraneNo = 1;
            this.craneMonitor4._CraneState = "X";
            this.craneMonitor4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.craneMonitor4.Location = new System.Drawing.Point(630, 205);
            this.craneMonitor4.Name = "craneMonitor4";
            this.craneMonitor4.Padding = new System.Windows.Forms.Padding(3);
            this.craneMonitor4.Size = new System.Drawing.Size(103, 90);
            this.craneMonitor4.TabIndex = 200;
            // 
            // bufferMonitor24
            // 
            this.bufferMonitor24._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor24._BufferName = "BufName";
            this.bufferMonitor24._CommandID = "00000";
            this.bufferMonitor24._Destination = "0";
            this.bufferMonitor24._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor24._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor24._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor24._ReturnRequest = false;
            this.bufferMonitor24.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor24.Location = new System.Drawing.Point(473, 137);
            this.bufferMonitor24.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor24.Name = "bufferMonitor24";
            this.bufferMonitor24.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor24.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor24.TabIndex = 198;
            // 
            // bufferMonitor25
            // 
            this.bufferMonitor25._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor25._BufferName = "BufName";
            this.bufferMonitor25._CommandID = "00000";
            this.bufferMonitor25._Destination = "0";
            this.bufferMonitor25._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor25._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor25._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor25._ReturnRequest = false;
            this.bufferMonitor25.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor25.Location = new System.Drawing.Point(421, 137);
            this.bufferMonitor25.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor25.Name = "bufferMonitor25";
            this.bufferMonitor25.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor25.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor25.TabIndex = 197;
            // 
            // bufferMonitor26
            // 
            this.bufferMonitor26._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor26._BufferName = "BufName";
            this.bufferMonitor26._CommandID = "00000";
            this.bufferMonitor26._Destination = "0";
            this.bufferMonitor26._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor26._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor26._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor26._ReturnRequest = false;
            this.bufferMonitor26.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor26.Location = new System.Drawing.Point(473, 70);
            this.bufferMonitor26.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor26.Name = "bufferMonitor26";
            this.bufferMonitor26.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor26.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor26.TabIndex = 196;
            // 
            // bufferMonitor27
            // 
            this.bufferMonitor27._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor27._BufferName = "BufName";
            this.bufferMonitor27._CommandID = "00000";
            this.bufferMonitor27._Destination = "0";
            this.bufferMonitor27._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor27._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor27._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor27._ReturnRequest = false;
            this.bufferMonitor27.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor27.Location = new System.Drawing.Point(421, 70);
            this.bufferMonitor27.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor27.Name = "bufferMonitor27";
            this.bufferMonitor27.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor27.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor27.TabIndex = 195;
            // 
            // bufferMonitor28
            // 
            this.bufferMonitor28._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor28._BufferName = "BufName";
            this.bufferMonitor28._CommandID = "00000";
            this.bufferMonitor28._Destination = "0";
            this.bufferMonitor28._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor28._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor28._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor28._ReturnRequest = false;
            this.bufferMonitor28.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor28.Location = new System.Drawing.Point(316, 70);
            this.bufferMonitor28.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor28.Name = "bufferMonitor28";
            this.bufferMonitor28.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor28.Size = new System.Drawing.Size(104, 66);
            this.bufferMonitor28.TabIndex = 194;
            // 
            // craneMonitor2
            // 
            this.craneMonitor2._CommandID = "";
            this.craneMonitor2._CraneMode = "X";
            this.craneMonitor2._CraneNo = 1;
            this.craneMonitor2._CraneState = "X";
            this.craneMonitor2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.craneMonitor2.Location = new System.Drawing.Point(421, 205);
            this.craneMonitor2.Name = "craneMonitor2";
            this.craneMonitor2.Padding = new System.Windows.Forms.Padding(3);
            this.craneMonitor2.Size = new System.Drawing.Size(103, 90);
            this.craneMonitor2.TabIndex = 193;
            // 
            // bufferMonitor22
            // 
            this.bufferMonitor22._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor22._BufferName = "BufName";
            this.bufferMonitor22._CommandID = "00000";
            this.bufferMonitor22._Destination = "0";
            this.bufferMonitor22._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor22._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor22._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor22._ReturnRequest = false;
            this.bufferMonitor22.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor22.Location = new System.Drawing.Point(264, 137);
            this.bufferMonitor22.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor22.Name = "bufferMonitor22";
            this.bufferMonitor22.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor22.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor22.TabIndex = 191;
            // 
            // bufferMonitor23
            // 
            this.bufferMonitor23._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor23._BufferName = "A113";
            this.bufferMonitor23._CommandID = "00000";
            this.bufferMonitor23._Destination = "0";
            this.bufferMonitor23._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor23._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor23._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor23._ReturnRequest = false;
            this.bufferMonitor23.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor23.Location = new System.Drawing.Point(212, 137);
            this.bufferMonitor23.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor23.Name = "bufferMonitor23";
            this.bufferMonitor23.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor23.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor23.TabIndex = 190;
            // 
            // bufferMonitor21
            // 
            this.bufferMonitor21._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor21._BufferName = "BufName";
            this.bufferMonitor21._CommandID = "00000";
            this.bufferMonitor21._Destination = "0";
            this.bufferMonitor21._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor21._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor21._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor21._ReturnRequest = false;
            this.bufferMonitor21.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor21.Location = new System.Drawing.Point(264, 70);
            this.bufferMonitor21.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor21.Name = "bufferMonitor21";
            this.bufferMonitor21.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor21.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor21.TabIndex = 189;
            // 
            // bufferMonitor20
            // 
            this.bufferMonitor20._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor20._BufferName = "A06";
            this.bufferMonitor20._CommandID = "00000";
            this.bufferMonitor20._Destination = "0";
            this.bufferMonitor20._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor20._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor20._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor20._ReturnRequest = false;
            this.bufferMonitor20.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor20.Location = new System.Drawing.Point(212, 70);
            this.bufferMonitor20.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor20.Name = "bufferMonitor20";
            this.bufferMonitor20.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor20.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor20.TabIndex = 188;
            // 
            // bufferMonitor5
            // 
            this.bufferMonitor5._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor5._BufferName = "A04";
            this.bufferMonitor5._CommandID = "00000";
            this.bufferMonitor5._Destination = "0";
            this.bufferMonitor5._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor5._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor5._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor5._ReturnRequest = false;
            this.bufferMonitor5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor5.Location = new System.Drawing.Point(55, 70);
            this.bufferMonitor5.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor5.Name = "bufferMonitor5";
            this.bufferMonitor5.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor5.Size = new System.Drawing.Size(79, 66);
            this.bufferMonitor5.TabIndex = 187;
            // 
            // bufferMonitor12
            // 
            this.bufferMonitor12._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor12._BufferName = "BufName";
            this.bufferMonitor12._CommandID = "00000";
            this.bufferMonitor12._Destination = "0";
            this.bufferMonitor12._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor12._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor12._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor12._ReturnRequest = false;
            this.bufferMonitor12.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor12.Location = new System.Drawing.Point(787, 3);
            this.bufferMonitor12.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor12.Name = "bufferMonitor12";
            this.bufferMonitor12.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor12.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor12.TabIndex = 186;
            // 
            // bufferMonitor13
            // 
            this.bufferMonitor13._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor13._BufferName = "BufName";
            this.bufferMonitor13._CommandID = "00000";
            this.bufferMonitor13._Destination = "0";
            this.bufferMonitor13._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor13._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor13._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor13._ReturnRequest = false;
            this.bufferMonitor13.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor13.Location = new System.Drawing.Point(734, 3);
            this.bufferMonitor13.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor13.Name = "bufferMonitor13";
            this.bufferMonitor13.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor13.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor13.TabIndex = 185;
            // 
            // bufferMonitor14
            // 
            this.bufferMonitor14._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor14._BufferName = "BufName";
            this.bufferMonitor14._CommandID = "00000";
            this.bufferMonitor14._Destination = "0";
            this.bufferMonitor14._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor14._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor14._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor14._ReturnRequest = false;
            this.bufferMonitor14.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor14.Location = new System.Drawing.Point(682, 3);
            this.bufferMonitor14.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor14.Name = "bufferMonitor14";
            this.bufferMonitor14.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor14.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor14.TabIndex = 184;
            // 
            // bufferMonitor15
            // 
            this.bufferMonitor15._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor15._BufferName = "BufName";
            this.bufferMonitor15._CommandID = "00000";
            this.bufferMonitor15._Destination = "0";
            this.bufferMonitor15._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor15._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor15._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor15._ReturnRequest = false;
            this.bufferMonitor15.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor15.Location = new System.Drawing.Point(630, 3);
            this.bufferMonitor15.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor15.Name = "bufferMonitor15";
            this.bufferMonitor15.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor15.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor15.TabIndex = 183;
            // 
            // bufferMonitor16
            // 
            this.bufferMonitor16._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor16._BufferName = "BufName";
            this.bufferMonitor16._CommandID = "00000";
            this.bufferMonitor16._Destination = "0";
            this.bufferMonitor16._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor16._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor16._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor16._ReturnRequest = false;
            this.bufferMonitor16.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor16.Location = new System.Drawing.Point(578, 3);
            this.bufferMonitor16.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor16.Name = "bufferMonitor16";
            this.bufferMonitor16.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor16.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor16.TabIndex = 182;
            // 
            // bufferMonitor17
            // 
            this.bufferMonitor17._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor17._BufferName = "BufName";
            this.bufferMonitor17._CommandID = "00000";
            this.bufferMonitor17._Destination = "0";
            this.bufferMonitor17._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor17._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor17._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor17._ReturnRequest = false;
            this.bufferMonitor17.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor17.Location = new System.Drawing.Point(525, 3);
            this.bufferMonitor17.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor17.Name = "bufferMonitor17";
            this.bufferMonitor17.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor17.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor17.TabIndex = 181;
            // 
            // bufferMonitor18
            // 
            this.bufferMonitor18._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor18._BufferName = "BufName";
            this.bufferMonitor18._CommandID = "00000";
            this.bufferMonitor18._Destination = "0";
            this.bufferMonitor18._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor18._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor18._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor18._ReturnRequest = false;
            this.bufferMonitor18.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor18.Location = new System.Drawing.Point(473, 3);
            this.bufferMonitor18.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor18.Name = "bufferMonitor18";
            this.bufferMonitor18.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor18.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor18.TabIndex = 180;
            // 
            // bufferMonitor19
            // 
            this.bufferMonitor19._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor19._BufferName = "BufName";
            this.bufferMonitor19._CommandID = "00000";
            this.bufferMonitor19._Destination = "0";
            this.bufferMonitor19._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor19._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor19._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor19._ReturnRequest = false;
            this.bufferMonitor19.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor19.Location = new System.Drawing.Point(421, 3);
            this.bufferMonitor19.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor19.Name = "bufferMonitor19";
            this.bufferMonitor19.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor19.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor19.TabIndex = 179;
            // 
            // bufferMonitor8
            // 
            this.bufferMonitor8._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor8._BufferName = "BufName";
            this.bufferMonitor8._CommandID = "00000";
            this.bufferMonitor8._Destination = "0";
            this.bufferMonitor8._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor8._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor8._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor8._ReturnRequest = false;
            this.bufferMonitor8.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor8.Location = new System.Drawing.Point(369, 3);
            this.bufferMonitor8.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor8.Name = "bufferMonitor8";
            this.bufferMonitor8.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor8.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor8.TabIndex = 178;
            // 
            // bufferMonitor9
            // 
            this.bufferMonitor9._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor9._BufferName = "BufName";
            this.bufferMonitor9._CommandID = "00000";
            this.bufferMonitor9._Destination = "0";
            this.bufferMonitor9._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor9._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor9._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor9._ReturnRequest = false;
            this.bufferMonitor9.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor9.Location = new System.Drawing.Point(316, 3);
            this.bufferMonitor9.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor9.Name = "bufferMonitor9";
            this.bufferMonitor9.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor9.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor9.TabIndex = 177;
            // 
            // bufferMonitor10
            // 
            this.bufferMonitor10._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor10._BufferName = "BufName";
            this.bufferMonitor10._CommandID = "00000";
            this.bufferMonitor10._Destination = "0";
            this.bufferMonitor10._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor10._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor10._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor10._ReturnRequest = false;
            this.bufferMonitor10.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor10.Location = new System.Drawing.Point(264, 3);
            this.bufferMonitor10.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor10.Name = "bufferMonitor10";
            this.bufferMonitor10.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor10.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor10.TabIndex = 176;
            // 
            // bufferMonitor11
            // 
            this.bufferMonitor11._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor11._BufferName = "BufName";
            this.bufferMonitor11._CommandID = "00000";
            this.bufferMonitor11._Destination = "0";
            this.bufferMonitor11._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor11._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor11._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor11._ReturnRequest = false;
            this.bufferMonitor11.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor11.Location = new System.Drawing.Point(212, 3);
            this.bufferMonitor11.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor11.Name = "bufferMonitor11";
            this.bufferMonitor11.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor11.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor11.TabIndex = 175;
            // 
            // bufferMonitor7
            // 
            this.bufferMonitor7._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor7._BufferName = "A05";
            this.bufferMonitor7._CommandID = "00000";
            this.bufferMonitor7._Destination = "0";
            this.bufferMonitor7._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor7._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor7._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor7._ReturnRequest = false;
            this.bufferMonitor7.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor7.Location = new System.Drawing.Point(135, 70);
            this.bufferMonitor7.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor7.Name = "bufferMonitor7";
            this.bufferMonitor7.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor7.Size = new System.Drawing.Size(76, 66);
            this.bufferMonitor7.TabIndex = 174;
            // 
            // bufferMonitor6
            // 
            this.bufferMonitor6._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor6._BufferName = "A02";
            this.bufferMonitor6._CommandID = "00000";
            this.bufferMonitor6._Destination = "0";
            this.bufferMonitor6._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor6._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor6._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor6._ReturnRequest = false;
            this.bufferMonitor6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor6.Location = new System.Drawing.Point(3, 70);
            this.bufferMonitor6.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor6.Name = "bufferMonitor6";
            this.bufferMonitor6.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor6.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor6.TabIndex = 173;
            // 
            // bufferMonitor4
            // 
            this.bufferMonitor4._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor4._BufferName = "BufName";
            this.bufferMonitor4._CommandID = "00000";
            this.bufferMonitor4._Destination = "0";
            this.bufferMonitor4._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor4._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor4._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor4._ReturnRequest = false;
            this.bufferMonitor4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor4.Location = new System.Drawing.Point(160, 3);
            this.bufferMonitor4.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor4.Name = "bufferMonitor4";
            this.bufferMonitor4.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor4.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor4.TabIndex = 172;
            // 
            // bufferMonitor3
            // 
            this.bufferMonitor3._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor3._BufferName = "BufName";
            this.bufferMonitor3._CommandID = "00000";
            this.bufferMonitor3._Destination = "0";
            this.bufferMonitor3._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor3._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor3._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor3._ReturnRequest = false;
            this.bufferMonitor3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor3.Location = new System.Drawing.Point(107, 3);
            this.bufferMonitor3.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor3.Name = "bufferMonitor3";
            this.bufferMonitor3.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor3.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor3.TabIndex = 171;
            // 
            // bufferMonitor2
            // 
            this.bufferMonitor2._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor2._BufferName = "BufName";
            this.bufferMonitor2._CommandID = "00000";
            this.bufferMonitor2._Destination = "0";
            this.bufferMonitor2._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor2._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor2._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor2._ReturnRequest = false;
            this.bufferMonitor2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor2.Location = new System.Drawing.Point(55, 3);
            this.bufferMonitor2.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor2.Name = "bufferMonitor2";
            this.bufferMonitor2.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor2.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor2.TabIndex = 170;
            // 
            // bufferMonitor1
            // 
            this.bufferMonitor1._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor1._BufferName = "BufName";
            this.bufferMonitor1._CommandID = "00000";
            this.bufferMonitor1._Destination = "0";
            this.bufferMonitor1._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor1._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor1._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor1._ReturnRequest = false;
            this.bufferMonitor1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor1.Location = new System.Drawing.Point(3, 3);
            this.bufferMonitor1.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor1.Name = "bufferMonitor1";
            this.bufferMonitor1.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor1.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor1.TabIndex = 169;
            // 
            // craneMonitor1
            // 
            this.craneMonitor1._CommandID = "";
            this.craneMonitor1._CraneMode = "X";
            this.craneMonitor1._CraneNo = 1;
            this.craneMonitor1._CraneState = "X";
            this.craneMonitor1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.craneMonitor1.Location = new System.Drawing.Point(212, 205);
            this.craneMonitor1.Name = "craneMonitor1";
            this.craneMonitor1.Padding = new System.Windows.Forms.Padding(3);
            this.craneMonitor1.Size = new System.Drawing.Size(103, 90);
            this.craneMonitor1.TabIndex = 168;
            // 
            // bufferMonitor52
            // 
            this.bufferMonitor52._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor52._BufferName = "BufName";
            this.bufferMonitor52._CommandID = "00000";
            this.bufferMonitor52._Destination = "0";
            this.bufferMonitor52._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor52._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor52._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor52._ReturnRequest = false;
            this.bufferMonitor52.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor52.Location = new System.Drawing.Point(1518, 3);
            this.bufferMonitor52.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor52.Name = "bufferMonitor52";
            this.bufferMonitor52.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor52.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor52.TabIndex = 238;
            // 
            // bufferMonitor53
            // 
            this.bufferMonitor53._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor53._BufferName = "BufName";
            this.bufferMonitor53._CommandID = "00000";
            this.bufferMonitor53._Destination = "0";
            this.bufferMonitor53._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor53._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor53._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor53._ReturnRequest = false;
            this.bufferMonitor53.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor53.Location = new System.Drawing.Point(1570, 3);
            this.bufferMonitor53.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor53.Name = "bufferMonitor53";
            this.bufferMonitor53.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor53.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor53.TabIndex = 237;
            // 
            // bufferMonitor54
            // 
            this.bufferMonitor54._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor54._BufferName = "BufName";
            this.bufferMonitor54._CommandID = "00000";
            this.bufferMonitor54._Destination = "0";
            this.bufferMonitor54._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor54._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor54._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor54._ReturnRequest = false;
            this.bufferMonitor54.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor54.Location = new System.Drawing.Point(1727, 3);
            this.bufferMonitor54.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor54.Name = "bufferMonitor54";
            this.bufferMonitor54.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor54.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor54.TabIndex = 236;
            // 
            // bufferMonitor55
            // 
            this.bufferMonitor55._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor55._BufferName = "BufName";
            this.bufferMonitor55._CommandID = "00000";
            this.bufferMonitor55._Destination = "0";
            this.bufferMonitor55._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor55._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor55._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor55._ReturnRequest = false;
            this.bufferMonitor55.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor55.Location = new System.Drawing.Point(1675, 3);
            this.bufferMonitor55.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor55.Name = "bufferMonitor55";
            this.bufferMonitor55.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor55.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor55.TabIndex = 235;
            // 
            // bufferMonitor56
            // 
            this.bufferMonitor56._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor56._BufferName = "BufName";
            this.bufferMonitor56._CommandID = "00000";
            this.bufferMonitor56._Destination = "0";
            this.bufferMonitor56._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor56._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor56._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor56._ReturnRequest = false;
            this.bufferMonitor56.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor56.Location = new System.Drawing.Point(1623, 3);
            this.bufferMonitor56.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor56.Name = "bufferMonitor56";
            this.bufferMonitor56.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor56.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor56.TabIndex = 234;
            // 
            // bufferMonitor57
            // 
            this.bufferMonitor57._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor57._BufferName = "BufName";
            this.bufferMonitor57._CommandID = "00000";
            this.bufferMonitor57._Destination = "0";
            this.bufferMonitor57._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor57._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor57._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor57._ReturnRequest = false;
            this.bufferMonitor57.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor57.Location = new System.Drawing.Point(1466, 3);
            this.bufferMonitor57.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor57.Name = "bufferMonitor57";
            this.bufferMonitor57.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor57.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor57.TabIndex = 233;
            // 
            // bufferMonitor58
            // 
            this.bufferMonitor58._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor58._BufferName = "BufName";
            this.bufferMonitor58._CommandID = "00000";
            this.bufferMonitor58._Destination = "0";
            this.bufferMonitor58._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor58._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor58._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor58._ReturnRequest = false;
            this.bufferMonitor58.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor58.Location = new System.Drawing.Point(1413, 3);
            this.bufferMonitor58.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor58.Name = "bufferMonitor58";
            this.bufferMonitor58.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor58.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor58.TabIndex = 232;
            // 
            // bufferMonitor59
            // 
            this.bufferMonitor59._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor59._BufferName = "BufName";
            this.bufferMonitor59._CommandID = "00000";
            this.bufferMonitor59._Destination = "0";
            this.bufferMonitor59._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor59._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor59._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor59._ReturnRequest = false;
            this.bufferMonitor59.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor59.Location = new System.Drawing.Point(1361, 3);
            this.bufferMonitor59.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor59.Name = "bufferMonitor59";
            this.bufferMonitor59.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor59.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor59.TabIndex = 231;
            // 
            // bufferMonitor60
            // 
            this.bufferMonitor60._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor60._BufferName = "BufName";
            this.bufferMonitor60._CommandID = "00000";
            this.bufferMonitor60._Destination = "0";
            this.bufferMonitor60._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor60._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor60._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor60._ReturnRequest = false;
            this.bufferMonitor60.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor60.Location = new System.Drawing.Point(1309, 3);
            this.bufferMonitor60.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor60.Name = "bufferMonitor60";
            this.bufferMonitor60.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor60.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor60.TabIndex = 230;
            // 
            // bufferMonitor61
            // 
            this.bufferMonitor61._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor61._BufferName = "BufName";
            this.bufferMonitor61._CommandID = "00000";
            this.bufferMonitor61._Destination = "0";
            this.bufferMonitor61._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor61._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor61._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor61._ReturnRequest = false;
            this.bufferMonitor61.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor61.Location = new System.Drawing.Point(1257, 3);
            this.bufferMonitor61.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor61.Name = "bufferMonitor61";
            this.bufferMonitor61.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor61.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor61.TabIndex = 229;
            // 
            // bufferMonitor62
            // 
            this.bufferMonitor62._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor62._BufferName = "BufName";
            this.bufferMonitor62._CommandID = "00000";
            this.bufferMonitor62._Destination = "0";
            this.bufferMonitor62._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor62._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor62._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor62._ReturnRequest = false;
            this.bufferMonitor62.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor62.Location = new System.Drawing.Point(1204, 3);
            this.bufferMonitor62.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor62.Name = "bufferMonitor62";
            this.bufferMonitor62.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor62.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor62.TabIndex = 228;
            // 
            // bufferMonitor63
            // 
            this.bufferMonitor63._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor63._BufferName = "BufName";
            this.bufferMonitor63._CommandID = "00000";
            this.bufferMonitor63._Destination = "0";
            this.bufferMonitor63._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor63._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor63._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor63._ReturnRequest = false;
            this.bufferMonitor63.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor63.Location = new System.Drawing.Point(1152, 3);
            this.bufferMonitor63.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor63.Name = "bufferMonitor63";
            this.bufferMonitor63.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor63.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor63.TabIndex = 227;
            // 
            // bufferMonitor51
            // 
            this.bufferMonitor51._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor51._BufferName = "BufName";
            this.bufferMonitor51._CommandID = "00000";
            this.bufferMonitor51._Destination = "0";
            this.bufferMonitor51._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor51._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor51._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor51._ReturnRequest = false;
            this.bufferMonitor51.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor51.Location = new System.Drawing.Point(1518, 70);
            this.bufferMonitor51.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor51.Name = "bufferMonitor51";
            this.bufferMonitor51.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor51.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor51.TabIndex = 250;
            // 
            // bufferMonitor64
            // 
            this.bufferMonitor64._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor64._BufferName = "BufName";
            this.bufferMonitor64._CommandID = "00000";
            this.bufferMonitor64._Destination = "0";
            this.bufferMonitor64._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor64._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor64._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor64._ReturnRequest = false;
            this.bufferMonitor64.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor64.Location = new System.Drawing.Point(1570, 70);
            this.bufferMonitor64.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor64.Name = "bufferMonitor64";
            this.bufferMonitor64.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor64.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor64.TabIndex = 249;
            // 
            // bufferMonitor65
            // 
            this.bufferMonitor65._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor65._BufferName = "BufName";
            this.bufferMonitor65._CommandID = "00000";
            this.bufferMonitor65._Destination = "0";
            this.bufferMonitor65._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor65._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor65._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor65._ReturnRequest = false;
            this.bufferMonitor65.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor65.Location = new System.Drawing.Point(1727, 70);
            this.bufferMonitor65.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor65.Name = "bufferMonitor65";
            this.bufferMonitor65.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor65.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor65.TabIndex = 248;
            // 
            // bufferMonitor66
            // 
            this.bufferMonitor66._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor66._BufferName = "BufName";
            this.bufferMonitor66._CommandID = "00000";
            this.bufferMonitor66._Destination = "0";
            this.bufferMonitor66._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor66._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor66._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor66._ReturnRequest = false;
            this.bufferMonitor66.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor66.Location = new System.Drawing.Point(1675, 70);
            this.bufferMonitor66.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor66.Name = "bufferMonitor66";
            this.bufferMonitor66.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor66.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor66.TabIndex = 247;
            // 
            // bufferMonitor67
            // 
            this.bufferMonitor67._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor67._BufferName = "BufName";
            this.bufferMonitor67._CommandID = "00000";
            this.bufferMonitor67._Destination = "0";
            this.bufferMonitor67._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor67._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor67._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor67._ReturnRequest = false;
            this.bufferMonitor67.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor67.Location = new System.Drawing.Point(1623, 70);
            this.bufferMonitor67.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor67.Name = "bufferMonitor67";
            this.bufferMonitor67.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor67.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor67.TabIndex = 246;
            // 
            // bufferMonitor68
            // 
            this.bufferMonitor68._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor68._BufferName = "BufName";
            this.bufferMonitor68._CommandID = "00000";
            this.bufferMonitor68._Destination = "0";
            this.bufferMonitor68._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor68._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor68._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor68._ReturnRequest = false;
            this.bufferMonitor68.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor68.Location = new System.Drawing.Point(1466, 70);
            this.bufferMonitor68.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor68.Name = "bufferMonitor68";
            this.bufferMonitor68.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor68.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor68.TabIndex = 245;
            // 
            // bufferMonitor69
            // 
            this.bufferMonitor69._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor69._BufferName = "BufName";
            this.bufferMonitor69._CommandID = "00000";
            this.bufferMonitor69._Destination = "0";
            this.bufferMonitor69._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor69._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor69._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor69._ReturnRequest = false;
            this.bufferMonitor69.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor69.Location = new System.Drawing.Point(1413, 70);
            this.bufferMonitor69.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor69.Name = "bufferMonitor69";
            this.bufferMonitor69.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor69.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor69.TabIndex = 244;
            // 
            // bufferMonitor70
            // 
            this.bufferMonitor70._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor70._BufferName = "BufName";
            this.bufferMonitor70._CommandID = "00000";
            this.bufferMonitor70._Destination = "0";
            this.bufferMonitor70._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor70._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor70._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor70._ReturnRequest = false;
            this.bufferMonitor70.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor70.Location = new System.Drawing.Point(1361, 70);
            this.bufferMonitor70.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor70.Name = "bufferMonitor70";
            this.bufferMonitor70.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor70.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor70.TabIndex = 243;
            // 
            // bufferMonitor71
            // 
            this.bufferMonitor71._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor71._BufferName = "BufName";
            this.bufferMonitor71._CommandID = "00000";
            this.bufferMonitor71._Destination = "0";
            this.bufferMonitor71._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor71._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor71._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor71._ReturnRequest = false;
            this.bufferMonitor71.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor71.Location = new System.Drawing.Point(1309, 70);
            this.bufferMonitor71.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor71.Name = "bufferMonitor71";
            this.bufferMonitor71.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor71.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor71.TabIndex = 242;
            // 
            // bufferMonitor72
            // 
            this.bufferMonitor72._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor72._BufferName = "BufName";
            this.bufferMonitor72._CommandID = "00000";
            this.bufferMonitor72._Destination = "0";
            this.bufferMonitor72._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor72._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor72._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor72._ReturnRequest = false;
            this.bufferMonitor72.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor72.Location = new System.Drawing.Point(1257, 70);
            this.bufferMonitor72.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor72.Name = "bufferMonitor72";
            this.bufferMonitor72.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor72.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor72.TabIndex = 241;
            // 
            // bufferMonitor73
            // 
            this.bufferMonitor73._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor73._BufferName = "BufName";
            this.bufferMonitor73._CommandID = "00000";
            this.bufferMonitor73._Destination = "0";
            this.bufferMonitor73._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor73._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor73._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor73._ReturnRequest = false;
            this.bufferMonitor73.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor73.Location = new System.Drawing.Point(1204, 70);
            this.bufferMonitor73.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor73.Name = "bufferMonitor73";
            this.bufferMonitor73.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor73.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor73.TabIndex = 240;
            // 
            // bufferMonitor74
            // 
            this.bufferMonitor74._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor74._BufferName = "BufName";
            this.bufferMonitor74._CommandID = "00000";
            this.bufferMonitor74._Destination = "0";
            this.bufferMonitor74._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor74._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor74._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor74._ReturnRequest = false;
            this.bufferMonitor74.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor74.Location = new System.Drawing.Point(1152, 70);
            this.bufferMonitor74.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor74.Name = "bufferMonitor74";
            this.bufferMonitor74.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor74.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor74.TabIndex = 239;
            // 
            // bufferMonitor75
            // 
            this.bufferMonitor75._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor75._BufferName = "BufName";
            this.bufferMonitor75._CommandID = "00000";
            this.bufferMonitor75._Destination = "0";
            this.bufferMonitor75._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor75._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor75._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor75._ReturnRequest = false;
            this.bufferMonitor75.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor75.Location = new System.Drawing.Point(1727, 204);
            this.bufferMonitor75.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor75.Name = "bufferMonitor75";
            this.bufferMonitor75.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor75.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor75.TabIndex = 252;
            // 
            // bufferMonitor76
            // 
            this.bufferMonitor76._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor76._BufferName = "BufName";
            this.bufferMonitor76._CommandID = "00000";
            this.bufferMonitor76._Destination = "0";
            this.bufferMonitor76._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor76._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor76._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor76._ReturnRequest = false;
            this.bufferMonitor76.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor76.Location = new System.Drawing.Point(1727, 137);
            this.bufferMonitor76.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor76.Name = "bufferMonitor76";
            this.bufferMonitor76.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor76.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor76.TabIndex = 251;
            // 
            // bufferMonitor77
            // 
            this.bufferMonitor77._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor77._BufferName = "BufName";
            this.bufferMonitor77._CommandID = "00000";
            this.bufferMonitor77._Destination = "0";
            this.bufferMonitor77._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor77._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor77._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor77._ReturnRequest = false;
            this.bufferMonitor77.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor77.Location = new System.Drawing.Point(1726, 473);
            this.bufferMonitor77.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor77.Name = "bufferMonitor77";
            this.bufferMonitor77.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor77.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor77.TabIndex = 256;
            // 
            // bufferMonitor78
            // 
            this.bufferMonitor78._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor78._BufferName = "BufName";
            this.bufferMonitor78._CommandID = "00000";
            this.bufferMonitor78._Destination = "0";
            this.bufferMonitor78._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor78._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor78._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor78._ReturnRequest = false;
            this.bufferMonitor78.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor78.Location = new System.Drawing.Point(1726, 406);
            this.bufferMonitor78.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor78.Name = "bufferMonitor78";
            this.bufferMonitor78.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor78.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor78.TabIndex = 255;
            // 
            // bufferMonitor79
            // 
            this.bufferMonitor79._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor79._BufferName = "BufName";
            this.bufferMonitor79._CommandID = "00000";
            this.bufferMonitor79._Destination = "0";
            this.bufferMonitor79._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor79._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor79._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor79._ReturnRequest = false;
            this.bufferMonitor79.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor79.Location = new System.Drawing.Point(1726, 339);
            this.bufferMonitor79.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor79.Name = "bufferMonitor79";
            this.bufferMonitor79.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor79.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor79.TabIndex = 254;
            // 
            // bufferMonitor80
            // 
            this.bufferMonitor80._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor80._BufferName = "BufName";
            this.bufferMonitor80._CommandID = "00000";
            this.bufferMonitor80._Destination = "0";
            this.bufferMonitor80._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor80._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor80._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor80._ReturnRequest = false;
            this.bufferMonitor80.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor80.Location = new System.Drawing.Point(1726, 272);
            this.bufferMonitor80.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor80.Name = "bufferMonitor80";
            this.bufferMonitor80.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor80.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor80.TabIndex = 253;
            // 
            // bufferMonitor81
            // 
            this.bufferMonitor81._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor81._BufferName = "BufName";
            this.bufferMonitor81._CommandID = "00000";
            this.bufferMonitor81._Destination = "0";
            this.bufferMonitor81._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor81._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor81._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor81._ReturnRequest = false;
            this.bufferMonitor81.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor81.Location = new System.Drawing.Point(1726, 540);
            this.bufferMonitor81.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor81.Name = "bufferMonitor81";
            this.bufferMonitor81.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor81.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor81.TabIndex = 257;
            // 
            // bufferMonitor82
            // 
            this.bufferMonitor82._Auto = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor82._BufferName = "BufName";
            this.bufferMonitor82._CommandID = "00000";
            this.bufferMonitor82._Destination = "0";
            this.bufferMonitor82._Error = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor82._Load = Mirle.ASRS.Buffer.Signal.Off;
            this.bufferMonitor82._Mode = Mirle.ASRS.Buffer.StnMode.None;
            this.bufferMonitor82._ReturnRequest = false;
            this.bufferMonitor82.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bufferMonitor82.Location = new System.Drawing.Point(1779, 540);
            this.bufferMonitor82.MinimumSize = new System.Drawing.Size(51, 66);
            this.bufferMonitor82.Name = "bufferMonitor82";
            this.bufferMonitor82.Padding = new System.Windows.Forms.Padding(3);
            this.bufferMonitor82.Size = new System.Drawing.Size(51, 66);
            this.bufferMonitor82.TabIndex = 258;
            // 
            // WCS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1370, 726);
            this.ControlBox = false;
            this.Controls.Add(this.sctMain1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WCS";
            this.Text = "WCS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WCS_FormClosing);
            this.Load += new System.EventHandler(this.WCS_Load);
            this.sctMain1.Panel1.ResumeLayout(false);
            this.sctMain1.Panel1.PerformLayout();
            this.sctMain1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sctMain1)).EndInit();
            this.sctMain1.ResumeLayout(false);
            this.tlpButton.ResumeLayout(false);
            this.tlpButton.PerformLayout();
            this.gpbMainState.ResumeLayout(false);
            this.gpbMainState.PerformLayout();
            this.tlpMainState.ResumeLayout(false);
            this.tlpLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ptbLogo)).EndInit();
            this.tbcMain.ResumeLayout(false);
            this.tbpSystemTrace.ResumeLayout(false);
            this.tbpUpdatePosted.ResumeLayout(false);
            this.tbpSingelMonitor.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCmdMst)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnlCraneBottom.ResumeLayout(false);
            this.pnlCraneBottom.PerformLayout();
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
        private System.Windows.Forms.Button btnReconnectSPLC;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpSystemTrace;
        private System.Windows.Forms.ListBox lsbSysTrace;
        private System.Windows.Forms.TabPage tbpUpdatePosted;
        private System.Windows.Forms.ListBox lsbUpdate;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvCmdMst;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Query;
        private System.Windows.Forms.TabPage tbpSingelMonitor;
        private System.Windows.Forms.Panel panel1;
        private BufferMonitor bufferMonitor50;
        private BufferMonitor bufferMonitor49;
        private BufferMonitor bufferMonitor39;
        private BufferMonitor bufferMonitor40;
        private BufferMonitor bufferMonitor41;
        private BufferMonitor bufferMonitor47;
        private BufferMonitor bufferMonitor48;
        private CraneMonitor craneMonitor5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private BufferMonitor bufferMonitor42;
        private BufferMonitor bufferMonitor43;
        private BufferMonitor bufferMonitor44;
        private BufferMonitor bufferMonitor45;
        private BufferMonitor bufferMonitor46;
        private BufferMonitor bufferMonitor34;
        private BufferMonitor bufferMonitor35;
        private BufferMonitor bufferMonitor36;
        private BufferMonitor bufferMonitor37;
        private BufferMonitor bufferMonitor38;
        private CraneMonitor craneMonitor3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private BufferMonitor bufferMonitor29;
        private BufferMonitor bufferMonitor30;
        private BufferMonitor bufferMonitor31;
        private BufferMonitor bufferMonitor32;
        private BufferMonitor bufferMonitor33;
        private CraneMonitor craneMonitor4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private BufferMonitor bufferMonitor24;
        private BufferMonitor bufferMonitor25;
        private BufferMonitor bufferMonitor26;
        private BufferMonitor bufferMonitor27;
        private BufferMonitor bufferMonitor28;
        private CraneMonitor craneMonitor2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label1;
        private BufferMonitor bufferMonitor22;
        private BufferMonitor bufferMonitor23;
        private BufferMonitor bufferMonitor21;
        private BufferMonitor bufferMonitor20;
        private BufferMonitor bufferMonitor5;
        private BufferMonitor bufferMonitor12;
        private BufferMonitor bufferMonitor13;
        private BufferMonitor bufferMonitor14;
        private BufferMonitor bufferMonitor15;
        private BufferMonitor bufferMonitor16;
        private BufferMonitor bufferMonitor17;
        private BufferMonitor bufferMonitor18;
        private BufferMonitor bufferMonitor19;
        private BufferMonitor bufferMonitor8;
        private BufferMonitor bufferMonitor9;
        private BufferMonitor bufferMonitor10;
        private BufferMonitor bufferMonitor11;
        private BufferMonitor bufferMonitor7;
        private BufferMonitor bufferMonitor6;
        private BufferMonitor bufferMonitor4;
        private BufferMonitor bufferMonitor3;
        private BufferMonitor bufferMonitor2;
        private BufferMonitor bufferMonitor1;
        private CraneMonitor craneMonitor1;
        private System.Windows.Forms.Panel pnlCraneBottom;
        private System.Windows.Forms.Label lblCraneBottom;
        private BufferMonitor bufferMonitor82;
        private BufferMonitor bufferMonitor81;
        private BufferMonitor bufferMonitor77;
        private BufferMonitor bufferMonitor78;
        private BufferMonitor bufferMonitor79;
        private BufferMonitor bufferMonitor80;
        private BufferMonitor bufferMonitor75;
        private BufferMonitor bufferMonitor76;
        private BufferMonitor bufferMonitor51;
        private BufferMonitor bufferMonitor64;
        private BufferMonitor bufferMonitor65;
        private BufferMonitor bufferMonitor66;
        private BufferMonitor bufferMonitor67;
        private BufferMonitor bufferMonitor68;
        private BufferMonitor bufferMonitor69;
        private BufferMonitor bufferMonitor70;
        private BufferMonitor bufferMonitor71;
        private BufferMonitor bufferMonitor72;
        private BufferMonitor bufferMonitor73;
        private BufferMonitor bufferMonitor74;
        private BufferMonitor bufferMonitor52;
        private BufferMonitor bufferMonitor53;
        private BufferMonitor bufferMonitor54;
        private BufferMonitor bufferMonitor55;
        private BufferMonitor bufferMonitor56;
        private BufferMonitor bufferMonitor57;
        private BufferMonitor bufferMonitor58;
        private BufferMonitor bufferMonitor59;
        private BufferMonitor bufferMonitor60;
        private BufferMonitor bufferMonitor61;
        private BufferMonitor bufferMonitor62;
        private BufferMonitor bufferMonitor63;
    }
}

