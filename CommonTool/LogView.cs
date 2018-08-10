using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonTool
{
   /// <summary>
   /// 日志显示到列表框的基类
   /// </summary>
   public class LogView
    {
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
        protected void ShowLog(string strLog)
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
    }
}
