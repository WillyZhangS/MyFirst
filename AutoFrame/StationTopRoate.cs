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
    /// 上盖转盘站
    /// </summary>
    class StationTopRoate : StationBase
    {
        public enum trayState
        {
            UnKnow = -1,
            NONE = 0,
            Have = 1,
        }
        trayState[] m_trayState = new trayState[6];
        public StationTopRoate(string strName):base(strName)
        {
            io_in = new string[] { "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.30" };
            io_out = new string[] { "3.4" };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
        }

        public override void InitSecurityState()
        {
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖转盘就绪, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖转盘就绪, true);
            for (int i = 0; i < m_trayState.Length; i++)
            {
                m_trayState[i] = trayState.UnKnow;
            }
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停
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
        public override void StationDeinit()
        {
            ShowLog("断Z轴伺服");
            MotionMgr.GetInstance().ServoOff(AxisZ);
        }

        public override void StationInit()
        {
            WaitRegBit((int)SysBitReg.上盖转盘就绪, false);//等待上盖移载站Z轴上升完成
            ShowLog("伺服上电");
            MotionMgr.GetInstance().ServoOn(AxisZ);
            ShowLog("上盖转盘站Z轴往初始化点运动");
            MotionMgr.GetInstance().Home(AxisZ, -1);//步进电机用8254卡
            ShowLog("上盖转盘站Z轴往初始化点运动");
            WaitHome(AxisZ, 50000);
            //MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[1].z, 10000);
            //WaitMotion(AxisZ);
            if (!IoMgr.GetInstance().GetIoInState(3, 3) && !IoMgr.GetInstance().GetIoInState(3, 4))
            {
                if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                    throw new StationException("上盖转盘卡在中间位置");
            }
            ShowLog("上盖转盘气缸回原始位");
            IoMgr.GetInstance().WriteIoBit(3, 4, false); //气缸回原始位
            WaitIo(3, 3, true, 5000);
            WaitIo(3, 4, false, 5000);
            ShowLog("初始化完成");
        }

  
        private bool IsTrayEmpty()
        {
            for (int i = 0; i < m_trayState.Length; i++)
            {
                if (m_trayState[i] != trayState.NONE)
                    return false;
            }
            return true;
        }

        private void RotatePanel(bool bInitState = false)
        {
            if (Math.Abs(MotionMgr.GetInstance().GetAixsPos(AxisZ)) > 1000)
            {
                IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                ShowMessage("上盖转盘Z轴没回到零位,禁止转盘转动");
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                return;
            }
            //气缸点动动作
            ShowLog("上盖转盘气缸回工作位");
            IoMgr.GetInstance().WriteIoBit(3, 4, true); //气缸到工作位,转盘旋转一次
            WaitIo(3, 3, false, 5000);
            WaitIo(3, 4, true, 5000);
            Thread.Sleep(200);
            ShowLog("上盖转盘气缸回原始位");
            IoMgr.GetInstance().WriteIoBit(3, 4, false); //气缸回原始位
            WaitIo(3, 3, true, 5000);
            WaitIo(3, 4, false, 5000);
            if (bInitState)
            {
                for (int i = 0; i < m_trayState.Length; i++)
                {
                    m_trayState[i] = trayState.UnKnow;
                }
            }
            else
            {
                trayState temp = m_trayState[5];
                for (int i = m_trayState.Length - 1; i > 0; i--)
                {
                    m_trayState[i] = m_trayState[i - 1];
                }
                m_trayState[0] = temp;
            }
        }

        public void CheckRoate()
        {
            while (true)
            {
                WaitTimeDelay(50);
                if (IoMgr.GetInstance().GetIoInState(3, 8)) //检测料盘有无
                {
                    m_trayState[0] = trayState.Have;
                }
                else
                {
                    m_trayState[0] = trayState.NONE;
                }
                if (IsTrayEmpty())
                {
                    IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                    IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                    if (DialogResult.Yes == ShowMessage("上盖转盘所有tray盘已空\n请更换料盘后按下启动按钮"))
                    //if (DialogResult.Yes == MessageBox.Show("上盖转盘所有tray盘已空\n请更换料盘后按下启动按钮","上盖换料提示", 
                    //MessageBoxButtons.YesNo))
                    {
                        WaitIo(1, 3, false);
                        WaitIo(1, 3, true);
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        RotatePanel(true);
                    }
                    else
                    {
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        WaitTimeDelay(2000); //空闲2s,可以停止程序
                    }
                }
                else
                    break;
            }
        }

        public override void StationProcess()
        {
            CheckRoate();
            if (m_trayState[1] != trayState.NONE)
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, 10000); //到第一个点,刚接触料盘
                WaitMotion(AxisZ, -1);
                while (true)
                {
                    WaitTimeDelay(500);//Thread.Sleep(50);
                    if (false == IoMgr.GetInstance().GetIoInState(3, 7) && false == IoMgr.GetInstance().GetIoInState(3, 30)) //上盖转盘当前tray盘无料
                    { 
                        if ((Math.Abs(MotionMgr.GetInstance().GetAixsPos(AxisZ) + m_dicPoint[2].z - m_dicPoint[1].z)) <= Math.Abs(m_dicPoint[3].z)) //判断轴有没有运动到最高点
                        {
                            //步进轴往上升一格
                            MotionMgr.GetInstance().RelativeMove(AxisZ, m_dicPoint[2].z - m_dicPoint[1].z, 10000);
                            WaitMotion(AxisZ, -1);
                        }
                        else if (Math.Abs(Math.Abs(m_dicPoint[3].z) - Math.Abs(MotionMgr.GetInstance().GetAixsPos(AxisZ))) >= (m_dicPoint[2].z - m_dicPoint[1].z) / 4)
                        {
                            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[3].z, 10000); //到最高点
                            WaitMotion(AxisZ, -1);
                        }
                        else
                        {
                            m_trayState[1] = trayState.NONE;
                            MotionMgr.GetInstance().AbsMove(AxisZ, 100/*m_dicPoint[1].z*/, 10000); //回初始化位
                            WaitMotion(AxisZ, -1);
                            break;
                        }
                    }//end if (true == IoMgr.GetInstance().GetIoInState(3, 7))
                    else
                    {
                        m_trayState[1] = trayState.Have;
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖转盘就绪, true.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖转盘就绪, true);
                        WaitRegBit((int)SysBitReg.上盖转盘就绪, false);  //等待搬运站取走一片料
                    }//end else
                    
                }//end while (true)
            }
            RotatePanel();
        }

    }
}
