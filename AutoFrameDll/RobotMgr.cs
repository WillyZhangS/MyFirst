using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using CommonTool;

namespace AutoFrameDll
{
    /// <summary>
    /// 机器人管理类
    /// </summary>
    public class RobotMgr : SingletonTemplate<RobotMgr>
    {
        /// <summary>
        /// IO表格列的项目名称
        /// </summary>
        private string[] m_strRobotIoItem = { "机器人序号", "机器人名字", "IO", "功能描述", "卡序号", "点序号", "有效电平" };
        /// <summary>
        /// 机器人表格列的项目名称
        /// </summary>
        private string[] m_strRobotItem = {"机器人序号", "机器人名字", "通讯方式", "索引号" };
        /// <summary>
        /// IO结构体
        /// </summary>
        public struct IoSysDefine
        {
            /// <summary>
            /// 输入:true  输出:false
            /// </summary>
            public bool bIsIn; 
            /// <summary>
            /// IO功能描述
            /// </summary>
            public string strName;
            /// <summary>
            /// 卡号
            /// </summary>
            public int nCard;
            /// <summary>
            /// 位号
            /// </summary>
            public int nBit;
            /// <summary>
            /// 电平高低, 0;1
            /// </summary>
            public int nLev;
        }

        /// <summary>
        /// 机器人类
        /// </summary>
        public class RobotDefine
        {
            /// <summary>
            /// 机器人序号
            /// </summary>
            public int nIndex; 
            /// <summary>
            /// 机器人名字
            /// </summary>
            public string strName; 
            /// <summary>
            /// 通讯方式，0网口 1串口
            /// </summary>
            public int nLinkMode; 
            /// <summary>
            /// 索引号码
            /// </summary>
            public int nPort;
            /// <summary>
            /// 常用命令列表
            /// </summary>
            public List<string> m_listStrCmd = new List<string>(); 
            /// <summary>
            /// 常用输入IO
            /// </summary>
            public List<IoSysDefine> m_listIoSystemIn = new List<IoSysDefine>(); 
            /// <summary>
            /// 常用输出IO
            /// </summary>
            public List<IoSysDefine> m_listIoSystemOut = new List<IoSysDefine>();
        }
        /// <summary>
        /// 机器人链表
        /// </summary>
        public List<RobotDefine> m_listRobot = new List<RobotDefine>();

        /// <summary>
        /// 从xml文件中加载系统机器人参数
        /// </summary>
        /// <param name="doc"></param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            m_listRobot.Clear();
            XmlNode xnSgl = doc.SelectSingleNode("/SystemCfg/" + "Robot");
            if (xnSgl != null)
            {
                XmlNodeList xnl = xnSgl.ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        string robotName = xn.Name; //机器人名字
                        string strIndex = xn.Attributes[m_strRobotItem[0]].Value;
                        string strName = xn.Attributes[m_strRobotItem[1]].Value;
                        string strLinkMode = xn.Attributes[m_strRobotItem[2]].Value;
                        string strPort = xn.Attributes[m_strRobotItem[3]].Value;
                        RobotDefine robotDef = new RobotDefine();
                        robotDef.nIndex = Convert.ToInt32(strIndex); //序号
                        robotDef.strName = strName;  //机器人名字
                        robotDef.nLinkMode = Convert.ToInt32(strLinkMode); //通讯方式
                        robotDef.nPort = Convert.ToInt32(strPort);   //索引号码

                        XmlNode xnd = xn.SelectSingleNode("IoIn"); //定位输入IO
                        if (xnd != null)
                        {
                            XmlNodeList xnlList = xnd.ChildNodes;  //输入IO元素下面的子节点
                            if (xnlList.Count > 0)
                            {
                                foreach (XmlNode xnIn1 in xnlList)  //遍历IO元素下的子节点
                                {
                                    XmlElement xe = (XmlElement)xnIn1;

                                    IoSysDefine ioIn = new IoSysDefine();
                                    int j = 2;
                                    string strIo = xe.GetAttribute(m_strRobotIoItem[j++]);//输入还是输出
                                    if (strIo == "输入")
                                        ioIn.bIsIn = true;
                                    else
                                        ioIn.bIsIn = false;
                                    ioIn.strName = xe.GetAttribute(m_strRobotIoItem[j++]); //功能描述
                                    ioIn.nCard = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//卡序号
                                    ioIn.nBit = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//点序号
                                    ioIn.nLev = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//有效电平
                                    robotDef.m_listIoSystemIn.Add(ioIn);
                                }
                            }
                        }

                        xnd = xn.SelectSingleNode("IoOut"); //定位输出IO
                        if (xnd != null)
                        {
                            XmlNodeList xnlList = xnd.ChildNodes;  //输出IO元素下面的子节点
                            if (xnlList.Count > 0)
                            {
                                foreach (XmlNode xnOut1 in xnlList)  //遍历IO元素下的子节点
                                {
                                    XmlElement xe = (XmlElement)xnOut1;

                                    IoSysDefine ioOut = new IoSysDefine();
                                    int j = 2;
                                    string strIo = xe.GetAttribute(m_strRobotIoItem[j++]);//输入还是输出
                                    if (strIo == "输入")
                                        ioOut.bIsIn = true;
                                    else
                                        ioOut.bIsIn = false;
                                    ioOut.strName = xe.GetAttribute(m_strRobotIoItem[j++]); //功能描述
                                    ioOut.nCard = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//卡序号
                                    ioOut.nBit = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//点序号
                                    ioOut.nLev = Convert.ToInt32(xe.GetAttribute(m_strRobotIoItem[j++]));//有效电平
                                    robotDef.m_listIoSystemOut.Add(ioOut);
                                }
                            }
                        }

                        xnd = xn.SelectSingleNode("Cmd"); //定位Cmd
                        if (xnd != null)
                        {
                            XmlNodeList xnlList = xnd.ChildNodes;  //输出Cmd元素下面的子节点
                            if (xnlList.Count > 0)
                            {
                                foreach (XmlNode xnCmd1 in xnlList)  //遍历Cmd元素下的子节点
                                {
                                    XmlElement xe = (XmlElement)xnCmd1;
                                    robotDef.m_listStrCmd.Add(xe.GetAttribute("命令"));
                                }
                            }
                        }

                        m_listRobot.Add(robotDef);
                    }
                }
            }
        }

        /// <summary>
        /// 更新系统机器人参数到表格
        /// </summary>
        /// <param name="gridRobot">机器人表格</param>
        /// <param name="gridRobotIoCmd">IO表格</param>
        /// <param name="listBoxCmd">命令列表框</param>
        /// <param name="n">当前选中机器人序号</param>
        /// <param name="bRefresh">是否刷新机器人列表</param>
        public void UpdateGridFromParam(DataGridView gridRobot, DataGridView gridRobotIoCmd, ListBox listBoxCmd, int n=1, bool bRefresh = true)
        {
            //gridRobot.Rows.Clear();
            gridRobotIoCmd.Rows.Clear();
            listBoxCmd.Items.Clear();
            if (m_listRobot.Count > 0 && n>0 && m_listRobot.Count>=n)
            {
                int i = 0, j = 0;
                if (bRefresh)
                {
                    gridRobot.Rows.Clear();
                    gridRobot.Rows.AddCopies(0, m_listRobot.Count);
                    foreach (RobotDefine t in m_listRobot)
                    {
                        j = 0;
                        gridRobot.Rows[i].Cells[j++].Value = t.nIndex.ToString();
                        gridRobot.Rows[i].Cells[j++].Value = t.strName;
                        if (0==t.nLinkMode)
                            gridRobot.Rows[i].Cells[j++].Value = "使用网口";
                        else
                            gridRobot.Rows[i].Cells[j++].Value = "使用串口";
                        gridRobot.Rows[i].Cells[j++].Value = t.nPort.ToString();
                        i++;
                    }
                }

                RobotDefine elFirst = m_listRobot.ElementAt(n-1);//选中的元素
                i = 0;
                int nIoIn = elFirst.m_listIoSystemIn.Count;
                int nIoOut = elFirst.m_listIoSystemOut.Count;
                if (nIoIn+nIoOut > 0)
                {
                    gridRobotIoCmd.Rows.AddCopies(0, nIoIn+nIoOut);
                    for (int h=0; h<nIoIn; h++)
                    {
                        j = 0;
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.nIndex.ToString();//索引
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.strName;  //机器人名字
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = "输入";   //IO方式
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemIn[h].strName;   //功能描述
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemIn[h].nCard.ToString(); ;   //卡号
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemIn[h].nBit.ToString();   //位号
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemIn[h].nLev.ToString();   //电平高低
                        i++;
                    }
                    for (int h=0; h<nIoOut; h++)
                    {
                        j = 0;
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.nIndex.ToString();//索引
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.strName;  //机器人名字
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = "输出";   //IO方式
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemOut[h].strName;   //功能描述
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemOut[h].nCard.ToString(); ;   //卡号
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemOut[h].nBit.ToString();   //位号
                        gridRobotIoCmd.Rows[i].Cells[j++].Value = elFirst.m_listIoSystemOut[h].nLev.ToString();   //电平高低
                        i++;
                    }
                }

                int nCmd = elFirst.m_listStrCmd.Count;
                for (int h = 0; h < nCmd; h++)
                {
                    listBoxCmd.Items.Add(elFirst.m_listStrCmd[h]);
                }
            }
        }

        /// <summary>
        /// 更新系统机器人参数
        /// 第一步：更新机器人表格中的数据到m_listRobot,如果新增的则添加。
        /// 第二步：找到io表格第1行对应机器人名字与机器人表格中匹配的项
        /// 第三步：用io表格第1行机对应机器人名字查找在m_listRobot中的索引号
        /// 第四步：更新m_listRobot中对应IO和命令
        /// </summary>
        /// <param name="gridRobot">机器人表格</param>
        /// <param name="gridRobotIoCmd">IO表格</param>
        /// <param name="listBoxCmd">命令列表</param>
        /// <param name="n">机器人索引</param>
        public void UpdataParamFromGrid(DataGridView gridRobot, DataGridView gridRobotIoCmd, ListBox listBoxCmd, int n)
        {
            int m = gridRobot.RowCount;
            List<string> listStrName = new List<string>();
            //第一步：更新机器人表格中的数据到m_listRobot,如果有新增的则添加。
            for (int i=0; i< m; i++)
            {
                if (gridRobot.Rows[i].Cells[0].Value != null && gridRobot.Rows[i].Cells[1].Value != null
                    && gridRobot.Rows[i].Cells[2].Value != null && gridRobot.Rows[i].Cells[3].Value != null)
                {
                    string strRbtName = gridRobot.Rows[i].Cells[1].Value.ToString();
                    listStrName.Add(strRbtName);
                    int nRbtCount = m_listRobot.Count;
                    int nRbtIndex = -1;
                    if (nRbtCount > 0)
                    {
                        for (int j = 0; j < nRbtCount; j++)
                        {
                            if (strRbtName == m_listRobot[j].strName)
                            {
                                nRbtIndex = j;
                                break;
                            }
                            if (j == nRbtCount - 1)
                            {
                                if (i+1 <= nRbtCount)  //修改
                                {
                                    m_listRobot[i].strName = strRbtName;
                                    m_listRobot[i].m_listIoSystemIn.Clear();
                                    m_listRobot[i].m_listIoSystemOut.Clear();
                                    m_listRobot[i].m_listStrCmd.Clear();
                                    nRbtIndex = i;
                                }
                                else //添加
                                {
                                    RobotDefine rbtItem = new RobotDefine();
                                    m_listRobot.Add(rbtItem);
                                    nRbtIndex = j + 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        RobotDefine rbtItem = new RobotDefine();
                        m_listRobot.Add(rbtItem);
                        nRbtIndex = 0;
                    }
                    m_listRobot[nRbtIndex].nIndex = Convert.ToInt32(gridRobot.Rows[i].Cells[0].Value.ToString());
                    m_listRobot[nRbtIndex].strName = gridRobot.Rows[i].Cells[1].Value.ToString();
                    DataGridViewComboBoxCell dataComboCel = (DataGridViewComboBoxCell)gridRobot.Rows[i].Cells[2];
                    m_listRobot[nRbtIndex].nLinkMode = dataComboCel.Items.IndexOf(dataComboCel.Value.ToString());
                    m_listRobot[nRbtIndex].nPort = Convert.ToInt32(gridRobot.Rows[i].Cells[3].Value.ToString());
                }
                else
                {
                    if (m_listRobot.Count > i)
                        m_listRobot.RemoveAt(i);
                }
            }
            if (listStrName.Count == 0)
            {
                m_listRobot.Clear();
                return;
            }

            //第二步：找到io表格第1行机器人名字与机器人表格中匹配的项
            int nGridIndex = -1;
            string strRobotName = "";
            for (int i=0; i< m; i++)
            {
                if (gridRobotIoCmd.Rows[0].Cells[1].Value == null || gridRobotIoCmd.Rows[0].Cells[2].Value == null
                       || gridRobotIoCmd.Rows[0].Cells[3].Value == null || gridRobotIoCmd.Rows[0].Cells[4].Value == null
                       || gridRobotIoCmd.Rows[0].Cells[5].Value == null || gridRobotIoCmd.Rows[0].Cells[6].Value == null)
                    break;
                if (gridRobot.Rows[i].Cells[0].Value == null || gridRobot.Rows[i].Cells[1].Value == null
                    || gridRobot.Rows[i].Cells[2].Value == null || gridRobot.Rows[i].Cells[3].Value == null)
                    continue;
                strRobotName = gridRobot.Rows[i].Cells[1].Value.ToString();
                if (gridRobotIoCmd.Rows[0].Cells[1].Value.ToString() == strRobotName)
                {
                    nGridIndex = i; //找到对应机器人
                    break;
                }
            }

            //第三步:用io表格第1行机器人名字查找在m_listRobot中的索引号
            int nRobotIndex = -1;
            if (nGridIndex > -1)
            {
                int nList = m_listRobot.Count;
                for (int i=0; i<nList; i++)
                {
                    if (m_listRobot[i].strName == strRobotName)
                    {
                        nRobotIndex = i;
                        break;
                    }
                }
            }

            //第四步：更新m_listRobot中对应IO和命令
            if (nRobotIndex > -1)
            {
                //找到对应机器人先清空io和命令
                m_listRobot[nRobotIndex].m_listIoSystemIn.Clear();
                m_listRobot[nRobotIndex].m_listIoSystemOut.Clear();
                m_listRobot[nRobotIndex].m_listStrCmd.Clear();

                if (gridRobot.Rows[nGridIndex].Cells[0].Value != null || gridRobot.Rows[nGridIndex].Cells[1].Value != null
                    || gridRobot.Rows[nGridIndex].Cells[2].Value != null || gridRobot.Rows[nGridIndex].Cells[3].Value != null)
                {
                    m_listRobot[nRobotIndex].nIndex = Convert.ToInt32(gridRobot.Rows[nGridIndex].Cells[0].Value.ToString());
                    m_listRobot[nRobotIndex].strName = gridRobot.Rows[nGridIndex].Cells[1].Value.ToString();
                    DataGridViewComboBoxCell dataComboCel = (DataGridViewComboBoxCell)gridRobot.Rows[nGridIndex].Cells[2];
                    m_listRobot[nRobotIndex].nLinkMode = dataComboCel.Items.IndexOf(dataComboCel.Value.ToString());
                    m_listRobot[nRobotIndex].nPort = Convert.ToInt32(gridRobot.Rows[nGridIndex].Cells[3].Value.ToString());

                    int nIo = gridRobotIoCmd.RowCount;
                    for (int i = 0; i < nIo; i++)
                    {
                        if (/*gridRobotIoCmd.Rows[i].Cells[1].Value == null || */gridRobotIoCmd.Rows[i].Cells[2].Value == null
                            || gridRobotIoCmd.Rows[i].Cells[3].Value == null || gridRobotIoCmd.Rows[i].Cells[4].Value == null
                            || gridRobotIoCmd.Rows[i].Cells[5].Value == null || gridRobotIoCmd.Rows[i].Cells[6].Value == null)
                            break;
                        IoSysDefine ioSysDef = new IoSysDefine();
                        string strIo = gridRobotIoCmd.Rows[i].Cells[2].Value.ToString();
                        ioSysDef.strName = gridRobotIoCmd.Rows[i].Cells[3].Value.ToString();
                        ioSysDef.nCard = Convert.ToInt32(gridRobotIoCmd.Rows[i].Cells[4].Value.ToString());
                        ioSysDef.nBit = Convert.ToInt32(gridRobotIoCmd.Rows[i].Cells[5].Value.ToString());
                        ioSysDef.nLev = Convert.ToInt32(gridRobotIoCmd.Rows[i].Cells[6].Value.ToString());
                        if (strIo == "输入")
                        {
                            ioSysDef.bIsIn = true;
                            m_listRobot[nRobotIndex].m_listIoSystemIn.Add(ioSysDef);
                        }
                        else
                        {
                            ioSysDef.bIsIn = false;
                            m_listRobot[nRobotIndex].m_listIoSystemOut.Add(ioSysDef);
                        }
                    }

                    foreach (var item in listBoxCmd.Items)
                    {
                        m_listRobot[nRobotIndex].m_listStrCmd.Add(item.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 保存系统机器人参数到xml
        /// </summary>
        /// <param name="doc"></param>
        public void  SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("Robot");
            xnl.AppendChild(root);

            foreach (RobotDefine t in m_listRobot)
            {
                XmlElement xe = doc.CreateElement(t.strName); //创建一个以机器人名字为name的元素,在这个元素中添加属性
                int j = 0;                                   
                xe.SetAttribute(m_strRobotItem[j++], t.nIndex.ToString()); 
                xe.SetAttribute(m_strRobotItem[j++], t.strName);
                xe.SetAttribute(m_strRobotItem[j++], t.nLinkMode.ToString());
                xe.SetAttribute(m_strRobotItem[j++], t.nPort.ToString());
                XmlElement xnIoIn = doc.CreateElement("IoIn");
                if (t.m_listIoSystemIn.Count > 0)
                {
                    foreach (IoSysDefine ioIn in t.m_listIoSystemIn)
                    {
                        XmlElement xnIoInChild = doc.CreateElement("IoIn");
                        j = 2;
                        if (ioIn.bIsIn)
                            xnIoInChild.SetAttribute(m_strRobotIoItem[j++], "输入");
                        else
                            xnIoInChild.SetAttribute(m_strRobotIoItem[j++], "输出");
                        xnIoInChild.SetAttribute(m_strRobotIoItem[j++], ioIn.strName);
                        xnIoInChild.SetAttribute(m_strRobotIoItem[j++], ioIn.nCard.ToString());
                        xnIoInChild.SetAttribute(m_strRobotIoItem[j++], ioIn.nBit.ToString());
                        xnIoInChild.SetAttribute(m_strRobotIoItem[j++], ioIn.nLev.ToString());
                        xnIoIn.AppendChild(xnIoInChild);
                    }
                }
                xe.AppendChild(xnIoIn);

                XmlElement xnIoOut = doc.CreateElement("IoOut");
                if (t.m_listIoSystemOut.Count > 0)
                {
                    foreach (IoSysDefine ioOut in t.m_listIoSystemOut)
                    {
                        XmlElement xnIoOutChild = doc.CreateElement("IoOut");
                        j = 2;
                        if (ioOut.bIsIn)
                            xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], "输入");
                        else
                            xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], "输出");
                        xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], ioOut.strName);
                        xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], ioOut.nCard.ToString());
                        xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], ioOut.nBit.ToString());
                        xnIoOutChild.SetAttribute(m_strRobotIoItem[j++], ioOut.nLev.ToString());
                        xnIoOut.AppendChild(xnIoOutChild);
                    }
                }
                xe.AppendChild(xnIoOut);

                XmlElement xnCmd = doc.CreateElement("Cmd");
                if (t.m_listStrCmd.Count > 0)
                {
                    foreach (string strCmd in t.m_listStrCmd)
                    {
                        XmlElement xnCmdChild = doc.CreateElement("Cmd");
                        xnCmdChild.SetAttribute("命令", strCmd);
                        xnCmd.AppendChild(xnCmdChild);
                    }
                }
                xe.AppendChild(xnCmd);

                root.AppendChild(xe);
            }
        }
    }
}
