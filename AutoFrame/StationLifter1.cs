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
    /// 工位1顶料站
    /// </summary>
    class StationLifter1 : StationBase
    {
        private int m_nFpcCount = 15;
        private int m_nStaCount = 0;
        private int m_nDistance = 0;

        public StationLifter1(string strName) : base(strName)
        {
            io_in = new string[] { "2.27", "2.28" };
            io_out = new string[] { "1.7", "1.15", "2.16", "2.21" };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
            m_nFpcCount = ProductInfo.GetInstance().m_nPanleItemCount;
            m_nStaCount = m_nFpcCount % 2 == 1 ? m_nFpcCount / 2 + 1 : m_nFpcCount / 2;
            m_nDistance = 100000 / m_nFpcCount;
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
            IoMgr.GetInstance().WriteIoBit(1, 7, false); //托盘1拍照OK
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1下降到位, false.ToString()));
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1下降到位, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1在顶升状态, false);
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停

        public override void StationDeinit()
        {
            ShowLog("断U轴伺服");
            MotionMgr.GetInstance().ServoOff(AxisU);
        }

        public override void StationInit()
        {
            WaitRegBit((int)SysBitReg.机器人1初始化完成, true);//等待机器人复位
            z_up();
            ShowLog("伺服上电");
            MotionMgr.GetInstance().ServoOn(AxisU);
            Thread.Sleep(50);
            MotionMgr.GetInstance().Home(AxisU, 2); //伺服电机,台达板卡控制,第二个参数为复位方式
            WaitHome(AxisU, 50000);
            ShowLog("U轴回升降安全点");
            MotionMgr.GetInstance().AbsMove(AxisU, m_dicPoint[1].u, 50000);//回安全点
            WaitMotion(AxisU, -1);
            z_down();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1下降到位, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1下降到位, true);
            ShowLog("初始化完成");
        }

        /// <summary>
        /// 升降气缸上抬
        /// </summary>
        private void z_up()
        {
            ShowLog("升降气缸上抬");
            IoMgr.GetInstance().WriteIoBit(2, 16, false);
            IoMgr.GetInstance().WriteIoBit(2, 21, true);
            WaitIo(2, 27, false, 10000);
            WaitIo(2, 28, true, 10000);
        }

        /// <summary>
        /// 升降气缸下降
        /// </summary>
        private void z_down()
        {
            ShowLog("升降气缸下降");
            IoMgr.GetInstance().WriteIoBit(2, 21, false);
            IoMgr.GetInstance().WriteIoBit(2, 16, true);
            WaitIo(2, 27, true, 10000);
            WaitIo(2, 28, false, 10000);
        }

        /// <summary>
        /// 拍照失败重试，待用
        /// </summary>
        /// <returns></returns>
        private bool snap_t21()
        {
            for (int i = 0; i < 3; i++)
            {
                if (VisionMgr.GetInstance().ProcessStep("T2_1")
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                {
                    ShowLog("通知机器人拍下盖完成");
                    IoMgr.GetInstance().WriteIoBit(1, 7, true);//告诉机器人拍下盖完成
                    WaitIo(1, 7, false);
                    WaitIo(1, 7, true);
                    return true;
                }
                else
                {
                    //未拍到重复三次
                    //return false;
                }
            }
            return false;
        }

        public override void StationProcess()
        {
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1在顶升状态, false);
            WaitRegBit((int)SysBitReg.顶料工位1产品到位, true);//等待流水线阻挡到位
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1产品到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1产品到位, false);
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1在顶升状态, true);
            IoMgr.GetInstance().WriteIoBit(1, 15, true); //打开光源2
            WaitTimeDelay(500);
            z_up();
            bool bIsLastThrow1 = false;
            double nAngleOffset = 0;
            bool bFirst = true;
            for (int i = 0; i < m_nStaCount; i++)
            {
                if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.机器人1抛料))
                {
                    WarningMgr.GetInstance().Info("等待机器人1抛料");
                    i--;
                    ShowLog("通知机器人1托盘1拍照OK,并等待机器人1组装完成");
                    IoMgr.GetInstance().WriteIoBit(1, 7, true); //告诉机器人1托盘1拍照OK
                    WaitIo(1, 7, false, -1, false, false);
                    WaitIo(1, 7, true, -1, false, false); //等待IO1.7的一个上升沿
                    IoMgr.GetInstance().WriteIoBit(1, 7, false);
                }
                else
                {
                    if (bIsLastThrow1)
                        break;
                    WarningMgr.GetInstance().Info("顶料1工位转一格");
                    int nPos = (int)(nAngleOffset * 100000 / 360);
                    MotionMgr.GetInstance().AbsMove(AxisU, m_dicPoint[2].u + i * m_nDistance + nPos, 55000); //第i个拍照点
                    WaitMotion(AxisU, -1);
                    WaitTimeDelay(200);
                    if ((ProductInfo.GetInstance().ProductName != "0503" && VisionMgr.GetInstance().ProcessStep("T2_1"))
                        || (ProductInfo.GetInstance().ProductName == "0503" && VisionMgr.GetInstance().ProcessStep("T2_1") && VisionMgr.GetInstance().ProcessStep("T2_503"))
                        || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                    {

                        if (bFirst )
                        {
                            bFirst = false;
                            i--;
                            Vision_T2_503 vb = (Vision_T2_503)(VisionMgr.GetInstance().GetVisionBase("T2_503"));
                            nAngleOffset = vb.m_ModelAngle - SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T21_A);
                        }
                        else
                        {
                            if (SystemMgr.GetInstance().Mode != SystemMode.Other_Mode)
                            {
                                ShowLog("通知机器人1托盘1拍照OK,并等待机器人1组装完成");
                                IoMgr.GetInstance().WriteIoBit(1, 7, true); //告诉机器人1托盘1拍照OK
                                WaitIo(1, 7, false, -1, false, false);
                                WaitIo(1, 7, true, -1, false, false); //等待IO1.7的一个上升沿
                                IoMgr.GetInstance().WriteIoBit(1, 7, false);
                            }
                        }
                    }
                    else
                    {
                        WarningMgr.GetInstance().Info("顶料1工位未拍到不组装");
                        //未拍到不组装,不直接转到下一个位置,报警人工处理
                        IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                        while (true)
                        {
                            WaitTimeDelay(100);
                            if (DialogResult.Yes == ShowMessage("顶料1工位下盖拍照失败，请检查！\n继续等待点击<继续运行>？转到下一个位置点击<退出运行>？"))
                            {
                                i--;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    }
                }
                if (i == m_nStaCount - 1)
                {
                    if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.机器人1抛料))
                    {
                        WarningMgr.GetInstance().Info("等待机器人1抛料");
                        i--;
                        ShowLog("通知机器人1托盘1拍照OK,并等待机器人1组装完成");
                        bIsLastThrow1 = true;
                    }
                }
            }
            IoMgr.GetInstance().WriteIoBit(1, 15, false); //关闭光源2
            MotionMgr.GetInstance().AbsMove(AxisU, m_dicPoint[1].u, 50000);//回安全点
            WaitMotion(AxisU, -1);
            WarningMgr.GetInstance().Info("顶料1工位回安全点");
            z_down();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1下降到位, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1下降到位, true);//告诉流水线组装完成
        }
    }
}
