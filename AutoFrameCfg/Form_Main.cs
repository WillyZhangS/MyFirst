using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CommonTool;
using Communicate;
using AutoFrameDll;

namespace AutoFrameCfg
{
    public partial class Form_Main : Form
    {

        string m_strCfgFile = "SystemCfg.xml";
        string m_strParamFile = "SystemParam.xml";
        string m_strPointFile = "Point.xml";
        const string m_strRobotFile = "Robot.xml";
        int m_nSelectSta = -1;//当前选中站位,-1为选中所有
        int m_nSelectRobot = 1;//当前选中机器人
        public Form_Main()
        {
            InitializeComponent();
        }
  
        //加载所有系统配置文件
        void LoadSystemCfg(string strFile)
        {
            ConfigMgr.GetInstance().LoadCfgFile(strFile);

            IoMgr.GetInstance().UpdateGridFromParam(dataGridView_IoCard, dataGridView_IoIn, dataGridView_IoOut, 
                dataGridView_SysteimIO, dataGridView_SysteimIOOut);
            MotionMgr.GetInstance().UpdateGridFromParam(dataGridView_Motion);
            StationMgr.GetInstance().UpdateGridFromParam(dataGridView_Station);

            TcpMgr.GetInstance().UpdateGridFromParam(dataGridView_Eth);
            ComMgr.GetInstance().UpdateGridFromParam(dataGridView_Com);
            RobotMgr.GetInstance().UpdateGridFromParam(dataGridView_robot, dataGridView_robot_io_cmd,listBox_cmd);
        }

        //保存所有系统配置文件
        void SaveSystemCfg(string strFile)
        {
            IoMgr.GetInstance().UpdateParamFromGrid(dataGridView_IoCard, dataGridView_IoIn,dataGridView_IoOut, 
                dataGridView_SysteimIO, dataGridView_SysteimIOOut);
            MotionMgr.GetInstance().UpdateParamFromGrid(dataGridView_Motion);
            StationMgr.GetInstance().UpdateParamFromGrid(dataGridView_Station);

            TcpMgr.GetInstance().UpdateParamFromGrid(dataGridView_Eth);
            ComMgr.GetInstance().UpdateParamFromGrid(dataGridView_Com);
            RobotMgr.GetInstance().UpdataParamFromGrid(dataGridView_robot,dataGridView_robot_io_cmd,listBox_cmd,m_nSelectRobot);

            ConfigMgr.GetInstance().SaveCfgFile(strFile);
       }

        //加载系统配置参数
        void LoadSystemParam(string strFile)
        {
            SystemMgr.GetInstance().LoadParamFile(strFile);
            SystemMgr.GetInstance().UpdateGridFromParam(dataGridView_param, true);
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            LoadSystemParam(m_strParamFile);
            LoadSystemCfg(m_strCfgFile);
            StationMgr.GetInstance().LoadPointFile(m_strPointFile);
            StationMgr.GetInstance().UpdatePointGrid(dataGridView_point);
        }

        //加载系统配置文件
        private void button_load_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadSystemCfg(openFileDialog1.FileName);
                    StationMgr.GetInstance().LoadPointFile(m_strPointFile);
                    StationMgr.GetInstance().UpdatePointGrid(dataGridView_point);
                    m_strCfgFile = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "系统配置文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //保存系统配置文件
        private void button_save_Click(object sender, EventArgs e)
        {
            SaveSystemCfg(m_strCfgFile);
        }

        //保存系统配置文件到指定目录
        private void button_save_as_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveSystemCfg(openFileDialog1.FileName);
            }
        }

        //加载系统配置参数
        private void button_load_system_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadSystemParam(openFileDialog1.FileName);
                    m_strParamFile = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "系统参数文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //保存系统配置参数
        private void button_save_system_Click(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().UpdateParamFromGrid(dataGridView_param, true);
            SystemMgr.GetInstance().SaveParamFile(m_strParamFile);
        }

        //保存系统配置参数到指定目录
        private void button_save_as_system_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SystemMgr.GetInstance().UpdateParamFromGrid(dataGridView_param, true);
                SystemMgr.GetInstance().SaveParamFile(openFileDialog1.FileName);
            }
        }

        //退出
        private void button_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //更新所有工站运动点位到表格
        private void button_point_all_Click(object sender, EventArgs e)
        {
            StationMgr.GetInstance().UpdatePointGrid(dataGridView_point);
            m_nSelectSta = -1;
        }

        //更新选中工站运动点位到表格
        private void button_point_sta_Click(object sender, EventArgs e)
        {
            if(dataGridView_Station.CurrentRow.Cells[0].Value != null)
            {
                int n = Convert.ToInt32( dataGridView_Station.CurrentRow.Cells[0].Value.ToString());
                StationMgr.GetInstance().UpdatePointGrid(dataGridView_point, n);
                m_nSelectSta = n;

            }

        }

        //加载指定的点位文件
        private void button_load_point_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
         //       try
               // {
                    StationMgr.GetInstance().LoadPointFile(openFileDialog1.FileName);
                    StationMgr.GetInstance().UpdatePointGrid(dataGridView_point);
                m_strPointFile = openFileDialog1.FileName;
       //         }
                //catch (Exception ex)
                //{
                //    MessageBox.Show("系统点位文件读取失败 " + ex.Message);

                //}
            }
        }

        //保存点位
        private void button_save_point_Click(object sender, EventArgs e)
        {
            StationMgr.GetInstance().UpdatePoint(dataGridView_point, m_nSelectSta);
            StationMgr.GetInstance().SavePointFile(m_strPointFile);
        }

        //保存点位到指定文件
        private void button_save_as_point_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StationMgr.GetInstance().UpdatePoint(dataGridView_point, m_nSelectSta);
                    StationMgr.GetInstance().SavePointFile(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "系统点位文件存储失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 增加一条机器人常用命令到列表框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_addRobotCmd_Click(object sender, EventArgs e)
        {
            int nIndex = listBox_cmd.SelectedIndex;
            if (textBox_addRobotCmd.Text.Length > 0)
            {
                if (nIndex >= 0)
                    listBox_cmd.Items.Insert(nIndex + 1, textBox_addRobotCmd.Text);
                else
                    listBox_cmd.Items.Add(textBox_addRobotCmd.Text);
            }
        }

        /// <summary>
        /// 删除选中行一条命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delRobotCmd_Click(object sender, EventArgs e)
        {
            int nIndex = listBox_cmd.SelectedIndex;
            if (nIndex>=0)
                listBox_cmd.Items.RemoveAt(nIndex);
        }

        /// <summary>
        /// 删除选中行所有命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delAllRobotCmd_Click(object sender, EventArgs e)
        {
            while (listBox_cmd.Items.Count>0)
                listBox_cmd.Items.RemoveAt(0);
        }

        /// <summary>
        /// 按机器人列表中索引显示当前信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_robot_current_Click(object sender, EventArgs e)
        {
            if (dataGridView_robot.CurrentRow != null)
            {
                if (dataGridView_robot.CurrentRow.Cells[0].Value != null)
                {
                    int n = Convert.ToInt32(dataGridView_robot.CurrentRow.Cells[0].Value.ToString());
                    if (n>0)
                    {
                        //RobotMgr.GetInstance().UpdateRobotGrid(dataGridView_robot_io_cmd, listBox_cmd, n);
                        RobotMgr.GetInstance().UpdateGridFromParam(dataGridView_robot,dataGridView_robot_io_cmd, listBox_cmd, n,false);
                        m_nSelectRobot = n;
                    }
                }
            }
        }

        /// <summary>
        /// 加载机器人配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_load_robot_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //RobotMgr.GetInstance().LoadRobotFile(openFileDialog1.FileName);
                    //RobotMgr.GetInstance().UpdateRobotGrid(dataGridView_robot_io_cmd,listBox_cmd, m_nSelectRobot);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "系统机器人文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 保存机器人配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_save_robot_Click(object sender, EventArgs e)
        {
            try
            {
                //RobotMgr.GetInstance().UpdateRobotParam(dataGridView_robot_io_cmd, listBox_cmd, m_nSelectRobot);
                //RobotMgr.GetInstance().SaveRobotFile(m_strRobotFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "系统机器人文件存储失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 另存为,保存机器人配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_robot_save_as_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //RobotMgr.GetInstance().UpdateRobotParam(dataGridView_robot_io_cmd, listBox_cmd, m_nSelectRobot);
                    //RobotMgr.GetInstance().SaveRobotFile(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "系统机器人文件存储失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 单元格内容被单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView_robot.CurrentRow != null)
            {
                if (dataGridView_robot.CurrentRow.Cells[0].Value != null)
                {
                    int n = Convert.ToInt32(dataGridView_robot.CurrentRow.Cells[0].Value.ToString());
                    if (n > 0)
                    {
                        //RobotMgr.GetInstance().UpdateRobotGrid(dataGridView_robot_io_cmd, listBox_cmd, n);
                        RobotMgr.GetInstance().UpdateGridFromParam(dataGridView_robot, dataGridView_robot_io_cmd, listBox_cmd, n, false);
                        m_nSelectRobot = n;
                    }
                }
            }
        }
    }
}
