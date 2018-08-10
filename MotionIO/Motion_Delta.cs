/********************************************************************
	created:	2017/03/03
	filename: 	MOTION_Delta
	author:		
	purpose:	台达运动控制卡的封装类
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using PCI_DMC;
using PCI_DMC_ERR;
using CommonTool;

namespace MotionIO
{
    /// <summary>
    /// 台达板卡
    /// </summary>
    public class Motion_Delta : Motion
    {
        /// <summary>
        /// 构造初始化
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion_Delta(int nCardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            :base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            //nMinAxisNo = nMinAxisNo - 1;
            m_bEnable = false;
        }

        /// <summary>
        ///轴卡初始化 
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            short existcard = 0;
            short ret = CPCI_DMC.CS_DMC_01_open(ref existcard); //open card
            if (existcard <= 0)
            {
                WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡查找失败, result = {0}", ret));
                return false;
            }
            //intial
            ushort i, card_no = 0;
            for (i = 0; i < existcard; i++)
            {
                ret = CPCI_DMC.CS_DMC_01_get_CardNo_seq(i, ref card_no);
                ret = CPCI_DMC.CS_DMC_01_pci_initial(card_no);
                m_nCardIndex = card_no;
                if (ret != 0)
                {
                    WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡初始化失败, result = {0}", ret));
                    return false;
                }
                ret = CPCI_DMC.CS_DMC_01_initial_bus(card_no);
                if (ret != 0)
                {
                    WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡初始化失败, result = {0}", ret));
                    return false;
                }
                else
                {
                    ushort DeviceInfo = 0;
                    uint[] SlaveTable = new uint[4];
                    ret = CPCI_DMC.CS_DMC_01_start_ring(card_no, 0);                      //Start communication                      
                    ret = CPCI_DMC.CS_DMC_01_get_device_table(card_no, ref DeviceInfo);   //Get Slave Node ID 
                    ret = CPCI_DMC.CS_DMC_01_get_node_table(card_no, ref SlaveTable[0]);
                    if (ret != 0)
                    {
                        WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡初始化通讯失败, result = {0}", ret));
                        return false;
                    }
                }
            }
            m_bEnable = true;
            return true;
        }

        /// <summary>
        ///关闭轴卡 
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            short ret = CPCI_DMC.CS_DMC_01_reset_card((ushort)m_nCardIndex); //重置
            CPCI_DMC.CS_DMC_01_close();
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,Delta板卡库文件关闭出错! result = {0}", ret));
                return false;
            }
        }

        /// <summary>
        ///开启使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            //台达卡上电时对报警状态复位
            short ret = CPCI_DMC.CS_DMC_01_set_ralm((ushort)m_nCardIndex, (ushort)nAxisNo, 0);
            Thread.Sleep(50);
            ret = CPCI_DMC.CS_DMC_01_ipo_set_svon((ushort)m_nCardIndex,(ushort)nAxisNo,(ushort)0,(ushort)1);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,Delta Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        ///断开使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            short ret = CPCI_DMC.CS_DMC_01_ipo_set_svon((ushort)m_nCardIndex, (ushort)nAxisNo, (ushort)0, (ushort)0);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,Delta Card Aixs {0} servo off Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        /// 读取伺服使能状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
            if (ret == 0)
            {
                if ((MC_done & (0x01 << 8)) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nMode">回零方式</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nMode)
        {
            nAxisNo = nAxisNo + 1;
            ushort Mode = (ushort)nMode;
            ushort offset = 0;
            ushort lowSpeed = 10, highSpeed = 50;
            double acc = 0.1;
			//TODO, 需要优化，在上层传入mode的差异即可
            if (1 == nAxisNo || 5 == nAxisNo)
                Mode = 19; //顶升气缸原点HOME模式
            else
                Mode = 17; //顶升气缸负极限HOME模式
            if(4 == nAxisNo || 8 == nAxisNo)
            {
                lowSpeed = 100;
                highSpeed = 800;
            }
            short ret = CPCI_DMC.CS_DMC_01_set_home_config((ushort)m_nCardIndex, (ushort)nAxisNo, 
                (ushort)0, Mode, offset, lowSpeed, highSpeed, acc);
            Thread.Sleep(50);
            ret = CPCI_DMC.CS_DMC_01_set_home_move((ushort)m_nCardIndex, (ushort)nAxisNo, 0);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                int nT = 0;
                while (true)
                {
                    nT++;
                    if (600 == nT) //指令发出后3s没检测到回Home动作则认为出错了
                    {
                        if (m_bEnable)
                        {
                            WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home TimeOut Error,result = {1}", nAxisNo, ret));
                        }
                        return false;
                    }
                    if (12 == GetMotionState(nAxisNo-1))//回Home状态中
                    {
                        return true;
                    }
                    Thread.Sleep(5);
                }
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        ///以绝对位置移动 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            nAxisNo = nAxisNo + 1;
            short ret = CPCI_DMC.CS_DMC_01_start_ta_move((ushort)m_nCardIndex, (ushort)nAxisNo, (ushort)0, nPos, 0, nSpeed, 0.1, 0.1);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} abs move Error,result is {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        ///相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            nAxisNo = nAxisNo + 1;
            short ret = CPCI_DMC.CS_DMC_01_start_sr_move((ushort)m_nCardIndex, (ushort)nAxisNo, (ushort)0, nPos, nSpeed/5, nSpeed, 0.05, 0.05);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            { 
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} relative move Error,result is {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        /// 两轴圆弧插补,S型曲线
        /// </summary>
        /// <param name="nAxisNo1"></param>
        /// <param name="nAxisNo2"></param>
        /// <param name="nEndXPos"></param>
        /// <param name="nEndYPos"></param>
        /// <param name="fAngle"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool Arc2xy(int nAxisNo1, int nAxisNo2, int nEndXPos, int nEndYPos, double fAngle, int nSpeed)
        {
            nAxisNo1 = nAxisNo1 + 1;
            nAxisNo2 = nAxisNo2 + 1;
            ushort []NodeIDArray = { (ushort)nAxisNo1, (ushort)nAxisNo2 };
            ushort []SlotID = { 0, 0 };
            short ret = CPCI_DMC.CS_DMC_01_start_sr_arc2_xy((ushort)m_nCardIndex, ref NodeIDArray[2], ref SlotID[2],
                nEndXPos, nEndYPos, fAngle, 0, nSpeed, 0.1, 0.1);
            if (0 == ret)
                return true;
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0},{1} Arc2xy move Error,result is {1}", nAxisNo1, nAxisNo2, ret));
                return false;
            }
        }

        /// <summary>
        ///速度模式
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {

            nAxisNo = nAxisNo + 1;
            return true;
        }

        /// <summary>
        ///jog运动 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool JogMove(int nAxisNo, bool bPositive, int bStrat, int nSpeed)
        {

            nAxisNo = nAxisNo + 1;
            return true;
        }

        /// <summary>
        ///轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            short ret = CPCI_DMC.CS_DMC_01_sd_stop((ushort)m_nCardIndex, (ushort)nAxisNo, 0, 0.1);
            if (ret!=0)
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,Delta Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));
                return false;
            }
            return true;
        }

        /// <summary>
        ///急停 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            short ret = CPCI_DMC.CS_DMC_01_emg_stop((ushort)m_nCardIndex, (ushort)nAxisNo, 0);
            if (ret != 0)
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,Delta Card emg stop axis {0} Error, result = {1}", nAxisNo, ret));
                return false;
            }
            return true;
        }

        /// <summary>
        ///获取轴卡运动状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            ushort MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_done((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
            /*
            MC_done:
            0:运动位移动作停止
            1:依加速时间进行运动位移
            2:依最大速度进行运动位移
            3:依减速时间进行运动位移
            5:指令在FIFO,尚未进入Buffer(伺服或04PI Md2)
            6:Buffer有运动指令(伺服或04PI Md2)
            12:回Home状态中(伺服或04PI Md2)
            21:PC与DSP的Counter没对准(04PI Md1)
            22:FIFO中有指令(04PI Md1)
            23:Buffer Full(04PI Md1)
            */
            if (ret==0)
            {
                return MC_done;
            }
            return -1;
        }

        /// <summary>
        ///获取轴卡运动IO信号 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
            //结合凌华排序0报警、1正限位、2负限位、3原点、4急停、5零位(EZ)、6到位、7励磁(伺服)
            long state = 0xFFD7;//原点、零位给0     1111 1111 1101/D 0111/7
            if (ret == 0)
            {
                if ((MC_done & (0x1 << 9)) == 0) //急停
                    state = state & 0xFFEF;//return 16;
                if ((MC_done & (0x1 << 5)) == 0) //报警
                    state = state & 0xFFFE;//return 1;
                if ((MC_done & (0x1 << 8)) == 0) //未servo on
                    state = state & 0xFF7F; //return 128;
                if ((MC_done & (0x1 << 14)) == 0) //正限位   
                    state = state & 0xFFFD;//return 2;
                if ((MC_done & (0x1 << 15)) == 0) //负限位
                    state = state & 0xFFFB;//return 4;
                if ((MC_done & (0x1 << 10)) == 0) //到位
                    state = state & 0xFFBF;//return 64;
                return state;
            }
            return -1;
            //ret = CPCI_DMC.CS_DMC_01_get_alm_code((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
        }

        /// <summary>
        ///获取轴的当前位置 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override long GetAixsPos(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            int nPos = 0;
            short ret = CPCI_DMC.CS_DMC_01_get_position((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref nPos);
            return nPos;
        }

        /// <summary>
        ///轴是否正常停止 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
            if (ret == 0)
            {
                if ((MC_done & (0x1 << 9)) != 0) //急停
                {
                    Debug.WriteLine("Axis {0} have Emg single \r\n", nAxisNo);
                    return 1;
                }
                if ((MC_done & (0x1 << 5)) != 0) //报警
                {
                    Debug.WriteLine("Axis {0} have Alm single \r\n", nAxisNo);
                    return 2;
                }
                if ((MC_done & (0x1 << 8)) == 0) //未servo on
                {
                    Debug.WriteLine("Axis {0} have servo single \r\n", nAxisNo);
                    return 3;
                }
                if ((MC_done & (0x1 << 14)) != 0) //正限位  
                {
                    Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                    return 4;
                }
                if ((MC_done & (0x1 << 15)) != 0) //负限位
                {
                    Debug.WriteLine("Axis {0} have MEL single \r\n", nAxisNo);
                    return 5;
                }
                if ((MC_done & (0x1 << 10)) != 0 && 0 == GetMotionState(nAxisNo-1)) //到位
                    return 0;
                return -1;
            }
            else
                return -1;//调用异常
        }

        /// <summary>
        /// 轴号是否在范围内
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            int nRet = IsAxisNormalStop(nAxisNo-1);
            if (nRet == 0)
            {
                int nTargetPos = 0;
                int nPos = 0;
                CPCI_DMC.CS_DMC_01_get_target_pos((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref nTargetPos);
                CPCI_DMC.CS_DMC_01_get_position((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref nPos);

                if (Math.Abs(nPos - nTargetPos) > 10000)
                    return 6;  //轴停止后位置超限
            }
            return nRet;
        }

        /// <summary>
        ///位置置零 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool SetPosZero(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            //清零时候给清除报警
            int ret = CPCI_DMC.CS_DMC_01_set_ralm((ushort)m_nCardIndex, (ushort)nAxisNo, 0);
            Thread.Sleep(50);
            return (CPCI_DMC.CS_DMC_01_set_position((ushort)m_nCardIndex, (ushort)nAxisNo, 0, 0)==0);
        }

        /// <summary>
        /// 回原点是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            nAxisNo = nAxisNo + 1;
            //回原点到位需要多个判断,轴正常停止且回Home动作已经完成
            if ((0 == IsAxisNormalStop(nAxisNo-1) && 0 == GetMotionState(nAxisNo-1)))
                return 0;
            else
                return -1;
        }
    }
}
