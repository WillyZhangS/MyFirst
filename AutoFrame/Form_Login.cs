using CommonTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoFrameDll;
using System.Xml;

namespace AutoFrame
{
    public partial class Form_Login : Form
    {
        public Form_Login()
        {
            InitializeComponent();
        }

        private void Form_Login_Load(object sender, EventArgs e)
        {
            WarningMgr.GetInstance().WarningEventHandler += new EventHandler(OnWarning);
            comboBox_Name.SelectedIndex = 0;
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;

            // Security.ChangeOpMode(); //操作工模式使能
            // Security.ChangeEngMode("secote456");//工程师模式使能，默认为工程师模式

            roundButton_Normal.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_dry_run.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_calib.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_simulate.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_cover.BaseColor = Color.FromArgb(220, 221, 224);


            OnWarning(this, EventArgs.Empty);
        }

        private void OnModeChanged()
        {
            if (Security.IsOpMode())
            {
                roundButton_production.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
                roundButton_CPK.BaseColor = roundButton_Engineering.BaseColor = Color.FromArgb(220, 221, 224);

                roundButton_Normal.Visible = false;
                roundButton_dry_run.Visible = false;
                roundButton_calib.Visible = false;
				roundButton_simulate.Visible = false;
                roundButton_cover.Visible = false;
            }
            else if (Security.IsEngMode())
            {
                roundButton_Engineering.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
                roundButton_CPK.BaseColor = roundButton_production.BaseColor = Color.FromArgb(220, 221, 224);

                roundButton_Normal.Visible = true;
                roundButton_dry_run.Visible = true;
                roundButton_calib.Visible = true;
				roundButton_simulate.Visible = true;
                roundButton_cover.Visible = true;
            }
            else
            {
                roundButton_CPK.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
                roundButton_production.BaseColor = roundButton_Engineering.BaseColor = Color.FromArgb(220, 221, 224);
                roundButton_Normal.Visible = true;
                roundButton_dry_run.Visible = true;
                roundButton_calib.Visible = true;
				roundButton_simulate.Visible = false;
                roundButton_cover.Visible = true;

            }
        }

        private void roundButton_login_Click(object sender, EventArgs e)
        {
            string strCode = textBox_Password.Text;
            if (comboBox_Name.SelectedIndex == 0)
            {
                 Security.ChangeOpMode();
            }
            else if(comboBox_Name.SelectedIndex == 1)
            {
                if (Security.ChangeFaeMode(strCode))
                {
                }
                else
                {
                    Security.ChangeOpMode();
                }
            }
            else if(comboBox_Name.SelectedIndex == 2)
            {
                if (Security.ChangeEngMode(strCode))
                {
                }
                else
                {
                    Security.ChangeOpMode();
                }
            }
            textBox_Password.Text = "";
        }
        private void OnWarning(object Sender, EventArgs e)
        {
            string strX;
            string strY;
            if (WarningMgr.GetInstance().HasErrorMsg())
            {
                strX = "Alarm Time\n" + WarningMgr.GetInstance().GetLastMsg().tm.ToLongTimeString();
                strY = WarningMgr.GetInstance().GetLastMsg().strLevel + "\n" + WarningMgr.GetInstance().GetLastMsg().strCode;
            }
            else
            {
                strX = "No Alarm";
                strY = "Alarm Time";
            }
            if (roundButton_time.InvokeRequired )
            {
                Action<string> actionDelegate = (x) => { roundButton_time.Text = x.ToString(); };
                roundButton_time.BeginInvoke(actionDelegate, strX);
            }
            else
            {
                roundButton_time.Text = strX;
            }
            if (roundButton_alarm.InvokeRequired)
            {
                Action<string> actionDelegate = (x) => { roundButton_alarm.Text = x.ToString(); };
                roundButton_alarm.BeginInvoke(actionDelegate, strY);
            }
            else
            {
                roundButton_alarm.Text = strY;
            }
        }

        private void roundButton_auto_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().ChangeMode(SystemMode.Normal_Run_Mode);
            roundButton_Normal.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_dry_run.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_calib.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_simulate.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_cover.BaseColor = Color.FromArgb(220, 221, 224);

        }

        private void roundButton_dry_run_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().ChangeMode( SystemMode.Dry_Run_Mode);
            roundButton_Normal.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_dry_run.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_calib.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_simulate.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_cover.BaseColor = Color.FromArgb(220, 221, 224);
        }

        private void roundButton_calib_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().ChangeMode(SystemMode.Calib_Run_Mode);
            roundButton_Normal.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_dry_run.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_calib.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_simulate.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_cover.BaseColor = Color.FromArgb(220, 221, 224);
        }

        private void roundButton_simulate_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().ChangeMode(SystemMode.Simulate_Run_Mode);
            roundButton_Normal.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_dry_run.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_calib.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_simulate.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_cover.BaseColor = Color.FromArgb(220, 221, 224);
        }

        private void roundButton_cover_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().ChangeMode(SystemMode.Other_Mode);
            roundButton_Normal.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_dry_run.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_calib.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_simulate.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_cover.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
        }
    }
}
