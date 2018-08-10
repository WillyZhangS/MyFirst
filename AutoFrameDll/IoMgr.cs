using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using CommonTool;
using MotionIO;
using System.Reflection;

namespace AutoFrameDll
{
    /// <summary>
    /// 三色灯及蜂鸣器状态定义
    /// </summary>
   public enum LightState
    {
        /// <summary>
        /// 只开红灯
        /// </summary>
        红灯开,
        /// <summary>
        /// 只开绿灯
        /// </summary>
        绿灯开,
        /// <summary>
        ///绿灯按时间频率开关
        /// </summary>
        绿灯闪,
        /// <summary>
        /// 只开黄灯
        /// </summary>
        黄灯开,
        /// <summary>
        /// 黄灯按时间频率开关
        /// </summary>
        黄灯闪,
        /// <summary>
        /// 蜂鸣开
        /// </summary>
        蜂鸣开,
        /// <summary>
        /// 蜂鸣关
        /// </summary>
        蜂鸣关,
        /// <summary>
        /// 蜂鸣按时间频率开关
        /// </summary>
        蜂鸣闪
    }

    /// <summary>
    /// 板卡IO管理类
    /// </summary>
    public class IoMgr : SingletonTemplate<IoMgr>
    {
        /// <summary>
        /// IO板卡属性
        /// </summary>
        private static string[] m_strIoCard = { "卡序号", "卡号", "卡类型" };
        /// <summary>
        /// IO板卡点位属性
        /// </summary>
        private static string[] m_strIoItem = { "卡序号", "点序号", "点位名称" };
        /// <summary>
        /// 系统输入IO属性
        /// </summary>
        private static string[] m_strIoSystemItem = { "功能描述", "卡序号", "点序号", "有效电平" };
        /// <summary>
        /// 系统输出IO属性
        /// </summary>
        private static string[] m_strIoSystemItemOut = { "功能描述", "卡序号", "点序号", "有效电平" };

        /// <summary>
        /// 定义一个IO状态变化委托函数
        /// </summary>
        /// <param name="nCard"></param>
        public delegate void IoChangedHandler(int nCard);
        /// <summary>
        /// 定义一个Io状态变化事件
        /// </summary>
        public event IoChangedHandler IoChangedEvent;

        //    string[] m_strIoSystem = { "红灯", "绿灯", "黄灯", "蜂鸣", "急停", "启动", "复位", "门开关" };

        IoSysDefine m_IoEmgStop = new IoSysDefine();  //系统急停Io
        IoSysDefine m_IoBegin = new IoSysDefine();      //系统开始Io
        IoSysDefine m_IoResetting = new IoSysDefine();  //系统复位Io
        IoSysDefine m_IoDoor = new IoSysDefine();  //系统门开关Io
        IoSysDefine m_IoRedLight = new IoSysDefine(); //红灯Io
        IoSysDefine m_IoYellowLight = new IoSysDefine(); //黄灯Io
        IoSysDefine m_IoGreenLight = new IoSysDefine(); //绿灯Io
        IoSysDefine m_IoBuzzing = new IoSysDefine(); //蜂鸣Io
        private Thread m_LightThread = null;
        private bool m_bRunLightThread = false;
        int m_nBuzzingTime = 0;
        int m_nYelloTime = 0;
        int m_nGreenTime = 0;

        /// <summary>
        /// 系统输入输出的结构体
        /// </summary>
        public struct IoSysDefine
        {
            /// <summary>
            /// 功能描述
            /// </summary>
            public string strName;
            /// <summary>
            /// 板卡号
            /// </summary>
            public int nCard;
            /// <summary>
            /// 点位号
            /// </summary>
            public int nBit;
            /// <summary>
            /// 电平高低,0-低电平，1-高电平
            /// </summary>
            public bool bLevel;
            /// <summary>
            /// 当前状态是否已经有效
            /// </summary>
            public bool bTrigger;
        }
        /// <summary>
        /// IO卡类指针向量
        /// </summary>
        public List<IoCtrl> m_listCard = new List<IoCtrl>();
        /// <summary>
        /// 系统常用IO输入
        /// </summary>
        public List<IoSysDefine> m_listIoSystemIn = new List<IoSysDefine>();
        /// <summary>
        /// 系统常用IO输出
        /// </summary>
        public List<IoSysDefine> m_listIoSystemOut = new List<IoSysDefine>();

        /// <summary>
        /// IO输入点名称与点位映射
        /// </summary>
        public Dictionary<string, Int64> m_dicIn = new Dictionary<string, Int64>();
        /// <summary>
        /// IO输出点名称与点位映射         
        /// </summary>
        public Dictionary<string, Int64> m_dicOut = new Dictionary<string, Int64>();


        /// <summary>
        /// 类构造函数
        /// </summary>
        public IoMgr()
        {
           
        }
        
        /// <summary>
        /// 三色灯以及蜂鸣器状态控制
        /// </summary>
        /// <param name="state"></param>
        /// <param name="nMilliSecond">闪烁间隔毫秒数, 默认为一秒</param>
        public void AlarmLight(LightState state, int nMilliSecond = 1000)
        {
            switch (state)
            {
                case LightState.红灯开:  //红灯
                    {
                        WrtieSystemIo(m_IoRedLight, true);
                        WrtieSystemIo(m_IoGreenLight, false);
                        WrtieSystemIo(m_IoYellowLight, false);
                        m_nYelloTime = 0;
                        m_nGreenTime = 0;
                    }
                    break;
                case LightState.黄灯开: //黄灯
                    {
                        WrtieSystemIo(m_IoRedLight, false);
                        WrtieSystemIo(m_IoGreenLight, false);
                        WrtieSystemIo(m_IoYellowLight, true);
                        m_nYelloTime = 0;
                        m_nGreenTime = 0;
                    }
                    break;
                case LightState.黄灯闪:
                    {
                        WrtieSystemIo(m_IoRedLight, false);
                        WrtieSystemIo(m_IoGreenLight, false);
                        WrtieSystemIo(m_IoYellowLight, true);
                        m_nYelloTime = nMilliSecond;
                    }
                    break;
                case LightState.绿灯开://绿灯
                    {
                        WrtieSystemIo(m_IoRedLight, false);
                        WrtieSystemIo(m_IoGreenLight, true);
                        WrtieSystemIo(m_IoYellowLight, false);
                        m_nGreenTime = 0;
                    }
                    break;
                case LightState.绿灯闪:
                    {
                        WrtieSystemIo(m_IoRedLight, false);
                        WrtieSystemIo(m_IoGreenLight, true);
                        WrtieSystemIo(m_IoYellowLight, false);
                        m_nGreenTime = nMilliSecond;
                    }
                    break;
                case LightState.蜂鸣开: //开蜂鸣
                    {
                        WrtieSystemIo(m_IoBuzzing, true);
                        m_nBuzzingTime = 0;
                    }
                    break;
                case LightState.蜂鸣关:　//关蜂鸣
                    {
                        WrtieSystemIo(m_IoBuzzing, false);
                        m_nBuzzingTime = 0;
                    }
                    break;
                case LightState.蜂鸣闪:　//关蜂鸣
                    {
                        WrtieSystemIo(m_IoBuzzing, true);
                        m_nBuzzingTime = nMilliSecond;
                    }
                    break;
                default:
                    break;
            }
        }

        private  void LightThread()
        {
            const  int nInterval = 100;
            bool bEnable = false;
            int nBuzzingTime = 0;
            int nGreenTime = 0;
            int nYellowTime = 0;
            while (m_bRunLightThread)
            {
                Thread.Sleep(nInterval);
                if(m_nBuzzingTime > 0 )
                {
                    nBuzzingTime += nInterval;
                    if(nBuzzingTime > m_nBuzzingTime)
                    {
                        nBuzzingTime = 0;
                        WrtieSystemIo(m_IoBuzzing, bEnable);
                    }
                }
                if (m_nGreenTime > 0)
                {
                    nGreenTime += nInterval;
                    if (nGreenTime > m_nGreenTime)
                    {
                        nGreenTime = 0;
                        WrtieSystemIo(m_IoGreenLight, bEnable);
                    }
                }
                if (m_nYelloTime > 0)
                {
                    nYellowTime += nInterval;
                    if (nYellowTime > m_nYelloTime)
                    {
                        nYellowTime = 0;
                        WrtieSystemIo(m_IoYellowLight, bEnable);
                    }
                }
                bEnable = !bEnable;
            }
            return ;
        }
        private void StartLightThread()
        {
            if (m_LightThread == null)
                m_LightThread = new Thread(LightThread);
            if (m_LightThread.ThreadState != ThreadState.Running)
            {
                m_bRunLightThread = true;
                m_LightThread.Start();
            }
        }
        private void StopLightThread()
        {
            if (m_LightThread != null)
            {
                m_bRunLightThread = false;
                if (m_LightThread.Join(5000) == false)
                    m_LightThread.Abort();
                m_LightThread = null;
            }
        }

        /// <summary>
        /// 卡号总数
        /// </summary>
        public int CountCard
        {
            get { return m_listCard.Count; }
        }

        private void IoChanged(int nCard)
        {
            if(IoChangedEvent != null)
            {
                IoChangedEvent(nCard);
            }
        }


        /// <summary>
        /// 根据板卡名字动态加载对应的板卡类
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="nIndex"></param>
        /// <param name="nCardNo"></param>
        private void AddCard(string strName, int nIndex, int nCardNo )
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IoCtrl));
            string strClassName = "MotionIO.IoCtrl_" + strName;
            Type card = assembly.GetType(strClassName);
            if (card == null)
            {
                throw new Exception(string.Format("IO卡{0}找不到可用的封装类，请确认motionio.dll是否正确或配置错误?" + strName));
            }
            else
            {
                object[] obj = { nIndex, nCardNo };
                m_listCard.Add(System.Activator.CreateInstance(card, obj) as IoCtrl);
            }            
        }
        /// <summary>
        ///读取系统配置文件里的IO板卡信息
        /// </summary>
        /// <param name="doc"></param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            m_listCard.Clear();
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "IoCard");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        string strIndex = xe.GetAttribute(m_strIoCard[0]).Trim();
                        string strNo = xe.GetAttribute(m_strIoCard[1]).Trim();
                        string strName = xe.GetAttribute(m_strIoCard[2]).Trim();

                        AddCard(strName, Convert.ToInt32(strIndex), Convert.ToInt32(strNo));
                    }
                }
            }
            ReadIoCfgFromXml(doc, true);
            ReadIoCfgFromXml(doc, false);
            ReadSystemIoFromXml(doc);
        }
   
        /// <summary>
        ///读取系统配置文件里IO输入输出点配置 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bIn"></param>
        private void ReadIoCfgFromXml(XmlDocument doc, bool bIn)
        {
            if (bIn)
                m_dicIn.Clear();
            else
                m_dicOut.Clear();

            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + (bIn ? "IoIn" : "IoOut"));
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        string strCardNo = xe.GetAttribute(m_strIoItem[0]).Trim();
                        string strIndex = xe.GetAttribute(m_strIoItem[1]).Trim();
                        string strName = xe.GetAttribute(m_strIoItem[2]).Trim();
                        int nCard = Convert.ToInt32(strCardNo);
                        int nIndex = Convert.ToInt32(strIndex) ;

                        if(strName != string.Empty)
                        {
                            try
                            {
                                if (bIn)
                                    m_dicIn.Add(strName, (nCard << 8) | (nIndex));
                                else
                                    m_dicOut.Add(strName, (nCard << 8) | nIndex);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString(), strName);
                            }
                        }
 

                        if (nCard-1 < m_listCard.Count)
                        {
                            string[] strArray;
                            if (bIn)
                                strArray = m_listCard.ElementAt(nCard-1).m_strArrayIn;
                            else
                                strArray = m_listCard.ElementAt(nCard-1).m_strArrayOut;
                            if (nIndex-1 < strArray.Length)
                            {
                                strArray[nIndex-1] = strName;
                            }
                            else
                            {
                                string str = string.Format("配置文件Io点配置出错,卡号{0}, IO点{1}", nCard , nIndex);
                                MessageBox.Show(str);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///读取系统配置文件里系统常用IO输入输出点配置 
        /// </summary>
        /// <param name="doc"></param>
        public void ReadSystemIoFromXml(XmlDocument doc)
        {
            m_listIoSystemIn.Clear();
            m_listIoSystemOut.Clear();
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "IoSystemIn");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        IoSysDefine id = new IoSysDefine();
                        id.strName = xe.GetAttribute(m_strIoSystemItem[0]).Trim();
                        string s = xe.GetAttribute(m_strIoSystemItem[1]).Trim();
                        id.nCard = s.Length != 0 ? Convert.ToInt32(s) : 0;
                        s = xe.GetAttribute(m_strIoSystemItem[2]).Trim();
                        id.nBit = s.Length != 0 ? Convert.ToInt32(s) : 0;
                        s = xe.GetAttribute(m_strIoSystemItem[3]).Trim();

                        if(s.Length != 0)
                        {
                            id.bLevel = s == "1" ? true : false;
                        }
                        else
                        {
                            id.bLevel = false;
                        }
                        

                        m_listIoSystemIn.Add(id);
                    }
                }
                int nCount = m_listIoSystemIn.Count;
                for (int i = 0; i < nCount; i++)
                {
                    string strName = m_listIoSystemIn.ElementAt(i).strName;
                    if (strName == "急停")
                    {
                        m_IoEmgStop = m_listIoSystemIn.ElementAt(i);
                    }
                    if (strName == "启动")
                    {
                        m_IoBegin = m_listIoSystemIn.ElementAt(i);
                    }
                    if (strName == "复位")
                    {
                        m_IoResetting = m_listIoSystemIn.ElementAt(i);
                    }
                    if (strName.IndexOf("安全门") != -1)
                    {
                        m_IoDoor = m_listIoSystemIn.ElementAt(i);
                    }
                }
            }

            xnl = doc.SelectNodes("/SystemCfg/" + "IoSystemOut");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        IoSysDefine id = new IoSysDefine();
                        id.strName = xe.GetAttribute(m_strIoSystemItemOut[0]).Trim();
                        string s = xe.GetAttribute(m_strIoSystemItemOut[1]).Trim();
                        id.nCard = s.Length != 0 ? Convert.ToInt32(s) : 0;
                        s = xe.GetAttribute(m_strIoSystemItemOut[2]).Trim();
                        id.nBit = s.Length != 0 ? Convert.ToInt32(s) : 0;
                        s = xe.GetAttribute(m_strIoSystemItemOut[3]).Trim();
                        if (s.Length != 0)
                        {
                            id.bLevel = s == "1" ? true : false;
                        }
                        else
                        {
                            id.bLevel = false;
                        }

                        m_listIoSystemOut.Add(id);
                    }
                }

                int nCount = m_listIoSystemOut.Count;
                for (int i = 0; i < nCount; i++)
                {
                    string strName = m_listIoSystemOut.ElementAt(i).strName;
                    if (strName == "红灯")
                    {
                        m_IoRedLight = m_listIoSystemOut.ElementAt(i);
                    }
                    if (strName == "黄灯")
                    {
                        m_IoYellowLight = m_listIoSystemOut.ElementAt(i);
                    }
                    if (strName == "绿灯")
                    {
                        m_IoGreenLight = m_listIoSystemOut.ElementAt(i);
                    }
                    if (strName.IndexOf("蜂鸣") != -1)
                    {
                        m_IoBuzzing = m_listIoSystemOut.ElementAt(i);
                    }
                }
                string strText = string.Empty;
                if (m_IoRedLight.strName == null || m_IoYellowLight.strName == null
                    || m_IoGreenLight.strName == null || m_IoBuzzing.strName == null)
                {
                    strText = "系统红/黄/绿灯/蜂鸣没配置好";

                }
                else if (m_IoBegin.strName == null || m_IoResetting.strName == null || m_IoEmgStop.strName == null)
                {
                    strText = "系统启动/复位/急停IO配置不正确";

                }
                if (strText.Length > 0)
                {
                    if (MessageBox.Show(strText, "IO配置出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        throw new Exception("IO配置出错:" + strText);
                    }
                }
            }
        }


        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="gridIoCard"></param>
        /// <param name="gridIoIn"></param>
        /// <param name="gridIoOut"></param>
        /// <param name="gridSystemIoIn"></param>
        /// <param name="gridSystemIoOut"></param>
        public void UpdateGridFromParam(DataGridView gridIoCard, DataGridView gridIoIn, DataGridView gridIoOut,
            DataGridView gridSystemIoIn, DataGridView gridSystemIoOut)
        {
            gridIoCard.Rows.Clear();
            gridIoIn.Rows.Clear();
            gridIoOut.Rows.Clear();
            gridSystemIoIn.Rows.Clear();
            gridSystemIoOut.Rows.Clear(); 

            if (m_listCard.Count > 0)
            {

                gridIoCard.Rows.AddCopies(0, m_listCard.Count);

                int i = 0;
                foreach (IoCtrl card in m_listCard)
                {
                    int j = 0;
                    gridIoCard.Rows[i].Cells[j++].Value = card.m_nIndex.ToString();
                    gridIoCard.Rows[i].Cells[j++].Value = card.m_nCardNo.ToString();
                    gridIoCard.Rows[i].Cells[j++].Value = card.m_strCardName;

                    int m = gridIoIn.Rows.Count - 1;
                    gridIoIn.Rows.Add(card.m_strArrayIn.Length);
                    int k = 1;
                    foreach (string strIoInName in card.m_strArrayIn)
                    {
                        gridIoIn.Rows[m].Cells[0].Value = card.m_nIndex .ToString();
                        gridIoIn.Rows[m].Cells[1].Value = k.ToString();
                        gridIoIn.Rows[m].Cells[2].Value = strIoInName;
                        m++;
                        k++;
                    }
                    m = gridIoOut.Rows.Count - 1; ;
                    gridIoOut.Rows.Add(card.m_strArrayOut.Length);
                    k = 1;
                    foreach (string strIoOutName in card.m_strArrayOut)
                    {
                        gridIoOut.Rows[m].Cells[0].Value = card.m_nIndex.ToString();
                        gridIoOut.Rows[m].Cells[1].Value = k.ToString();
                        gridIoOut.Rows[m].Cells[2].Value = strIoOutName;
                        m++;
                        k++;
                    }
                    i++;
                }
            }
            if (m_listIoSystemIn.Count > 0)
            {
                gridSystemIoIn.Rows.Add(m_listIoSystemIn.Count);
                int t = 0;
                foreach (IoSysDefine id in m_listIoSystemIn)
                {
                    gridSystemIoIn.Rows[t].Cells[0].Value = id.strName;
                    gridSystemIoIn.Rows[t].Cells[1].Value = id.nCard.ToString();
                    gridSystemIoIn.Rows[t].Cells[2].Value = id.nBit.ToString();
                    gridSystemIoIn.Rows[t].Cells[3].Value = id.bLevel ? "1" : "0";
                    t++;
                }
            }
            if (m_listIoSystemOut.Count > 0)
            {
                gridSystemIoOut.Rows.Add(m_listIoSystemOut.Count);
                int t = 0;
                foreach (IoSysDefine id in m_listIoSystemOut)
                {
                    gridSystemIoOut.Rows[t].Cells[0].Value = id.strName;
                    gridSystemIoOut.Rows[t].Cells[1].Value = id.nCard.ToString();
                    gridSystemIoOut.Rows[t].Cells[2].Value = id.nBit.ToString();
                    gridSystemIoOut.Rows[t].Cells[3].Value = id.bLevel ? "1" : "0";
                    t++;
                }
            }
        }

        /// <summary>
        /// 更新内存参数
        /// </summary>
        /// <param name="gridIoCard"></param>
        /// <param name="gridIoIn"></param>
        /// <param name="gridIoOut"></param>
        /// <param name="gridSystemIO"></param>
        /// <param name="gridSystemIoOut"></param>
        public void UpdateParamFromGrid(DataGridView gridIoCard, DataGridView gridIoIn, DataGridView gridIoOut,
            DataGridView gridSystemIO, DataGridView gridSystemIoOut)
        {
            int m = gridIoCard.RowCount;
            int n = gridIoCard.ColumnCount;

            m_listCard.Clear(); 
            m_listIoSystemIn.Clear() ;
            m_listIoSystemOut.Clear();

            for (int i = 0; i < m; ++i)
            {
                if (gridIoCard.Rows[i].Cells[0].Value == null)
                    continue;

                string strIndex = gridIoCard.Rows[i].Cells[0].Value.ToString();
                string strNo = gridIoCard.Rows[i].Cells[1].Value.ToString();
                string strName = gridIoCard.Rows[i].Cells[2].Value.ToString();

                AddCard(strName, Convert.ToInt32(strIndex), Convert.ToInt32(strNo));
            }

            m = gridIoIn.RowCount;
            for (int i = 0; i < m; ++i)
            {
                if (gridIoIn.Rows[i].Cells[0].Value == null)
                    continue;
                string strCardNo = gridIoIn.Rows[i].Cells[0].Value.ToString();
                string strIndex = gridIoIn.Rows[i].Cells[1].Value.ToString();
            //    string strName = gridIoIn.Rows[i].Cells[2].Value.ToString();

                if (strCardNo.Length > 0)
                {
                    int nCard = Convert.ToInt32(strCardNo) - 1;
                    int nIndex = Convert.ToInt32(strIndex) - 1;
                    if (gridIoIn.Rows[i].Cells[2].Value != null)
                        m_listCard.ElementAt(nCard).m_strArrayIn[nIndex] = gridIoIn.Rows[i].Cells[2].Value.ToString();
                    else
                        m_listCard.ElementAt(nCard).m_strArrayIn[nIndex] = string.Empty;
                }
            }
            m = gridIoOut.RowCount;
            for (int i = 0; i < m; ++i)
            {
                if (gridIoOut.Rows[i].Cells[0].Value == null)
                    continue;
                string strCardNo = gridIoOut.Rows[i].Cells[0].Value.ToString();
                string strIndex = gridIoOut.Rows[i].Cells[1].Value.ToString();
                //string strName = gridIoOut.Rows[i].Cells[2].Value.ToString();

                if (strCardNo.Length > 0)
                {
                    int nCard = Convert.ToInt32(strCardNo) - 1;
                    int nIndex = Convert.ToInt32(strIndex) - 1;
                    //m_listCard.ElementAt(nCard).m_strArrayOut[nIndex] = strName;
                    if (gridIoOut.Rows[i].Cells[2].Value != null)
                        m_listCard.ElementAt(nCard).m_strArrayOut[nIndex] = gridIoOut.Rows[i].Cells[2].Value.ToString();
                    else
                        m_listCard.ElementAt(nCard).m_strArrayOut[nIndex] = string.Empty;
                }
            }
            m = gridSystemIO.RowCount;
            for (int i = 0; i < m; ++i)
            {
                if (gridSystemIO.Rows[i].Cells[0].Value == null)
                    continue;
                string strName = gridSystemIO.Rows[i].Cells[0].Value.ToString();
                string strCardNo = gridSystemIO.Rows[i].Cells[1].Value.ToString();
                string strIndex = gridSystemIO.Rows[i].Cells[2].Value.ToString();
                string strLev = gridSystemIO.Rows[i].Cells[3].Value.ToString();

                IoSysDefine id = new IoSysDefine();
                id.strName = strName;
                id.nCard = strCardNo.Length != 0 ? Convert.ToInt32(strCardNo) : 0;
                id.nBit = strIndex.Length != 0 ? Convert.ToInt32(strIndex) : 0;

                if (strLev.Length != 0)
                {
                    id.bLevel = strLev == "1" ? true : false;
                }
                else
                {
                    id.bLevel = false;
                }
                m_listIoSystemIn.Add(id);
            }

            m = gridSystemIoOut.RowCount;
            for (int i = 0; i < m; ++i)
            {
                if (gridSystemIoOut.Rows[i].Cells[0].Value == null)
                    continue;
                string strName = gridSystemIoOut.Rows[i].Cells[0].Value.ToString();
                string strCardNo = gridSystemIoOut.Rows[i].Cells[1].Value.ToString();
                string strIndex = gridSystemIoOut.Rows[i].Cells[2].Value.ToString();
                string strLev = gridSystemIoOut.Rows[i].Cells[3].Value.ToString();

                IoSysDefine id = new IoSysDefine();
                id.strName = strName;
                id.nCard = strCardNo.Length != 0 ? Convert.ToInt32(strCardNo) : 0;
                id.nBit = strIndex.Length != 0 ? Convert.ToInt32(strIndex) : 0;
                if (strLev.Length != 0)
                {
                    id.bLevel = strLev == "1" ? true : false;
                }
                else
                {
                    id.bLevel = false;
                }

                m_listIoSystemOut.Add(id);
            }
        }

        /// <summary>
        /// 保存IO板卡,IO输入输出,系统IO信息到配置文件SystemCfg
        /// </summary>
        /// <param name="doc"></param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("IoCard");
            xnl.AppendChild(root);

            foreach (IoCtrl t in m_listCard)
            {
                XmlElement xe = doc.CreateElement("IoCard");
                xe.SetAttribute(m_strIoCard[0], t.m_nIndex.ToString());
                xe.SetAttribute(m_strIoCard[1], t.m_nCardNo.ToString());
                xe.SetAttribute(m_strIoCard[2], t.m_strCardName);
                root.AppendChild(xe);
            }

            xnl = doc.SelectSingleNode("SystemCfg");
            root = doc.CreateElement("IoIn");
            xnl.AppendChild(root);

            foreach (IoCtrl t in m_listCard)
            {
                int n = t.m_strArrayIn.Length;
                for (int i = 0; i < n; ++i)
                {
                    XmlElement xe = doc.CreateElement("IoIn");
                    xe.SetAttribute(m_strIoItem[0], t.m_nIndex.ToString());
                    xe.SetAttribute(m_strIoItem[1], (i+1).ToString());
                    xe.SetAttribute(m_strIoItem[2], t.m_strArrayIn[i]);
                    root.AppendChild(xe);
                }
            }
            xnl = doc.SelectSingleNode("SystemCfg");
            root = doc.CreateElement("IoOut");
            xnl.AppendChild(root);

            foreach (IoCtrl t in m_listCard)
            {
                int n = t.m_strArrayOut.Length;
                for (int i = 0; i < n; ++i)
                {
                    XmlElement xe = doc.CreateElement("IoOut");
                    xe.SetAttribute(m_strIoItem[0], t.m_nIndex.ToString());
                    xe.SetAttribute(m_strIoItem[1], (i+1).ToString());
                    xe.SetAttribute(m_strIoItem[2], t.m_strArrayOut[i]);
                    root.AppendChild(xe);
                }
            }

            xnl = doc.SelectSingleNode("SystemCfg");
            root = doc.CreateElement("IoSystemIn");
            xnl.AppendChild(root);
            foreach (IoSysDefine t in m_listIoSystemIn)
            {
                XmlElement xe = doc.CreateElement("IoSystemIn");
                xe.SetAttribute(m_strIoSystemItem[0], t.strName);
                xe.SetAttribute(m_strIoSystemItem[1], t.nCard.ToString());
                xe.SetAttribute(m_strIoSystemItem[2], t.nBit.ToString());
                xe.SetAttribute(m_strIoSystemItem[3], t.bLevel == true ? "1" : "0");
                root.AppendChild(xe);
            }

            xnl = doc.SelectSingleNode("SystemCfg");
            root = doc.CreateElement("IoSystemOut");
            xnl.AppendChild(root);
            foreach (IoSysDefine t in m_listIoSystemOut)
            {
                XmlElement xe = doc.CreateElement("IoSystemOut");
                xe.SetAttribute(m_strIoSystemItemOut[0], t.strName);
                xe.SetAttribute(m_strIoSystemItemOut[1], t.nCard.ToString());
                xe.SetAttribute(m_strIoSystemItemOut[2], t.nBit.ToString());
                xe.SetAttribute(m_strIoSystemItemOut[3], t.bLevel == true ? "1" : "0");
                root.AppendChild(xe);
            }
        }

        /// <summary>
        /// 读取指定卡的输入状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool ReadIoIn(int nCardIndex,ref int nData)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).ReadIOIn(ref nData);
        }

        /// <summary>
        /// 读取指定卡的输入位状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool ReadIoInBit(int nCardIndex, int nIndex)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).ReadIoInBit(nIndex);
        }

        /// <summary>
        /// 读取IO输入点状态
        /// </summary>
        /// <param name="strIoName">IO 输入点名称</param>
        /// <returns></returns>
        public bool ReadIoInBit(string strIoName)
        {
            Int64 nData;
            if(m_dicIn.TryGetValue(strIoName, out nData))
            {
                return ReadIoInBit((int)(nData >> 8), (int)(nData & 0xff));
            }
            else
            {
                string strInfo = string.Format("不存在的IO输入点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO读取输入点出错");
                return false;
            }
        }

        /// <summary>
        /// 获取指定IO输入点的缓冲状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetIoInState(int nCardIndex, int nIndex)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).GetIoInState(nIndex);
        }


        /// <summary>
        /// 获取指定IO输入点的缓冲状态
        /// </summary>
        /// <param name="strIoName">IO 输入点名称</param>
        /// <returns></returns>
        public bool GetIoInState(string strIoName)
        {
            Int64 nData;
            if (m_dicIn.TryGetValue(strIoName, out nData))
            {
                return GetIoInState((int)(nData >> 8), (int)(nData & 0xff));
            }
            else
            {
                string strInfo = string.Format("不存在的IO输入点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO读取输入缓冲状态出错");
                return false;
            }
        }

        /// <summary>
        /// 读取指定卡的输出状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool ReadIoOut( int nCardIndex,ref int nData)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).ReadIOOut(ref nData);
        }

        /// <summary>
        /// 读取指定卡IO点的输出缓冲状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetIoOutState(int nCardIndex, int nIndex)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).GetIoOutState(nIndex);
        }

        /// <summary>
        /// 读取IO点的输出缓冲状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        /// <returns></returns>
        public bool GetIoOutState(string strIoName)
        {
            Int64 nData;
            if (m_dicOut.TryGetValue(strIoName, out nData))
            {
                return GetIoOutState((int)(nData >> 8), (int)(nData & 0xff));
            }
            else
            {
                string strInfo = string.Format("不存在的IO输出点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO读取输出点缓冲状态出错");
                return false;
            }
        }

        /// <summary>
        /// 获取指定IO输出点的状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool ReadIoOutBit(int nCardIndex, int nIndex)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).ReadIoOutBit(nIndex);
        }

        /// <summary>
        /// 读取IO点的输出状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        /// <returns></returns>
        public bool ReadIoOutBit(string strIoName)
        {
            Int64 nData;
            if (m_dicOut.TryGetValue(strIoName, out nData))
            {
                return ReadIoOutBit((int)(nData >> 8), (int)(nData & 0xff));
            }
            else
            {
                string strInfo = string.Format("不存在的IO输出点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO读取输出点状态出错");
                return false;
            }
        }

        /// <summary>
        /// 设置指定IO输出点的状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        /// <returns></returns>
        public bool WriteIoBit(int nCardIndex, int nIndex, bool bBit)
        {
            if (nCardIndex > m_listCard.Count)
                return false;
            return m_listCard.ElementAt(nCardIndex - 1).WriteIoBit(nIndex, bBit);
        }
        /// <summary>
        /// 设置IO点的输出状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        ///  <param name="bBit">将要被设置的状态</param>
        /// <returns></returns>
        /// 
        public bool WriteIoBit(string strIoName, bool bBit)
        {
            Int64 nData;
            if (m_dicOut.TryGetValue(strIoName, out nData))
            {
                return WriteIoBit((int)(nData >> 8), (int)(nData & 0xff), bBit);
            }
            else
            {
                string strInfo = string.Format("不存在的IO输出点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO设置输出点状态出错");
                return false;
            }
        }

        /// <summary>
        /// 得到输入IO的名字
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public string GetIoInName(int nCardIndex, int nIndex)
        {
            if (nCardIndex > m_listCard.Count)
                return string.Empty;
            return m_listCard.ElementAt(nCardIndex - 1).m_strArrayIn[nIndex];
        }


        /// <summary>
        /// 写入系统功能IO
        /// </summary>
        /// <param name="isd"></param>
        /// <param name="bEnable"></param>
        private void WrtieSystemIo(IoSysDefine isd, bool bEnable )
        {
            if(isd.strName != null)
            {
                WriteIoBit(isd.nCard, isd.nBit, bEnable ? isd.bLevel : !isd.bLevel);
            }
        }
      
 
        /// <summary>
        ///初始化所有IO板卡
        /// </summary>
        /// <returns></returns>
        public bool InitAllCard()
        {
            bool bRet = true;
            foreach (IoCtrl card in m_listCard)
            {
                if (card.Init() == false)
                    bRet = false;
            }

            StartLightThread();
            return bRet;
        }

        /// <summary>
        /// 反初始化所有IO板卡
        /// </summary>
        public void DeinitAllCard()
        {
            StopLightThread();
            foreach (IoCtrl card in m_listCard)
            {
                if (card.IsEnable())
                    card.DeInit();
            }

        }

         /// <summary>
         /// 判断系统ＩＯ是否要触发站位状态变更
         /// </summary>
         /// <param name="isd"></param>
         /// <returns></returns>
         public bool IsSystemIoTrigger(IoSysDefine isd)
        {
            if (isd.strName != null)
            {
                bool bEnable = GetIoInState(isd.nCard, isd.nBit);
                if (bEnable == isd.bLevel)
                {
                    if (isd.bTrigger == false)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }                   
                }
                else
                {
                    if (isd.bTrigger == true)
                    {
                        isd.bTrigger = false;
                    }
                    return false;
                }
            }
            else
                return false;

        }

        /// <summary>
        /// 系统自动扫描线程函数
        /// </summary>
        public override void ThreadMonitor()
        {
            int nData = 0;
            bool nRet = false;

            m_IoResetting.bTrigger = false;
            m_IoBegin.bTrigger = false;
            m_IoEmgStop.bTrigger = false;
            m_IoDoor.bTrigger = false;

            for (int i = 0; i < m_listCard.Count; ++i)//所有卡先预读一次
            {
                nRet = m_listCard.ElementAt(i).ReadIOIn(ref nData);
                if (nRet)
                {
                    if (i == 0)
                    {
                        if (IsSystemIoTrigger(m_IoEmgStop))
                        {
                            StationMgr.GetInstance().EmgStopAllStation();
                            WarningMgr.GetInstance().Error(string.Format("10001,ERR-ESB01-10001,急停按钮被按下"));
                        }
                    }
                }
            }


            while (m_bRunThread)
            {
                for (int i = 0; i < m_listCard.Count; ++i)
                {
                    nRet = m_listCard.ElementAt(i).ReadIOIn(ref nData);
                    if (nRet && m_listCard.ElementAt(i).IsDataChange())
                    {
                        if (i == 0)
                        {
                            if (IsSystemIoTrigger(m_IoEmgStop))
                            {
                                StationMgr.GetInstance().EmgStopAllStation();
                                WarningMgr.GetInstance().Error(string.Format("10001,ERR-ESB01-10001,急停按钮被按下"));
                            }

                            if (IsSystemIoTrigger(m_IoResetting))
                            {
                                StationMgr.GetInstance().ResetAllStation();
                            }

                            if (IsSystemIoTrigger(m_IoBegin))
                            {
                                StationMgr.GetInstance().AutoStartAllStation(); ;
                            }
                            if (IsSystemIoTrigger(m_IoDoor) && SystemMgr.GetInstance().GetParamBool("SafetyDoor"))
                            {
                                StationMgr.GetInstance().PauseAllStation();
                                WarningMgr.GetInstance().Warning("安全门被打开");
                            }
                        }
                        IoChanged(i);
                    }
                    Thread.Sleep(10);
                }
            }
        }
    }
}
