using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using AutoFrameDll;

namespace AutoFrameVision
{
    /// <summary>
    /// 用于四点标定的标定转换类
    /// </summary>
    public class TrayPointTranslate
    {
        /// <summary>
        /// 计算变化的矩阵
        /// </summary>
        HTuple m_hommatId;
        /// <summary>
        /// 四点标定的X像素座标
        /// </summary>
        HTuple m_xPixel = new HTuple();
        /// <summary>
        /// 四点标定的Y像素座标
        /// </summary>
        HTuple m_yPixel = new HTuple();
        /// <summary>
        /// 四点标定的X物理座标
        /// </summary>
        HTuple m_xPos = new HTuple();
        /// <summary>
        /// 四点标定的Y物理座标
        /// </summary>
        HTuple m_yPos = new HTuple();

        public HTuple m_xMaxOffset=0;
        public HTuple m_yMaxOffset=0;

        HDevelopExport hde = new HDevelopExport();

        /// <summary>
        /// 构造函数,预先定义矩阵ID
        /// </summary>
        public TrayPointTranslate()
        {
            HOperatorSet.HomMat2dIdentity(out m_hommatId);
        }

        /// <summary>
        /// 应用四点标定的矩阵进行座标转换
        /// </summary>
        /// <param name="xIn"></param>
        /// <param name="yIn"></param>
        /// <param name="xOut"></param>
        /// <param name="yOut"></param>
        /// <returns></returns>
        public bool Translate(double xIn, double yIn, out double xOut, out double yOut)
        {
           HTuple x, y;
            try
            {
                HOperatorSet.AffineTransPoint2d(m_hommatId, xIn, yIn, out x, out y);
            }
            catch (HalconException HDevExpDefaultException1)
            {
                System.Windows.Forms.MessageBox.Show(HDevExpDefaultException1.ToString());
                xOut = yOut = 0;
                return false;
            }
            xOut = x.D;
            yOut = y.D;
            return true;
        }

        /// <summary>
        /// 清空坐标映射数据及中心数据
        /// </summary>
        public void ClearAllData()
        {
            ClearPointData();

            m_xMaxOffset = 0;
            m_yMaxOffset = 0;
        }
        /// <summary>
        /// 清楚坐标映射数据
        /// </summary>
        public void ClearPointData()
        {
            m_xPixel = new HTuple(); 
            m_yPixel = new HTuple(); 
            m_xPos = new HTuple(); 
            m_yPos = new HTuple(); 
        }

        public void InitData()
        {
            ClearAllData();
            for (int i = 0; i < 4; i++)
            {
                m_xPixel.Append(0);
                m_yPixel.Append(0);
                m_xPos.Append(0);
                m_yPos.Append(0);
            }
        }

        /// <summary>
        /// 存储的座标的个数
        /// </summary>
        /// <returns></returns>
        public int GetDataLength()
        {
            return m_xPixel.Length;
        }
        /// <summary>
        /// 获取指定行列的座标
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public string GetData(int Row, int col)
        {
            if(Row < m_xPixel.Length && col < 4)
            {
                if (col == 0)
                    return m_xPixel[Row].D.ToString();
                else if (col == 1)
                    return m_yPixel[Row].D.ToString();
                else if (col == 2)
                    return m_xPos[Row].D.ToString();
                else if (col == 3)
                    return m_yPos[Row].D.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 向座标列表中加入座标映射数据
        /// </summary>
        /// <param name="xPixel"></param>
        /// <param name="yPixel"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public void AppendPointData(double xPixel, double yPixel, double xPos, double yPos)
        {
            m_xPixel.Append(xPixel);
            m_yPixel.Append(yPixel);
            m_xPos.Append(xPos);
            m_yPos.Append(yPos);
        }

        /// <summary>
        /// 根据四点标定的数据计算矩阵,像素大小，同时计算旋转中心，并计算标定的评估数据
        /// </summary>
        public bool CalcCalib()
        {
            m_hommatId = 0;
            try
            {
                BuildPointMap();
                GetMaxOffset(out m_xMaxOffset, out m_yMaxOffset);
            }
            catch (HalconException HDevExpDefaultException1)
            {
                System.Windows.Forms.MessageBox.Show(HDevExpDefaultException1.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算标定转换后的最大偏差值
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        private bool GetMaxOffset(out HTuple xOffset, out HTuple yOffset)
        {
            HTuple hommatId = 0;
            HTuple xPix, yPix;
            try
            {
                HOperatorSet.HomMat2dIdentity(out hommatId);
                HOperatorSet.VectorToHomMat2d(m_xPos, m_yPos, m_xPixel, m_yPixel, out hommatId);
                HOperatorSet.AffineTransPoint2d(hommatId, m_xPos, m_yPos, out xPix , out yPix);
                xPix =  xPix.TupleSub(m_xPixel);
                yPix = yPix.TupleSub(m_yPixel);

                xOffset = xPix.TupleMax().D;
                yOffset = yPix.TupleMax().D;
            }
            catch (HalconException HDevExpDefaultException1)
            {
                System.Windows.Forms.MessageBox.Show(HDevExpDefaultException1.ToString());
                xOffset = 9999;
                yOffset = 9999;
                return false;
            }
            return true;
        }

        private void   BuildPointMap()
        {
            HOperatorSet.VectorToHomMat2d(m_xPixel, m_yPixel, m_xPos, m_yPos, out m_hommatId);
        }
    }
}
