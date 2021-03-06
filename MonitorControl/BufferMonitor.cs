﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mirle.ASRS
{
    public partial class BufferMonitor : UserControl
    {
        private delegate void Update_EventHandler(Label label, string text, Color color);
        private Buffer.Signal autoSignal = Buffer.Signal.Off;
        private Buffer.Signal loadSignal = Buffer.Signal.Off;
        private Buffer.Signal errorSignal = Buffer.Signal.Off;
        private Buffer.StnMode stnMode = Buffer.StnMode.None;

        public string _BufferName
        {
            get { return lblBufferName.Text; }
            set
            {
                funUpdate(lblBufferName, value, Color.DimGray);
            }
        }

        public Buffer.Signal _Auto
        {
            get { return autoSignal; }
            set
            {
                autoSignal = value;
                if (autoSignal == Buffer.Signal.On)
                    funUpdate(lblAuto, "1", Color.Lime);
                else
                    funUpdate(lblAuto, "0", Color.Red);
            }
        }

        public Buffer.Signal _Load
        {
            get { return loadSignal; }
            set
            {
                loadSignal = value;
                if (loadSignal == Buffer.Signal.On)
                    funUpdate(lblLoad, "V", Color.LightSkyBlue);
                else
                    funUpdate(lblLoad, string.Empty, Color.White);
            }
        }

        public Buffer.Signal _Error
        {
            get { return errorSignal; }
            set
            {
                errorSignal = value;
                if (errorSignal == Buffer.Signal.On)
                    funUpdate(lblError, "X", Color.Red);
                else
                    funUpdate(lblError, string.Empty, Color.White);
            }
        }

        public string _CommandID
        {
            get { return lblCommandID.Text; }
            set
            {
                funUpdate(lblCommandID, value, Color.White);
            }
        }

        public Buffer.StnMode _Mode
        {
            get { return stnMode; }
            set
            {
                stnMode = value;
                funUpdate(lblMode, ((int)stnMode).ToString(), Color.Lime);
            }
        }

        public string _Destination
        {
            get { return lblDestination.Text; }
            set
            {
                funUpdate(lblDestination, value, Color.White);
            }
        }

        public string _ReturnRequest
        {
            get { return lblReturnRequest.Text; }
            set
            {
                funUpdate(lblReturnRequest, value, Color.White);
            }
        }

        public BufferMonitor()
        {
            InitializeComponent();
        }

        private void funUpdate(Label label, string text, Color color)
        {
            if (label.InvokeRequired)
            {
                Update_EventHandler Update = new Update_EventHandler(funUpdate);
                this.Invoke(Update, label, text, color);
            }
            else
            {
                label.Text = text;
                label.BackColor = color;
            }
        }
    }
}
