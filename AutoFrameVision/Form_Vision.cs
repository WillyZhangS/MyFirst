using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CommonTool;

namespace AutoFrameVision
{
    /// <summary>
    /// 自动显示的视觉界面
    /// </summary>
    public partial class Form_Vision : Form
    {
        /// <summary>
        /// 构造函数,初始化系统中要用到的显示控件,关联视觉步骤的关系,以及日志显示控件
        /// </summary>
        public Form_Vision()
        {
            InitializeComponent();

            visionControl1.InitWindow();
            visionControl2.InitWindow();
            visionControl3.InitWindow();

            VisionMgr.GetInstance().BindWindow("T1_1", visionControl1);
            VisionMgr.GetInstance().BindWindow("T1_2", visionControl1);
            VisionMgr.GetInstance().BindWindow("T1_3", visionControl1);
            VisionMgr.GetInstance().BindWindow("T1_4", visionControl1);
            VisionMgr.GetInstance().BindWindow("T1_calib", visionControl2);
            VisionMgr.GetInstance().BindWindow("T1_calib2", visionControl2);

            VisionMgr.GetInstance().BindWindow("T1_10", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_11", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_12", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_13", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_14", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_15", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_16", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_17", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_18", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_19", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_20", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_21", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_22", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_23", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_24", visionControl3);
            VisionMgr.GetInstance().BindWindow("T1_25", visionControl3);

            //加载加工产品名称
            T1_Add.Items[0] = SystemMgr.GetInstance().GetParamString("OtherProName1");
            T1_Add.Items[1] = SystemMgr.GetInstance().GetParamString("OtherProName2");
            T1_Add.Items[2] = SystemMgr.GetInstance().GetParamString("OtherProName3");
            T1_Add.Items[3] = SystemMgr.GetInstance().GetParamString("OtherProName4");
            T1_Add.Items[4] = SystemMgr.GetInstance().GetParamString("OtherProName5");
            T1_Add.Items[5] = SystemMgr.GetInstance().GetParamString("OtherProName6");
            T1_Add.Items[6] = SystemMgr.GetInstance().GetParamString("OtherProName7");
            T1_Add.Items[7] = SystemMgr.GetInstance().GetParamString("OtherProName8");
            T1_Add.Items[8] = SystemMgr.GetInstance().GetParamString("OtherProName9");
            
            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;

            VisionMgr.GetInstance().SetLogListBox(listbox_log);
            VisionMgr.GetInstance().LogEvent += OnLogView;
        }

        /// <summary>
        /// 窗口加载函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Vision_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnModeChanged()
        {
            if (Security.IsOpMode())
            {
                roundPanel_button.Enabled = false;
            }
            else
            {
                roundPanel_button.Enabled = true;
            }
        }
        public void OnLogView(ListBox logListBox, string strLog)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (logListBox.Items.Count > 2000)
                    logListBox.Items.Clear();
                logListBox.Items.Add(strLog);
                logListBox.TopIndex = logListBox.Items.Count - (int)(logListBox.Height / logListBox.ItemHeight); ;

            });
        }

        /// <summary>
        /// 清除全部日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_clear_Click(object sender, EventArgs e)
        {
            listbox_log.Items.Clear();
        }

        /// <summary>
        /// 显示手动调试窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_debug_Click(object sender, EventArgs e)
        {
                 Form_Vision_debug frm = new Form_Vision_debug();
                frm.ShowDialog(this);        
        }

        /// <summary>
        /// T1处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_T1_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_1");
        }

        private void button_T1_2_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_2");

        }
        private void button_T1_3_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_3");
        }
        private void button_T1_4_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_4");
        }
        private void buttonT1_calib_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_calib");
        }
        private void buttonT1_calib2_Click(object sender, EventArgs e)
        {
            VisionMgr.GetInstance().ProcessStep("T1_calib2");
        }
        private void T1_Add_SelectedIndexChanged(object sender, EventArgs e)
        {
            string T1_N = "T1_" + (T1_Add.SelectedIndex+10).ToString();
            VisionMgr.GetInstance().ProcessStep(T1_N);
        }
        /// <summary>
        /// 删除指定项的日志显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delete_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection indices  = listbox_log.SelectedIndices;
            if (indices.Count > 0)
            {
                for (int n = indices.Count - 1; n >= 0; --n)
                {
                    listbox_log.Items.RemoveAt(indices[n]);
                }
            }
        }

        /// <summary>
        /// 调用标定对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cali_Click(object sender, EventArgs e)
        {
            Form_CaliNPoint cnp = new Form_CaliNPoint();
            cnp.ShowDialog(this);
        }

        private void button_config_Click(object sender, EventArgs e)
        {
            Form_Vision_config frm = new Form_Vision_config();
            frm.ShowDialog(this);
        }
    }
}
