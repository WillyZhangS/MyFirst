using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTool;


namespace MotionIO
{
    /// <summary>
    /// 运动控制卡封装基类
    /// </summary>
    public abstract class Motion
    {
        /// <summary>
        /// 判定卡是否启用或初始化成功
        /// </summary>
        protected bool m_bEnable = false;
        /// <summary>
        /// 当前卡在系统中的索引号
        /// </summary>
        protected int m_nCardIndex = 0;
        /// <summary>
        /// 卡类型名称
        /// </summary>
        private string m_strName = string.Empty;
        /// <summary>
        /// 卡类型支持的最小轴号
        /// </summary>
        private int m_nMinAxisNo = 0;
        /// <summary>
        /// 卡类型支持的最大轴号
        /// </summary>
        private int m_nMaxAxisNo=16;				

        /// <summary>
        /// 构造初始化
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion(int nCardIndex,string strName, int nMinAxisNo, int nMaxAxisNo)
        {
            m_nCardIndex = nCardIndex;
            m_strName = strName;
            m_nMinAxisNo = nMinAxisNo;
            m_nMaxAxisNo = nMaxAxisNo;
        }
        /// <summary>
        /// 卡是否启用成功
        /// </summary>
        /// <returns></returns>
        public bool IsEnable()
        {
            return m_bEnable;

        }

        /// <summary>
        ///以绝对位置移动 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool AbsMove(int nAxisNo, int nPos, int nSpeed);

        /// <summary>
        ///相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool RelativeMove(int nAxisNo, int nPos, int nSpeed);

        /// <summary>
        ///速度模式
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool VelocityMove(int nAxisNo, int nSpeed);
        
        /// <summary>
        ///jog运动 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool JogMove(int axis, bool bPositive, int bStrat, int nSpeed);
       
        /// <summary>
        ///获取轴卡运动状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract long GetMotionState(int nAxisNo);
        
        /// <summary>
        ///获取轴卡运动IO信号 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract long GetMotionIoState(int nAxisNo);
        /// <summary>
        ///获取轴的当前位置 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract long GetAixsPos(int nAxisNo);
       
        /// <summary>
        ///轴是否正常停止 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract int IsAxisNormalStop(int nAxisNo);

        /// <summary>
        /// 轴号是否在范围内
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract int IsAxisInPos(int nAxisNo);

        /// <summary>
        ///回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nMode"></param>
        /// <returns></returns>
        public abstract bool Home(int nAxisNo, int nParam);
        
        /// <summary>
        ///位置置零 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool SetPosZero(int nAxisNo);

        /// <summary>
        ///开启使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool ServoOn(int nAxisNo);

        /// <summary>
        ///断开使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool ServoOff(int nAxisNo);

        /// <summary>
        /// 读取伺服使能状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool GetServoState(int nAxisNo);
        
        /// <summary>
        ///轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool StopAxis(int nAxisNo);

        /// <summary>
        ///急停 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool StopEmg(int nAxisNo);

        /// <summary>
        ///轴卡初始化 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        ///关闭轴卡 
        /// </summary>
        /// <returns></returns>
        public abstract bool DeInit();
        /// <summary>
        /// 回原点是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract int IsHomeNormalStop(int nAxisNo);

        /// <summary>
        ///获取运动控制卡名称 
        /// </summary>
        /// <returns></returns>
        public string GetCardName() { return m_strName; }

        /// <summary>
        /// 获取最小轴号
        /// </summary>
        /// <returns></returns>
        public int GetMinAxisNo() { return m_nMinAxisNo; }

        /// <summary>
        /// //获取最大轴号
        /// </summary>
        /// <returns></returns>
        public int GetMaxAxisNo() { return m_nMaxAxisNo; }    
        
        /// <summary>
        /// 判断当前轴号是否属于此卡
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool AxisInRang(int nAxisNo)
        {
            return nAxisNo >= m_nMinAxisNo && nAxisNo <= m_nMaxAxisNo;
        }

        /// <summary>
        /// 获取卡的系统索引号
        /// </summary>
        /// <returns></returns>
        public int GetCardIndex()
        {
            return m_nCardIndex;
        }
    }
}
