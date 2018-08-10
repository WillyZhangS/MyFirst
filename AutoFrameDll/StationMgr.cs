using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using CommonTool;
using System.IO;

namespace AutoFrameDll
{
    /// <summary>
    /// 站位状态
    /// </summary>
    public enum StationState
    {
        /// <summary>
        /// 手动状态
        /// </summary>
        STATE_MANUAL,
        /// <summary>
        /// 自动状态
        /// </summary>
        STATE_AUTO , 
        /// <summary>
        /// 就绪状态
        /// </summary>
        STATE_READY ,
        /// <summary>
        /// 急停状态
        /// </summary>
        STATE_EMG ,
        /// <summary>
        /// 暂停状态
        /// </summary>
        STATE_PAUSE ,
    };

    /// <summary>
    /// 站位管理类
    /// </summary>
    public class StationMgr : SingletonTemplate<StationMgr>
    {
        /// <summary>
        /// 站位属性项
        /// </summary>
        private static string[] _strStationItem = { "站序号", "站名定义", "X轴号", "Y轴号", "Z轴号", "U轴号" };
        /// <summary>
        /// 轴名字属性
        /// </summary>
        private static string[] _strAxisItem = { "x", "y", "z", "u" };
        /// <summary>
        /// 点位文件的存储路径
        /// </summary>
        private string m_strPointFile = string.Empty;
        /// <summary>
        /// 站位链表
        /// </summary>
        public List<StationBase> m_lsStation = new List<StationBase>();
        /// <summary>
        /// 站位集合
        /// </summary>
        public Dictionary<Form, StationBase> m_dicFromStation = new Dictionary<Form, StationBase>();
        private StationState m_nCurState; //站位当前状态
        private bool m_bAutoMode;   //是否全自动模式

        /// <summary>
        ///当前是否全自动运行
        /// </summary>
        public bool BAutoMode
        {
            get { return m_bAutoMode; }
            set { m_bAutoMode = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StationMgr()
        {
            WarningMgr.GetInstance().WarningEventHandler += new EventHandler(OnWarning);
        }

        /// <summary>
        ///响应报警函数 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void OnWarning(object Sender, EventArgs e)
        {
            WarningEventData wed = (WarningEventData)e;
            if (wed.bAdd) //增加一条异常信息
            {
                WarningMgr.WARNING_DATA wd = WarningMgr.GetInstance().GetWarning(wed.nIndex);
                if (wd.strLevel != "WARN")
                {
                    if (IsEmg() == false)
                    {
                        EmgStopAllStation();
                    }
                }
            }
        }
        /// <summary>
        /// 定义一个站位状态变化委托函数
        /// </summary>
        /// <param name="OldState"></param>
        /// <param name="NewState"></param>
        public delegate void StateChangedHandler(StationState OldState, StationState NewState);
        /// <summary>
        /// 定义一个站位状态变化事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;


        /// <summary>
        ///站位切换状态同时触发状态变更事件
        /// </summary>
        /// <param name="NewState"></param>
        public void ChangeState(StationState NewState)
        {
            if (m_nCurState != NewState)
            {
                StationState ss = m_nCurState;
                m_nCurState = NewState;
                StateChangedEvent(ss, m_nCurState);
                //        ShowLog("站位状态变化至" + NewState.ToString());
            }
        }

        /// <summary>
        ///根据站位名称获取一个站位类指针 
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public StationBase GetStation(string strName)
        {
            foreach (StationBase s in m_lsStation)
            {
                if (s.Name == strName)
                    return s;
            }
            return null;
        }

        /// <summary>
        /// 获取手动页面窗口对应的站位类
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        public StationBase GetStation(Form frm)
        {
            return m_dicFromStation[frm];
        }

        /// <summary>
        ///当前是否在自动运行状态 
        /// </summary>
        /// <returns></returns>
        public bool IsAutoRunning()
        {
            return m_nCurState != StationState.STATE_MANUAL;
        }

        /// <summary>
        /// 当前是否在暂停状态
        /// </summary>
        /// <returns></returns>
        public bool IsPause()
        {
            return m_nCurState == StationState.STATE_PAUSE;
        }

        /// <summary>
        /// 当前是否处于急停模式
        /// </summary>
        /// <returns></returns>
        public bool IsEmg()
        {
            return m_nCurState == StationState.STATE_EMG;
        }

        /// <summary>
        ///当前模式是否允许暂停 
        /// </summary>
        /// <returns></returns>
        public bool AllowPause()
        {
            if (m_nCurState != StationState.STATE_MANUAL && m_nCurState != StationState.STATE_EMG
                && m_nCurState != StationState.STATE_PAUSE)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 向站位管理器增加站位,由于站位列表已经从配置中读取, 需要更换列表中的类对象引用
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="station"></param>
        public void AddStation(Form frm, StationBase station)
        {
            int i = 0;
            foreach (StationBase sb in m_lsStation)
            {
                if (sb.Name == station.Name)
                {
                    frm.Text = station.Name;  //工站名字
                    m_dicFromStation.Add(frm, station);
                    station.m_dicPoint = m_lsStation[i].m_dicPoint;
                    station.m_nAxisArray = m_lsStation[i].m_nAxisArray;
                    m_lsStation[i] = station;
                    return;
                }
                else
                    i++;
            }
            //读取的配置中未找到指定的站位名,无法匹配
            throw (new Exception(station.Name + "站位配置未找到"));
        }

        /// <summary>
        ///读取系统配置文件里的站位信息 
        /// </summary>
        /// <param name="doc"></param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            m_lsStation.Clear();
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "Station");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;
                        string strNo = xe.GetAttribute(_strStationItem[0]).Trim();
                        if (strNo == null)
                            continue;

                        string strName = (xe.GetAttribute(_strStationItem[1])).Trim();
                        if (strName == null)
                            continue;

                        StationBase s = new StationBase(strName);
                        s.Index = Convert.ToInt32(strNo);
                        for (int i = 0; i < 4; ++i)
                        {
                            string strAxisNo = xe.GetAttribute(_strStationItem[i + 2]).Trim();
                            if (strAxisNo.Length != 0)
                                s.SetAxisNo(i, Convert.ToInt32(strAxisNo)); //设置工站轴号
                        }
                        m_lsStation.Add(s);
                    }
                }
            }
        }

        /// <summary>
        ///从内存中的参数更新到界面表格 
        /// </summary>
        /// <param name="grid"></param>
        public void UpdateGridFromParam(DataGridView grid)
        {
            grid.Rows.Clear();
            if (m_lsStation.Count > 0)
            {
                grid.Rows.AddCopies(0, m_lsStation.Count);

                int i = 0;
                foreach (StationBase station in m_lsStation)
                {
                    int j = 0;
                    grid.Rows[i].Cells[j++].Value = (i + 1).ToString();
                    grid.Rows[i].Cells[j++].Value = station.Name;
                    for (int k = 0; k < 4; ++k)
                    {
                        if (station.m_nAxisArray[k] != 0)
                            grid.Rows[i].Cells[j++].Value = station.m_nAxisArray[k].ToString();
                        else
                            j++;   //todo
                    }
                    i++;
                }
            }
        }

        /// <summary>
        ///从界面表格更新到内存中参数表 
        /// </summary>
        /// <param name="grid"></param>
        public void UpdateParamFromGrid(DataGridView grid)
        {
            int m = grid.RowCount;
            int n = grid.ColumnCount;

            m_lsStation.Clear();
            for (int i = 0; i < m; ++i)
            {
                if (grid.Rows[i].Cells[0].Value == null)
                    continue;

                string strNo = grid.Rows[i].Cells[0].Value.ToString();

                StationBase s = new StationBase(grid.Rows[i].Cells[1].Value.ToString());

                s.Index = Convert.ToInt32(strNo);
                for (int j = 0; j < 4; ++j)
                {
                    if (grid.Rows[i].Cells[2 + j].Value != null)
                    {
                        string ss = grid.Rows[i].Cells[2 + j].Value.ToString();
                        s.SetAxisNo(j, Convert.ToInt32(grid.Rows[i].Cells[2 + j].Value.ToString()));
                    }
                }
                m_lsStation.Add(s);
            }
        }

        /// <summary>
        ///保存xml配置文件SystemCfg中的站位信息 
        /// </summary>
        /// <param name="doc"></param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("Station");
            xnl.AppendChild(root);

            foreach (StationBase t in m_lsStation)
            {
                XmlElement xe = doc.CreateElement("Station");

                int j = 0;
                xe.SetAttribute(_strStationItem[j++], t.Index.ToString());
                xe.SetAttribute(_strStationItem[j++], t.Name);
                for (int i = 0; i < 4; ++i)
                {
                    if (t.m_nAxisArray[i] > 0)
                        xe.SetAttribute(_strStationItem[j++], t.m_nAxisArray[i].ToString());
                    else
                        xe.SetAttribute(_strStationItem[j++], string.Empty);
                }
                root.AppendChild(xe);
            }
        }

        /// <summary>
        ///加载站位下的运动点位 
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public bool LoadPointFile(string strFile)
        {
            XmlDocument doc = new XmlDocument();
            m_strPointFile = strFile;
            try
            {
                doc.Load(strFile);
                if (doc.HasChildNodes)
                {
                    foreach (StationBase t in m_lsStation)
                    {
                        t.m_dicPoint.Clear();
                        XmlNodeList xnl = doc.SelectNodes("/PointCfg/" + t.Name);
                        if (xnl == null || xnl.Count == 0)
                            continue;
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            foreach (XmlNode xn in xnl)
                            {
                                XmlElement xe = (XmlElement)xn;

                                PointInfo pp = new PointInfo();

                                int n = Convert.ToInt32(xe.GetAttribute("index"));
                                pp.strName = xe.GetAttribute("name");
                                string s = xe.GetAttribute(_strAxisItem[0]);
                                if (s.Length > 0)
                                    pp.x = Convert.ToInt32(s);
                                else
                                    pp.x = -1;
                                s = xe.GetAttribute(_strAxisItem[1]);
                                if (s.Length > 0)
                                    pp.y = Convert.ToInt32(s);
                                else
                                    pp.y = -1;
                                s = xe.GetAttribute(_strAxisItem[2]);
                                if (s.Length > 0)
                                    pp.z = Convert.ToInt32(s);
                                else
                                    pp.z = -1;
                                s = xe.GetAttribute(_strAxisItem[3]);
                                if (s.Length > 0)
                                    pp.u = Convert.ToInt32(s);
                                else
                                    pp.u = -1;

                                t.m_dicPoint.Add(n, pp);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), string.Format("点位文件{0}读取失败", strFile),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 更新点位表格
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="nIndex"></param>
        public void UpdatePointGrid(DataGridView grid, int nIndex = -1)
        {
            if (m_lsStation.Count > 0)
            {
                grid.Rows.Clear();
                foreach (StationBase s in m_lsStation)
                {
                    if (nIndex == -1 || nIndex == s.Index)
                    {
                        if (s.m_dicPoint.Count > 0)
                        {
                            foreach (KeyValuePair<int, PointInfo> kvp in s.m_dicPoint)
                            {
                                int k = 0;
                                grid.Rows.Add();
                                int j = grid.Rows.Count - 2;
                                grid.Rows[j].Cells[k++].Value = s.Index;
                                grid.Rows[j].Cells[k++].Value = s.Name;
                                grid.Rows[j].Cells[k++].Value = kvp.Key.ToString();
                                grid.Rows[j].Cells[k++].Value = kvp.Value.strName;

                                if (kvp.Value.x != -1)
                                    grid.Rows[j].Cells[k++].Value = kvp.Value.x;
                                else
                                    k++;
                                if (kvp.Value.y != -1)
                                    grid.Rows[j].Cells[k++].Value = kvp.Value.y;
                                else
                                    k++;
                                if (kvp.Value.z != -1)
                                    grid.Rows[j].Cells[k++].Value = kvp.Value.z;
                                else
                                    k++;
                                if (kvp.Value.u != -1)
                                    grid.Rows[j].Cells[k++].Value = kvp.Value.u;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 通过界面数据更新指定站位的点位数据
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="nIndex"></param>
        public void UpdatePoint(DataGridView grid, int nIndex)
        {
            foreach (StationBase s in m_lsStation)
            {
                if (s.Index == nIndex || nIndex == -1)
                {
                    Dictionary<int, PointInfo> pp = s.m_dicPoint;
                    pp.Clear();

                    for (int i = 0; i < grid.RowCount; ++i)
                    {
                        if (grid.Rows[i].Cells[0].Value != null)
                        {
                            int n = Convert.ToInt32(grid.Rows[i].Cells[0].Value.ToString());
                            if (n == s.Index)
                            {
                                PointInfo p = new PointInfo();
                                int k = Convert.ToInt32(grid.Rows[i].Cells[2].Value.ToString());
                                p.strName = grid.Rows[i].Cells[3].Value.ToString();
                                int[] pos = { -1, -1, -1, -1 };
                                for (int j = 0; j < 4; ++j)
                                {
                                    if (grid.Rows[i].Cells[4 + j].Value != null)
                                        pos[j] = Convert.ToInt32(grid.Rows[i].Cells[4 + j].Value.ToString());
                                }
                                p.x = pos[0];
                                p.y = pos[1];
                                p.z = pos[2];
                                p.u = pos[3];
                                pp.Add(k, p);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///保存站位下的运动点位 
        /// </summary>       
        public void SavePointFile(string strFile)
        {
            m_strPointFile = strFile;
            SavePointFile();
        }
        /// <summary>
        ///保存站位下的运动点位 
        /// </summary>       
        public void SavePointFile()
        {
            FileInfo fi = new FileInfo(m_strPointFile);
            if(fi.Exists == false)
            {
                string strText = string.Format("点位文件{0}未找到,是否重新创建?", m_strPointFile);
                if(MessageBox.Show(strText, "当前产品配置的点位文件不存在", MessageBoxButtons.YesNo,MessageBoxIcon.Question)
                     == DialogResult.Cancel)
                {
                    return;
                }
            }

            XmlDocument doc = new XmlDocument();

            XmlDeclaration dec2 = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec2);

            XmlElement root2 = doc.CreateElement("PointCfg");
            doc.AppendChild(root2);

            if (doc.HasChildNodes)
            {
                foreach (StationBase s in m_lsStation)
                {
                    XmlNode xn = doc.SelectSingleNode("PointCfg");
                    XmlElement xe = doc.CreateElement(s.Name);
                    xn.AppendChild(xe);

                    xn = doc.SelectSingleNode("PointCfg/" + s.Name);
                    foreach(KeyValuePair<int, PointInfo> kvp in s.m_dicPoint)
                    {
                        XmlElement xx = doc.CreateElement("Point");

                        xx.SetAttribute("index", kvp.Key.ToString());
                        xx.SetAttribute("name", kvp.Value.strName);
                       
                        if (kvp.Value.x != -1)
                            xx.SetAttribute(_strAxisItem[0], kvp.Value.x.ToString());
                        else
                            xx.SetAttribute(_strAxisItem[0],string.Empty);
                        if (kvp.Value.y != -1)
                             xx.SetAttribute(_strAxisItem[1], kvp.Value.y.ToString());
                        else
                            xx.SetAttribute(_strAxisItem[1], string.Empty);
                        if (kvp.Value.z != -1)
                            xx.SetAttribute(_strAxisItem[2], kvp.Value.z.ToString());
                        else
                            xx.SetAttribute(_strAxisItem[2], string.Empty);
                        if (kvp.Value.u != -1)
                            xx.SetAttribute(_strAxisItem[3], kvp.Value.u.ToString());
                        else
                            xx.SetAttribute(_strAxisItem[3], string.Empty);

                        xn.AppendChild(xx);
                    }
                }
            }
            doc.Save(m_strPointFile);
        }

        /// <summary>
        /// 确定当前是否为全自动模式，无需启动按钮
        /// </summary>
        public override void ThreadMonitor()
        {
            while (m_bRunThread)
            {
                if (m_nCurState == StationState.STATE_READY)
                {
                  if (BAutoMode == true)           //是否全自动运行
                    {
                        AutoStartAllStation();
                        ShowLog("当前全自动模式，自动开始运行");
                    }
                }
                else
                {                   
                    if (IsAllReady())//所有站位就绪
                    {
                        ChangeState(StationState.STATE_READY);//站位就绪
                        ShowLog("所有站位进入就绪状态，等待开始");
                    }
                }
                Thread.Sleep(SystemMgr.GetInstance().ScanTime);
            }
        }

        /// <summary>
        ///所有站位开始运行，自动模式
        /// </summary>
        /// <returns></returns>
        public bool StartRun()
        {
          if (m_nCurState != StationState.STATE_MANUAL)
                return false;

            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                ShowLog("模拟运行模式,运动和IO等待默认为true");                
            }

            InitAllStationSecurity();
            IoMgr.GetInstance().StartMonitor();
            MotionMgr.GetInstance().StartMonitor();
            StartMonitor();
            
            foreach (StationBase s in m_lsStation)
            {
                s.SwitchState(StationState.STATE_AUTO);
                s.StartRun();
            }
            ChangeState(StationState.STATE_AUTO);

            ShowLog("所有站位进入自动运行状态");
            return true;
         }

        /// <summary>
        ///所有站位停止运行，转为手动 
        /// </summary>
        /// <returns></returns>
        public bool StopRun()
        {
            foreach(StationBase s in m_lsStation)
            {
                s.SwitchState(StationState.STATE_MANUAL);
                s.EmgStop();
            }

            IoMgr.GetInstance().StopMonitor();
            MotionMgr.GetInstance().StopMonitor();
            StationMgr.GetInstance().StopMonitor();

            foreach (StationBase s in m_lsStation)
            {
                s.StopRun();
            }
            ChangeState(StationState.STATE_MANUAL);
            ShowLog("所有站位退出");
            return true;
          }

        /// <summary>
        ///所有站在就绪后开始运行一个循环 
        /// </summary>
        /// <returns></returns>
        public bool AutoStartAllStation()
        {
            if (m_nCurState != StationState.STATE_READY)
                return false;

            foreach (StationBase s in m_lsStation)
            {
                s.BeginCycle = true;
                s.SwitchState(StationState.STATE_AUTO);
            }
            ChangeState(StationState.STATE_AUTO);
            ShowLog("所有站位开始自动运行");
            return true;
        }

        /// <summary>
        ///初始化所有站位为安全状态 
        /// </summary>
        private void InitAllStationSecurity()
        {
            foreach (StationBase s in m_lsStation)
            {
                s.InitSecurityState();
            }
            ShowLog("初台化所有站位为安全状态");
        }

        /// <summary>
        ///急停所有站位 
        /// </summary>
        /// <returns></returns>
        public bool EmgStopAllStation()
        {
            if (m_nCurState == StationState.STATE_EMG || m_nCurState == StationState.STATE_MANUAL)
                return false;
           
            foreach (StationBase s in m_lsStation)
            {
                s.SwitchState(StationState.STATE_EMG);
            }
            foreach(StationBase s in m_lsStation)
            {
                s.EmgStop();              
            }
            ShowLog("报警急停!!!");
            ChangeState(StationState.STATE_EMG);
            //IoMgr.GetInstance().EmgStop();
        
            return true;
        }

        /// <summary>
        ///暂停所有站位运行 
        /// </summary>
        /// <returns></returns>
        public bool PauseAllStation()
        {
            if (m_nCurState != StationState.STATE_MANUAL
             && m_nCurState != StationState.STATE_EMG
             && m_nCurState != StationState.STATE_PAUSE)
            {
                ChangeState(StationState.STATE_PAUSE);
                foreach (StationBase s in m_lsStation)
                {
                    s.OnPause();
                    s.SwitchState(StationState.STATE_PAUSE);
                }
                ShowLog("所有站位进入暂停状态");
            }
            return true;
        }

        /// <summary>
        ///将所有站位由暂停中恢复运行 
        /// </summary>
        /// <returns></returns>
        public bool ResumeAllStation()
        {
            if (m_nCurState != StationState.STATE_PAUSE)
                return false;
            ChangeState(StationState.STATE_AUTO);
            foreach(StationBase s in m_lsStation)
            {
                s.OnResume();
                s.SwitchState(StationState.STATE_AUTO);
            }
            ChangeState(StationState.STATE_AUTO);
            ShowLog("所有站位恢复运行");
            return true;
        }

        /// <summary>
        ///将所有站位从急停中复位 
        /// </summary>
        /// <returns></returns>
        public bool ResetAllStation()
        {
            if (m_nCurState != StationState.STATE_EMG)
                return false;

            WarningMgr.GetInstance().ClearAllWarning();
            ShowLog("所有站位复位");
            InitAllStationSecurity();
            foreach (StationBase s in m_lsStation)
            {
                s.SwitchState(StationState.STATE_AUTO);
            }
            
            ChangeState(StationState.STATE_AUTO);
            return true;
         }

        /// <summary>
        ///判断所有工站是否全部就绪状态
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            foreach(StationBase s in m_lsStation)
            {
                if(false == s.IsReady())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 测试接口，暂未使用
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public bool TestRun(StationBase sb)
        {
            return true;
        }
    }
}
