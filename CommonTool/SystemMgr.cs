using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using System.IO;

namespace CommonTool
{
   /// <summary>
   /// 系统运行模式设这一，自动运行模式，空跑模式，标定模式，其它自定义模式，
   /// </summary>
   public enum SystemMode
    {
        /// <summary>
        /// 正常运行模式
        /// </summary>
        Normal_Run_Mode,
        /// <summary>
        /// 空跑模式，是否带料由流程决定
        /// </summary>
        Dry_Run_Mode,
        /// <summary>
        /// 自动标定模式
        /// </summary>
        Calib_Run_Mode,
        /// <summary>
        /// 模拟运行模式
        /// </summary>
        Simulate_Run_Mode,
        /// <summary>
        /// 其它自定义模式
        /// </summary>
        Other_Mode,
    }
    /// <summary>
    /// 系统管理类
    /// </summary>
    public class SystemMgr: SingletonTemplate<SystemMgr>
    {
        /// <summary>
        /// 系统参数配置文件表头
        /// </summary>
        private static readonly string[] m_strDescribe = { "键值", "当前值", "单位", "参数描述", "最小值", "最大值" };
        /// <summary>
        /// 系统参数配置文件表头类
        /// </summary>
        public class SystemParam
        {
            /// <summary>
            /// 键值
            /// </summary>
            public string m_strKey = string.Empty;      
            /// <summary>
            /// 当前值
            /// </summary>
            public string m_strValue = string.Empty;    
            /// <summary>
            /// 单位
            /// </summary>
            public string m_strUnit = string.Empty;     
            /// <summary>
            /// 最小值
            /// </summary>
            public string m_strMin = string.Empty;      
            /// <summary>
            /// 最大值
            /// </summary>
            public string m_strMax = string.Empty;      
            /// <summary>
            /// 参数描述
            /// </summary>
            public string m_strDescribe = string.Empty; 
        }
     
       private bool[] m_bitArray;     //系统位变量数组
       private int[] m_intArray;      //系统整型变量数组
       private double[] m_doubleArray;//系统浮点型变量数组
        FileSystemWatcher[] m_fileWatcher = {null,null,null,null }; //文件监视对象
        private int m_nScanTime = 20; //系统扫描周期时间

        /// <summary>
        /// 定义一个字符串变量,保存系统参数配置文件的名字
        /// </summary>
        public string m_strSystemParamName = "systemParam.xml";
        /// <summary>
        /// 定义一个集合变量，保存系统配置文件*SystemParam*.xml的信息
        /// </summary>
        public Dictionary<string, SystemParam> m_DicParam =new Dictionary<string, SystemParam>();
        /// <summary>
        /// 定义一个集合变量，保存系统配置文件*SystemParam*.xml的信息,此变量保存切换文件时的值
        /// </summary>
        private Dictionary<string, SystemParam> m_DicTempParam = new Dictionary<string, SystemParam>();
        /// <summary>
        /// 定义一个系统位寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        public delegate void BitChangedHandler(int nIndex, bool bBit);
        /// <summary>
        /// 系统位寄存器变更事件
        /// </summary>
        public event BitChangedHandler BitChangedEvent;

        /// <summary>
        /// 定义一个系统整型寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="nData"></param>
        public delegate void IntChangedHandler(int nIndex, int nData);
        /// <summary>
        /// 系统整型寄存器变更事件
        /// </summary>
        public event IntChangedHandler IntChangedEvent;

        /// <summary>
        /// 定义一个系统浮点型寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="fData"></param>
        public delegate void DoubleChangedHandler(int nIndex, double fData);
        /// <summary>
        /// 系统浮点型寄存器变更事件
        /// </summary>
        public event DoubleChangedHandler DoubleChangedEvent;

        /// <summary>
        /// 系统配置参数文件头信息
        /// </summary>
        public string[] m_strFileDescribe = {"","","","","","" }; 
        /// <summary>
        /// 文件监控目录下有新文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnCreate(object source, FileSystemEventArgs e);
        /// <summary>
        /// 文件监控目录下有删除文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnDelete(object source, FileSystemEventArgs e);
        /// <summary>
        /// 文件监控目录下有发生文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnChange(object source, FileSystemEventArgs e);
        /// <summary>
        /// 文件监控目录下文件名字有变化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnRename(object source, RenamedEventArgs e);

        private SystemMode _mode = SystemMode.Normal_Run_Mode;


        /// <summary>
        /// 状态变更委托
        /// </summary>
        /// <param name="Mode"></param>
        public delegate void StateChangedHandler(SystemMode Mode);
        /// <summary>
        /// 定义状态变更事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;

        /// <summary>
        /// 当前系统的运行模式
        /// </summary>
        public SystemMode Mode
        {
            get { return _mode; }
        }

        /// <summary>
        /// 改变系统运行模式
        /// </summary>
        /// <param name="Mode"></param>
        public void ChangeMode(SystemMode Mode)
        {
            if(_mode != Mode)
            {
                _mode = Mode;
                if (StateChangedEvent != null)
                {
                    StateChangedEvent(Mode);
                }
            }
        }

        /// <summary>
        /// 判断当前是否属于空跑模式
        /// </summary>
        /// <returns></returns>
        public bool IsDryRunMode()
        {
            return _mode == SystemMode.Dry_Run_Mode;
        }
        /// <summary>
        /// 判断当前是否属于自动标定模式
        /// </summary>
        /// <returns></returns>
        public bool IsAutoCalibMode()
        {
            return _mode == SystemMode.Calib_Run_Mode;
        }
        /// <summary>
        /// 判断当前是否属于模拟运行模式
        /// </summary>
        /// <returns></returns>
        public bool IsSimulateRunMode()
        {
            return _mode == SystemMode.Simulate_Run_Mode;
        }
        /// <summary>
        /// 判断当前是否属于正常运行模式
        /// </summary>
        /// <returns></returns>
        public bool IsNormalRunMode()
        {
            return _mode == SystemMode.Normal_Run_Mode;
        }

        /// <summary>
        /// 构造函数, 初始化内存区
        /// </summary>
        public SystemMgr()
        {
            const int MaxBuffer = 10240;
            m_bitArray = new bool [MaxBuffer];
            m_intArray = new int[MaxBuffer];
            m_doubleArray = new double[MaxBuffer];

            m_bitArray.Initialize();
            m_intArray.Initialize();
            m_doubleArray.Initialize();
        }

        /// <summary>
        /// 获取系统扫描周期时间
        /// </summary>
        public int ScanTime
        {
            get{ return m_nScanTime; }
        }
        void InitParam()
        {
            m_nScanTime = GetParamInt("ScanTime");
            if (m_nScanTime == 0)
                m_nScanTime = 20;
        }

        /// <summary>
        /// 从XML文件中读取配置文件参数,如加载SystemParam.xml文件
        /// </summary>
        /// <param name="strFile">文件路径</param>
        /// <returns></returns>
        public bool LoadParamFile(string strFile)
        {
            Security.LoadPassword();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(strFile);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/SystemParam/" + "Param");
                    if (xnl.Count > 0)
                    {
                        m_DicParam.Clear();
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            foreach (XmlNode xn in xnl)
                            {
                                XmlElement xe = (XmlElement)xn;
                                SystemParam sp = new SystemParam();
                                sp.m_strKey = xe.GetAttribute(m_strDescribe[0]);
                                sp.m_strValue = xe.GetAttribute(m_strDescribe[1]);
                                sp.m_strUnit = xe.GetAttribute(m_strDescribe[2]);
                                sp.m_strDescribe = xe.GetAttribute(m_strDescribe[3]);
                                sp.m_strMin = xe.GetAttribute(m_strDescribe[4]);
                                sp.m_strMax = xe.GetAttribute(m_strDescribe[5]);

                                m_DicParam.Add(sp.m_strKey, sp);
                            }
                        }
                    }
                    
                    xnl = doc.SelectNodes("/SystemParam/" + "ParamDescribe");
                    if (xnl.Count > 0)
                    {
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            foreach (XmlNode xn in xnl)
                            {
                                XmlElement xe = (XmlElement)xn;
                                //m_strFileDescribe[0] = xe.GetAttribute("文件路径");
                                FileInfo fi = new FileInfo(strFile);
                                m_strFileDescribe[0] = string.Format("创建时间:" + fi.CreationTime.ToString());
                                m_strFileDescribe[1] = string.Format("写入时间:" + fi.LastWriteTime);
                                m_strFileDescribe[2] = string.Format("访问时间:" + fi.LastAccessTime);
                                m_strFileDescribe[3] = "修改者:"+xe.GetAttribute("修改者");
                                m_strFileDescribe[4] = "文件描述:" + xe.GetAttribute("文件描述");
                            }
                        }
                    }
                    InitParam();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "系统参数文件SystemParam.xml读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }


        /// <summary>
        /// 加载参数文件到表格
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool LoadParamFileToGrid(string strFile, DataGridView grid)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(strFile);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/SystemParam/" + "Param");
                    if (xnl.Count > 0)
                    {                     
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            int i = 0;
                            grid.Rows.Clear();
                            foreach (XmlNode xn in xnl)
                            {
                                int j = 0;
                                grid.Rows.Add();
                                XmlElement xe = (XmlElement)xn;
                                grid.Rows[i].Cells[j++].Value = xe.GetAttribute(m_strDescribe[1]);
                                grid.Rows[i].Cells[j++].Value = xe.GetAttribute(m_strDescribe[2]);
                                grid.Rows[i].Cells[j++].Value = xe.GetAttribute(m_strDescribe[3]);
                                grid.Rows[i].Cells[j++].Value = xe.GetAttribute(m_strDescribe[4]);
                                grid.Rows[i].Cells[j++].Value = xe.GetAttribute(m_strDescribe[5]);

                                i++;
                            }
                        }
                    }

                    xnl = doc.SelectNodes("/SystemParam/" + "ParamDescribe");
                    if (xnl.Count > 0)
                    {
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            foreach (XmlNode xn in xnl)
                            {
                                XmlElement xe = (XmlElement)xn;
                                //m_strFileDescribe[0] = xe.GetAttribute("文件路径");
                                FileInfo fi = new FileInfo(strFile);
                                m_strFileDescribe[0] = string.Format("创建时间:" + fi.CreationTime.ToString());
                                m_strFileDescribe[1] = string.Format("写入时间:" + fi.LastWriteTime);
                                m_strFileDescribe[2] = string.Format("访问时间:" + fi.LastAccessTime);
                                m_strFileDescribe[3] = "修改者:" + xe.GetAttribute("修改者");
                                m_strFileDescribe[4] = "文件描述:" + xe.GetAttribute("文件描述");
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "系统参数文件SystemParam.xml读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 更新保存配置文件,如更新m_DicParam到SystemParam.xml文件
        /// </summary>
        /// <param name="strFile">文件路径</param>
        /// <param name="strModifier">修改者</param>
        /// <param name="strFileDescribe">文件描述</param>
        /// <param name="bConfigMode">保存模式,取m_DicTempParam,m_DicParam其中之一</param>
        /// <returns></returns>
        public bool SaveParamFile(string strFile,string strModifier="Admin", string strFileDescribe= "系统配置参数",bool bConfigMode=false)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration dec2 = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec2);

                XmlElement root2 = doc.CreateElement("SystemParam");
                doc.AppendChild(root2);
                if (doc.HasChildNodes)
                {
                    XmlNode xn = doc.SelectSingleNode("SystemParam");
                    xn.AppendChild(doc.CreateElement("ParamDescribe"));
                    xn = doc.SelectSingleNode("/SystemParam/ParamDescribe");
                    XmlElement xe1 = doc.CreateElement("ParamDescribe");
                    //xe1.SetAttribute("文件路径", strFile);
                    xe1.SetAttribute("修改者", strModifier);
                    xe1.SetAttribute("文件描述", strFileDescribe);
                    xn.AppendChild(xe1);

                    xn = doc.SelectSingleNode("SystemParam");
                    xn.AppendChild(doc.CreateElement("Param"));
                    xn = doc.SelectSingleNode("/SystemParam/Param");

                    Dictionary<string, SystemParam> dic = new Dictionary<string, SystemParam>();
                    if (bConfigMode) //如果为true,用中间变量,系统运行时不影响运行参数
                        dic = m_DicTempParam;
                    else
                        dic = m_DicParam;
                    foreach (KeyValuePair<string, SystemParam> kvp in dic)
                    {
                        int j = 0;
                        XmlElement xe = doc.CreateElement("Param");

                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strKey);
                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strValue);
                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strUnit);
                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strDescribe);
                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strMin);
                        xe.SetAttribute(m_strDescribe[j++], kvp.Value.m_strMax);
                        xn.AppendChild(xe);
                    }
                }
                doc.Save(strFile);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "保存文件异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            return false;
        }

        /// <summary>
        /// 从系统文件中获取参数配置文件名
        /// </summary>
        /// <param name="strFile">加载文件名</param>
        /// <returns></returns>
        public bool GetSystemParamName(string strFile)
        {
            string strOutFileName = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + strFile);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "SystemParamName");
                    if (xnl == null || xnl.Count == 0)
                        return false;
                    xnl = xnl.Item(0).ChildNodes;
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;
                        if (xn.Name == "SystemParamName")
                        {
                            strOutFileName = xe.GetAttribute("系统参数文件名");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "系统配置文件systemCfg.xml读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (strOutFileName == "")
            {
                return false;
            }
            m_strSystemParamName = strOutFileName;
            return true;
        }

        /// <summary>
        /// 设置保存系统参数文件名
        /// </summary>
        /// <param name="doc"></param>
        public void SaveSystemParamCfgXml(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("SystemParamName");
            xnl.AppendChild(root);
            
            if (m_strSystemParamName == string.Empty)
            {
                m_strSystemParamName = "systemParam.xml";
            }
            XmlElement xe = doc.CreateElement("SystemParamName");
            xe.SetAttribute("系统参数文件名", m_strSystemParamName);
            root.AppendChild(xe);
        }
        
        /// <summary>
        /// 初始化时在systemCfg.xml中没有找到参数配置文件则默认添加systemParam.xml
        /// </summary>
        /// <param name="strFile"></param>
        public void AppendSystemParamName(string strFile)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + strFile);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "SystemParamName");
                    foreach (XmlNode xn in xnl)
                    {
                        xn.ParentNode.RemoveChild(xn);
                    }
                    SaveSystemParamCfgXml(doc);
                    doc.Save(strFile);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "系统配置文件systemCfg.xml读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
    /// 更新参数并输出
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
        private string GetCellString( object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString().Trim();
        }

        /// <summary>
        /// 更新运行参数
        /// </summary>
        public void UpdateParamFromTemp()
        {
            int nCount = m_DicParam.Count;
            int nCountT = m_DicTempParam.Count;
            foreach (KeyValuePair<string, SystemParam> kvp in m_DicParam)
            {
                if (m_DicTempParam.ContainsKey(kvp.Key))
                {
                    kvp.Value.m_strValue = m_DicTempParam[kvp.Key].m_strValue;
                }
            }
        }

        /// <summary>
        /// 更新界面表格内容到变量m_DicParam, 支持增加参数
        /// </summary>
        /// <param name="grid">表格控件关联变量</param>
        /// <param name="bConfigMode">当前是否为配置模式(可增加新参数)</param>
        public void UpdateParamFromGrid(DataGridView grid, bool bConfigMode = false)
        {
            if (bConfigMode)
            {
                int n = grid.Rows.Count;
                m_DicParam.Clear();
                for (int i = 0; i < n; ++i)
                {
                    if (grid.Rows[i].Cells[0].Value == null)
                        continue;
                    int j = 0;
                    SystemParam sp = new SystemParam();
                    sp.m_strKey = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strValue = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strUnit = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strDescribe = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strMin = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strMax = GetCellString(grid.Rows[i].Cells[j++].Value);

                    if(sp.m_strKey != string.Empty)
                         m_DicParam.Add(sp.m_strKey, sp);
                }
            }
            else
            {
                //int i = 0;
                //foreach (KeyValuePair<string, SystemParam> kvp in m_DicParam)
                //{
                //    int j = 0;
                //    kvp.Value.m_strValue = GetCellString(grid.Rows[i].Cells[j++].Value);
                //    kvp.Value.m_strUnit = GetCellString(grid.Rows[i].Cells[j++].Value);
                //    kvp.Value.m_strDescribe = GetCellString(grid.Rows[i].Cells[j++].Value);

                //    kvp.Value.m_strMin = GetCellString(grid.Rows[i].Cells[j++].Value);
                //    kvp.Value.m_strMax = GetCellString(grid.Rows[i].Cells[j++].Value);

                //    i++;
                //}
                int i = 0;
                m_DicTempParam.Clear();
                foreach (KeyValuePair<string, SystemParam> kvp in m_DicParam)
                {
                    int j = 0;
                    SystemParam sp = new SystemParam();
                    sp.m_strKey = kvp.Key;
                    sp.m_strValue = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strUnit = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strDescribe = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strMin = GetCellString(grid.Rows[i].Cells[j++].Value);
                    sp.m_strMax = GetCellString(grid.Rows[i].Cells[j++].Value);
                    m_DicTempParam.Add(sp.m_strKey, sp);
                    i++;
                }
            }
        }

        /// <summary>
        /// 更新变量m_DicParam内容到界面表格
        /// </summary>
        /// <param name="grid">表格控件关联变量</param>
        /// <param name="bConfig">默认为false </param>
        public void UpdateGridFromParam(DataGridView grid, bool bConfig = false)
        {
            int i = 0;
            grid.Rows.Clear();
            grid.Rows.AddCopies(0, m_DicParam.Count);
            foreach (KeyValuePair<string, SystemParam> kvp in m_DicParam)
            {
                int j = 0;
                if(bConfig)
                    grid.Rows[i].Cells[j++].Value = kvp.Value.m_strKey;

                grid.Rows[i].Cells[j++].Value = kvp.Value.m_strValue;
                grid.Rows[i].Cells[j++].Value = kvp.Value.m_strUnit;
                grid.Rows[i].Cells[j++].Value = kvp.Value.m_strDescribe;
                grid.Rows[i].Cells[j++].Value = kvp.Value.m_strMin;
                grid.Rows[i].Cells[j++].Value = kvp.Value.m_strMax;

                i++;
            }
        }

        /// <summary>
        /// 获取指定的位寄存器的状态
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <returns></returns>
        public bool GetRegBit(int nIndex)
        {
              return m_bitArray[nIndex];
        }

        /// <summary>
        /// 向指定的位寄存器写入状态
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="bBit">状态值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegBit(int nIndex, bool bBit, bool bNotify = true)
        {
            m_bitArray[nIndex] = bBit;
            if (bNotify)
            {
                BitChangedEvent(nIndex, bBit);
            }
        }

        /// <summary>
        /// 获取指定的整型寄存器的值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <returns></returns>
        public int GetRegInt(int nIndex)
        {
            return m_intArray[nIndex];
        }

        /// <summary>
        /// 向指定的整型寄存器写入值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="nData">要写入的数值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegInt(int nIndex, int nData, bool bNotify = true)
        {
            m_intArray[nIndex] = nData;
            if (bNotify )
            {
                IntChangedEvent(nIndex, nData);
            }
        }

        /// <summary>
        /// 获取一个浮点型寄存器的值
        /// </summary>
        /// <param name="nIndex">寄存器的值</param>
        /// <returns></returns>
        public double GetRegDouble(int nIndex)
        {
            return m_doubleArray[nIndex];
        }

        /// <summary>
        /// 向一个浮点数寄存器写入值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="fData">要写入的值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegDouble(int nIndex, double fData, bool bNotify = true)
        {
            m_doubleArray[nIndex] = fData;
            if (bNotify )
            {
                DoubleChangedEvent(nIndex, fData);
            }
        }

        /// <summary>
        /// 获取参数整型值
        /// </summary>
        /// <param name="szParam">字符串值索引</param>
        /// <returns></returns>
        public int GetParamInt(string szParam)
        {
            if(m_DicParam.ContainsKey(szParam))   //判断是否存在指定的键值szParam
            {
                return Convert.ToInt32(m_DicParam[szParam].m_strValue);
            }
            else
            {
                MessageBox.Show(string.Format("系统配置文件中{0}参数不存在", szParam), "参数配置错误");

                //      WarningMgr.GetInstance().Error(string.Format("50105, ERR-SSW, 找不到对应的整型参数值, param = {0}", szParam));
            }
            return 0;
        }

        /// <summary>
        /// 获取参数布尔值
        /// </summary>
        /// <param name="szParam">字符串值索引</param>
        /// <returns></returns>
        public bool GetParamBool(string szParam)
        {
            if (m_DicParam.ContainsKey(szParam))  //判断是否存在指定的键值szParam
            {
                return (m_DicParam[szParam].m_strValue) != "0";
            }
            else
            {
                MessageBox.Show(string.Format("系统配置文件中{0}参数不存在", szParam), "参数配置错误");

                //     WarningMgr.GetInstance().Error(string.Format("50106, ERR-SSW, 参数配置找不到对应的布尔型参数值, param = {0}", szParam));
            }
            return false;
        }

        /// <summary>
        /// 获取参数浮点数值
        /// </summary>
        /// <param name="szParam">字符串值索引</param>
        /// <returns></returns>
        public double GetParamDouble(string szParam)
        {
            if (m_DicParam.ContainsKey(szParam))  //判断是否存在指定的键值szParam
            {
                return Convert.ToDouble(m_DicParam[szParam].m_strValue);
            }
            else
            {
                MessageBox.Show(string.Format("系统配置文件中{0}参数不存在", szParam), "参数配置错误");

                //   WarningMgr.GetInstance().Error(string.Format("50107, ERR-SSW, 参数配置找不到对应的浮点型参数值, param = {0}", szParam));
            }
            return 0.00;
        }

        /// <summary>
        /// 获取参数字符串值
        /// </summary>
        /// <param name="szParam">字符串值索引</param>
        /// <returns></returns>
        public string GetParamString(string szParam)
        {
            if (m_DicParam.ContainsKey(szParam))  //判断是否存在指定的键值szParam
            {
                return m_DicParam[szParam].m_strValue;
            }
            else
            {
                MessageBox.Show(string.Format("系统配置文件中{0}参数不存在", szParam), "参数配置错误");
         //       WarningMgr.GetInstance().Error(string.Format("50108, ERR-SSW, 参数配置找不到对应的字符串型参数值, param = {0}", szParam));
            }
            return string.Empty;
        }

        private string GetOrCreateDir(string strDir,string strSubDir)
        {
            if(System.IO.Directory.Exists(strDir+strSubDir) == false)
            {
                System.IO.Directory.CreateDirectory(strDir + strSubDir);
            }        
            return strDir+strSubDir;           
        }

        /// <summary>
        /// 得到保存log路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetLogPath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("LogSavePath"),strSubDir);
        }

  
        /// <summary>
        /// 得到保存Image路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetImagePath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("ImageSavePath"), strSubDir);
        }

        /// <summary>
        /// 得到保存data路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetDataPath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("DataSavePath"), strSubDir);
        }

        /// <summary>
        /// 删除指定路径下(包括子文件夹)所有过期的文件
        /// </summary>
        /// <param name="strDir">待删除目录文件夹</param>
        /// <param name="strExt">待删除文件的后缀名</param>
        /// <param name="fDays">天数,可以为浮点数</param>
        public void DeleteOvertimeFiles(string strDir, string strExt, double fDays)
        {
            if (!Directory.Exists(strDir) || fDays<=0)
            {
                return;
            }
            try
            {
                DirectoryInfo di = new DirectoryInfo(@strDir);
             
                //获得当前文件夹下所有子文件夹
                List<string> folders = new List<string>(Directory.GetDirectories(strDir));
                folders.ForEach(c =>
                {
                    string childDir = Path.Combine(new string[] { strDir, Path.GetFileName(c) });
                    DeleteOvertimeFiles(childDir, strExt, fDays);//采用递归的方法实现
                });

                FileInfo[] fi = di.GetFiles("*." + strExt);
                DateTime dtNow = DateTime.Now;
                foreach (FileInfo tmpfi in fi)
                {
                    TimeSpan ts = dtNow.Subtract(tmpfi.LastWriteTime);
                    if (ts.TotalHours > 24 * fDays) //超过设置天数
                    {
                        tmpfi.Delete();//删除文件
                    }
                }

                folders = new List<string>(Directory.GetDirectories(strDir));
                fi = di.GetFiles("*.*" );
                if (folders.Count == 0 && fi.Length == 0)
                    Directory.Delete(strDir);

            }
            catch(Exception e)
            {
        


            }
        }

        /// <summary>
        /// 复制文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destPath">目标文件夹</param>
        public void CopyFiles(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath); //目标目录不存在则创建
                }
                //获得源文件下所有文件
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    File.Copy(c, destFile, true);//覆盖模式
                });
                //获得源文件下所有子目录
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    CopyFiles(c, destDir);//采用递归的方法实现
                });
            }
        }

        /// <summary>
        /// 移动文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="strDir">源文件夹</param>
        /// <param name="strDestDir">目的文件夹</param>
        public void MoveFiles(string strDir, string strDestDir)
        {
            //Directory.Move(strDir, strDestDir); //必须要在同一个根目录下移动才有效，不能在不同卷中移动
            if (Directory.Exists(strDir))
            {
                if (!Directory.Exists(strDestDir))//目标目录不存在则创建  
                {
                    Directory.CreateDirectory(strDestDir);
                }
                //获得源文件下所有文件  
                List<string> files = new List<string>(Directory.GetFiles(strDir));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { strDestDir, Path.GetFileName(c) });
                    //覆盖模式  
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                //获得源文件下所有子文件夹
                List<string> folders = new List<string>(Directory.GetDirectories(strDir));
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { strDestDir, Path.GetFileName(c) });
                    MoveFiles(c, destDir); //采用递归的方法实现  
                });
            }
        }

        /// <summary>
        /// 监控指定目录文件夹下文件的增加、改变、删除、重命名
        /// </summary>
        /// <param name="numberFile">数组索引</param>
        /// <param name="strMonitorFloder">要监控的指定目录文件夹</param>
        /// <param name="strFilter">指定文件类型,可以过滤掉其他类型的文件</param>
        /// <param name="fileCreate">文件创建事件</param>
        /// <param name="fileDelete">文件删除事件</param>
        /// <param name="fileRename">文件重命名事件</param>
        /// <param name="fileChange">文件内容改变事件</param>
        /// <returns></returns>
        public bool MonitorImgFile(int numberFile, string strMonitorFloder,string strFilter,
            OnCreate fileCreate, OnDelete fileDelete,OnRename fileRename, OnChange fileChange)
        {
            try
            {
                m_fileWatcher[numberFile] = new FileSystemWatcher();
                m_fileWatcher[numberFile].Path = strMonitorFloder;//这个属性告诉FileSystemWatcher它需要监控哪条路径
                m_fileWatcher[numberFile].Filter = strFilter; //这个属性允许你过滤掉某些类型的文件发生的变化。
                                                  //例如，如果我们只希望在TXT文件被修改 / 新建 / 删除时提交通知，可以将这个属性设为“*txt”。
                                                  //在处理高流量或大型目录时，使用这个属性非常方便。 
                m_fileWatcher[numberFile].Changed += new FileSystemEventHandler(fileChange);
                m_fileWatcher[numberFile].Created += new FileSystemEventHandler(fileCreate);
                m_fileWatcher[numberFile].Deleted += new FileSystemEventHandler(fileDelete);
                m_fileWatcher[numberFile].Renamed += new RenamedEventHandler(fileRename);
                m_fileWatcher[numberFile].EnableRaisingEvents = true;//如果没有将EnableRaisingEvents设为真，系统不会提交任何一个事件。
                m_fileWatcher[numberFile].NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName 
                                      | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                m_fileWatcher[numberFile].IncludeSubdirectories = true; //这个属性说明FileSystemWatcher对象是否应该监控子目录中发生的改变。
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 停止对文件夹的监视
        /// </summary>
        /// <returns></returns>
        public bool StopMonitorImgFile(int numberFile)
        {
            try
            {
                if (m_fileWatcher[numberFile] != null)
                    m_fileWatcher[numberFile].EnableRaisingEvents = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        /// <summary>
        /// 监视文件增加处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string str = string.Format("文件新建事件处理逻辑 {0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(str);
        }

        /// <summary>
        /// 监视文件改变处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //当被监控的目录中有一个文件被修改时，就提交这个事件。值得注意的是，这个事件可能会被提交多次，
            //即使文件的内容仅仅发生一项改变。这是由于在保存文件时，文件的其它属性也发生了改变。 
            string str = string.Format("文件改变事件处理逻辑{0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(str);
        }

        /// <summary>
        /// 监视文件增删除处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            string str = string.Format("文件删除事件处理逻辑{0}  {1}   {2}", e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(str);
        }

        /// <summary>
        /// 监视文件重命名处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string str = string.Format("文件重命名事件处理逻辑{0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(str);
        }

        /// <summary>
        /// 检测屏幕鼠标键盘是否无操作超过设定的时间
        /// </summary>
        /// <returns></returns>
        public bool CheckSystemIdle()
        {
            long nTime = GetLastInputTime();
            //todo
            if (nTime >  GetParamInt("IdleTime") * 60/*15*60*/ )  //大于10分钟
            {
                if (Security.IsOpMode() == false)
                {                    
                    Security.ChangeOpMode();
                    return true;
                }
            }
            return false;
        }
        // 创建结构体用于返回捕获时间
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            // 设置结构体块容量
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            // 捕获的时间
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        /// <summary>
        /// 获取键盘和鼠标没有操作的时间
        /// </summary>
        /// <returns></returns>
        private static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            // 捕获时间
            if (!GetLastInputInfo(ref vLastInputInfo))
                return 0;
            else
                return (Environment.TickCount - (long)vLastInputInfo.dwTime)/1000;//当前闲置秒数
        }

        /// <summary>
        /// 监控线程
        /// </summary>
        public override void ThreadMonitor()
        {
            DeleteOvertimeFiles(GetImagePath(), "bmp", GetParamDouble("ImageKeepTime"));
            DeleteOvertimeFiles(GetImagePath(), "png", GetParamDouble("LogKeepTime"));
            DeleteOvertimeFiles(GetDataPath(), "csv", GetParamDouble("DataKeepTime"));
            DeleteOvertimeFiles(GetLogPath(),"csv", GetParamDouble("LogKeepTime"));
            int nHour = DateTime.Now.Hour;
            int nOldHour = nHour;
            while (m_bRunThread)
            {
                Thread.Sleep(2000);

                nHour = DateTime.Now.Hour;
                if (nHour > nOldHour || (nHour == 0 && nHour < nOldHour))
                {
                    nOldHour = nHour;
                    DeleteOvertimeFiles(GetImagePath(), "bmp", GetParamDouble("ImageKeepTime"));
                    DeleteOvertimeFiles(GetImagePath(), "png", GetParamDouble("LogKeepTime"));
                    DeleteOvertimeFiles(GetDataPath(), "csv", GetParamDouble("DataKeepTime"));
                    DeleteOvertimeFiles(GetLogPath(), "log", GetParamDouble("LogKeepTime"));
                }

            }
        }
    }
}
