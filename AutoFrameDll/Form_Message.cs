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

namespace AutoFrameDll
{
    /// <summary>
    /// 线程专用消息对话框
    /// </summary>
    public partial class Form_Message : Form
    {
        /// <summary>
        /// 
        /// </summary>
        StationBase m_station = null;
        int m_nTimeRemain = 20;
        /// <summary>
        /// 线程专用消息对话框
        /// </summary>
        ///        
        public Form_Message(StationBase sb)
        {
            InitializeComponent();
            m_station = sb;
            m_nTimeRemain = SystemMgr.GetInstance().GetParamInt("MessageTimeOut");
        }
        //todo , 10s不够，需要根据调试或运行情况动态调整
     //   int m_nTimeRemain = 20;
        /// <summary>
        /// 显示超时对话框
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="Title"></param>
        /// <param name="btns"></param>
        /// <returns></returns>
        public DialogResult MessageShow(string strText, string Title, MessageBoxButtons btns)
        {
            this.Text = Title;
            this.label_Info.Text = strText;
            if(btns == MessageBoxButtons.YesNoCancel)
            {
                button_cancel.Visible = false;
                button_cancel.Enabled = false;

                button_yes.Location = button_no.Location;
                button_no.Location = button_cancel.Location;
                button_cancel.Location = new Point(0, 0);

            }
            //this.TopMost = true;
            //this.Focused = true;
            return ShowDialog();
        }



        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void button_yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void button_cancel_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void timer1_Tick(object sender, EventArgs e) 
        {
            if(m_station != null)
            {
                m_station.CheckContinue();
            }
            if (--m_nTimeRemain == 0)
            {
                this.DialogResult = DialogResult.Yes;
            }
            else
            {
                textBox_time.Text = m_nTimeRemain.ToString();
            }

        }

        private void Form_Message_Load(object sender, EventArgs e)
        {
            this.Activate();
            textBox_time.Text = m_nTimeRemain.ToString();
        }

        private int GetMessageTimeOut()
        {
            int n = SystemMgr.GetInstance().GetParamInt("MessageTimeOut");
            if (n == 0)
                return 20;
            else
                return n;
        }
    }

}
