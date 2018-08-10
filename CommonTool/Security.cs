using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace CommonTool
{
    /// <summary>
    /// 系统运行模式
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// 生产模式, 对应OP权限
        /// </summary>
        Production,  
        /// <summary>
        /// CPK数据模式，对应客服工程师
        /// </summary>     
        CPK_GRR,
        /// <summary>
        /// 工程师模式，对应软件工程师
        /// </summary>
        Engineer,
    }
    /// <summary>
    /// 安全权限控制类
    /// </summary>
    public class Security
    {
        private static string[] m_strPassword = { "dinnar456", "dinnar456" };

        private static RunMode m_runMode = RunMode.Engineer;

        /// <summary>
        /// 定义一个模式变化委托函数
        /// </summary>
        public delegate void ModeChangedHandler();
        /// <summary>
        /// 定义模式变化事件
        /// </summary>
        public static event ModeChangedHandler ModeChangedEvent;

        /// <summary>
        /// 获取当前模式
        /// </summary>
        /// <returns></returns>
        public static RunMode GetMode()
        {
            return m_runMode;
        }

        /// <summary>
        /// 是否为OP模式
        /// </summary>
        /// <returns></returns>
        public static bool IsOpMode()
        {
            return m_runMode == RunMode.Production;
        }
        /// <summary>
        /// 是否为CPK模式
        /// </summary>
        /// <returns></returns>
        public static bool IsCpkMode()
        {
            return m_runMode == RunMode.CPK_GRR;
        }
        /// <summary>
        /// 是否为工程师模式
        /// </summary>
        /// <returns></returns>
        public static bool IsEngMode()
        {
            return m_runMode == RunMode.Engineer;
        }

        /// <summary>
        /// 系统权限切换到OP等级
        /// </summary>
        public static void ChangeOpMode()
        {
            m_runMode = RunMode.Production;
            if(ModeChangedEvent != null)
              ModeChangedEvent();
        }
        /// <summary>
        /// 系统权限切换到FAE等级
        /// </summary>
        /// <param name="strPassword"></param>
        /// <returns>切换成功或切换失败，成功则对整个系统触发权限变更事件</returns>
        public static bool ChangeFaeMode(string strPassword)
        {
            if (m_strPassword[0] == strPassword)
            {
                m_runMode = RunMode.CPK_GRR;
                if (ModeChangedEvent != null)
                    ModeChangedEvent();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 系统权限切换到工程师等级
        /// </summary>
        /// <param name="strPassword"></param>
        /// <returns>切换成功或切换失败，成功则对整个系统触发权限变更事件</returns>
        public static bool ChangeEngMode(string strPassword)
        {
            if (m_strPassword[1] == strPassword)
            {
                m_runMode = RunMode.Engineer;
                if (ModeChangedEvent != null)
                    ModeChangedEvent();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 从系统中读取密码文件,如果不存在,则创建它
        /// </summary>
        public static void LoadPassword()
        {
            string strPasswordFile = "C:\\Windows\\System32\\AFrame";
            if (LoadPasswordXml(strPasswordFile) == false)
            {
                SavePasswordXml(strPasswordFile);
                LoadPasswordXml(strPasswordFile);
            }
        }

        /// <summary>
        /// 读取密码XML文件
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        private static bool LoadPasswordXml(string strFile)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(strFile);
                if (doc.HasChildNodes)
                {
                    XmlNodeList xnl = doc.SelectNodes("/LoginCodeCfg/Code");
                    if (xnl == null || xnl.Count == 0)
                        return false;
                    xnl = xnl.Item(0).ChildNodes;
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;
                        if (xn.Name == "Code")
                        {
                            m_strPassword[0] = xe.GetAttribute("调试员").Trim();
                            m_strPassword[1] = xe.GetAttribute("工程师").Trim();
                        }
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
		
        /// <summary>
        /// 保存密码文件
        /// </summary>
        /// <param name="strFile"></param>
        private static void SavePasswordXml(string strFile)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec2 = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec2);
            XmlElement root2 = doc.CreateElement("LoginCodeCfg");
            doc.AppendChild(root2);

            if (doc.HasChildNodes)
            {
                XmlNode xnl = doc.SelectSingleNode("LoginCodeCfg");  //子节点
                XmlNode root = doc.CreateElement("Code"); //子节点
                XmlElement rootChild1 = doc.CreateElement("Code");  //子元素
                rootChild1.SetAttribute("调试员", m_strPassword[0]);
                //root.AppendChild(rootChild1);
                //XmlElement rootChild2 = doc.CreateElement("Code");  //子元素
                rootChild1.SetAttribute("工程师", m_strPassword[1]);
                root.AppendChild(rootChild1);
                xnl.AppendChild(root);
            }
            doc.Save(strFile);
        }
    }
}
