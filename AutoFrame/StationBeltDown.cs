using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AutoFrameDll;
using CommonTool;
using Communicate;
using AutoFrameVision;


namespace AutoFrame
{
    /// <summary>
    /// 下层流水线站
    /// </summary>
    class StationBeltDown : StationBase
    {
        private bool m_bCylStatus1 = true;//气缸1状态
        private bool m_bCylStatus2 = true;//气缸2状态
        private DateTime m_IdleTime; //开始空闲时间
        private bool m_bIdle = false;
        private bool m_bBeltSts = false;
        private int m_nOldData = 0; 

        public StationBeltDown(string strName):base(strName)
        {
            io_in = new string[] { "3.1", "3.2" };
            io_out = new string[] { "3.1", "3.2", "3.3" };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
        }

       //当所有站位均为全自动运行模式时，不需要重载该函数
        //当所有站位为半自动运行模式时，也不需要重载该函数， 只需要在站位流程开始时插入WaitBegin()即可保证所有站位同步开始。
        //当所有站位中，有的为半自动，有的为全自动时，半自动的站位不重载该函数，使用WaitBegin()控制同步，全自动的站位重载该函数返回true即可。
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public override bool IsReady()
        //{
        //    return true;
        //}


        public override void InitSecurityState()
        {
            ShowLog("阻挡气缸1/2上升");
            IoMgr.GetInstance().WriteIoBit(3, 2, false); //下层传送阻挡气缸1
            IoMgr.GetInstance().WriteIoBit(3, 3, false); //下层传送阻挡气缸2
            m_bCylStatus1 = true;
            m_bCylStatus2 = true;
        }

        public override void EmgStop()
        {
            ShowLog("关闭下层流水线皮带");
            IoMgr.GetInstance().WriteIoBit(3, 1, false); //关闭下层传送
            m_bIdle = false;
            m_bBeltSts = false;
        }//站位急停

        public override void StationDeinit()
        {
            ShowLog("关闭下层流水线皮带");
            IoMgr.GetInstance().WriteIoBit(3, 1, false); //关闭下层传送
        }

        public override void StationInit()
        {
            //如果感应2有料,感应1无料,则把阻挡气缸2下降
            if (IoMgr.GetInstance().GetIoInState(3, 2) && !IoMgr.GetInstance().GetIoInState(3, 1))
            {
                ShowLog("感应2有料,感应1无料,阻挡气缸2下降");
                IoMgr.GetInstance().WriteIoBit(3, 3, true); //下层传送阻挡气缸2下降
                m_bCylStatus2 = false;
            }
            IoMgr.GetInstance().ReadIoIn(3, ref m_nOldData);
            m_nOldData &= 0x3;
            m_bIdle = false;
            m_bBeltSts = false;
            ShowLog("开启下层流水线皮带");
            IoMgr.GetInstance().WriteIoBit(3, 1, true); //下层传送皮带
            ShowLog("初始化完成");
        }

        /// <summary>
        /// 判断固定点位电平是否有跳变
        /// </summary>
        /// <param name="nOldData"></param>
        /// <param name="nNewData"></param>
        /// <param name="nIndex"></param>
        /// <param name="bLevel"></param>
        /// <returns></returns>
        private bool IsIoTrigger(int nOldData, int nNewData, int nIndex, bool bLevel)
        {
            bool bEnable = (nNewData & (1 << (nIndex-1))) != 0;//true高 false低
            if (bEnable != bLevel)
                return false;
            bool bOldEnable = (nOldData & (1 << (nIndex - 1))) != 0;
            if (bEnable != bOldEnable)
                return true;
            else
                return false;
        }

        public override void StationProcess()
        {
            int nData = 0;
            IoMgr.GetInstance().ReadIoIn(3, ref nData);
            nData &= 0x3;
            if (m_nOldData != nData)
            {
                if (IsIoTrigger(m_nOldData, nData, 1, false))//如果需要使气缸延时上升,添加控制变量？
                {
                    ShowLog("感应1下降沿,气缸1上升");
                    IoMgr.GetInstance().WriteIoBit(3, 2, false); //气缸1上升
                    m_bCylStatus1 = true;
                }
                if (IsIoTrigger(m_nOldData, nData, 2, false))
                {
                    ShowLog("感应2下降沿,气缸2上升");
                    IoMgr.GetInstance().WriteIoBit(3, 3, false); //气缸2上升
                    m_bCylStatus2 = true;
                }

                if (IsIoTrigger(m_nOldData, nData, 1, true))
                {
                    ShowLog("感应1上升沿");
                    if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.进料皮带下降到位))
                    {
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带下降到位, false.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带下降到位, false);
                        ShowLog("进料平台下降到位,气缸1下降");
                        IoMgr.GetInstance().WriteIoBit(3, 2, true); //气缸1下降
                        m_bCylStatus1 = false;
                        if (IoMgr.GetInstance().GetIoInState(3, 2))
                        {
                            ShowLog("感应2有料,气缸2下降");
                            IoMgr.GetInstance().WriteIoBit(3, 3, true); //气缸2下降
                            m_bCylStatus2 = false;
                        }
                    }
                }

                //此处感应2不能为触发模式,因为如果感应1/2有料->感应1无料,在感应1下降沿时,感应2电平没有跳变,程序就会卡死在此状态
                if (IoMgr.GetInstance().GetIoInState(3, 2) && (false == IoMgr.GetInstance().GetIoInState(3, 1)))
                //if (IsIoTrigger(m_nOldData, nData, 2, true) && (false == IoMgr.GetInstance().GetIoInState(3, 1)))
                {
                    ShowLog("感应1无料,感应2有料,气缸2下降");
                    IoMgr.GetInstance().WriteIoBit(3, 3, true); //气缸2下降
                    m_bCylStatus2 = false;
                }

                //两个气缸是上升状态,且都感应有料,停留5s再关闭传送带,因为如果此时有两个挨着的治具,前一个治具还没进入进料平台,
                //后一个治具就触发感应1上升沿,此时又进入到感应1/2都有料,气缸都为上升,则立马会关闭传送带。会进入死状态。
                if (!m_bBeltSts && m_bCylStatus1 && m_bCylStatus2 && IoMgr.GetInstance().GetIoInState(3, 1) && IoMgr.GetInstance().GetIoInState(3, 2))
                {
                    ShowLog("两个气缸是上升状态,且都感应有料,停留5s再关闭传送带");
                    m_bIdle = true;
                    m_bBeltSts = true;
                    m_IdleTime = DateTime.Now;
                    //IoMgr.GetInstance().WriteIoBit(3, 1, false); //关闭下层传送皮带
                }
                else
                {
                    if (m_bBeltSts)
                    {
                        m_bBeltSts = false;
                        ShowLog("传送带从关闭状态转为开启");
                        WarningMgr.GetInstance().Info("传送带从关闭状态转为开启");
                        IoMgr.GetInstance().WriteIoBit(3, 1, true); //开启下层传送皮带
                    }
                    m_bIdle = false;
                }

                m_nOldData = nData;
            }

            if (m_bIdle && (DateTime.Now - m_IdleTime).TotalSeconds > 5)
            {
                m_IdleTime = DateTime.Now;
                if (IoMgr.GetInstance().GetIoInState(2, 15) && !IoMgr.GetInstance().GetIoInState(2, 16)) //添加判断,进料平台不在下边才关闭
                {
                    ShowLog("进料平台在下边,关闭下层传送带");
                    WarningMgr.GetInstance().Info("进料平台在下边,关闭下层传送带");
                    IoMgr.GetInstance().WriteIoBit(3, 1, false); //关闭下层传送皮带
                }
                m_bIdle = false;
            }

            CheckContinue();
        }

    }
}
