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
namespace AutoFrame
{
    public partial class Form_ParmSaveAs : Form
    {
        public Form_ParmSaveAs()
        {
            InitializeComponent();
        }

        public void Init()
        {
            textBox_directory.Text = AppDomain.CurrentDomain.BaseDirectory+ "systemParam.xml";
            textBox_Modifier.Text = "Admin";
            textBox_fileDescribe.Text = "系统配置参数"; 
        }
        
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="strDir">路径</param>
        /// <param name="modifier">修改者</param>
        /// <param name="fileDescribe">文件描述</param>
        public void GetParam(ref string strDir, ref string modifier, ref string fileDescribe)
        {
            strDir = textBox_directory.Text;
            modifier = textBox_Modifier.Text;
            fileDescribe = textBox_fileDescribe.Text;
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            if (textBox_directory.Text == string.Empty)
            {
                MessageBox.Show("请输入文件保存路径");
                return;
            }
            else if (textBox_Modifier.Text == string.Empty)
            {
                MessageBox.Show("请输入文件修改者");
                return;
            }
            else if (textBox_fileDescribe.Text == string.Empty)
            {
                MessageBox.Show("请输入文件描述");
                return;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_saveDir_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FilePath = saveFileDialog1.FileName.ToString(); //获得文件路径 
                string fileNameExt = FilePath.Substring(FilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                textBox_directory.Text = FilePath;
            }
        }

        private void Form_ParmSaveAs_Load(object sender, EventArgs e)
        {
            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;
        }


        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnModeChanged()
        {
         if (Security.IsEngMode())
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
        }
    
    }
}
