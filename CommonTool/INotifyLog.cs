using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace CommonTool
{
    /// <summary>
    /// 日志显示通知基类
    /// </summary>
    public interface NotifyLog
    {
        /// <summary>
        /// 
        /// </summary>
        ListView m_ListViewLog { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strLog"></param>
        void ShowLog(string strLog);
    }

    static class LogExtensionMethods
    {
        public static void ShowLogView(this NotifyLog log, string strLog)
        {
            if (log.m_ListViewLog != null && !log.m_ListViewLog.IsDisposed)
            {
                log. m_ListViewLog.Invoke((MethodInvoker)delegate
                {
                    log.m_ListViewLog.Items.Add(strLog);
                    log.m_ListViewLog.EnsureVisible(log.m_ListViewLog.Items.Count - 1);
                });
            }
        }

    }

}
