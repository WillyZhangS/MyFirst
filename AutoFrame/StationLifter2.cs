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
    /// 工位2顶料站
    /// </summary>
    class StationLifter2 : StationBase
    {
        private int m_nFpcCount = 15;
        private int m_nStaCount = 0;
        private int m_nDistance = 0;
        public StationLifter2(string strName):base(strName)
        {
            io_in = new string[] { "2.29", "2.30" };
            io_out = new string[] { "1.8", "1.16", "2.17", "2.22" };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
            m_nFpcCount = ProductInfo.GetInstance().m_nPanleItemCount;
            m_nStaCount = m_nFpcCount / 2;
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
            IoMgr.GetInstance().WriteIoBit(1, 8, false); //托盘2拍照OK
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2下降到位, false.ToString()));
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2下降到位, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2在顶升状态, false);
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
            WaitRegBit((int)SysBitReg.机器人2初始化完成, true);//等待机器人复位
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
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2下降到位, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2下降到位, true);
            ShowLog("初始化完成");
        }

        /// <summary>
        /// 升降气缸上抬
        /// </summary>
        private void z_up()
        {
            ShowLog("升降气缸上抬");
            IoMgr.GetInstance().WriteIoBit(2, 17, false);
            IoMgr.GetInstance().WriteIoBit(2, 22, true);
            WaitIo(2, 29, false, 10000);
            WaitIo(2, 30, true, 10000);
        }

        /// <summary>
        /// 升降气缸下降
        /// </summary>
        private void z_down()
        {
            ShowLog("升降气缸下降");
            IoMgr.GetInstance().WriteIoBit(2, 22, false);
            IoMgr.GetInstance().WriteIoBit(2, 17, true);
            WaitIo(2, 29, true, 10000);
            WaitIo(2, 30, false, 10000);
        }

        public override void StationProcess()
        {
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2在顶升状态, false);
            WaitRegBit((int)SysBitReg.顶料工位2产品到位, true);//等待流水线阻挡到位
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2产品到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2产品到位, false);
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2在顶升状态, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2在顶升状态, true);
            IoMgr.GetInstance().WriteIoBit(1, 16, true); //打开光源3
            WaitTimeDelay(500);
            z_up();
            bool bIsLastThrow2 = false;
            double nAngleOffset = 0;
            bool bFirst = true;
            for (int i = 0; i < m_nStaCount; i++)
            {
                if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.机器人2抛料))
                {
                    WarningMgr.GetInstance().Info("等待机器人2抛料");
                    i--;
                    ShowLog("通知机器人2托盘2拍照OK,并等待机器人2组装完成");
                    IoMgr.GetInstance().WriteIoBit(1, 8, true); //告诉机器人1托盘1拍照OK
                    WaitIo(1, 8, false, -1, false, false);
                    WaitIo(1, 8, true, -1, false, false); //等待IO1.8的一个上升沿
                    IoMgr.GetInstance().WriteIoBit(1, 8, false);
                }
                else
                {
                    if (bIsLastThrow2)
                        break;
                    WarningMgr.GetInstance().Info("顶料2工位转一格");
                    int nPos = (int)(nAngleOffset * 100000 / 360);
                    MotionMgr.GetInstance().AbsMove(AxisU, m_dicPoint[2].u - i * m_nDistance+ nPos, 55000); //第i个拍照点
                    WaitMotion(AxisU, -1);
                    WaitTimeDelay(200);
                    if ((ProductInfo.GetInstance().ProductName != "0503" && VisionMgr.GetInstance().ProcessStep("T3_1"))
                        || (ProductInfo.GetInstance().ProductName == "0503" && VisionMgr.GetInstance().ProcessStep("T3_1") && VisionMgr.GetInstance().ProcessStep("T3_503"))
                        || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                    {
                        if (bFirst)
                        {
                            bFirst = false;
                            i--;
                            Vision_T3_503 vb = (Vision_T3_503)(VisionMgr.GetInstance().GetVisionBase("T3_503"));
                            nAngleOffset = vb.m_ModelAngle - SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T31_A);
                        }
                        else
                        {
                            if (i==m_nStaCount-1)
                            {
                                Vision_T3_503 vb = (Vision_T3_503)(VisionMgr.GetInstance().GetVisionBase("T3_503"));
                                double fTA = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T31_A);
                                double ModelA = vb.m_ModelAngle;
                                if (ModelA < 0)
                                {
                                    ModelA = 360 + ModelA;
                                }
                                if (fTA < 0)
                                {
                                    fTA = 360.0 + fTA;
                                } //模板角度减去实际角度,偏差角,大于0时T5往逆时针(正脉冲)补偿,小于0时相反
                                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T3_OffsetA1, ModelA - fTA - nAngleOffset, false);
                            }
                            if (SystemMgr.GetInstance().Mode != SystemMode.Other_Mode)
                            {
                                ShowLog("通知机器人2托盘2拍照OK,并等待机器人2组装完成");
                                IoMgr.GetInstance().WriteIoBit(1, 8, true); //告诉机器人1托盘1拍照OK
                                WaitIo(1, 8, false, -1, false, false);
                                WaitIo(1, 8, true, -1, false, false); //等待IO1.8的一个上升沿
                                IoMgr.GetInstance().WriteIoBit(1, 8, false);
                            }
                        }
                    }
                    else
                    {
                        WarningMgr.GetInstance().Info("顶料2工位未拍到不组装");
                        //未拍到不组装,不直接转到下一个位置,报警人工处理
                        IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                        while (true)
                        {
                            WaitTimeDelay(100);
                            if (DialogResult.Yes == ShowMessage("顶料2工位下盖拍照失败，请检查！\n继续等待点击<继续运行>？转到下一个位置点击<退出运行>？"))
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
                if (i == m_nStaCount-1)
                {
                    if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.机器人2抛料))
                    {
                        WarningMgr.GetInstance().Info("等待机器人2抛料");
                        i--;
                        bIsLastThrow2 = true;
                    }
                }
            }
            IoMgr.GetInstance().WriteIoBit(1, 16, false); //关闭光源3
            MotionMgr.GetInstance().AbsMove(AxisU, m_dicPoint[1].u, 50000);//回安全点
            WaitMotion(AxisU, -1);
            WarningMgr.GetInstance().Info("顶料2工位回安全点");
            z_down();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2下降到位, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2下降到位, true);//告诉流水线组装完成
        }
    }
}
