using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using log4net;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace CommonTool
{
    /// <summary>
    /// 报警事件类封装
    /// </summary>
    public class WarningEventData : EventArgs
    {
        /// <summary>
        ///判断是增加还是删除信息 
        /// </summary>
        public bool bAdd;
        /// <summary>
        ///增加或删除信息的索引号 
        /// </summary>
        public int nIndex;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bAdd"></param>
        /// <param name="nIndex"></param>
        public WarningEventData(bool bAdd, int nIndex)
        {
            this.bAdd = bAdd;
            this.nIndex = nIndex;
        }
    }
    /// <summary>
    /// 报警日志管理类
    /// </summary>
    public class WarningMgr : SingletonTemplate<WarningMgr>
    {    
        /// <summary>
        /// 报警数据结构体
        /// </summary>
        public struct WARNING_DATA
        {
            /// <summary>
            /// 报警时间
            /// </summary>
            public DateTime tm;    
            /// <summary>
            /// 报警等级
            /// </summary>
            public string strLevel; 
            /// <summary>
            /// 报警错误码
            /// </summary>
            public string strCode;  
            /// <summary>
            /// 报警种类
            /// </summary>
            public string strCategory;  
            /// <summary>
            /// 报警信息
            /// </summary>
            public string strMsg;      
        }
        /// <summary>
        /// 内存中的错误列表
        /// </summary>
        private List<WARNING_DATA> m_listError = new List<WARNING_DATA>();
        /// <summary>
        /// 报警事件
        /// </summary>
         public event EventHandler WarningEventHandler;

        /// <summary>
        /// 多线程互斥锁
        /// </summary>
        private readonly object m_syncLock = new object();
        private bool m_bEnableSnapScreen = true;

        /// <summary>
        ///初始化,加载启动目录下的配置文件log4net.config 
        /// </summary>
        public WarningMgr()
        {

        }

        /// <summary>
        /// 读取日志参数配置
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public bool ReadXmlConfig(string strFile)
        {
         //   string strPath = System.Windows.Forms.Application.StartupPath;
            CheckLogPath();
            FileInfo f = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            log4net.Config.XmlConfigurator.Configure(f);

            return true;
        }

        /// <summary>
        /// 根据当前配置的日志文件路径变更log4net的存储路径
        /// </summary>
        private void CheckLogPath()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                string lsvName = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
                doc.Load(lsvName);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/configuration/" + "log4net");
                    bool bFlag = false;
                    if (xnl.Count > 0)
                    {
                        xnl = xnl.Item(0).ChildNodes;
                        if (xnl.Count > 0)
                        {
                            foreach (XmlNode xn in xnl)
                            {
                                if (xn.Name == "appender")
                                {
                                    XmlElement xe = (XmlElement)xn;
                                    XmlNodeList xx = xe.ChildNodes;
                                    foreach (XmlNode xxn in xx)
                                    {
                                        if (xxn.Name == "file")
                                        {
                                            XmlElement xxe = (XmlElement)xxn;
                                            string strSubDir = xxe.GetAttribute("value");
                                            if (strSubDir.Length > 0)
                                            {
                                                string strLogSavePath = SystemMgr.GetInstance().GetParamString("LogSavePath");
                                                //string []arrayString = strSubDir.Split('\\');
                                                string firstString="", SecondString="";
                                                int indexEnd = strSubDir.LastIndexOf('\\');  //查找最后一个反斜杠
                                                int indexStart = strSubDir.LastIndexOf('\\', strSubDir.Length-2, strSubDir.Length - 1); //查找除去最后一位后最后出现的正斜杠,并不一定是倒数第二个
                                                if (indexEnd == -1 && indexStart == -1)  //路径中没有反斜杠,查找正斜杠
                                                {
                                                    indexEnd = strSubDir.LastIndexOf('/');  //查找最后一个反斜杠
                                                    indexStart = strSubDir.LastIndexOf('/', strSubDir.Length - 2, strSubDir.Length - 1); //查找除去最后一位后最后出现的正斜杠,并不一定是倒数第二个
                                                }
                                                if (indexStart!=-1 && indexEnd!=-1 && indexEnd>indexStart) 
                                                {
                                                    firstString = strSubDir.Substring(0, indexStart);
                                                    SecondString = strSubDir.Substring(indexStart + 1, indexEnd-indexStart-1);
                                                    if (strLogSavePath != firstString)
                                                    {
                                                        strLogSavePath = strLogSavePath + '\\' + SecondString + '\\';
                                                        xxe.SetAttribute("value", strLogSavePath);
                                                        bFlag = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((indexEnd==-1 || indexEnd==indexStart) && (indexStart!=-1)) //结尾没有斜杠的情况或者找到的索引相等
                                                    {
                                                        firstString = strSubDir.Substring(0, indexStart);
                                                        SecondString = strSubDir.Substring(indexStart + 1, strSubDir.Length - indexStart - 1);
                                                        strLogSavePath = strLogSavePath + '\\' + SecondString + '\\';
                                                        xxe.SetAttribute("value", strLogSavePath);
                                                        bFlag = true;
                                                    }
                                                    else   //其他情况返回报错
                                                    {
                                                        MessageBox.Show("log4net.config配置文件路径设置错误");
                                                        bFlag = false;
                                                        return;
                                                    }
                                                }
                                            } /* end of if (strSubDir.Length > 0) */
                                        } 
                                    } 
                                } 
                            } 
                        }
                    } /* end of if (xnl.Count > 0) */
                    if (true == bFlag)
                    {
                        doc.Save(lsvName);
                    }                   
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "日志系统配置文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///保存一条信息日志 
        /// </summary>
        /// <param name="strMsg"></param>
        public void Info(string strMsg)
        {
            LogManager.GetLogger("root").Info(strMsg);
        }

        /// <summary>
        ///系统产生警告时调用,增加一条警告日志,并保存到错误日志文件和系统日志 
        /// </summary>
        /// <param name="strMsg"></param>
        public void Warning(string strMsg)
        {
            //CString ss = CStationMgr::_Instance()->GetCurState();
            //CStationMgr::_Instance()->SwitchToPause();
            //SystemMgr.GetInstance().WriteBit(Bit_Show_Error, true);

            AddNewError(strMsg, "WARN");
            LogManager.GetLogger("root").Warn(strMsg);
            //      LogManager.GetLogger("info").Warning(strMsg);
        }

        /// <summary>
        ///系统产生错误时调用,增加一条错误日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="strMsg"></param>
        public void Error(string strMsg)
        {
            //CString ss = CStationMgr::_Instance()->GetCurState();
            //CStationMgr::_Instance()->SwitchToEmg();
            //SystemMgr.GetInstance().WriteBit(Bit_Show_Error, true);
            AddNewError(strMsg, "ERROR");
            LogManager.GetLogger("root").Error(strMsg);
            //      LogManager.GetLogger("info").Error(strMsg);
            //  CSystem::_Instance()->ScreenShot();

        }


        /// <summary>
        ///增加一条新的错误信息 
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="strLevel"></param>
        void AddNewError(string strMessage, string strLevel)
        {
            WARNING_DATA wd = new WARNING_DATA();
            wd.tm = DateTime.Now;

            //ls.Add(DateTime.Now.ToShortDateString());
            //ls.Add(DateTime.Now.ToShortTimeString());
            wd.strLevel = strLevel;

            string[] data = strMessage.Split(',');
            if(data.Length == 1)
            {
                wd.strCode = "0000";
                wd.strCategory = "ERR-UND";
                wd.strMsg = strMessage;
            }
            else
            {
                wd.strCode = data[0];
                wd.strCategory = data[1];
                if (data.Length > 3)
                {
                    StringBuilder sb = new StringBuilder(data[2]);
                    for (int k = 3; k < data.Length; ++k)
                    {
                        sb.Append(",");
                        sb.Append(data[k]);
                    }
                    wd.strMsg = sb.ToString();
                }
                else
                    wd.strMsg = data[2];
            }
            lock (m_syncLock)  //获取互斥锁同步调用
            {
                m_listError.Add(wd);   //增加一条信息
                WarningEventHandler.Invoke(this, new WarningEventData(true, m_listError.Count - 1));//同步调用
                if (m_bEnableSnapScreen)
                {
                    SnapScreen(); //错误则截屏
                    m_bEnableSnapScreen = false;
                }
            }

        }


        /// <summary>
        ///判断当前是否存在错误信息 
        /// </summary>
        /// <returns></returns>
        public bool HasErrorMsg()
        {
            return m_listError.Count > 0;
        }

        /// <summary>
        /// 当前错误报警信息的总个数
        /// </summary>
        public int Count
        {
            get { return m_listError.Count; }
        }

        /// <summary>
        ///得到错误信息中的最后一条信息 
        /// </summary>
        /// <returns></returns>
        public WARNING_DATA GetLastMsg()
        {
            return m_listError.Last();
        }

        /// <summary>
        /// 获取指定索引位的错误信息
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public WARNING_DATA GetWarning(int nIndex)
        {
            if(nIndex < m_listError.Count)
            {
                return m_listError.ElementAt(nIndex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("WarningMgr GetWarning, nIndex > m_listError.Count");
                return new WARNING_DATA();
            }

        }


        /// <summary>
        ///保存错误信息到错误日志文件 
        /// </summary>
        /// <param name="wd"></param>
        private void SaveError(WARNING_DATA wd)
        {
            StringBuilder sb = new StringBuilder(wd.tm.ToShortDateString());
            sb.Append(",");
            sb.Append(wd.tm.ToLongTimeString());
            sb.Append(",");
            sb.Append(wd.strLevel);
            sb.Append(",");
            sb.Append(wd.strCode);
            sb.Append(",");
            sb.Append(wd.strCategory);
            sb.Append(",");
            sb.Append(wd.strMsg);
            sb.Append(",");

            TimeSpan span = DateTime.Now - wd.tm;
            sb.Append(span.ToString(@"hh\:mm\:ss"));

            //todo , errorlog的目录指定
            LogManager.GetLogger("ErrorLog").Info(sb.ToString());
        }


        /// <summary>
        ///清除m_listError的一条信息,并保存此条信息到错误日志文件 
        /// </summary>
        /// <param name="nIndex"></param>
        public void ClearWarning(int nIndex)
        {
            //最后退出时不需要通知
            if (nIndex < 0)
                return;

            if (nIndex < m_listError.Count)
            {
                lock (m_syncLock)
                {
                    SaveError(m_listError.ElementAt(nIndex));
                    m_listError.RemoveAt(nIndex);
                }
                WarningEventHandler.Invoke(this, new WarningEventData(false, nIndex));//同步调用
            }
            if(m_listError.Count == 0)
            {
                m_bEnableSnapScreen = true;
            }
        }


        /// <summary>
        ///清空当前错误信息列表,并保存信息到错误日志文件 
        /// </summary>
        public void ClearAllWarning()
        {
            //最后退出时不需要通知
            if (m_listError.Count > 0)
            {
                lock (m_syncLock)
                {
                    foreach (WARNING_DATA wd in m_listError)
                    {
                        SaveError(wd);
                    }
                    m_listError.Clear();
                }
                WarningEventHandler.Invoke(this, new WarningEventData(false, -1));//同步调用
                m_bEnableSnapScreen = true;
            }
        }

        /// <summary>
        /// 截屏
        /// </summary>
        public void SnapScreen()
        {
            Action<object> action = (object obj) =>
            {
                int width = Screen.PrimaryScreen.Bounds.Width;
                int height = Screen.PrimaryScreen.Bounds.Height;
                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));

                DateTime dt = DateTime.Now;
                string strPath = SystemMgr.GetInstance().GetImagePath() + "\\ErrorPicture\\" +
                                string.Format("{0:yyyy-MM-dd}", dt);

                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFilename = strPath + "\\" + string.Format("{0:HH-mm-ss-fff}.png", dt);
                bmp.Save(strFilename, ImageFormat.Png);
            };
            Task t1 = new Task(action, "");
            t1.Start();
        }
    }
}
