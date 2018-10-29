using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mirle.ASRS
{
    public partial class CraneMonitor : UserControl
    {
        private delegate void Update_EventHandler(string text);
        private int intCraneNo = 1;

        public int _CraneNo
        {
            get { return intCraneNo; }
            set
            {
                intCraneNo = value;
                funSetCraneNo(intCraneNo.ToString());
            }
        }

        public string _CommandID
        {
            get { return lblCommandID.Text; }
            set { funSetCommandID(value); }
        }

        public string _CraneMode
        {
            get
            {
                if(lblCraneMode.Text.Length > 1)
                    return lblCraneMode.Text.Remove(1);
                else
                    return lblCraneMode.Text;
            }
            set { funUpdateCraneMode(value); }
        }

        public string _CraneState
        {
            get
            {
                if(lblCraneState.Text.Length > 1)
                    return lblCraneState.Text.Remove(1);
                else
                    return lblCraneState.Text;
            }
            set { funUpdateCraneState(value); }
        }

        public CraneMonitor()
        {
            InitializeComponent();
        }

        private void funSetCraneNo(string craneNo)
        {
            if(lblCraneNo.InvokeRequired)
            {
                Update_EventHandler Update = new Update_EventHandler(funSetCraneNo);
                this.Invoke(Update, craneNo);
            }
            else
                lblCraneNo.Text = "Crane " + craneNo;
        }

        private void funSetCommandID(string commandID)
        {
            if(lblCommandID.InvokeRequired)
            {
                Update_EventHandler Update = new Update_EventHandler(funSetCommandID);
                this.Invoke(Update, commandID);
            }
            else
                lblCommandID.Text = commandID;
        }

        private void funUpdateCraneMode(string text)
        {
            if(lblCraneMode.InvokeRequired)
            {
                Update_EventHandler Update = new Update_EventHandler(funUpdateCraneMode);
                this.Invoke(Update, text);
            }
            else
            {
                switch(text)
                {
                    case "C":
                        lblCraneMode.Text = "C:电脑模式";
                        lblCraneMode.BackColor = Color.Lime;
                        break;
                    case "R":
                        lblCraneMode.Text = "R:地上盘模式";
                        lblCraneMode.BackColor = Color.Red;
                        break;
                    case "M":
                        lblCraneMode.Text = "M:维护模式";
                        lblCraneMode.BackColor = Color.Red;
                        break;
                    case "I":
                        lblCraneMode.Text = "I:维护模式";
                        lblCraneMode.BackColor = Color.Red;
                        break;
                    case "N":
                        lblCraneMode.Text = "N:未开启";
                        lblCraneMode.BackColor = Color.Red;
                        break;
                    case "X":
                        lblCraneMode.Text = "X:未连线";
                        lblCraneMode.BackColor = Color.Red;
                        break;
                    default:
                        lblCraneMode.Text = text;
                        lblCraneMode.BackColor = Color.White;
                        break;
                }
            }
        }

        private void funUpdateCraneState(string text)
        {
            if(lblCraneState.InvokeRequired)
            {
                Update_EventHandler Update = new Update_EventHandler(funUpdateCraneState);
                this.Invoke(Update, text);
            }
            else
            {
                switch(text)
                {
                    case "W":
                        lblCraneState.Text = "W:待机中";
                        lblCraneState.BackColor = Color.Lime;
                        break;
                    case "A":
                        lblCraneState.Text = "A:动作中";
                        lblCraneState.BackColor = Color.Lime;
                        break;
                    case "E":
                        lblCraneState.Text = "E:异常中";
                        lblCraneState.BackColor = Color.Red;
                        break;
                    case "I":
                        lblCraneState.Text = "I:检查中";
                        lblCraneState.BackColor = Color.Lime;
                        break;
                    case "N":
                        lblCraneState.Text = "N:未开启";
                        lblCraneState.BackColor = Color.Red;
                        break;
                    case "X":
                        lblCraneState.Text = "X:未连线";
                        lblCraneState.BackColor = Color.Red;
                        break;
                    default:
                        lblCraneState.Text = text;
                        lblCraneState.BackColor = Color.White;
                        break;
                }
            }
        }
    }
}