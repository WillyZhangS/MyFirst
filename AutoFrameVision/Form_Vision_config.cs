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

using System.Threading;

using HalconDotNet;

namespace AutoFrameVision
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Form_Vision_config : Form
    {
         /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <returns></returns>
        public Form_Vision_config()
        {
            InitializeComponent();
            //加载加工产品名称
            comboBox1.Items[0] = SystemMgr.GetInstance().GetParamString("OtherProName1");
            comboBox1.Items[1] = SystemMgr.GetInstance().GetParamString("OtherProName2");
            comboBox1.Items[2] = SystemMgr.GetInstance().GetParamString("OtherProName3");
            comboBox1.Items[3] = SystemMgr.GetInstance().GetParamString("OtherProName4");
            comboBox1.Items[4] = SystemMgr.GetInstance().GetParamString("OtherProName5");
            comboBox1.Items[5] = SystemMgr.GetInstance().GetParamString("OtherProName6");
            comboBox1.Items[6] = SystemMgr.GetInstance().GetParamString("OtherProName7");
            comboBox1.Items[7] = SystemMgr.GetInstance().GetParamString("OtherProName8");
            comboBox1.Items[8] = SystemMgr.GetInstance().GetParamString("OtherProName9");
            comboBox1.SelectedIndex = 0;
        }
        /// <summary>
        /// 初始化时根据视觉管理器配置添加各相机及步骤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Vision_config_Load(object sender, EventArgs e)
        {
            VisionBase vb = VisionMgr.GetInstance().GetVisionBase("T1_1");
            numericUpDown_T1_1.Value = vb.m_ExposureTime;
            //numericUpDown_angle_T1_1.Value = (decimal)((Vision_T1_2)vb).m_ModelAngle;

            vb = VisionMgr.GetInstance().GetVisionBase("T1_2");
            numericUpDown_T1_2.Value = vb.m_ExposureTime;
            //numericUpDown_angle_T1.Value = (decimal)((Vision_T1_2)vb).m_ModelAngle;

            vb = VisionMgr.GetInstance().GetVisionBase("T1_3");
            numericUpDown_T1_3.Value = vb.m_ExposureTime;
            
            vb = VisionMgr.GetInstance().GetVisionBase("T1_4");
            numericUpDown_T1_4.Value = vb.m_ExposureTime;

            vb = VisionMgr.GetInstance().GetVisionBase("T1_10");
            numericUpDown_Add.Value = vb.m_ExposureTime;

            vb = VisionMgr.GetInstance().GetVisionBase("T1_calib");
            numericUpDown_T1_calib.Value = vb.m_ExposureTime;

            
            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;
        }

        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnModeChanged()
        {
            if (Security.IsOpMode())
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            VisionBase vb = VisionMgr.GetInstance().GetVisionBase("T1_1");
            vb.m_ExposureTime = (int)numericUpDown_T1_1.Value;
            vb.SaveParam();

            vb = VisionMgr.GetInstance().GetVisionBase("T1_2");
            vb.m_ExposureTime= (int)numericUpDown_T1_2.Value ;

            vb.SaveParam();

            vb = VisionMgr.GetInstance().GetVisionBase("T1_3");
            vb.m_ExposureTime = (int)numericUpDown_T1_3.Value;
            vb.SaveParam();

            vb = VisionMgr.GetInstance().GetVisionBase("T1_4");
            vb.m_ExposureTime = (int)numericUpDown_T1_4.Value;
            vb.SaveParam();

            string T1_N = "T1_"+ (comboBox1.SelectedIndex+10).ToString();
            vb = VisionMgr.GetInstance().GetVisionBase(T1_N);
            vb.m_ExposureTime = (int)numericUpDown_Add.Value;
            vb.SaveParam();
            
            vb = VisionMgr.GetInstance().GetVisionBase("T1_calib");
            vb.m_ExposureTime = (int)numericUpDown_T1_calib.Value;
            vb.SaveParam();


            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string T1_N = "T1_" + (comboBox1.SelectedIndex + 10).ToString();
            VisionBase vb = VisionMgr.GetInstance().GetVisionBase(T1_N);
            numericUpDown_Add.Value = vb.m_ExposureTime;
        }
    }
}
