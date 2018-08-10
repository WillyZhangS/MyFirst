using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTool;
using AutoFrameDll;
using System.Windows.Forms;

namespace AutoFrameVision
{
    public class ProductInfo : SingletonTemplate<ProductInfo>
    {
        const string m_strDefaultType = "0610";
        private string m_strCfgName;

        /// <summary>
        /// 产品型号名
        /// </summary>
        private string m_strProductName; 

        public string[] m_strAllType = { m_strDefaultType };
        /// <summary>
        /// 一个下盖总放料个数
        /// </summary>
        public int m_nPanleItemCount = 15;  
        /// <summary>
        /// tray盘工位单个料盘放料总个数
        /// </summary>
        public int m_nTrayItemCount = 63; 
        /// <summary>
        /// 奇数行个数
        /// </summary>
        public int m_nTrayOddNum = 5;  
        /// <summary>
        /// 偶数行个数
        /// </summary>
        public int m_nTrayEvenNum = 4;
        /// <summary>
        /// 总行数
        /// </summary>
        public int m_nTrayRows = 9;
        /// <summary>
        /// tray盘点位的坐标，方向
        /// </summary>
        public struct POS_DIR
        {
            public double x;
            public double y;
            public int nDir;
        };
        public List<POS_DIR> m_listPosDir = new List<POS_DIR>(); //记录tray盘点位坐标信息
        public List<POS_DIR> m_listVertex = new List<POS_DIR>(); //记录tray盘四个顶点点位坐标信息

        /// <summary>
        /// 状态变更委托
        /// </summary>
        public delegate void StateChangedHandler();
        /// <summary>
        /// 定义状态变更事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;


        public ProductInfo()
        {
            m_strCfgName = AppDomain.CurrentDomain.BaseDirectory + "AutoFrame.ini";
            m_strProductName = m_strDefaultType;
        }

        public string ProductName
        {
            get { return m_strProductName; }
        }

        public void LoadProductInfo()
        {
            string strType = IniOperation.GetStringValue(m_strCfgName, "Info", "DefaultType", m_strDefaultType);

            string strAllType = IniOperation.GetStringValue(m_strCfgName, "Info", "AllType", m_strDefaultType);
            m_strAllType = strAllType.Split(',');

            ChangeType(strType);
			
			GetTrayInfo();	
        }

        public int GetProductTypeCount()
        {
            //if(m_strAllType.Length)
            return m_strAllType.Length;
        }

        public void ChangeType(string strTypeName)
        {
            string[] strSection = IniOperation.GetAllSectionNames(m_strCfgName);
            if (strSection.Length > 0)
            {
                if (Array.IndexOf(strSection, strTypeName) != -1)
                {
                    m_strProductName = strTypeName;
                    string strData;
                    strData = IniOperation.GetStringValue(m_strCfgName, strTypeName, "PanelItemCount", "16");
                    m_nPanleItemCount = Convert.ToInt32(strData);

                    strData = IniOperation.GetStringValue(m_strCfgName, strTypeName, "TrayItemCount", "67");
                    m_nTrayItemCount = Convert.ToInt32(strData);
                    strData = IniOperation.GetStringValue(m_strCfgName, strTypeName, "TrayOddNum", "9");
                    m_nTrayOddNum = Convert.ToInt32(strData);
                    strData = IniOperation.GetStringValue(m_strCfgName, strTypeName, "TrayEvenNum", "10");
                    m_nTrayEvenNum = Convert.ToInt32(strData);
                    strData = IniOperation.GetStringValue(m_strCfgName, strTypeName, "TrayRows", "7");
                    m_nTrayRows = Convert.ToInt32(strData);

                    int nCount = 0;
                    for (int i = 0; i < m_nTrayRows; ++i)
                    {
                        if (i % 2 == 1)
                            nCount += m_nTrayOddNum;
                        else
                            nCount += m_nTrayEvenNum;
                    }

                    //计算TRAY的行列值是否与总数匹配
                    if (m_nTrayItemCount != nCount)
                    {
                        MessageBox.Show("产品类型" + m_strProductName + "对应的配置数据不正确，请检查确认！", "提示信息");
                    }
                    else
                    {
                        //保存选中的类型到ini
                        IniOperation.WriteValue(m_strCfgName, "Info", "DefaultType", strTypeName);
                        if ("0396" == m_strProductName)
                        {
                            m_nTrayItemCount -= 2;
                        }
                        else if ("0610" == m_strProductName)
                        {
                            m_nTrayItemCount -= 4;
                        }
                        else if ("0291_0376" == m_strProductName)
                        {
                            m_nTrayItemCount -= 1;
                        }
                    }
                    if (StateChangedEvent != null)
                    {
                        StateChangedEvent();
                    }
                }
                else
                {
                    MessageBox.Show("未找到产品类型" + strTypeName +
                        "对应的配置数据，已加载默认产品类型，\r\n请确认" + m_strCfgName + "是否正确！", "提示信息");
                }
            }
        }

        private void GetTrayInfo()
        {
            m_listPosDir.Clear();
            m_listVertex.Clear();
            switch (m_strProductName)
            {
                case "0254":
                    info_0254();
                    break;
                case "0291_0376":
                    info_0291_0376();
                    break;
                case "0311":
                    info_0311();
                    break;
                case "0396":
                    info_0396();
                    break;
                case "0503":
                    info_0503();
                    break;
                case "0610":
                    info_0610();
                    break;
                default:
                    break;
            }
        }

        private void info_0254()
        {
            double xDis = 42, yDis = 46;
            double xDisCritical = 61.35, yDisCritical = 5.62;
            POS_DIR[] a = new POS_DIR[42];
            for (int i=0; i<42; i++)
            {
                int col = i % 6; //对列数求余
                int row = i / 6;
                if (col < 3)  //小于3
                {
                    a[i].x = col * xDis;
                    a[i].y = row * yDis;
                    a[i].nDir = 1;
                }
                else
                {
                    a[i].x = xDisCritical + (col-1) * xDis;
                    a[i].y = (-1)*yDisCritical + row * yDis;
                    a[i].nDir = -1;
                }
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i=0; i<42; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0254", string.Format("{0}",i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[5]);
            m_listVertex.Add(a[36]);
            m_listVertex.Add(a[41]);
        }

        private void info_0291_0376()
        {
            double xDis = 50, yDis = 48;
            double xDisCritical = 74.81, yDisCritical = 14.91;
            POS_DIR[] a = new POS_DIR[34];
            for (int i = 0; i < 34; i++)
            {
                int col = i % 5; //对列数求余
                int row = i / 5;
                if (i>16)
                {
                    col = (i + 1) % 5;
                    row = (i + 1) / 5;
                }
                if (col < 2)  //小于2
                {
                    a[i].x = col * xDis;
                    a[i].y = row * yDis;
                    a[i].nDir = 1;
                }
                else if (2 == col) //等于2为中间特殊列
                {
                    if (row < 3)
                    {
                        a[i].x = xDisCritical + (col-1) * xDis;
                        a[i].y = yDisCritical + row * yDis;
                        a[i].nDir = -1;
                    }
                    else
                    {
                        a[i].x = col * xDis;
                        a[i].y = row * yDis;
                        a[i].nDir = 1;
                    }
                }
                else
                {
                    a[i].x = xDisCritical + (col-1) * xDis;
                    a[i].y = yDisCritical + row * yDis;
                    a[i].nDir = -1;
                }
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i = 0; i < 34; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0291_0376", string.Format("{0}", i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[4]);
            m_listVertex.Add(a[29]);
            m_listVertex.Add(a[33]);
        }

        private void info_0311()
        {
            double xDis = 40, yDis = 35;
            POS_DIR[] a = new POS_DIR[45];
            for (int i = 0; i < 45; i++)
            {
                int col = i % 5; //对列数求余
                int row = i / 5;
                a[i].x = col * xDis;
                a[i].y = row * yDis;
                a[i].nDir = 1;
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i = 0; i < 45; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0311", string.Format("{0}", i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[4]);
            m_listVertex.Add(a[40]);
            m_listVertex.Add(a[44]);
        }

        private void info_0396()
        {
            double xDis = 48, yDis = 32;
            double xDisCritical = 70.85, yDisCritical = 17.24;
            POS_DIR[] a = new POS_DIR[48];
            for (int i = 0; i < 48; i++)
            {
                int col = i % 5; //对列数求余
                int row = i / 5;
                int colTemp = col;
                int rowTemp = row;
                if (i > 25)
                {
                    col = (i + 2) % 5;
                    row = (i + 2) / 5;
                }
                else if (i > 21)
                {
                    col = (i + 1) % 5;
                    row = (i + 1) / 5;
                }
                
                if (col < 2)  //小于2
                {
                    a[i].x = col * xDis;
                    a[i].y = row * yDis;
                    a[i].nDir = 1;
                }
                else if (2 == col) //等于2为中间特殊列
                {
                    if (row < 4)
                    {
                        a[i].x = xDisCritical + (col - 1) * xDis;
                        a[i].y = (-1)*yDisCritical + row * yDis;
                        a[i].nDir = -1;
                    }
                    else
                    {
                        a[i].x = col * xDis;
                        a[i].y = row * yDis;
                        a[i].nDir = 1;
                    }
                }
                else
                {
                    a[i].x = xDisCritical + (col - 1) * xDis;
                    a[i].y = (-1)*yDisCritical + row * yDis;
                    a[i].nDir = -1;
                }
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i = 0; i < 48; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0396", string.Format("{0}", i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[4]);
            m_listVertex.Add(a[43]);
            m_listVertex.Add(a[47]);
        }

        private void info_0503()
        {
            double xDis = 35, yDis = 35;
            POS_DIR[] a = new POS_DIR[63];
            for (int i = 0; i < 63; i++)
            {
                int col = i % 7; //对列数求余
                int row = i / 7;
                a[i].x = col * xDis;
                a[i].y = row * yDis;
                a[i].nDir = 1;
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i = 0; i < 63; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0503", string.Format("{0}", i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[6]);
            m_listVertex.Add(a[56]);
            m_listVertex.Add(a[62]);
        }

        private void info_0610()
        {
            double xDis = 40, yDis = 36;
            POS_DIR[] a = new POS_DIR[50];
            for (int i = 0; i < 50; i++)
            {
                int col = i % 6; //对列数求余
                int row = i / 6;
                if (i>=49)
                {
                    col = (i + 4) % 6; //对列数求余
                    row = (i + 4) / 6;
                }
                else if (i >= 47)
                {
                    col = (i + 3) % 6; //对列数求余
                    row = (i + 3) / 6;
                }
                else if (i >= 3)
                {
                    col = (i + 2) % 6; //对列数求余
                    row = (i + 2) / 6;
                }
                else if (i >=1)
                {
                    col = (i + 1) % 6; //对列数求余
                    row = (i + 1) / 6;
                }
                a[i].x = col * xDis;
                a[i].y = row * yDis;
                a[i].nDir = 1;
            }
            string m_strProductInfo = AppDomain.CurrentDomain.BaseDirectory + "AutoFrameProductInfo.ini";
            string strInfo = "";
            for (int i = 0; i < 50; i++)
            {
                strInfo = string.Format("{0:0.000},{1:0.000},{2}", a[i].x, a[i].y, a[i].nDir);
                IniOperation.WriteValue(m_strProductInfo, "info_0610", string.Format("{0}", i), strInfo);
                m_listPosDir.Add(a[i]);
            }
            m_listVertex.Add(a[0]);
            m_listVertex.Add(a[3]);
            m_listVertex.Add(a[46]);
            m_listVertex.Add(a[49]);
        }

    }
}
