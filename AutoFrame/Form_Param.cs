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
using CommonTool;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoFrame
{
    public partial class Form_Param : Form
    {
        public Form_Param()
        {
            InitializeComponent();
        }
        private void Form_Param_Load(object sender, EventArgs e)
        {
            SystemMgr.GetInstance().UpdateGridFromParam(dataGridView_AllParam); //从内存加载参数到界面表格
                                                                       //   dataGridView_AllParam.;
            try
            {
                treeviewUpdate();
            }
            catch{}

            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;
        }

        /// <summary>
        /// 更新树型控件
        /// </summary>
        private void treeviewUpdate()
        {
            treeView1.Nodes.Clear();
            string strPath = AppDomain.CurrentDomain.BaseDirectory;//获取当前路径
            TreeNode nodeParent = treeView1.Nodes.Add(strPath);
            GetSystemParamFiles(strPath, nodeParent);
            nodeParent.Expand();
            int n = treeView1.Nodes[0].GetNodeCount(false);
            string strName = Path.GetFileName(SystemMgr.GetInstance().m_strSystemParamName);
            for (int i = 0; i < n; i++)
            {
                if (treeView1.Nodes[0].Nodes[i].Text == /*"systemParam.xml"*/strName)
                {
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[i];
                }
            }
        }

        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnModeChanged()
        {
             if (Security.IsOpMode())
            {
                dataGridView_AllParam.Enabled = false;
                button_update.Enabled = false;
                button_save.Enabled = false;
                button_save_as.Enabled = false;
                roundButtonSetParam.Enabled = false;
            }
            else
            {
                dataGridView_AllParam.Enabled = true;
                button_update.Enabled = true;
                button_save.Enabled = true;
                button_save_as.Enabled = true;
                roundButtonSetParam.Enabled = true;
            }
        }

        /// <summary>
        /// 得到指定路径下系统文件，添加到指定当前路径下的根节点，空节点不添加
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="tn"></param>
        public void GetSystemParamFiles(string strPath, TreeNode tn)
        {
            DirectoryInfo di = new DirectoryInfo(strPath);
            FileInfo[] fi = di.GetFiles("*systemParam*.xml");
            try
            {
                foreach (FileInfo tmpfi in fi)
                {
                    string fileName = tmpfi.Name;
                    fileName = fileName.Substring(fileName.LastIndexOf("\\")+1);
                    TreeNode tnode = new TreeNode(Path.GetFileName(fileName), 0, 0);
                    tn.Nodes.Add(tnode);
                }

                //遍历当前文件夹下所有子文件夹
                List<string> folders = new List<string>(Directory.GetDirectories(strPath));
                folders.ForEach(c =>
                {
                    string childDir = Path.Combine(new string[] { strPath, Path.GetFileName(c) });
                    TreeNode tnode = new TreeNode(Path.GetFileName(Path.GetFileName(c)), 0, 0);
                    tn.Nodes.Add(tnode);
                    GetSystemParamFiles(childDir,tnode);//递归
                    if (tnode.Nodes.Count == 0)
                    {
                        tn.Nodes.Remove(tnode);
                    }
                });
            }
            catch{}
        }

        /// <summary>
        /// 更新参数到内存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_update_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定更新当前参数到内存？", "更新参数", MessageBoxButtons.OKCancel))
            {
                SystemMgr.GetInstance().UpdateParamFromGrid(dataGridView_AllParam);
                SystemMgr.GetInstance().UpdateParamFromTemp();
                MessageBox.Show("参数更新成功");
            }
            //this.Hide();
        }

        /// <summary>
        /// 保存参数到xml文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_save_Click(object sender, EventArgs e)
        {
            string strFileName = SystemMgr.GetInstance().m_strSystemParamName;
            string str = "是否保存文件:" + strFileName;
            if (DialogResult.OK == MessageBox.Show(str, "保存文件", MessageBoxButtons.OKCancel))
            {
                SystemMgr.GetInstance().UpdateParamFromGrid(dataGridView_AllParam);
                int index = SystemMgr.GetInstance().m_strFileDescribe[3].IndexOf(":");
                string strfdn = SystemMgr.GetInstance().m_strFileDescribe[3].Substring(index+1);
                index = SystemMgr.GetInstance().m_strFileDescribe[4].IndexOf(":");
                string strfds = SystemMgr.GetInstance().m_strFileDescribe[4].Substring(index+1);
                if (SystemMgr.GetInstance().SaveParamFile(strFileName, strfdn, strfds, true))
                    MessageBox.Show("参数已更新且文件保存成功,文件名为:"+ strFileName);
            }
            //this.Hide();
        }

        /// <summary>
        /// 更新参数并输出
        /// </summary>
        /// <param name="strOut">输出对象</param>
        /// <param name="obj">输入对象</param>
        private void GetCellParam(out string strOut, object obj)
        {
            if (obj == null)
                strOut = string.Empty;
            else
                strOut = obj.ToString().Trim();
        }

        /// <summary>
        /// 表格单元内容改变触发事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">附带数据的对象</param>
        private void dataGridView_param_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                string strValue, strMin, strMax;
                GetCellParam(out strValue, dataGridView_AllParam.Rows[e.RowIndex].Cells[0].Value);
                GetCellParam(out strMin, dataGridView_AllParam.Rows[e.RowIndex].Cells[3].Value);
                GetCellParam(out strMax, dataGridView_AllParam.Rows[e.RowIndex].Cells[4].Value);

                if (strMin != string.Empty && strMax != string.Empty && strValue != strMax)
                {
                    double value = 0;
                    try
                    {
                        value = Convert.ToDouble(strValue);
                    }
                    catch
                    {
                        dataGridView_AllParam.Rows[e.RowIndex].Cells[0].Value =
                            SystemMgr.GetInstance().m_DicParam.ElementAt(e.RowIndex).Value.m_strValue;//恢复原值
                        return;
                    }
                    double min = Convert.ToDouble(strMin);
                    double max = Convert.ToDouble(strMax);
                    if (value > max || value < min)
                    {
                        MessageBox.Show("参数超过限制值", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dataGridView_AllParam.Rows[e.RowIndex].Cells[0].Value =
                            SystemMgr.GetInstance().m_DicParam.ElementAt(e.RowIndex).Value.m_strValue;//恢复原值
                    }
                }
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_save_as_Click(object sender, EventArgs e)
        {
            Form_ParmSaveAs frm = new Form_ParmSaveAs();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Init();
            frm.ShowDialog();
            if (DialogResult.OK == frm.DialogResult)
            {
                string strDir="", modifier="", fileDescribe="";
                frm.GetParam(ref strDir, ref modifier, ref fileDescribe);
                //保存
                SystemMgr.GetInstance().UpdateParamFromGrid(dataGridView_AllParam);
                SystemMgr.GetInstance().SaveParamFile(strDir,modifier,fileDescribe,true);
                MessageBox.Show("参数文件保存成功");
                treeviewUpdate();
            }
            else if (DialogResult.Cancel == frm.DialogResult)
            {
                //取消
            }
        }

        /// <summary>
        /// 树形控件被选中项触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string strFile = treeView1.SelectedNode.FullPath;
            FileInfo fileInfo = new FileInfo(strFile);
            if  (fileInfo.Exists)
            {
                SystemMgr.GetInstance().m_strSystemParamName = strFile;          
                SystemMgr.GetInstance().LoadParamFileToGrid(strFile, dataGridView_AllParam);
                int nCount = SystemMgr.GetInstance().m_strFileDescribe.Length;
                listView1.Items.Clear();
                for (int i=1; i<nCount; i++)
                {
                    listView1.Items.Add(SystemMgr.GetInstance().m_strFileDescribe[i]);
                }
            }
        }

        /// <summary>
        /// 选中当前文件名作为下次启动的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SetParam_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string strFullPath = treeView1.SelectedNode.FullPath;
                string strPath = Path.GetDirectoryName(strFullPath);
                string strFileName = Path.GetFileName(strFullPath);
                string sDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                if (strPath != sDir)
                {
                    MessageBox.Show("请选择当前目录下文件!");
                    return;
                }
                if (strFileName.Length >= 11)
                {
                    string str = "是否设置当前文件:" + strFileName + "作为参数配置文件 ?";
                    if (DialogResult.OK == MessageBox.Show(str, "选择文件", MessageBoxButtons.OKCancel))
                    {
                        SystemMgr.GetInstance().m_strSystemParamName = strFileName;
                        //ConfigMgr.GetInstance().SaveCfgFile("SystemCfg.xml");
                        SystemMgr.GetInstance().AppendSystemParamName("SystemCfg.xml");
                        MessageBox.Show("参数文件保存成功");
                        treeviewUpdate();
                    }
                }
                else
                {
                    MessageBox.Show("参数文件名长度不够");
                    return;
                }
            }
        }
    }
}
