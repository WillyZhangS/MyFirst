/********************************************************************
	created:	2013/11/12
	filename: 	CONFIGMGR
	file ext:	cpp
	author:		ShengZhang
	purpose:	配置管理类，实现系统配置的存取
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CommonTool;
using Communicate;

namespace AutoFrameDll
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    public class ConfigMgr : SingletonTemplate<ConfigMgr>
    {
        /// <summary>
        /// 读取系统配置
        /// </summary>
        /// <param name="strFile">配置文件 </param>
        /// <returns></returns>
        public bool LoadCfgFile(string strFile)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + strFile);
            if (doc.HasChildNodes)
            {
                IoMgr.GetInstance().ReadCfgFromXml(doc);
                MotionMgr.GetInstance().ReadCfgFromXml(doc);

                StationMgr.GetInstance().ReadCfgFromXml(doc);
                TcpMgr.GetInstance().ReadCfgFromXml(doc);
                ComMgr.GetInstance().ReadCfgFromXml(doc);
                RobotMgr.GetInstance().ReadCfgFromXml(doc);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存系统配置
        /// </summary>
        /// <param name="strFile">配置文件 </param>
        public void SaveCfgFile(string strFile)
        {
            XmlDocument doc = new XmlDocument();

            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement("SystemCfg");
            doc.AppendChild(root);
            if (doc.HasChildNodes)
            {
                SystemMgr.GetInstance().SaveSystemParamCfgXml(doc);

                IoMgr.GetInstance().SaveCfgXML(doc);

                MotionMgr.GetInstance().SaveCfgXML(doc);
                StationMgr.GetInstance().SaveCfgXML(doc);

                TcpMgr.GetInstance().SaveCfgXML(doc);
                ComMgr.GetInstance().SaveCfgXML(doc);
                RobotMgr.GetInstance().SaveCfgXML(doc);
                doc.Save(strFile);
            }
        }
    }
}


