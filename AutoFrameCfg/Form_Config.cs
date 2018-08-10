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
using AutoFrameDll;



namespace AutoFrameCfg
{
    public partial class Form_Config : Form
    {
        string m_strFileName = "SystemCfg.xml";
        private static Form_Config m_instance = null;
        public static Form_Config GetInstance()
        {
            if (m_instance == null || m_instance.IsDisposed)
            {
                m_instance = new Form_Config();
            }
            return m_instance;
        }

        private Form_Config()
        {
            InitializeComponent();
        }


        private void UpdateGridFromCfg(XmlDocument doc, string strKey, DataGridView grid, string[] strType)
        {
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/"+ strKey);
            if(xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if(xnl.Count > 0)
                {
                    grid.Rows.Clear();
                    grid.Rows.AddCopies(0, xnl.Count);
                    int i = 0;
                    foreach(XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;
                        for(int n = 0; n < strType.Length; ++n)
                            grid.Rows[i].Cells[n].Value = xe.GetAttribute(strType[n]);
                        ++i;
                    }
                }
            }
        }
        private void SaveCfgFromGrid(XmlDocument doc, string strKey, DataGridView grid, string[] strType)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");


            XmlNode root = doc.CreateElement(strKey);
            xnl.AppendChild(root);

            int m = grid.RowCount;
            int n = grid.ColumnCount;

            for(int i=0; i<m; ++i)
            {
                if (grid.Rows[i].Cells[0].Value == null)
                    break;
                XmlElement xe = doc.CreateElement(strKey);
                for (int j = 0; j < n; ++j)
                    xe.SetAttribute(strType[j], grid.Rows[i].Cells[j].Value.ToString());
                root.AppendChild(xe);
            }
          
        }


        private void ReadAllConfig(string strFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(strFile);

            if (doc.HasChildNodes)
            {
                ConfigMgr mgr = ConfigMgr.GetInstance();

                UpdateGridFromCfg(doc, "Motion", dataGridView_Motion, ConfigMgr.GetInstance().m_strMotionCard);
                UpdateGridFromCfg(doc, "IoCard", dataGridView_IoCard, ConfigMgr.m_strIoCard);
                UpdateGridFromCfg(doc, "IoIn", dataGridView_IoIn, ConfigMgr.m_strIoIn);
                UpdateGridFromCfg(doc, "IoOut", dataGridView_IoOut, ConfigMgr.m_strIoOut);
                UpdateGridFromCfg(doc, "Station", dataGridView_Station, ConfigMgr.m_strStation);
                UpdateGridFromCfg(doc, "Com", dataGridView_Com, ConfigMgr.m_strCom);
                UpdateGridFromCfg(doc, "Eth",  dataGridView_Eth, ConfigMgr.m_strEth);

            }
            else
            {
                MessageBox.Show("打开配置文件失败，请确认文件是否正确？");

            }
        }

        private void WrtieAllConfig(string strFile)
        {
            XmlDocument doc = new XmlDocument();
          
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement("SystemCfg");
            doc.AppendChild(root);
            if (doc.HasChildNodes)
            {
                SaveCfgFromGrid(doc, "Motion", dataGridView_Motion,  ConfigMgr.GetInstance().m_strMotionCard);
                SaveCfgFromGrid(doc, "IoCard", dataGridView_IoCard,  ConfigMgr.m_strIoCard);
                SaveCfgFromGrid(doc, "IoIn", dataGridView_IoIn,  ConfigMgr.m_strIoIn);
                SaveCfgFromGrid(doc, "IoOut", dataGridView_IoOut,  ConfigMgr.m_strIoOut);
                SaveCfgFromGrid(doc, "Station", dataGridView_Station,  ConfigMgr.m_strStation);
                SaveCfgFromGrid(doc, "Com", dataGridView_Com,  ConfigMgr.m_strCom);
                SaveCfgFromGrid(doc, "Eth", dataGridView_Eth,  ConfigMgr.m_strEth);

                doc.Save(strFile);
            }
            else
            {
                MessageBox.Show("打开配置文件失败，请确认文件是否正确？");

            }
        }

        private void Form_Config_Load(object sender, EventArgs e)
        {
            ReadAllConfig(m_strFileName);

        }

 
        private void button_load_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReadAllConfig(openFileDialog1.FileName);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            WrtieAllConfig(m_strFileName);
        }

        private void button_save_as_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WrtieAllConfig(openFileDialog1.FileName);
            }
        }
    }
}
