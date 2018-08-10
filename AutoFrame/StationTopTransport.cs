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
using System.IO;

namespace AutoFrame
{
    /// <summary>
    /// 上盖移载站
    /// </summary>
    class StationTopTransport : StationBase
    {   
        public StationTopTransport(string strName) : base(strName)
        {
            io_in = new string[] { "3.5", "3.6", "3.9", "3.10", "3.11", "3.12", "3.13", "3.14", "3.15", "3.16", "3.17"};
            io_out = new string[] { "1.18", "3.5", "3.6", "3.7", "3.8", "3.9", "3.19", "3.20" };
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
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装完成, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装完成, false);
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装在顶升状态, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装在顶升状态, false);
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
            IoMgr.GetInstance().WriteIoBit(1, 18, true); //打开光源5
            WaitRegBit((int)SysBitReg.上盖转盘就绪, true);  //等待转盘站料就绪
            loosen_top();
            z_up();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖转盘就绪, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖转盘就绪, false);
            lift_down();
            ShowLog(string.Format("{0}系统位寄存器{1},有效值{2}", Name, (int)SysBitReg.上盖组装完成, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装完成, true);
            ShowLog("伺服上电");
            MotionMgr.GetInstance().ServoOn(AxisU);
            MotionMgr.GetInstance().Home(AxisU, -1);
            WaitHome(AxisU, 50000);
            l_work();
            s_work();
            SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T3_OffsetA3,0,false);//初始化清零
            ShowLog("初始化完成");
            WarningMgr.GetInstance().Info("上盖搬运站初始化完成");
        }

        /// <summary>
        /// 顶伸气缸上抬
        /// </summary>
        private void lift_up()
        {
            ShowLog("顶伸气缸上抬");
            IoMgr.GetInstance().WriteIoBit(3, 5, true);
            WaitIo(3, 5, false, 10000);
            WaitIo(3, 6, true, 10000);
        }

        /// <summary>
        /// 顶伸气缸下降
        /// </summary>
        private void lift_down()
        {
            ShowLog("顶伸气缸下降");
            IoMgr.GetInstance().WriteIoBit(3, 5, false);
            WaitIo(3, 5, true, 10000); 
            WaitIo(3, 6, false, 10000);
        }

        /// <summary>
        /// Z轴气缸上抬
        /// </summary>
        private void z_up(bool bShowLog = true)
        {
            if (bShowLog)
                ShowLog("Z轴气缸上抬");
            IoMgr.GetInstance().WriteIoBit(3, 8, false);
            WaitIo(3, 14, true, 10000);
            WaitIo(3, 15, false, 10000);
        }

        /// <summary>
        /// Z轴气缸下降
        /// </summary>
        private void z_down()
        {
            ShowLog("Z轴气缸下降");
            IoMgr.GetInstance().WriteIoBit(3, 8, true);
            WaitIo(3, 14, false, 10000);
            WaitIo(3, 15, true, 10000);
        }

        /// <summary>
        /// 长轴气缸移到工作位
        /// </summary>
        private void l_work()
        {
            z_up(false);
            ShowLog("长轴气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(3, 6, false);
            IoMgr.GetInstance().WriteIoBit(3, 19, true);
            WaitIo(3, 10, false, 10000);
            WaitIo(3, 11, true, 10000);
        }

        /// <summary>
        /// 长轴气缸移到原位
        /// </summary>
        private void l_home()
        {
            z_up(false);
            ShowLog("长轴气缸回Home状态");
            IoMgr.GetInstance().WriteIoBit(3, 19, false);
            IoMgr.GetInstance().WriteIoBit(3, 6, true);
            WaitIo(3, 10, true, 10000);
            WaitIo(3, 11, false, 10000);
        }

        /// <summary>
        /// 短轴气缸移到工作位
        /// </summary>
        private void s_work()
        {
            z_up(false);
            ShowLog("短轴气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(3, 7, false);
            IoMgr.GetInstance().WriteIoBit(3, 20, true);
            WaitIo(3, 12, false, 10000);
            WaitIo(3, 13, true, 10000);
        }

        /// <summary>
        /// 短轴气缸移到原位
        /// </summary>
        private void s_home()
        {
            z_up(false);
            ShowLog("短轴气缸回Home状态");
            IoMgr.GetInstance().WriteIoBit(3, 20, false);
            IoMgr.GetInstance().WriteIoBit(3, 7, true);
            WaitIo(3, 12, true, 10000);
            WaitIo(3, 13, false, 10000);
        }

        /// <summary>
        /// 夹爪气缸夹住
        /// </summary>
        private void clamp_top()
        {
            ShowLog("夹爪气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(3, 9, true); //夹住
            WaitTimeDelay(500);
            //WaitIo(3, 16, false, 10000);因为去掉3.16和3.17的传感器，所以，3.16和3.17不再用
            //WaitIo(3, 17, true, 10000);
        }

        /// <summary>
        /// 夹爪气缸松开
        /// </summary>
        private void loosen_top()
        {
            ShowLog("夹爪气缸到Home状态");
            IoMgr.GetInstance().WriteIoBit(3, 9, false); //松开
            WaitTimeDelay(500);
            //WaitIo(3, 16, true, 10000);//因为去掉3.16和3.17的传感器，所以，3.16和3.17不再用
            //WaitIo(3, 17, false, 10000);
        }

        /// <summary>
        /// 检测上盖抓取有料感应
        /// </summary>
        private void reaction_check()
        {
            ShowLog("上盖抓取有料感应检测");
            if (SystemMgr.GetInstance().Mode != SystemMode.Dry_Run_Mode) //空跑模式屏蔽光电检测
                WaitIo(3, 9, true, 10000);
        }

        /// <summary>
        /// 产品拍照失败处理 -1:需要取料  0:继续拍照检查
        /// </summary>
        /// <returns></returns>
        private int top_fail()
        {
            //没有抓到组装孔,提示人工取料  
            IoMgr.GetInstance().AlarmLight(LightState.红灯开);
            IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
            if (DialogResult.Yes == ShowMessage("上盖产品定位失败,\n请取走NG料后按启动按钮\n继续拍照点击No"))
            //if (DialogResult.Yes == MessageBox.Show("上盖产品定位失败,\n请取走NG料后按启动按钮\n继续拍照点击No", 
            //    "产品定位NG", MessageBoxButtons.YesNo))
            {
                WaitIo(1, 3, false);
                WaitIo(1, 3, true);     
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                return -1;
            }
            else
            {
                IoMgr.GetInstance().WriteIoBit(1, 18, true); //打开光源5
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                WaitTimeDelay(2000);
                return 0;
            }
        }

        /// <summary>
        /// 检查料盘状态 0:继续拍照检查 1:可以去放料 -1:需要取料
        /// </summary>
        /// <returns></returns>
        private int check_top()
        {
            WarningMgr.GetInstance().Info("上盖搬运T5开始拍照");
            if (VisionMgr.GetInstance().ProcessStep("T5_1")
                || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
            {
                if (VisionMgr.GetInstance().ProcessStep("T5_2")
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                {
                    CheckTopPosOk();//拍照之前
                    WarningMgr.GetInstance().Info("上盖搬运T5拍照成功");
                    s_home();
                    z_down();
                    clamp_top();
                    z_up();
                    CheckTopPosOk();//上升之后
                    reaction_check();
                    double fAngle = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T5_A);
                    if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode)
                        fAngle = 6;
                    if (fAngle<0)
                    {
                        fAngle = 360.0 + fAngle;
                    }
                    long lPluse = (int)(fAngle * 9972.0 / 360.0);//要转动的脉冲
                    MotionMgr.GetInstance().SetPosZero(AxisU);//当前位置设置为0

                    MotionMgr.GetInstance().RelativeMove(AxisU, (int)(fAngle * 9972.0 / 360.0), 5000);//步进驱动10000/R
                    l_home();
                    WaitMotion(AxisU, -1);

                    while (true)
                    {
                        long lResultPluse = MotionMgr.GetInstance().GetAixsPos(AxisU);
                        long lAbs = lPluse - lResultPluse;
                        if (Math.Abs(lAbs) > 20)
                        {
                            MotionMgr.GetInstance().RelativeMove(AxisU, (int)lAbs, 5000);
                            WaitMotion(AxisU, -1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return 1;
                }
                else
                {
                    double fResult = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T5_A);
                    if (fResult > -1001 && fResult < -999) //判断图像返回-1000为没有料
                        return -1;
                    else
                        return top_fail();
                }
           }
            else //没有拍到料
            {
                double fResult = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T5_A);
                if (fResult > -1001 && fResult < -999) //判断图像返回-1000为没有料
                    return -1;
                else
                    return top_fail();
            }
        }

        /// <summary>
        /// 检查上盖组装产品到位信号,顶伸气缸上抬,流水线结合气缸上升状态判断是否需要关闭流水线皮带
        /// </summary>
        private void CheckTopPosOk()
        {
            if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装产品到位))
            {
                ShowLog("顶伸气缸上抬");
                IoMgr.GetInstance().WriteIoBit(3, 5, true);
                //WaitIo(3, 5, false, 10000);
                //WaitIo(3, 6, true, 10000);
            }
        }

        public override void StationProcess()
        {
            int nStatus = check_top();
            if (1 == nStatus)
            {
                WarningMgr.GetInstance().Info("上盖搬运拍照OK,等待组装");
                IoMgr.GetInstance().WriteIoBit(1, 18, false);//关闭光源5
                WaitRegBit((int)SysBitReg.上盖组装完成, false); //等待流水线把组装好的料盘运走(或者流水线初始化完成)
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装在顶升状态, false.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装在顶升状态, false);
                WaitRegBit((int)SysBitReg.上盖组装产品到位, true);  //等待流水线上盖组装站就绪
                System.Diagnostics.Debug.WriteLine("上盖搬运获取：上盖组装产品到位:"+SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装产品到位).ToString());
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装产品到位, false.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装产品到位, false);
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装在顶升状态, true.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装在顶升状态, true);

                double fT3A3 = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T3_OffsetA3);
                System.Diagnostics.Debug.WriteLine(string.Format("T3角度补偿:{0}",fT3A3));
                MotionMgr.GetInstance().RelativeMove(AxisU, (int)(fT3A3 * 9972.0 / 360.0), 5000);//步进驱动10000/R
                WaitMotion(AxisU, -1);

                WaitTimeDelay(200);
                lift_up();
                WaitTimeDelay(200);//下降之前,停留200ms
                z_down();
                loosen_top();
                WaitTimeDelay(1000);
                z_up();
                lift_down();
                MotionMgr.GetInstance().AbsMove(AxisU, 0, 5000);//Move back to the absolution 0 Point
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装完成, true.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装完成, true);
                l_work();
                s_work();
                WaitMotion(AxisU, -1);
                //MotionMgr.GetInstance().AbsMove(AxisU, 0,3000); 
                //WaitMotion(AxisU, -1);
                nStatus = -1;
            }
            if (-1 == nStatus)
            {
                CheckTopPosOk();
                WarningMgr.GetInstance().Info("上盖搬运组装OK,等待取料");
                WaitRegBit((int)SysBitReg.上盖转盘就绪, true);  //等待转盘站料就绪
                CheckTopPosOk();//下降之前
                z_down();
                WaitTimeDelay(200);
                clamp_top();
                WaitTimeDelay(200);
                z_up();
                CheckTopPosOk();//上升之后
                reaction_check();
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖转盘就绪, false.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖转盘就绪, false);
                s_home();
                z_down();
                loosen_top();
                z_up();
                CheckTopPosOk();//放料上升之后
                IoMgr.GetInstance().WriteIoBit(1, 18, true); //打开光源5
                s_work();
            }
            //nStatus为0时继续拍照检查
        }
    }
}
