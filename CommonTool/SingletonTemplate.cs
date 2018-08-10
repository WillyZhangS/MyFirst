using System.Threading;
using System.Windows.Forms;

namespace CommonTool
{
    /// <summary>
    /// 实现单件的模板类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class SingletonTemplate<T> where T : class, new()
    {
        /// <summary>
        /// 实例引用
        /// </summary>
        private static T m_instance;
        /// <summary>
        /// 线程互斥对像
        /// </summary>
        private static readonly object syslock = new object();

        /// <summary>
        /// 日志信息显示委托函数
        /// </summary>
        /// <param name="logListBox"></param>
        /// <param name="strLog"></param>
        public delegate void LogHandler(ListBox logListBox, string strLog);

        /// <summary>
        /// 日志信息显示事件
        /// </summary>
        public event LogHandler LogEvent;

        /// <summary>
        /// 用来显示站位运行日志的列表框
        /// </summary>
        private ListBox m_LogListBox = null;

        /// <summary>
        /// 触发日志显示事件
        /// </summary>
        /// <param name="strLog"></param>
        public void ShowLog(string strLog)
        {
            if (LogEvent != null && m_LogListBox != null)
                LogEvent(m_LogListBox, strLog);
        }

        /// <summary>
        /// 设置日志要显示在哪个列表框上
        /// </summary>
        /// <param name="logListBox"></param>
        public void SetLogListBox(ListBox logListBox)
        {
            m_LogListBox = logListBox;
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            if (m_instance == null )
            {
                lock (syslock)  //用lock关键字获取互斥锁来实现线程单步运行
                {
                    if (m_instance == null)
                    {
                        m_instance = new T();
                    }
                }
            }
            return m_instance;
        }

        /// <summary>
        /// 实例对像
        /// </summary>
        private Thread m_thread = null;
        /// <summary>
        /// 线程是否运行中
        /// </summary>
        protected bool m_bRunThread;                          //线程是否运行中
        /// <summary>
        /// 线程函数
        /// </summary>
        public virtual void ThreadMonitor()
        {
            while (m_bRunThread)
            {

                Thread.Sleep(100);
                return;
            }

        }
        /// <summary>
        ///开始监视线程 
        /// </summary>
        public void StartMonitor()
        {
            if (m_thread == null)
                m_thread = new Thread(ThreadMonitor);
            if (m_thread.ThreadState != ThreadState.Running)
            {
                m_bRunThread = true;
                m_thread.Start();
            }

        }
        /// <summary>
        ///结束监视线程 
        /// </summary>
        public void StopMonitor()
        {
            if (m_thread != null)
            {
                m_bRunThread = false;
                if (m_thread.Join(5000) == false)
                    m_thread.Abort();
                m_thread = null;
            }
        }
    }
}
