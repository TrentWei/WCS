namespace Mirle.ASRS
{
    partial class BufferMonitor
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
            this.tlpBuffer = new System.Windows.Forms.TableLayoutPanel();
            this.lblAuto = new System.Windows.Forms.Label();
            this.lblLoad = new System.Windows.Forms.Label();
            this.lblBufferName = new System.Windows.Forms.Label();
            this.lblReturnRequest = new System.Windows.Forms.Label();
            this.lblCommandID = new System.Windows.Forms.Label();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.tlpBuffer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpBuffer
            // 
            this.tlpBuffer.BackColor = System.Drawing.Color.White;
            this.tlpBuffer.ColumnCount = 3;
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpBuffer.Controls.Add(this.lblAuto, 0, 3);
            this.tlpBuffer.Controls.Add(this.lblLoad, 1, 3);
            this.tlpBuffer.Controls.Add(this.lblBufferName, 0, 0);
            this.tlpBuffer.Controls.Add(this.lblCommandID, 0, 1);
            this.tlpBuffer.Controls.Add(this.lblError, 2, 3);
            this.tlpBuffer.Controls.Add(this.lblMode, 0, 2);
            this.tlpBuffer.Controls.Add(this.lblReturnRequest, 2, 2);
            this.tlpBuffer.Controls.Add(this.lblDestination, 1, 2);
            this.tlpBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBuffer.Location = new System.Drawing.Point(3, 3);
            this.tlpBuffer.Name = "tlpBuffer";
            this.tlpBuffer.RowCount = 4;
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpBuffer.Size = new System.Drawing.Size(45, 60);
            this.tlpBuffer.TabIndex = 3;
            // 
            // lblAuto
            // 
            this.lblAuto.BackColor = System.Drawing.Color.Red;
            this.lblAuto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAuto.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAuto.Location = new System.Drawing.Point(0, 45);
            this.lblAuto.Margin = new System.Windows.Forms.Padding(0);
            this.lblAuto.Name = "lblAuto";
            this.lblAuto.Size = new System.Drawing.Size(14, 15);
            this.lblAuto.TabIndex = 476;
            this.lblAuto.Text = "0";
            this.lblAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoad
            // 
            this.lblLoad.BackColor = System.Drawing.Color.White;
            this.lblLoad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLoad.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblLoad.Location = new System.Drawing.Point(14, 45);
            this.lblLoad.Margin = new System.Windows.Forms.Padding(0);
            this.lblLoad.Name = "lblLoad";
            this.lblLoad.Size = new System.Drawing.Size(15, 15);
            this.lblLoad.TabIndex = 481;
            this.lblLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBufferName
            // 
            this.lblBufferName.BackColor = System.Drawing.Color.DimGray;
            this.lblBufferName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblBufferName, 3);
            this.lblBufferName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblBufferName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBufferName.ForeColor = System.Drawing.Color.White;
            this.lblBufferName.Location = new System.Drawing.Point(0, 0);
            this.lblBufferName.Margin = new System.Windows.Forms.Padding(0);
            this.lblBufferName.Name = "lblBufferName";
            this.lblBufferName.Size = new System.Drawing.Size(45, 15);
            this.lblBufferName.TabIndex = 480;
            this.lblBufferName.Text = "BufName";
            this.lblBufferName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblReturnRequest
            // 
            this.lblReturnRequest.BackColor = System.Drawing.Color.DarkGray;
            this.lblReturnRequest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReturnRequest.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblReturnRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblReturnRequest.Location = new System.Drawing.Point(29, 30);
            this.lblReturnRequest.Margin = new System.Windows.Forms.Padding(0);
            this.lblReturnRequest.Name = "lblReturnRequest";
            this.lblReturnRequest.Size = new System.Drawing.Size(16, 15);
            this.lblReturnRequest.TabIndex = 478;
            this.lblReturnRequest.Text = "0";
            this.lblReturnRequest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCommandID
            // 
            this.lblCommandID.BackColor = System.Drawing.Color.White;
            this.lblCommandID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblCommandID, 3);
            this.lblCommandID.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCommandID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCommandID.Location = new System.Drawing.Point(0, 15);
            this.lblCommandID.Margin = new System.Windows.Forms.Padding(0);
            this.lblCommandID.Name = "lblCommandID";
            this.lblCommandID.Size = new System.Drawing.Size(45, 15);
            this.lblCommandID.TabIndex = 473;
            this.lblCommandID.Text = "00000";
            this.lblCommandID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDestination
            // 
            this.lblDestination.BackColor = System.Drawing.Color.Azure;
            this.lblDestination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDestination.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDestination.Location = new System.Drawing.Point(14, 30);
            this.lblDestination.Margin = new System.Windows.Forms.Padding(0);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(15, 15);
            this.lblDestination.TabIndex = 475;
            this.lblDestination.Text = "0";
            this.lblDestination.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.White;
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblError.Location = new System.Drawing.Point(29, 45);
            this.lblError.Margin = new System.Windows.Forms.Padding(0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(16, 15);
            this.lblError.TabIndex = 479;
            this.lblError.Text = "0";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMode
            // 
            this.lblMode.BackColor = System.Drawing.Color.Lime;
            this.lblMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMode.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMode.Location = new System.Drawing.Point(0, 30);
            this.lblMode.Margin = new System.Windows.Forms.Padding(0);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(14, 15);
            this.lblMode.TabIndex = 474;
            this.lblMode.Text = "0";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BufferMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.tlpBuffer);
            this.MinimumSize = new System.Drawing.Size(51, 66);
            this.Name = "BufferMonitor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(51, 66);
            this.tlpBuffer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpBuffer;
        private System.Windows.Forms.Label lblAuto;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblReturnRequest;
        private System.Windows.Forms.Label lblLoad;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblBufferName;
        private System.Windows.Forms.Label lblCommandID;
        public System.Windows.Forms.Label lblMode;
    }
}
