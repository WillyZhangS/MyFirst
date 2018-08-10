using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using CommonTool;
using MotionIO;
using System.Windows.Forms;
using System.Reflection;

namespace AutoFrameDll
{
    /// <summary>
    /// 控制板卡管理类
    /// </summary>
    public class MotionMgr : SingletonTemplate<MotionMgr>
    {
        /// <summary>
        /// 板卡属性项
        /// </summary>
        private static string[] m_strMotionCard = { "序号", "卡类型", "最小轴号", "最大轴号" };
        private List<Motion> m_listCard = new List<Motion>();
      
        /// <summary>
        /// 类构造函数
        /// </summary>
        public MotionMgr()
        {
 
        }
        private void AddCard(string strName, int nCardNo, int nAxisMin, int nAxisMax)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Motion));
            string strClassName = "MotionIO.Motion_" + strName;
            Type card = assembly.GetType(strClassName);
            if (card == null)
            {
                throw new Exception(string.Format("运动控制卡{0}找不到可用的封装类，请确认motionio.dll是否正确或配置错误?" + strName));
            }
            else
            {
                object[] obj = { nCardNo,strName, nAxisMin, nAxisMax };
                m_listCard.Add(System.Activator.CreateInstance(card, obj) as Motion);
            }
        }

        /// <summary>
        ///读取系统配置文件里的运动卡信息 
        /// </summary>
        /// <param name="doc"></param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            m_listCard.Clear();
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "Motion");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;
                        string strNo = xe.GetAttribute(m_strMotionCard[0]);
                        string strName = xe.GetAttribute(m_strMotionCard[1]);
                        string strMin = xe.GetAttribute(m_strMotionCard[2]);
                        string strMax = xe.GetAttribute(m_strMotionCard[3]);

                        AddCard(strName, Convert.ToInt32(strNo), Convert.ToInt32(strMin), Convert.ToInt32(strMax) );
                     }
                }
            }
        }

        /// <summary>
        ///更新表格中的配置信息 
        /// </summary>
        /// <param name="grid"></param>
        public void UpdateGridFromParam(DataGridView grid)
        {
            grid.Rows.Clear();
            if (m_listCard.Count > 0)
            {
             
                grid.Rows.AddCopies(0, m_listCard.Count);

                int i = 0;
                foreach (Motion card in m_listCard)
                {
                    int j = 0;
                    grid.Rows[i].Cells[j++].Value = card.GetCardIndex().ToString();
                    grid.Rows[i].Cells[j++].Value = card.GetCardName();
                    grid.Rows[i].Cells[j++].Value = card.GetMinAxisNo().ToString();
                    grid.Rows[i].Cells[j++].Value = card.GetMaxAxisNo().ToString();

                    i++;
                }
            }
        }

        /// <summary>
        ///更新表格中的数据到内存参数 
        /// </summary>
        /// <param name="grid"></param>
        public void UpdateParamFromGrid(DataGridView grid)
        {
            int m = grid.RowCount;
            int n = grid.ColumnCount;

            m_listCard.Clear();

            for (int i = 0; i < m; ++i)
            {
                if (grid.Rows[i].Cells[0].Value == null) 
                    continue;

                string strNo = grid.Rows[i].Cells[0].Value.ToString();
                string strName = grid.Rows[i].Cells[1].Value.ToString();
                string strMin = grid.Rows[i].Cells[2].Value.ToString();
                string strMax = grid.Rows[i].Cells[3].Value.ToString();

                AddCard(strName, Convert.ToInt32(strNo), Convert.ToInt32(strMin), Convert.ToInt32(strMax));
             }
        }

        /// <summary>
        ///保存配置到xml文件
        /// </summary>
        /// <param name="doc"></param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("Motion");
            xnl.AppendChild(root);

            foreach (Motion t in m_listCard)
            {
                XmlElement xe = doc.CreateElement("Motion");

                int j = 0;
                xe.SetAttribute(m_strMotionCard[j++], t.GetCardIndex().ToString());
                xe.SetAttribute(m_strMotionCard[j++], t.GetCardName());
                xe.SetAttribute(m_strMotionCard[j++], t.GetMinAxisNo().ToString());
                xe.SetAttribute(m_strMotionCard[j++], t.GetMaxAxisNo().ToString());

                root.AppendChild(xe);
            }
        }
   
        /// <summary>
        ///得到轴所在的板卡对象 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public Motion GetMotionCard(int nAxisNo)
        {
            foreach(Motion m in m_listCard)
            {
                if(m.AxisInRang(nAxisNo))
                    return m;
            }
            WarningMgr.GetInstance().Error(string.Format("50109, ERR-SSW,轴号索引值{0}错误,无法找到对应的运动控制卡", nAxisNo));
            return null;
        }


        private double GetSpeedPercent()
        {
            int n = SystemMgr.GetInstance().GetParamInt("SystemSpeed");
            if (n == 0)
                return 1.0;
            else
                return (double)n / 100;
        }
        /// <summary>
        /// 绝对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.AbsMove(nAxisNo - pMotion.GetMinAxisNo(), nPos,
                    (int)(nSpeed * GetSpeedPercent()));
            else
                return false;
            
        }
        
        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.RelativeMove(nAxisNo - pMotion.GetMinAxisNo(), nPos, (int)(nSpeed * GetSpeedPercent()));
            else
                return false;
            
        }

        /// <summary>
        /// jog运动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool JogMove(int nAxisNo, bool bPositive, int bStrat, int nSpeed)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null && nAxisNo != 0)
                return pMotion.JogMove(nAxisNo - pMotion.GetMinAxisNo(), bPositive, bStrat, (int)(nSpeed * GetSpeedPercent()));
            else
                return false;
          
        }

        /// <summary>
        /// 相对运行
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool VelocityMove(int nAxisNo, int nSpeed)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.VelocityMove(nAxisNo - pMotion.GetMinAxisNo(), (int)(nSpeed * GetSpeedPercent()));
            else
                return false;
         }

        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public long GetMotionState(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.GetMotionState(nAxisNo - pMotion.GetMinAxisNo());
            else
                return -1;
          }

        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public long GetMotionIoState(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.GetMotionIoState(nAxisNo - pMotion.GetMinAxisNo());
            else
                return -1;
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public long GetAixsPos(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.GetAixsPos(nAxisNo - pMotion.GetMinAxisNo());
            else
                return 0xFFFFFFFF;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsAxisNormalStop(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.IsAxisNormalStop(nAxisNo - pMotion.GetMinAxisNo());
            else
                return -1;
        }
        
        /// <summary>
        /// 判定轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        ///
        public int IsAxisInPos(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.IsAxisInPos(nAxisNo - pMotion.GetMinAxisNo());
            else
                return -1;
        }

        /// <summary>
        /// 回原点 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nParam">针对不同类型的卡，此参数含义不同，可以是方向，也可以是模式,也可以不使用</param>
        /// <returns></returns>
        public bool Home(int nAxisNo, int nParam)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.Home(nAxisNo - pMotion.GetMinAxisNo(), nParam);
            else
                return false;
        }

        /// <summary>
        /// 位置置零
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool SetPosZero(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.SetPosZero(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 开启使能
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool ServoOn(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.ServoOn(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 断开使能
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool ServoOff(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.ServoOff(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 读取使能状态
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool GetServoState(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.GetServoState(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool StopAxis(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.StopAxis(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool StopEmg(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.StopEmg(nAxisNo - pMotion.GetMinAxisNo());
            else
                return false;
        }

        /// <summary>
        /// 回原点过程中检测是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsHomeNormalStop(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.IsHomeNormalStop(nAxisNo - pMotion.GetMinAxisNo());
            else
                return 0;

        }

        /// <summary>
        /// 获取运动控制卡名称
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        string GetCardName(int nAxisNo)
        {
            Motion pMotion = GetMotionCard(nAxisNo);
            if (pMotion != null)
                return pMotion.GetCardName();
            else
                return null;
        }

        /// <summary>
        ///初始化所有轴卡 
        /// </summary>
        /// <returns></returns>
        public bool InitAllCard()
        {
            bool bRet = true;
            foreach (Motion card in m_listCard)
            {
                if(card.Init() == false)
                    bRet = false ;
            }
            return bRet;
        }

        /// <summary>
        /// 反初化所有轴卡
        /// </summary>
        public void DeinitAllCard()
        {
            foreach (Motion card in m_listCard)
            {
                if (card.IsEnable() )
                    card.DeInit();
            }
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        public override void ThreadMonitor()
        {
            while (m_bRunThread)
            {

                Thread.Sleep(100);
            }

        }

    }
}
