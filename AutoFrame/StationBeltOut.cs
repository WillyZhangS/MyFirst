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
    /// 流水线出料站
    /// </summary>
    class StationBeltOut : StationBase
    {
        private bool m_bHaveFix = false;
        public StationBeltOut(string strName):base(strName)
        {
            io_in = new string[] { "3.18", "3.19", "3.20", "3.21","3.22", "3.23",
                 "3.24", "3.25", "3.26", "3.27", "3.28", "3.29" };
            io_out = new string[] { "3.10", "3.11", "3.12", "3.13", "3.14", "3.15", "3.21", "3.22", "3.23" };
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
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.出料皮带上升到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.出料皮带上升到位, false);
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停

        public override void StationDeinit()
        {
            ShowLog("关闭出料流水线转动");
            move_belt(0);
        }

        public override void StationInit()
        {
            m_bHaveFix = false;
            ShowLog("关闭出料流水线转动");
            move_belt(0);
            fix_down();
            ShowLog("初始化完成");
        }

        /// <summary>
        /// 整个工站下降
        /// </summary>
        private void station_down(bool bCheckIo = true)
        {
            ShowLog("出料平台升降气缸下降");
            IoMgr.GetInstance().WriteIoBit(3, 21, false);
            IoMgr.GetInstance().WriteIoBit(3, 11, true); //出料平台升降气缸下降
            if (bCheckIo)
            {
                WaitIo(3, 18, false, 10000);
                WaitIo(3, 19, true, 10000);
            }
        }

        /// <summary>
        /// 整个工站上升
        /// </summary>
        private void station_up()
        {
            ShowLog("出料平台升降气缸上升");
            IoMgr.GetInstance().WriteIoBit(3, 11, false); //出料平台升降气缸上升
            IoMgr.GetInstance().WriteIoBit(3, 21, true);
            WaitIo(3, 18, true, 10000);
            WaitIo(3, 19, false, 10000);
        }
        
        /// <summary>
        /// 载具升降气缸下降
        /// </summary>
        private void fix_down()
        {
            ShowLog("出料平台治具顶伸气缸下降");
            IoMgr.GetInstance().WriteIoBit(3, 12, false);  //出料平台治具顶伸气缸
            WaitIo(3, 20, true, 10000);
            WaitIo(3, 21, false, 10000);
        }

        /// <summary>
        /// 载具升降气缸上升
        /// </summary>
        private void fix_up()
        {
            ShowLog("出料平台治具顶伸气缸上升");
            IoMgr.GetInstance().WriteIoBit(3, 12, true); //出料平台治具顶伸气缸
            WaitIo(3, 20, false, 10000);
            WaitIo(3, 21, true, 10000);
        }

        /// <summary>
        /// 出料平台平移气缸到工作位
        /// </summary>
        private void l_work()
        {
            ShowLog("出料平台平移气缸到Work状态");
            IoMgr.GetInstance().WriteIoBit(3, 13, false);
            IoMgr.GetInstance().WriteIoBit(3, 22, true);
            WaitIo(3, 24, false, 10000);
            WaitIo(3, 25, true, 10000);
        }

        /// <summary>
        /// 出料平台平移气缸回原位
        /// </summary>
        private void l_home()
        {
            ShowLog("出料平台平移气缸到home状态");
            IoMgr.GetInstance().WriteIoBit(3, 22, false);
            IoMgr.GetInstance().WriteIoBit(3, 13, true);
            WaitIo(3, 24, true, 10000);
            WaitIo(3, 25, false, 10000);
        }

        /// <summary>
        /// 出料平台Z轴气缸下降
        /// </summary>
        private void z_down()
        {
            ShowLog("出料平台Z轴气缸下降");
            IoMgr.GetInstance().WriteIoBit(3, 14, true);
            WaitIo(3, 26, false, 10000);
            WaitIo(3, 27, true, 10000);
        }

        /// <summary>
        /// 出料平台Z轴气缸上升
        /// </summary>
        private void z_up()
        {
            ShowLog("出料平台Z轴气缸上升");
            IoMgr.GetInstance().WriteIoBit(3, 14, false);
            WaitIo(3, 26, true, 10000);
            WaitIo(3, 27, false, 10000);
        }

        /// <summary>
        /// 出料平台夹爪气缸夹住
        /// </summary>
        private void clamp_top()
        {
            ShowLog("出料平台夹爪气缸夹住");
            IoMgr.GetInstance().WriteIoBit(3, 15, true);
            WaitIo(3, 28, false, 10000);
            WaitIo(3, 29, true, 10000);
        }

        /// <summary>
        /// 出料平台夹爪气缸松开
        /// </summary>
        private void loosen_top()
        {
            ShowLog("出料平台夹爪气缸松开");
            IoMgr.GetInstance().WriteIoBit(3, 15, false);
            WaitIo(3, 28, true, 10000);
            WaitIo(3, 29, false, 10000);
        }

        /// <summary>
        /// 出料平台产品有无检测
        /// </summary>
        private void reaction_check()
        {
            ShowLog("出料平台产品有无检测");
            if (SystemMgr.GetInstance().Mode != SystemMode.Dry_Run_Mode) //空跑模式屏蔽光电检测
                WaitIo(3, 23, true, 10000); //对射光电检测,产品无时为true
        }

        /// <summary>
        /// 获取托盘治具
        /// </summary>
        private void get_fix()
        {
            station_up();
            fix_down();
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.出料皮带上升到位, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.出料皮带上升到位, true);
            WaitRegBit((int)SysBitReg.出料皮带上升到位, false);
            move_belt(1);//正转,接收治具
            WaitIo(3, 22, true, 10000);  //上盖传送治具检测
            WaitTimeDelay(10);//电平防抖动
            WaitIo(3, 22, true);
            WaitTimeDelay(500);
            m_bHaveFix = true;
            //move_belt(0);
        }

        /// <summary>
        /// 运送盖子
        /// </summary>
        private void send_cover()
        {
            z_up();
            l_work();
            loosen_top();
            z_down();
            clamp_top();
            z_up();
            reaction_check();
            station_down(false);
            l_home();
            //z_down();
            loosen_top();
            WaitTimeDelay(100);
            z_up();
            l_work();
        }

        /// <summary>
        /// 出料平台流水线转动 1:正转 -1:反转 0:停止
        /// </summary>
        /// <param name="nDir">1:正转 -1:反转 0:停止</param>
        private void move_belt(int nDir = 0)
        {
            if (1 == nDir)
            {
                ShowLog("出料流水线正转");
                IoMgr.GetInstance().WriteIoBit(3, 23, false); //关闭出料平台流水线反转 
                IoMgr.GetInstance().WriteIoBit(3, 10, true); //开启出料平台流水线正转
            }
            else if (-1 == nDir)
            {
                ShowLog("出料流水线反转");
                IoMgr.GetInstance().WriteIoBit(3, 10, false); //关闭出料平台流水线正转
                IoMgr.GetInstance().WriteIoBit(3, 23, true); //开启出料平台流水线反转 
            }
            else
            {
                ShowLog("出料流水线关闭");
                IoMgr.GetInstance().WriteIoBit(3, 10, false); //关闭出料平台流水线正转
                IoMgr.GetInstance().WriteIoBit(3, 23, false); //关闭出料平台流水线反转 
            }
        }

        public override void StationProcess()
        {
            fix_down();
            WaitTimeDelay(50);
            if (IoMgr.GetInstance().GetIoInState(3, 22) || m_bHaveFix) //检测有无治具
            {
                m_bHaveFix = true;
                WaitTimeDelay(500); //Delay about 500 ms
                fix_up();
                move_belt(0);//关闭
                WaitTimeDelay(500);
                if (!IoMgr.GetInstance().GetIoInState(3, 23)   //检测治具上有料,对射光电有料挡住时为false
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode)  //空跑模式屏蔽有料检测
                {
                    station_up();
                    send_cover();
                }
                station_down();
                fix_down();
                WaitIo(3, 2, false);//下层传送2感应
                WaitTimeDelay(10);//电平防抖动
                WaitIo(3, 2, false);

                move_belt(-1);//反转,送出治具

                WaitIo(3, 2, true);
                WaitTimeDelay(10);//电平防抖动
                WaitIo(3, 2, true);

                WaitIo(3, 22, false);//等待出料平台治具检测感应
                WaitTimeDelay(10);//电平防抖动
                WaitIo(3, 22, false);

                m_bHaveFix = false;
                move_belt(0);//关闭
                get_fix();
            }
            else
            {
                //如果整个工站在下边,检测下层皮带感应2没有信号,皮带往前转动5s,初始化时用
                if (!IoMgr.GetInstance().GetIoInState(3, 18) && IoMgr.GetInstance().GetIoInState(3, 19))
                {
                    if (!IoMgr.GetInstance().GetIoInState(3, 2))
                    {
                        ShowLog("工站在下方,下层皮带感应2无信号,开启传送电机5s");
                        move_belt(-1);//反转
                        WaitTimeDelay(5000);
                        move_belt(0);//关闭
                        get_fix();
                    }
                    else
                    {
                        get_fix();
                    }
                }
                else
                {
                    move_belt(1);//正转3s,接收治具
                    WaitTimeDelay(3000);
                    if (!IoMgr.GetInstance().GetIoInState(3, 22))
                    {
                        get_fix();
                    }
                    else
                    {
                        m_bHaveFix = true;
                    }
                }
            }
        }
    }
}
