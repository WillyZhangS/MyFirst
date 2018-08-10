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
using AutoFrameVision;
using CommonTool;
using Communicate;

namespace AutoFrame
{
    public partial class Form_Auto : Form
    {
        private int m_okCount1 = 0;
        private int m_okCount2 = 0;
        private int m_ngCount1 = 0;//抛料1
        private int m_ngCount2 = 0;//抛料2
        private int SN_Focus = 0;
        
        private DateTime m_tmCTBegin;
        private TimeSpan m_tsBestCT;
        private TimeSpan m_tsSoftware;
        private TimeSpan m_tsMachine;
        
        public Form_Auto()
        {
            InitializeComponent();
            m_tsBestCT = TimeSpan.Zero;
            m_tsSoftware = TimeSpan.Zero;
            m_tsMachine = TimeSpan.Zero;

            //清空打标数据
            strSn.ScanCode_Data = string.Empty;
            textBox_sn.Text = string.Empty;
            //加载加工产品名称
            comboBox1_WaitMarkPro.Items[0]= SystemMgr.GetInstance().GetParamString("ProName_逆变");
            comboBox1_WaitMarkPro.Items[1] = SystemMgr.GetInstance().GetParamString("ProName_上盖");
            comboBox1_WaitMarkPro.Items[2] = SystemMgr.GetInstance().GetParamString("ProName_整机");
            comboBox1_WaitMarkPro.Items[3] = SystemMgr.GetInstance().GetParamString("ProName_上盖RST");

            comboBox1_WaitMarkPro.Items[4]= SystemMgr.GetInstance().GetParamString("OtherProName1");
            comboBox1_WaitMarkPro.Items[5]= SystemMgr.GetInstance().GetParamString("OtherProName2");
            comboBox1_WaitMarkPro.Items[6]= SystemMgr.GetInstance().GetParamString("OtherProName3");
            comboBox1_WaitMarkPro.Items[7]= SystemMgr.GetInstance().GetParamString("OtherProName4");
            comboBox1_WaitMarkPro.Items[8]= SystemMgr.GetInstance().GetParamString("OtherProName5");
            comboBox1_WaitMarkPro.Items[9] = SystemMgr.GetInstance().GetParamString("OtherProName6");
            comboBox1_WaitMarkPro.Items[10] = SystemMgr.GetInstance().GetParamString("OtherProName7");
            comboBox1_WaitMarkPro.Items[11] = SystemMgr.GetInstance().GetParamString("OtherProName8");
            comboBox1_WaitMarkPro.Items[12] = SystemMgr.GetInstance().GetParamString("OtherProName9");
            //以下为预留的视觉处理
            //comboBox1_WaitMarkPro.Items[13] = SystemMgr.GetInstance().GetParamString("OtherProName10");
            //comboBox1_WaitMarkPro.Items[14] = SystemMgr.GetInstance().GetParamString("OtherProName11");
            //comboBox1_WaitMarkPro.Items[15] = SystemMgr.GetInstance().GetParamString("OtherProName12");
            //comboBox1_WaitMarkPro.Items[16] = SystemMgr.GetInstance().GetParamString("OtherProName13");
            //comboBox1_WaitMarkPro.Items[17] = SystemMgr.GetInstance().GetParamString("OtherProName14");
            //comboBox1_WaitMarkPro.Items[18] = SystemMgr.GetInstance().GetParamString("OtherProName15");
            //comboBox1_WaitMarkPro.Items[19] = SystemMgr.GetInstance().GetParamString("OtherProName16");
            
        }
        private void Form_Auto_Load(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().BitChangedEvent += OnSystemBitChanged;  //委托中添加系统位寄存器响应函数操作 
            SystemMgr.GetInstance().IntChangedEvent += OnSystemIntChanged;  //委托中添加系统整型寄存器响应函数操作 
            SystemMgr.GetInstance().DoubleChangedEvent += OnSystemDoubleChanged;  //委托中添加系统浮点型寄存器响应函数操作 

            StationMgr.GetInstance().StateChangedEvent += OnStationStateChanged; //委托中添加站位状态变化响应函数操作
            StationMgr.GetInstance().StopRun();
            StationMgr.GetInstance().SetLogListBox(listBox_station_mgr);
            StationMgr.GetInstance().LogEvent += OnLogView; //委托中添加站位状态变化响应函数操作
            if(SystemMgr.GetInstance().GetParamBool("AutoCycle"))
            {
                StationMgr.GetInstance().BAutoMode = true;  //设置半自动运行属性
                roundButton_auto.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);   //给自动按钮赋初始化颜色
                roundButton_manual.BaseColor = Color.FromArgb(220, 221, 224); //给手动操作按钮赋初始化颜色
            }
            else
            {
                StationMgr.GetInstance().BAutoMode = false;  //设置半自动运行属性
                roundButton_auto.BaseColor = Color.FromArgb(220, 221, 224);   //给自动按钮赋初始化颜色
                roundButton_manual.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff); //给手动操作按钮赋初始化颜色
            }

            WarningMgr.GetInstance().WarningEventHandler += OnWarning;//添加自动界面报警信息响应函数委托
            OnWarning(this, EventArgs.Empty);  //清除自动界面报警信息

            //增加权限等级变更通知
            OnChangeMode();
            Security.ModeChangedEvent += OnChangeMode;


            IoMgr.GetInstance().IoChangedEvent += OnIoChanged;

            StationMgr.GetInstance().GetStation("打标站").SetLogListBox(listBox_laserEngraving);
            StationMgr.GetInstance().GetStation("打标站").LogEvent += OnLogView;
            
            comboBox_product.Items.Clear();
            foreach(string s in ProductInfo.GetInstance().m_strAllType)
            {
                comboBox_product.Items.Add(s);
            }
            comboBox_product.SelectedIndex = comboBox_product.Items.IndexOf(ProductInfo.GetInstance().ProductName);
           
        }

        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnChangeMode()
        {
            if (Security.IsOpMode())
            {
                comboBox_product.Enabled = false;
                //button_reset.Enabled = false;
            }
            else 
            {
                comboBox_product.Enabled = true;
                //button_reset.Enabled = true;
            }         
        }

        /// <summary>
        /// 站位状态变化委托响应函数
        /// </summary>
        /// <param name="state">站位状态值</param>
        private void OnStationStateChanged(StationState OldState,StationState NewState)
        {
            switch (NewState)
            {
                case StationState.STATE_MANUAL:  //手动状态
                    label_sta_manual.ImageIndex = 1;
                    label_sta_pause.ImageIndex = 0;
                    label_sta_auto.ImageIndex = 0;
                    label_sta_ready.ImageIndex = 0;
                    label_sta_emg.ImageIndex = 0;

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        label_stop_time.Text = DateTime.Now.ToString("HH:mm:ss");                       
                    });
                    break;
                case StationState.STATE_AUTO:   //自动运行状态
                     label_sta_auto.ImageIndex = 1;
                    label_sta_manual.ImageIndex = 0;
                    label_sta_pause.ImageIndex = 0;
                    label_sta_ready.ImageIndex = 0;
                    label_sta_emg.ImageIndex = 0;

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        label_start_time.Text = DateTime.Now.ToString("HH:mm:ss");
                    });
                    break;
                case StationState.STATE_READY:  //等待开始
                    label_sta_ready.ImageIndex = 1;
                    break;
                case StationState.STATE_EMG:         //急停状态
                    label_sta_emg.ImageIndex =2;
                    label_sta_pause.ImageIndex = 0;
                    label_sta_ready.ImageIndex = 0;

                    break;
                case StationState.STATE_PAUSE:       //暂停状态
                    label_sta_pause.ImageIndex = 1;
                    break;
                 default:
                    break;
            }
        }

        /// <summary>
        /// 响应IO变化事件
        /// </summary>
        /// <param name="nCard"></param>
        private void OnIoChanged(int nCard)
        {
            if(nCard == 1)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    label_load.ImageIndex = IoMgr.GetInstance().GetIoInState(1, 5) ? 1 : 0;
                });
            }
        }

        /// <summary>
        /// 报警信息委托调用函数
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void OnWarning(object Sender, EventArgs e)
        {
            if (WarningMgr.GetInstance().HasErrorMsg())
            {
                if (label_warning.BackColor == Color.FromKnownColor(KnownColor.Control))
                {
                    label_warning.BackColor = Color.Red;
                }
                if (label_warning.InvokeRequired)  //c#中禁止跨线程直接访问控件，InvokeRequired是为了解决这个问题而产生的,用一个异步执行委托
                {
                    Action<string> actionDelegate = (x) => { this.label_warning.Text = x.ToString(); };
                    // 或者
                    // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                    this.label_warning.BeginInvoke(actionDelegate, WarningMgr.GetInstance().GetLastMsg().strMsg);

                }
                else
                    label_warning.Text = WarningMgr.GetInstance().GetLastMsg().strMsg;

            }
            else
            {
                label_warning.BackColor = Color.FromKnownColor(KnownColor.Control);
                if (label_warning.InvokeRequired) //c#中禁止跨线程直接访问控件，InvokeRequired是为了解决这个问题而产生的,用一个异步执行委托
                {
                    Action<string> actionDelegate = (x) => { this.label_warning.Text = x.ToString(); };
                    // 或者
                    // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                    this.label_warning.BeginInvoke(actionDelegate, string.Empty);

                }
                else
                    label_warning.Text = string.Empty;
            }
        }

        /// <summary>
        /// 系统位寄存器变化委托响应函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        protected void OnSystemBitChanged(int nIndex, bool bBit)
        {
            SysBitReg sbr = (SysBitReg)nIndex;
            switch (sbr)
            {
                case SysBitReg.扫码上料完成:
                    label_load.ImageIndex = bBit ? 1 : 0;
                    break;
                case SysBitReg.标刻完成:
                    label_assm2.ImageIndex = bBit ? 1 : 0;
                    break;
                case SysBitReg.条码更新:
                    CrossDelegate_Code1 da = new CrossDelegate_Code1(GetSn);
                    this.BeginInvoke(da, 1); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    break;
                case SysBitReg.扫码数据清除:
                    CrossDelegate_Code2 da0 = new CrossDelegate_Code2(DelSn);
                    this.BeginInvoke(da0, 1); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    break;
                case SysBitReg.是否扫码:
                    Delegate_IsScan da1 = new Delegate_IsScan(IsScan);
                    this.BeginInvoke(da1, 1); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    break;
            }
        }
        void GetSn(int nStep)
        {
            strSn.ScanCode_Data = textBox_sn.Text;
            SelectProStyle();
        }
        void DelSn(int nStep)
        {
            strSn.ScanCode_Data = string.Empty;
            textBox_sn.Text =string.Empty;
        }
        void IsScan(int nStep)
        {
            if (checkBox1_IsScan.CheckState == CheckState.Checked)
            {
                SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.是否扫码, 1, false);
            }
            else
            {
                SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.是否扫码, 2, false);
            }
        }
        /// <summary>
        /// 进度条时间刷新
        /// </summary>
        /// <param name="nStep"></param>
        void ProcessStep(int nStep)
        {
            if (nStep <= 100 && nStep >= 0)
            {
                progressBar_all.Value = nStep;
                label_percent.Text = string.Format("{0}%", nStep);

                if (nStep == 0)
                {
                    label_current_CT.Text = "0";
                    m_tmCTBegin = DateTime.Now;
                }
                else if (nStep == 100)
                {
                    int number = 0;
                    if (Int32.TryParse(label_current_num.Text, out number))
                    {
                        label_current_num.Text = (++number).ToString();

                        int nTarNum;
                        if (Int32.TryParse(numericUpDown1.Text, out nTarNum))
                        {
                            if (number >= nTarNum  && nTarNum != 0)//订单已完成
                            {
                                this.roundButton_manual.PerformClick();//切换为半自动
                            }
                        }
                    }

                    TimeSpan ts = DateTime.Now - m_tmCTBegin;
                    label_current_CT.Text = ts.TotalSeconds.ToString("f2");

                    if (ts < m_tsBestCT || m_tsBestCT.TotalSeconds < 0.01 )
                    {
                        m_tsBestCT = ts;
                        label_best_CT.Text = m_tsBestCT.TotalSeconds.ToString("f2");
                    }
                    m_tmCTBegin = DateTime.MinValue;
                }
            }
        }

        //定义一个关联进度条时间刷新的委托
        public delegate void CrossDelegate(int nStep);
        public delegate void CrossDelegate_Code1(int nStep);
        public delegate void CrossDelegate_Code2(int nStep);
        public delegate void Delegate_type(int nStep);
        public delegate void Delegate_IsScan(int nStep);
        public delegate void Delegate_IsRST(int nStep);
        public delegate void Dele_Style(int nStep);
        public delegate void Delegate_Enable(int nStep);
        //public delegate void LabelThrow1Delegate();
        //public delegate void LabelThrow2Delegate();

        void LabelCountShow(int nStep)
        {
            switch(nStep)
            {
                case 1:
                    //if (label_ok_num.InvokeRequired)
                    {
                        //label_ok_num.Text = (m_okCount1 + m_okCount2).ToString();
                    }
                    break;
                case 2:
                    //if (label_throw1.InvokeRequired)
                    {
                        //label_throw1.Text = m_ngCount1.ToString();
                        //CsvOperation cs = new CsvOperation("抛料数");
                        //cs[0, 0] = m_ngCount1.ToString();
                        //cs[0, 1] = DateTime.Now.ToShortDateString();
                        //cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                        //cs[0, 3] = "robot1";
                        //cs.Save();
                    }
                    break;
                case 3:
                    //if (label_throw2.InvokeRequired)
                    {
                        //label_throw2.Text = m_ngCount2.ToString();
                        //CsvOperation cs = new CsvOperation("抛料数");
                        //cs[0, 0] = m_ngCount2.ToString();
                        //cs[0, 1] = DateTime.Now.ToShortDateString();
                        //cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                        //cs[0, 3] = "robot2";
                        //cs.Save();
                    }
                    break;
            }
        }

        /// <summary>
        /// 系统整型寄存器变化委托响应函数
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="nData">寄存器值</param>
        protected void OnSystemIntChanged(int nIndex, int nData)
        {
            switch (nIndex)
            {
                case (int)SysIntReg.进度百分比:
                    // ProcessStep(nData);
                    CrossDelegate da = new CrossDelegate(ProcessStep);
                    this.BeginInvoke(da, nData); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    break;
                case (int)SysIntReg.工件类型:
                    Delegate_type daprostyle = new Delegate_type(prostyle);
                    this.BeginInvoke(daprostyle, nData); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    break;
            }
        }
        void prostyle(int nStep)
        {
            switch (comboBox1_WaitMarkPro.SelectedIndex)
            {
                //*******************************现有模块**********************************************
                case 0://逆变模块
                    label5.Text = comboBox1_WaitMarkPro.Items[0].ToString();
                    break;
                case 1://上盖模块
                    label5.Text = comboBox1_WaitMarkPro.Items[1].ToString();
                    break;
                case 2://整机模块
                    label5.Text = comboBox1_WaitMarkPro.Items[2].ToString();
                    break;
                case 3://上盖模块RST
                    label5.Text = comboBox1_WaitMarkPro.Items[3].ToString();
                    break;
                //*******************************其他新增模块******************************************
                default:////产品类型序号新增模块序号比大1；工件类型最多到9，因为预留的新增模块只有五个
                    int m_style = comboBox1_WaitMarkPro.SelectedIndex;
                    if (m_style < 0)
                    {
                        label5.Text = "NONE";
                    }
                    else
                    {
                        label5.Text = comboBox1_WaitMarkPro.Items[m_style].ToString();
                    }
                    break;
            }
        }

        public delegate void CrossDelegateDouble(int nIndex);
        void ProcessDoubleChange(int nIndex)
        {
            SysFloatReg enumReg = (SysFloatReg)(nIndex);
            switch (enumReg)
            {
                case SysFloatReg.T11_X:
                    {
                        //int index = dataGridView_assm1.Rows.Add();
                        //int k = 0;
                        //dataGridView_assm1.Rows[index].Cells[k++].Value = (index + 1).ToString();
                        //for (int i = 0; i < 3; ++i)
                        //    dataGridView_assm1.Rows[index].Cells[k++].Value =
                        //        SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_X + i).ToString("F3");
                        //CsvOperation cs = new CsvOperation("T1");
                        //cs[0, 0] = index.ToString();
                        //cs[0, 1] = DateTime.Now.ToShortDateString();
                        //cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                        //for (int i = 0; i < 3; ++i)
                        //{
                        //    cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_X + i).ToString("F3");
                        //}
                        //cs.Save();
                    }
                    break;
                //case SysFloatReg.T11_Y:
                //    {
                //        int index = dataGridView_assm1.Rows.Add();
                //        int k = 0;
                //        dataGridView_assm1.Rows[index].Cells[k++].Value = (index + 1).ToString();
                //        for (int i = 0; i < 3; ++i)
                //            dataGridView_assm1.Rows[index].Cells[k++].Value =
                //                SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_Y + i).ToString("F3");
                //        CsvOperation cs = new CsvOperation("T1");
                //        cs[0, 0] = index.ToString();
                //        cs[0, 1] = DateTime.Now.ToShortDateString();
                //        cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                //        for (int i = 0; i < 3; ++i)
                //        {
                //            cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_Y + i).ToString("F3");
                //        }
                //        cs.Save();
                //    }
                    break;
                case SysFloatReg.T13_X:
                    //{
                    //    int index = dataGridView_assm2.Rows.Add();
                    //    int k = 0;
                    //    dataGridView_assm2.Rows[index].Cells[k++].Value = (index + 1).ToString();
                    //    for(int i=0; i<3; ++i)
                    //        dataGridView_assm2.Rows[index].Cells[k++].Value =
                    //        SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_X + i).ToString("F3");
                    //    CsvOperation cs = new CsvOperation("T1");
                    //    cs[0, 0] = index.ToString();
                    //    cs[0, 1] = DateTime.Now.ToShortDateString();
                    //    cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                    //    for (int i = 0; i < 3; ++i)
                    //    {
                    //        cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_X + i).ToString("F3");
                    //    }
                    //    cs.Save();
                    //}
                    break;
                //case SysFloatReg.T13_Y:
                //    {
                //        int index = dataGridView_assm2.Rows.Add();
                //        int k = 0;
                //        dataGridView_assm2.Rows[index].Cells[k++].Value = (index + 1).ToString();
                //        for (int i = 0; i < 3; ++i)
                //            dataGridView_assm2.Rows[index].Cells[k++].Value =
                //            SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_Y + i).ToString("F3");
                //        CsvOperation cs = new CsvOperation("T1");
                //        cs[0, 0] = index.ToString();
                //        cs[0, 1] = DateTime.Now.ToShortDateString();
                //        cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                //        for (int i = 0; i < 3; ++i)
                //        {
                //            cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_Y + i).ToString("F3");
                //        }
                //        cs.Save();
                //    }
                //    break;
                case SysFloatReg.T14_X:
                    //{
                    //    int index = dataGridView_assm3.Rows.Add();
                    //    int k = 0;
                    //    dataGridView_assm3.Rows[index].Cells[k++].Value = (index + 1).ToString();
                    //    for (int i = 0; i < 3; ++i)
                    //        dataGridView_assm3.Rows[index].Cells[k++].Value =
                    //            SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_X + i).ToString("F3");
                    //    CsvOperation cs = new CsvOperation("T1");
                    //    cs[0, 0] = index.ToString();
                    //    cs[0, 1] = DateTime.Now.ToShortDateString();
                    //    cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                    //    for (int i = 0; i < 3; ++i)
                    //    {
                    //        cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_X + i).ToString("F3");
                    //    }
                    //    cs.Save();
                    //}
                    break;
                //case SysFloatReg.T14_Y:
                //    {
                //        int index = dataGridView_assm3.Rows.Add();
                //        int k = 0;
                //        dataGridView_assm3.Rows[index].Cells[k++].Value = (index + 1).ToString();
                //        for (int i = 0; i < 3; ++i)
                //            dataGridView_assm3.Rows[index].Cells[k++].Value =
                //                SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_Y + i).ToString("F3");
                //        CsvOperation cs = new CsvOperation("T1");
                //        cs[0, 0] = index.ToString();
                //        cs[0, 1] = DateTime.Now.ToShortDateString();
                //        cs[0, 2] = DateTime.Now.ToString("HH:mm:ss");
                //        for (int i = 0; i < 3; ++i)
                //        {
                //            cs[0, i + 3] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_Y + i).ToString("F3");
                //        }
                //        cs.Save();
                //    }
                //    break;
            }
        }

        /// <summary>
        /// 系统浮点型寄存器变化委托响应函数
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="fData">寄存器值</param>
        protected void OnSystemDoubleChanged(int nIndex, double fData)
        {
            CrossDelegateDouble dl = new CrossDelegateDouble(ProcessDoubleChange);
            this.BeginInvoke(dl, nIndex);
        }

        /// <summary>
        /// 清除界面数据记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_clean_Click(object sender, EventArgs e)
        {
            m_okCount1 = 0;
            m_okCount2 = 0;
            m_ngCount1 = 0;
            m_ngCount2 = 0;
            //label_ok_num.Text = string.Empty;
            //label_ng_num.Text = string.Empty;
            //label_ok_percent.Text = string.Empty;
            //label_throw1.Text = string.Empty;
            //label_throw2.Text = string.Empty;
            
            //dataGridView_assm1.Rows.Clear();
            //dataGridView_assm2.Rows.Clear();
            //dataGridView_assm3.Rows.Clear();
            
            label_time_soft_total.Text = string.Empty;
            label_current_CT.Text = string.Empty;
            label_best_CT.Text = string.Empty;

            label_current_num.Text = "0";
            //m_timeRunBegin = DateTime.Now;
            m_tsBestCT = TimeSpan.Zero;
            
           // listBox_bottomRoate.Items.Clear();
          //  listBox_bottomTransport.Items.Clear();
          //  listBox_station_mgr.Items.Clear();

        }

        /// <summary>
        /// 清除界面最后一条报警信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_warning_clean_Click(object sender, EventArgs e)
        {
            if (WarningMgr.GetInstance().HasErrorMsg())
            {
                WarningMgr.GetInstance().ClearWarning(WarningMgr.GetInstance().Count - 1);
            }
        }

        /// <summary>
        /// 计时,定时1000ms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {

            SN_Focus++;
            if (SN_Focus==30)
            {
                SN_Focus = 0;
                textBox_sn.Focus();//光标聚焦于SN输入框；
            }
            
            m_tsSoftware += new TimeSpan(0, 0, 1);
            label_time_soft_total.Text = string.Format("{0}天{1}小时{2}分{3}秒", m_tsSoftware.Days,
                                    m_tsSoftware.Hours, m_tsSoftware.Minutes, m_tsSoftware.Seconds);
            if(StationMgr.GetInstance().IsAutoRunning())
            {
                m_tsMachine += new TimeSpan(0,0, 1);
                label_time_machine_total.Text = string.Format("{0}天{1}小时{2}分{3}秒", m_tsMachine.Days,
                                    m_tsMachine.Hours, m_tsMachine.Minutes, m_tsMachine.Seconds);

                if (m_tmCTBegin != DateTime.MinValue)
                {
                    TimeSpan ts = DateTime.Now - m_tmCTBegin;
                    label_current_CT.Text = ts.TotalSeconds.ToString("f2");
                }
            }
            if (label_warning.Text.Length > 0)
            {
                if (label_warning.BackColor == Color.Red)
                {
                    label_warning.BackColor = Color.FromKnownColor(KnownColor.Control); 
                }
                else
                {
                    label_warning.BackColor = Color.Red;
                }
            }

        }

        /// <summary>
        /// 双击报警框,打开界面报警信息 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label_warning_DoubleClick(object sender, EventArgs e)
        {
            if (label_warning.Text != string.Empty)
            {
                Form_Warning fw = new Form_Warning();
                fw.ShowDialog(this);
            }
        }

        /// <summary>
        /// 自动循环模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roundButton_auto_Click(object sender, EventArgs e)
        {
            StationMgr.GetInstance().BAutoMode = true;  
            roundButton_auto.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
            roundButton_manual.BaseColor = Color.FromArgb(220, 221, 224);
        }

        /// <summary>
        /// 切换单步作业模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roundButton_manual_Click(object sender, EventArgs e)
        {
            StationMgr.GetInstance().BAutoMode = false;
            roundButton_auto.BaseColor = Color.FromArgb(220, 221, 224);
            roundButton_manual.BaseColor = Color.FromArgb(0xb3, 0xca, 0xff);
        }
        
        /// <summary>
        /// 在列表框中显示字符串
        /// </summary>
        /// <param name="strLog"></param>

        public void OnLogView(ListBox logListBox,string strLog)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (logListBox.Items.Count > 2000)
                    logListBox.Items.Clear();
                logListBox.Items.Add(strLog);
                logListBox.TopIndex = logListBox.Items.Count - (int)(logListBox.Height / logListBox.ItemHeight); ;

            });
        }

        private void Form_Auto_FormClosed(object sender, FormClosedEventArgs e)
        {
            AutoTool.SaveLifeTime(m_tsMachine, m_tsSoftware);
        }
        private void comboBox_product_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox_product.Text != ProductInfo.GetInstance().ProductName)
            {
                ProductInfo.GetInstance().ChangeType(comboBox_product.Text);
            }
        }
        /// <summary>
        /// 选择待加工工件的类型；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_WaitMarkPro_SelectedValueChanged_1(object sender, EventArgs e)
        {
            SelectProStyle();
        }
        private void SelectProStyle()
        {
            switch (comboBox1_WaitMarkPro.SelectedIndex)
            {
                //*******************************现有模块**********************************************
                case 0://逆变模块
                    SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.工件类型, 2, true);
                    checkBox1_IsScan.Visible = false;
                    checkBox1_MarkingSecond.Visible = false;
                    break;
                case 1://上盖模块
                    SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.工件类型, 3, true);
                    checkBox1_IsScan.Visible = false;
                    checkBox1_MarkingSecond.Visible = false;
                    break;
                case 2://整机模块
                    SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.工件类型, 4, true);
                    checkBox1_IsScan.Visible = false;
                    checkBox1_MarkingSecond.Visible = false;
                    break;
                case 3://上盖模块RST
                    SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.工件类型, 1, true);
                    checkBox1_IsScan.Visible = false;
                    checkBox1_MarkingSecond.Visible = false;
                    break;
                //*******************************其他新增模块******************************************
                default:////产品类型序号比新增模块序号大1；工件类型最多到9，因为预留的新增模块只有五个
                    int m_style = comboBox1_WaitMarkPro.SelectedIndex;
                    SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.工件类型, m_style + 1, true);
                    checkBox1_IsScan.Visible = true;
                    checkBox1_MarkingSecond.Visible = true;
                    break;
            }
        }
        /// <summary>
        /// 选择是否打两次打标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_MarkingSecond_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1_MarkingSecond.CheckState== CheckState.Checked)
            {
                SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.是否打两次标, 1, false);
            }
            else
            {
                SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.是否打两次标, 2, false);
            }
        }
    }
}
