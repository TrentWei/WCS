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
            this.lblFunNotice = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblBufferName = new System.Windows.Forms.Label();
            this.lblStnMode = new System.Windows.Forms.Label();
            this.lblCmdSno = new System.Windows.Forms.Label();
            this.lblLoad = new System.Windows.Forms.Label();
            this.tlpBuffer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpBuffer
            // 
            this.tlpBuffer.BackColor = System.Drawing.Color.White;
            this.tlpBuffer.ColumnCount = 3;
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.Controls.Add(this.lblAuto, 0, 1);
            this.tlpBuffer.Controls.Add(this.lblFunNotice, 2, 3);
            this.tlpBuffer.Controls.Add(this.lblError, 2, 1);
            this.tlpBuffer.Controls.Add(this.lblDestination, 1, 3);
            this.tlpBuffer.Controls.Add(this.lblBufferName, 0, 0);
            this.tlpBuffer.Controls.Add(this.lblStnMode, 0, 3);
            this.tlpBuffer.Controls.Add(this.lblCmdSno, 0, 2);
            this.tlpBuffer.Controls.Add(this.lblLoad, 1, 1);
            this.tlpBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBuffer.Location = new System.Drawing.Point(3, 3);
            this.tlpBuffer.Name = "tlpBuffer";
            this.tlpBuffer.RowCount = 4;
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBuffer.Size = new System.Drawing.Size(60, 80);
            this.tlpBuffer.TabIndex = 3;
            // 
            // lblAuto
            // 
            this.lblAuto.BackColor = System.Drawing.Color.Red;
            this.lblAuto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAuto.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAuto.Location = new System.Drawing.Point(0, 20);
            this.lblAuto.Margin = new System.Windows.Forms.Padding(0);
            this.lblAuto.Name = "lblAuto";
            this.lblAuto.Size = new System.Drawing.Size(20, 20);
            this.lblAuto.TabIndex = 476;
            this.lblAuto.Text = "0";
            this.lblAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFunNotice
            // 
            this.lblFunNotice.BackColor = System.Drawing.Color.DarkGray;
            this.lblFunNotice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFunNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFunNotice.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblFunNotice.Location = new System.Drawing.Point(40, 60);
            this.lblFunNotice.Margin = new System.Windows.Forms.Padding(0);
            this.lblFunNotice.Name = "lblFunNotice";
            this.lblFunNotice.Size = new System.Drawing.Size(20, 20);
            this.lblFunNotice.TabIndex = 478;
            this.lblFunNotice.Text = "0";
            this.lblFunNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.White;
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblError.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblError.Location = new System.Drawing.Point(40, 20);
            this.lblError.Margin = new System.Windows.Forms.Padding(0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(20, 20);
            this.lblError.TabIndex = 479;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDestination
            // 
            this.lblDestination.BackColor = System.Drawing.Color.Azure;
            this.lblDestination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDestination.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDestination.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDestination.Location = new System.Drawing.Point(20, 60);
            this.lblDestination.Margin = new System.Windows.Forms.Padding(0);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(20, 20);
            this.lblDestination.TabIndex = 475;
            this.lblDestination.Text = "0";
            this.lblDestination.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBufferName
            // 
            this.lblBufferName.BackColor = System.Drawing.Color.DimGray;
            this.lblBufferName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblBufferName, 3);
            this.lblBufferName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBufferName.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBufferName.ForeColor = System.Drawing.Color.White;
            this.lblBufferName.Location = new System.Drawing.Point(0, 0);
            this.lblBufferName.Margin = new System.Windows.Forms.Padding(0);
            this.lblBufferName.Name = "lblBufferName";
            this.lblBufferName.Size = new System.Drawing.Size(60, 20);
            this.lblBufferName.TabIndex = 480;
            this.lblBufferName.Text = "BufName";
            this.lblBufferName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStnMode
            // 
            this.lblStnMode.BackColor = System.Drawing.Color.SpringGreen;
            this.lblStnMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStnMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStnMode.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStnMode.Location = new System.Drawing.Point(0, 60);
            this.lblStnMode.Margin = new System.Windows.Forms.Padding(0);
            this.lblStnMode.Name = "lblStnMode";
            this.lblStnMode.Size = new System.Drawing.Size(20, 20);
            this.lblStnMode.TabIndex = 474;
            this.lblStnMode.Text = "0";
            this.lblStnMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCmdSno
            // 
            this.lblCmdSno.BackColor = System.Drawing.Color.White;
            this.lblCmdSno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblCmdSno, 3);
            this.lblCmdSno.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCmdSno.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCmdSno.Location = new System.Drawing.Point(0, 40);
            this.lblCmdSno.Margin = new System.Windows.Forms.Padding(0);
            this.lblCmdSno.Name = "lblCmdSno";
            this.lblCmdSno.Size = new System.Drawing.Size(60, 20);
            this.lblCmdSno.TabIndex = 473;
            this.lblCmdSno.Text = "99999";
            this.lblCmdSno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoad
            // 
            this.lblLoad.BackColor = System.Drawing.Color.White;
            this.lblLoad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoad.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblLoad.Location = new System.Drawing.Point(20, 20);
            this.lblLoad.Margin = new System.Windows.Forms.Padding(0);
            this.lblLoad.Name = "lblLoad";
            this.lblLoad.Size = new System.Drawing.Size(20, 20);
            this.lblLoad.TabIndex = 481;
            this.lblLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BufferMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tlpBuffer);
            this.MaximumSize = new System.Drawing.Size(66, 86);
            this.MinimumSize = new System.Drawing.Size(66, 86);
            this.Name = "BufferMonitor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(66, 86);
            this.tlpBuffer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpBuffer;
        private System.Windows.Forms.Label lblAuto;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblFunNotice;
        private System.Windows.Forms.Label lblStnMode;
        private System.Windows.Forms.Label lblLoad;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblBufferName;
        private System.Windows.Forms.Label lblCmdSno;
    }
}
