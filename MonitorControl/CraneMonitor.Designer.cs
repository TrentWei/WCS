namespace Mirle.ASRS
{
    partial class CraneMonitor
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpCrane = new System.Windows.Forms.TableLayoutPanel();
            this.lblCraneState = new System.Windows.Forms.Label();
            this.lblCraneNo = new System.Windows.Forms.Label();
            this.lblCommandID = new System.Windows.Forms.Label();
            this.lblCraneMode = new System.Windows.Forms.Label();
            this.tlpCrane.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpCrane
            // 
            this.tlpCrane.BackColor = System.Drawing.Color.White;
            this.tlpCrane.ColumnCount = 1;
            this.tlpCrane.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCrane.Controls.Add(this.lblCraneState, 0, 2);
            this.tlpCrane.Controls.Add(this.lblCraneNo, 0, 0);
            this.tlpCrane.Controls.Add(this.lblCommandID, 0, 3);
            this.tlpCrane.Controls.Add(this.lblCraneMode, 0, 1);
            this.tlpCrane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCrane.Location = new System.Drawing.Point(3, 3);
            this.tlpCrane.Name = "tlpCrane";
            this.tlpCrane.RowCount = 4;
            this.tlpCrane.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCrane.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCrane.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCrane.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCrane.Size = new System.Drawing.Size(100, 80);
            this.tlpCrane.TabIndex = 4;
            // 
            // lblCraneState
            // 
            this.lblCraneState.BackColor = System.Drawing.Color.Red;
            this.lblCraneState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCraneState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCraneState.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCraneState.Location = new System.Drawing.Point(0, 40);
            this.lblCraneState.Margin = new System.Windows.Forms.Padding(0);
            this.lblCraneState.Name = "lblCraneState";
            this.lblCraneState.Size = new System.Drawing.Size(100, 20);
            this.lblCraneState.TabIndex = 476;
            this.lblCraneState.Text = "X:未連線";
            this.lblCraneState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCraneNo
            // 
            this.lblCraneNo.BackColor = System.Drawing.Color.DimGray;
            this.lblCraneNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCraneNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCraneNo.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCraneNo.ForeColor = System.Drawing.Color.White;
            this.lblCraneNo.Location = new System.Drawing.Point(0, 0);
            this.lblCraneNo.Margin = new System.Windows.Forms.Padding(0);
            this.lblCraneNo.Name = "lblCraneNo";
            this.lblCraneNo.Size = new System.Drawing.Size(100, 20);
            this.lblCraneNo.TabIndex = 480;
            this.lblCraneNo.Text = "Crane 1";
            this.lblCraneNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCommandID
            // 
            this.lblCommandID.BackColor = System.Drawing.Color.White;
            this.lblCommandID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCommandID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCommandID.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCommandID.Location = new System.Drawing.Point(0, 60);
            this.lblCommandID.Margin = new System.Windows.Forms.Padding(0);
            this.lblCommandID.Name = "lblCommandID";
            this.lblCommandID.Size = new System.Drawing.Size(100, 20);
            this.lblCommandID.TabIndex = 473;
            this.lblCommandID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCraneMode
            // 
            this.lblCraneMode.BackColor = System.Drawing.Color.Red;
            this.lblCraneMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCraneMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCraneMode.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCraneMode.Location = new System.Drawing.Point(0, 20);
            this.lblCraneMode.Margin = new System.Windows.Forms.Padding(0);
            this.lblCraneMode.Name = "lblCraneMode";
            this.lblCraneMode.Size = new System.Drawing.Size(100, 20);
            this.lblCraneMode.TabIndex = 474;
            this.lblCraneMode.Text = "X:未連線";
            this.lblCraneMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CraneMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.tlpCrane);
            this.Name = "CraneMonitor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(106, 86);
            this.tlpCrane.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpCrane;
        private System.Windows.Forms.Label lblCraneState;
        private System.Windows.Forms.Label lblCraneNo;
        private System.Windows.Forms.Label lblCommandID;
        private System.Windows.Forms.Label lblCraneMode;
    }
}
