using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonTool;
using AutoFrameUI;
using AutoFrameVision;
using AutoFrameDll;
using System.Threading;

namespace AutoFrame
{
    public partial class Form_Main : Form
    {
        Dictionary<RoundButton, Form> m_dicForm = new Dictionary<RoundButton, Form>();
        Form m_currentForm = null;
        RoundButton m_currentButton = null;

        /// <summary>
        /// 关闭应用程序事件
        /// </summary>
        //static public event EventHandler CloseProgrmEventHandler = null;

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            if (SystemMgr.GetInstance().GetParamBool("FullScreen"))
            {
                this.SetVisibleCore(false);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.SetVisibleCore(true);
            }

            //关联页面和站位
            m_dicForm.Add(RoundButton_Auto, new Form_Auto());
            m_dicForm.Add(RoundButton_Manual, new Form_Manual());
            m_dicForm.Add(RoundButton_Vision, new Form_Vision());
            m_dicForm.Add(RoundButton_Alarm, new Form_Alarm());
            m_dicForm.Add(RoundButton_Data, new Form_Data());
            m_dicForm.Add(RoundButton_Machine, new Form_Machine());
            m_dicForm.Add(RoundButton_File, new Form_File());
            m_dicForm.Add(RoundButton_Image, new Form_Image());
            m_dicForm.Add(RoundButton_Login, new Form_Login());
            //初始化页面属性
            foreach (KeyValuePair<RoundButton, Form> kp in m_dicForm)
            {
                kp.Value.TopLevel = false;
                kp.Value.Parent = this.panel_main;
                kp.Value.Dock = DockStyle.Fill;
            }



            //显示主页面
            RoundButton_Auto.PerformClick();
            //修改标题栏
            this.Text = this.Text + "－" + SystemMgr.GetInstance().GetParamString("DeviceName")
                + "－" + SystemMgr.GetInstance().GetParamString("DeviceID");
            //主界面添加报警信息委托,有报警时修改背景色提示
            WarningMgr.GetInstance().WarningEventHandler += new EventHandler(OnWarning);
            OnWarning(this, EventArgs.Empty);
            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;
            //添加站位状态变化响应函数操作
            StationMgr.GetInstance().StateChangedEvent += OnStationStateChanged;
            toolStripStatusLabel_current_state.Text = "设备停止运行中";


            OnProductChanged();
            SystemMgr.GetInstance().StateChangedEvent += OnSystemStateChanged;
            ProductInfo.GetInstance().StateChangedEvent += OnProductChanged;

            this.WindowState = FormWindowState.Maximized;

        }

        /// <summary>
        /// 定时器,在状态栏上以秒为最小单位实时显示时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            toolStripStatusLabel_Time.Text = s;

            SystemMgr.GetInstance().CheckSystemIdle();
        }

        private void OnProductChanged()
        {
            toolStripStatusLabel_product_type.Text = "打标机 " +  ProductInfo.GetInstance().ProductName;
        }

        private void  OnSystemStateChanged(SystemMode Mode)
        {
            if(Mode == SystemMode.Normal_Run_Mode)
            {
                toolStripStatusLabel_system_mode.Text = "正常运行模式";
            }
           else if (Mode == SystemMode.Dry_Run_Mode)
            {
                toolStripStatusLabel_system_mode.Text = "空跑运行模式";
            }
            else if (Mode == SystemMode.Calib_Run_Mode)
            {
                toolStripStatusLabel_system_mode.Text = "自动标定模式";
            }
            else if (Mode == SystemMode.Simulate_Run_Mode)
            {
                toolStripStatusLabel_system_mode.Text = "模拟运行模式";
            }
            else if (Mode == SystemMode.Other_Mode)
            {
                toolStripStatusLabel_system_mode.Text = "其它运行模式";
            }

        }
        private void OnStationStateChanged(StationState OldState, StationState NewState)
        {
            switch (NewState)
            {
                case StationState.STATE_MANUAL:  //手动状态
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel_current_state.ForeColor = System.Drawing.SystemColors.MenuHighlight;
                        toolStripStatusLabel_current_state.Text = "设备停止运行中";
                        //亮红灯并蜂鸣
                        IoMgr.GetInstance().AlarmLight(LightState.黄灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    });
                    break;
                case StationState.STATE_AUTO:   //自动运行状态
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel_current_state.ForeColor = Color.Green ;
                        toolStripStatusLabel_current_state.Text = "设备自动运行中";
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    });
                    break;
                case StationState.STATE_READY:  //等待开始
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel_current_state.ForeColor = Color.Green;
                        toolStripStatusLabel_current_state.Text = "设备准备就绪，请启动！";
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    });
                    break;
                case StationState.STATE_EMG:         //急停状态
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel_current_state.ForeColor = Color.Red; 
                        toolStripStatusLabel_current_state.Text = "设备异常急停，请检查！";
                        //亮红灯并蜂鸣
                        IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                    });
                    break;
                case StationState.STATE_PAUSE:       //暂停状态
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        RoundButton_Start.ImageIndex = 11;
                        RoundButton_Pause.ImageIndex = 12;
                        RoundButton_Stop.ImageIndex = 15;
                        toolStripStatusLabel_current_state.ForeColor = Color.DarkGreen;
                        toolStripStatusLabel_current_state.Text = "设备暂停！";
                        IoMgr.GetInstance().AlarmLight(LightState.黄灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    });
                    break;
                default:
                    break;
            }
        }


        private void OnModeChanged()
        {         
            if (Security.IsOpMode())
            {
                toolStripStatusLabel_user.Text = "操作员";
            }
            else if (Security.IsEngMode())
            {
                toolStripStatusLabel_user.Text = "管理员";
            }
            else
            {
                toolStripStatusLabel_user.Text = "调试员";
            }
        }

        /// <summary>
        /// 点击自动界面按钮,显示自动界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Auto_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Auto);
        }

        /// <summary>
        /// 显示手动界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Manual_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Manual);
        }

        /// <summary>
        /// 显示相机界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Vision_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Vision);
        }

        /// <summary>
        /// 显示报警界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Alarm_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Alarm);
        }

        /// <summary>
        /// 显示数据界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Data_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Data);
        }

        /// <summary>
        /// 显示机台编号信息界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Machine_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Machine);
        }

        /// <summary>
        /// 开始自动流程,如果是暂停则恢复运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Start_Click(object sender, EventArgs e)
        {
            if(WarningMgr.GetInstance().HasErrorMsg())
            {
                MessageBox.Show("设备存在异常未处理，暂不能运行", "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (StationMgr.GetInstance().IsPause())
            {
                StationMgr.GetInstance().ResumeAllStation();
                RoundButton_Start.ImageIndex = 10;
                RoundButton_Pause.ImageIndex = 13;
                RoundButton_Stop.ImageIndex = 15;
            }
            else if (false == StationMgr.GetInstance().IsAutoRunning())
            {
                if(StationMgr.GetInstance().StartRun())
                {
                    RoundButton_Start.ImageIndex = 10;
                    RoundButton_Pause.ImageIndex = 13;
                    RoundButton_Stop.ImageIndex = 15;
                }
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Pause_Click(object sender, EventArgs e)
        {
            if (StationMgr.GetInstance().AllowPause())
            {
                StationMgr.GetInstance().PauseAllStation();
                RoundButton_Start.ImageIndex = 11;
                RoundButton_Pause.ImageIndex = 12;
                RoundButton_Stop.ImageIndex = 15;
            }
        }

        /// <summary>
        /// 结束自动流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Stop_Click(object sender, EventArgs e)
        {
            StationMgr.GetInstance().StopRun();
            RoundButton_Start.ImageIndex = 11;
            RoundButton_Pause.ImageIndex = 12;
            RoundButton_Stop.ImageIndex = 14;
        }

        /// <summary>
        /// 显示相关文件信息界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_File_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_File);
        }

        /// <summary>
        /// 显示图片显示界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Image_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Image);
        }

        /// <summary>
        /// 显示用户登录界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoundButton_Login_Click(object sender, EventArgs e)
        {
            SwitchWnd(RoundButton_Login);
        }

          /// <summary>
        /// 关闭程序,清空报警类数据
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">附带数据的对象</param>
        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (StationMgr.GetInstance().IsAutoRunning())
                StationMgr.GetInstance().StopRun();

            WarningMgr.GetInstance().ClearAllWarning();

            foreach (KeyValuePair<RoundButton, Form> kp in m_dicForm)
            {
                kp.Value.Close();
            }

        }

        /// <summary>
        /// 报警信息委托调用函数,改变整体对话框界面的颜色
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void OnWarning(object Sender, EventArgs e)
        {
            if (WarningMgr.GetInstance().HasErrorMsg())
            {
                this.BackColor = Color.FromArgb(250, 215, 214);
                foreach (KeyValuePair<RoundButton, Form> kp in m_dicForm)
                {
                    if(kp.Value.InvokeRequired)
                    {
                        Action<Color> actionDelegate = (x) => { kp.Value.BackColor = x; };
                        kp.Value.BeginInvoke(actionDelegate, Color.FromArgb(250, 215, 214));
                    }
                    else
                        kp.Value.BackColor = Color.FromArgb(250, 215, 214);
                }
            }
            else
            {
                this.BackColor = Color.FromArgb(255, 255, 255);
                foreach (KeyValuePair<RoundButton, Form> kp in m_dicForm)
                {
                    if (kp.Value.InvokeRequired)
                    {
                        Action<Color> actionDelegate = (x) => { kp.Value.BackColor = x; };
                        kp.Value.BeginInvoke(actionDelegate, Color.FromArgb(250, 215, 214));
                    }
                    else
                        kp.Value.BackColor = Color.FromArgb(255, 255, 255);
                }
            }
        }

        /// <summary>
        /// 根据权限来登录点击的界面
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="btn"></param>
        /// <param name="bPopBox"></param>
        /// <returns></returns>
        public void SwitchWnd(RoundButton btn)
        {
            if (m_currentButton != btn)
            {

                if (m_currentButton != null)
                    m_currentButton.ImageIndex--;
                m_currentButton = btn;
                m_currentButton.ImageIndex++;

                if (m_currentForm != null)
                    m_currentForm.Hide();
                if (m_currentForm != m_dicForm[btn])
                {
                    m_currentForm = m_dicForm[btn];
                    m_currentForm.Show();
                }
            }
        }
    }
}

