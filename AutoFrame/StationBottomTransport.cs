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
    /// 下盖移载站
    /// </summary>
    class StationBottomTransport : StationBase
    {   
        public StationBottomTransport(string strName) : base(strName)
        {
            io_in = new string[] { "2.3", "2.4", "2.7", "2.8", "2.9" , "2.10", "2.11" , "2.12", "2.13", "2.14" };
            io_out = new string[] { "1.14","2.2", "2.3", "2.4", "2.5", "2.6", "2.19", "2.20" };
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
            IoMgr.GetInstance().WriteIoBit(1, 14, true); //打开光源1
            WaitRegBit((int)SysBitReg.下盖转盘就绪, true); 
            ShowLog("关吸盘真空");
            IoMgr.GetInstance().WriteIoBit(2, 6, false);
            z_up();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.下盖转盘就绪, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.下盖转盘就绪, false);
            ShowLog("伺服上电");
            MotionMgr.GetInstance().ServoOn(AxisU); 
            MotionMgr.GetInstance().Home(AxisU, -1);
            WaitHome(AxisU, 50000);
            l_work();
            s_work();
            loosen_bottom();
            ShowLog("初始化完成");
            System.Diagnostics.Debug.WriteLine("初始化完成");
        }

        /// <summary>
        /// Z轴气缸上台
        /// </summary>
        private void z_up(bool bShowLog = true)
        {
            if (bShowLog)
                ShowLog("Z轴气缸上台");
            IoMgr.GetInstance().WriteIoBit(2, 5, false);
            WaitIo(2, 12, true, 10000);
            WaitIo(2, 13, false, 10000);
        }

        /// <summary>
        /// Z轴气缸下降
        /// </summary>
        private void z_down()
        {
            ShowLog("Z轴气缸下降");
            IoMgr.GetInstance().WriteIoBit(2, 5, true);
            WaitIo(2, 12, false, 10000);
            WaitIo(2, 13, true, 10000);
        }

        /// <summary>
        /// 长轴气缸移到工作位
        /// </summary>
        private void l_work()
        {
            z_up(false);
            ShowLog("长轴气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(2, 3, false);
            IoMgr.GetInstance().WriteIoBit(2, 19, true);
            WaitIo(2, 8, false, 10000);
            WaitIo(2, 9, true, 10000);
        }

        /// <summary>
        /// 长轴气缸移到原位
        /// </summary>
        private void l_home()
        {
            z_up(false);
            ShowLog("长轴气缸回Home状态");
            IoMgr.GetInstance().WriteIoBit(2, 19, false);
            IoMgr.GetInstance().WriteIoBit(2, 3, true);
            WaitIo(2, 8, true, 10000);
            WaitIo(2, 9, false, 10000);
        }

        /// <summary>
        /// 短轴气缸移到工作位
        /// </summary>
        private void s_work()
        {
            z_up(false);
            ShowLog("短轴气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(2, 4, false);
            IoMgr.GetInstance().WriteIoBit(2, 20, true);
            WaitIo(2, 10, false, 10000);
            WaitIo(2, 11, true, 10000);
        }

        /// <summary>
        /// 短轴气缸移到原位
        /// </summary>
        private void s_home()
        {
            z_up(false);
            ShowLog("短轴气缸回Home状态");
            IoMgr.GetInstance().WriteIoBit(2, 20, false);
            IoMgr.GetInstance().WriteIoBit(2, 4, true);
            WaitIo(2, 10, true, 10000);
            WaitIo(2, 11, false, 10000);
        }

        /// <summary>
        /// 定位气缸夹住
        /// </summary>
        private void clamp_bottom()
        {
            ShowLog("定位气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(2, 2, true); //夹住
            WaitIo(2, 3, false, 10000);
            //WaitIo(2, 4, true, 10000);
            WaitTimeDelay(200);
        }

        /// <summary>
        /// 定位气缸松开
        /// </summary>
        private void loosen_bottom()
        {
            ShowLog("定位气缸到Home状态");
            IoMgr.GetInstance().WriteIoBit(2, 2, false); //松开
            WaitIo(2, 3, true, 10000);
            WaitIo(2, 4, false, 10000);
        }

        /// <summary>
        /// 开真空
        /// </summary>
        private void open_vacuum()
        {
            ShowLog("开真空");
            IoMgr.GetInstance().WriteIoBit(2, 6, true); 
            if (SystemMgr.GetInstance().Mode != SystemMode.Dry_Run_Mode) //空跑模式屏蔽真空检测
                WaitIo(2, 14, true, 10000);
            else if(SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽真空检测)
                WaitTimeDelay(2000); //
        }

        /// <summary>
        /// 关真空
        /// </summary>
        private void close_vacuum()
        {
            ShowLog("关真空");
            IoMgr.GetInstance().WriteIoBit(2, 6, false);
            if (SystemMgr.GetInstance().Mode != SystemMode.Dry_Run_Mode) //空跑模式屏蔽真空检测 
                WaitIo(2, 14, false, 10000);
            else if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽真空检测)
                WaitTimeDelay(2000); //
        }

        /// <summary>
        /// 产品拍照失败处理 -1:需要取料  0:继续拍照检查
        /// </summary>
        /// <returns></returns>
        private int bottom_fail()
        {
            //没有抓到柱子,提示人工取料
            loosen_bottom();
            IoMgr.GetInstance().AlarmLight(LightState.红灯开);
            IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
            if (DialogResult.Yes == ShowMessage("下盖产品定位失败,\n请取走NG料后按启动按钮点击Yes\n继续拍照点击No"))
            //if (DialogResult.Yes == MessageBox.Show("下盖产品定位失败,\n请取走NG料后按启动按钮点击Yes\n继续拍照点击No",
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
                IoMgr.GetInstance().WriteIoBit(1, 14, true); //打开光源1
                clamp_bottom();
                WaitTimeDelay(2000);
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                return 0;
            }
        }

        /// <summary>
        /// 气缸抓取下盖旋转一个角度
        /// </summary>
        /// <returns></returns>
        private int rotate_bottom()
        {
            l_work();
            s_home();
            z_down();
            open_vacuum();
            loosen_bottom();
            WaitTimeDelay(100);
            z_up();
            double fAngle = 15.0;
            long lPluse = (int)(fAngle * 10000.0 / 360.0);//要转动的脉冲
            MotionMgr.GetInstance().RelativeMove(AxisU, (int)(fAngle * 9972.0 / 360.0), 5000);//1530/3604.2
            WaitMotion(AxisU, -1);
            WaitTimeDelay(100);
            z_down();
            WaitTimeDelay(100);
            close_vacuum();
            z_up();
            MotionMgr.GetInstance().RelativeMove(AxisU, (int)(-fAngle * 9972.0 / 360.0), 5000);//1530/3604.2
            s_work();
            WaitMotion(AxisU, -1);

            return 0;
        }

        /// <summary>
        /// T1_1/2图像处理,1为处理成功,0为模板/roi处理失败,-1为无料
        /// </summary>
        /// <returns>1, 0, -1 </returns>
        private int t1_process()
        {
            if (VisionMgr.GetInstance().ProcessStep("T1_1")
                || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
            {
                if (VisionMgr.GetInstance().ProcessStep("T1_2")
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                {
                    s_home();
                    z_down();
                    open_vacuum();
                    loosen_bottom();
                    WaitTimeDelay(100);
                    z_up();
                    double fAngle = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_A);
                    if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode)
                        fAngle = 6;
                    //if (fAngle > 180)
                    //    fAngle = fAngle - 360.0;
                    //else if (fAngle < -180)
                    //    fAngle = fAngle + 360.0;

                    if (fAngle < 0)
                    {
                        fAngle = 360.0 + fAngle;
                    }
                    long lPluse = (int)(fAngle * 9972.0 / 360.0);//要转动的脉冲
                    MotionMgr.GetInstance().SetPosZero(AxisU);//当前位置设置为0

                    MotionMgr.GetInstance().RelativeMove(AxisU, (int)(fAngle * 9972.0 / 360.0), 5000);//1530/3604.2
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
                    //return bottom_fail();
                    return 0;
                }
            }
            else //没有拍到料
            {
                double fResult = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_X);
                if (fResult > -1001 && fResult < -999) //判断图像返回-1000为没有料
                {
                    loosen_bottom();
                    return -1;
                }
                else
                {
                    //return bottom_fail();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 检查料盘状态 0:继续拍照检查 1:可以去放料 -1:需要取料
        /// </summary>
        /// <returns></returns>
        private int check_bottom()
        {
            int nTimes = 0; //气缸抓料旋转次数
            clamp_bottom(); //夹住松开重复一次
            WaitTimeDelay(200);
            loosen_bottom();
            WaitTimeDelay(200);
            clamp_bottom();
            WaitTimeDelay(200);
            while(true)
            {
                int nRet = t1_process();
                if (1 == nRet) //处理成功
                {
                    return 1;
                }
                else
                {
                    if (-1 == nRet)
                        return -1;
                    else
                    {
                        if (nTimes >= 1)
                            return bottom_fail();
                        else
                            rotate_bottom();
                        nTimes++;
                    }
                }
            }
            //if (VisionMgr.GetInstance().ProcessStep("T1_1") 
            //    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
            //{
            //    if (VisionMgr.GetInstance().ProcessStep("T1_2")
            //        || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
            //    {
            //        s_home();
            //        z_down();
            //        open_vacuum();
            //        loosen_bottom();
            //        WaitTimeDelay(100);
            //        z_up();
            //        double fAngle = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_A);
            //        if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode)
            //            fAngle = 6;
            //        //if (fAngle > 180)
            //        //    fAngle = fAngle - 360.0;
            //        //else if (fAngle < -180)
            //        //    fAngle = fAngle + 360.0;

            //        if (fAngle < 0)
            //        {
            //            fAngle = 360.0 + fAngle;
            //        }
            //        long lPluse = (int)(fAngle * 10000.0 / 360.0);//要转动的脉冲
            //        MotionMgr.GetInstance().SetPosZero(AxisU);//当前位置设置为0

            //        MotionMgr.GetInstance().RelativeMove(AxisU, (int)(fAngle * 10000.0 / 360.0), 10000);//1530/3604.2
            //        l_home();
            //        WaitMotion(AxisU,-1);
            //        while(true)
            //        {
            //            long lResultPluse = MotionMgr.GetInstance().GetAixsPos(AxisU);
            //            long lAbs = lPluse - lResultPluse;
            //            if (Math.Abs(lAbs) > 20)
            //            {
            //                MotionMgr.GetInstance().RelativeMove(AxisU, (int)lAbs, 5000);
            //                WaitMotion(AxisU, -1);
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }

            //        return 1;
            //    }
            //    else
            //    {
            //        return bottom_fail();
            //    }
            //}
            //else //没有拍到料
            //{
            //    double fResult = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_X);
            //    if (fResult>-1001 && fResult<-999) //判断图像返回-1000为没有料
            //    {
            //        loosen_bottom();
            //        return -1;
            //    }
            //    else
            //        return bottom_fail();
            //}
        }

        public override void StationProcess()
        {
            int nStatus = check_bottom(); //第一次假定有料盘
            if (1 == nStatus)
            {
                IoMgr.GetInstance().WriteIoBit(1, 14, false); //关闭打开光源1
                WaitRegBit((int)SysBitReg.进料皮带上升到位, true);  //等待流水线进料站就绪
                z_down();
                close_vacuum();
                WaitTimeDelay(1000);
                z_up();
                WaitTimeDelay(200);
                z_down();
                WaitTimeDelay(500);
                z_up();
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带上升到位, false.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带上升到位, false);
                MotionMgr.GetInstance().AbsMove(AxisU, 0, 5000);
                l_work();
                s_work();
                WaitMotion(AxisU, -1);
                nStatus = -1;
            }
            if (-1 == nStatus)
            {
                WaitRegBit((int)SysBitReg.下盖转盘就绪, true);  //等待转盘站料就绪
                s_work();
                z_down();
                open_vacuum();
                z_up();
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.下盖转盘就绪, false.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.下盖转盘就绪, false);
                s_home();
                z_down();
                close_vacuum();
                z_up();
                IoMgr.GetInstance().WriteIoBit(1, 14, true); //打开光源1
                s_work();
            }
            //nStatus为0时继续拍照检查
        }
    }
}
