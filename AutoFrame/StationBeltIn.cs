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
    /// 流水线进料站
    /// </summary>
    class StationBeltIn : StationBase
    {
        private bool m_bHaveFix = false;
        private bool m_bHaveFpc = false;
        public StationBeltIn(string strName):base(strName)
        {
            io_in = new string[] { "2.15", "2.16", "2.17", "2.18", "2.19", "2.20" };
            io_out = new string[] { "2.7", "2.8", "2.9", "2.18", "2.23" };
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
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带上升到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带上升到位, false);
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带下降到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带下降到位, false);
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停

        public override void StationDeinit()
        {
            ShowLog("关闭进料流水线转动");
            move_belt(0);
        }

        public override void StationInit()
        {
            m_bHaveFix = false;
            m_bHaveFpc = false;
            ShowLog("关闭进料流水线转动");
            move_belt(0);
            fix_down();
            ShowLog("初始化完成");
        }

        /// <summary>
        /// 整个工站下降
        /// </summary>
        private void station_down()
        {
            ShowLog("进料平台升降气缸下降");
            IoMgr.GetInstance().WriteIoBit(2, 18, false);
            IoMgr.GetInstance().WriteIoBit(2, 8, true); //下盖传送升降气缸下降
            WaitIo(2, 15, false, 10000);
            WaitIo(2, 16, true, 10000);
        }

        /// <summary>
        /// 整个工站上升
        /// </summary>
        private void station_up()
        {
            ShowLog("进料平台升降气缸上升");
            IoMgr.GetInstance().WriteIoBit(2, 8, false); //下盖传送升降气缸上升
            IoMgr.GetInstance().WriteIoBit(2, 18, true);
            WaitIo(2, 15, true, 10000);
            WaitIo(2, 16, false, 10000);
        }

        /// <summary>
        /// 载具升降气缸下降
        /// </summary>
        private void fix_down()
        {
            ShowLog("进料治具顶伸气缸下降");
            IoMgr.GetInstance().WriteIoBit(2, 9, false);  //下盖传送顶伸气缸
            WaitIo(2, 17, true, 10000);
            WaitIo(2, 18, false, 10000);
        }

        /// <summary>
        /// 载具升降气缸上升
        /// </summary>
        private void fix_up()
        {
            ShowLog("进料治具顶伸气缸上升");
            IoMgr.GetInstance().WriteIoBit(2, 9, true); //下盖传送顶伸气缸
            WaitIo(2, 17, false, 10000);
            WaitIo(2, 18, true, 10000);
        }
        
        /// <summary>
        /// 获取托盘治具
        /// </summary>
        private void get_fix()
        {
            move_belt(0);//进料流水线关闭
            station_down();
            fix_down();
            if (IoMgr.GetInstance().GetIoInState(3, 1)) //下层流水线感应1
            {
                ShowLog("下层流水线感应1有,气缸1下降,开启传送带");
                IoMgr.GetInstance().WriteIoBit(3, 2, true); //气缸1下降
                IoMgr.GetInstance().WriteIoBit(3, 1, true); //开启下层传送皮带
            }
            else
            {
                ShowLog("下层流水线感应1无");
                ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带下降到位, true.ToString()));
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带下降到位, true);
                WaitRegBit((int)SysBitReg.进料皮带下降到位, false);
            }
            move_belt(-1);//进料流水线反转

            WaitIo(2, 19, true, 50000); //进料平台治具检测
            WaitTimeDelay(10);//消除电平抖动
            WaitIo(2, 19, true);

            m_bHaveFix = true;
            //move_belt(0);//进料流水线关闭
            //Thread.Sleep(100);
            WaitTimeDelay(100);
            fix_up();
            move_belt(0);//进料流水线关闭
            station_up();

            WaitTimeDelay(50);
            if (IoMgr.GetInstance().GetIoInState(2, 20)) //获取治具上升后,检测到有料盘,报警
            {
                IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                while (true)
                {
                    if (DialogResult.No == ShowMessage("进料站平台上升后检测到治具上有料,请检查感应器!,确认后点击<退出运行>"))
                    {
	                    if (!IoMgr.GetInstance().GetIoInState(2, 20))
	                    {
	                        break;
	                    }
                    }
                }
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
            }
        }

        /// <summary>
        /// 进料平台流水线转动 1:正转 -1:反转 0:停止
        /// </summary>
        /// <param name="nDir">1:正转 -1:反转 0:停止</param>
        private void move_belt(int nDir = 0)
        {
            if (1 == nDir)
            {
                ShowLog("进料流水线正转");
                IoMgr.GetInstance().WriteIoBit(2, 23, false); //关闭进料平台流水线反转 
                IoMgr.GetInstance().WriteIoBit(2, 7, true); //开启进料平台流水线正转
            }
            else if (-1 == nDir)
            {
                ShowLog("进料流水线反转");
                IoMgr.GetInstance().WriteIoBit(2, 7, false); //关闭进料平台流水线正转
                IoMgr.GetInstance().WriteIoBit(2, 23, true); //开启进料平台流水线反转 
            }
            else
            {
                ShowLog("进料流水线关闭");
                IoMgr.GetInstance().WriteIoBit(2, 7, false); //关闭进料平台流水线正转
                IoMgr.GetInstance().WriteIoBit(2, 23, false); //关闭进料平台流水线反转 
            }
        }

        public override void StationProcess()
        {
            fix_down();
            if (IoMgr.GetInstance().GetIoInState(2, 19) || m_bHaveFix) //检测有无治具
            {
                m_bHaveFix = true;
                fix_up();
                station_up();
                WaitTimeDelay(500);
                if (IoMgr.GetInstance().GetIoInState(2, 20) || m_bHaveFpc //检测治具上有料
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽治具有料检测
                {
                    fix_down();
                    WaitIo(2, 21, false); //组装传送阻挡1感应
                    WaitTimeDelay(10);//消除电平抖动
                    WaitIo(2, 21, false);

                    move_belt(1);//正转送料

                    WaitIo(2, 21, true);
                    WaitTimeDelay(10);//消除电平抖动
                    WaitIo(2, 21, true);

                    WaitIo(2, 19, false);//等待治具检测无
                    WaitTimeDelay(10);//消除电平抖动
                    WaitIo(2, 19, false);

                    m_bHaveFix = false;
                    m_bHaveFpc = false;
                    get_fix();

                    if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式配合下盖搬运站用
                    {
                        fix_up();
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带上升到位, true.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带上升到位, true);
                        WaitRegBit((int)SysBitReg.进料皮带上升到位, false);
                        //WaitIo(2, 20, true, 10000);
                        m_bHaveFix = true;
                    }
                }
                else
                {
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.进料皮带上升到位, true.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.进料皮带上升到位, true);
                    WaitRegBit((int)SysBitReg.进料皮带上升到位, false);
                    WaitTimeDelay(200);
                    if (!IoMgr.GetInstance().GetIoInState(2, 20))
                    {
                        IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                        WaitIo(2, 20, true, 100);
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯开);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                    }
                    m_bHaveFpc = true;
                    m_bHaveFix = true;
                }
            }
            else
            {
                // 如果整个工站在下边,检测下层皮带感应1没有信号,等待下层皮带往前转动5s,初始化时用
                if (!IoMgr.GetInstance().GetIoInState(2, 15) && IoMgr.GetInstance().GetIoInState(2, 16))
                {//只在初始化时才进入此步骤
                    if (!IoMgr.GetInstance().GetIoInState(3, 1))
                    {
                        ShowLog("工站在下方,下层皮带感应1无信号,等待5s");
                        fix_down();
                        move_belt(-1);//反转
                        WaitTimeDelay(5000);
                        //move_belt(0);//关闭
                        if (!IoMgr.GetInstance().GetIoInState(2, 19)) 
                        {
                            //5s内没有检测到治具则进入正常获取步骤get_fix
                            get_fix();
                        }
                        else
                        {
                            m_bHaveFix = true;
                        }
                    }
                    else
                    {
                        get_fix();
                    }
                }
                else
                {
                    //检测不到治具,反转3s,再次检测是否有无治具
                    move_belt(-1);//反转
                    WaitTimeDelay(3000);
                    if (!IoMgr.GetInstance().GetIoInState(2, 19))
                    {
                        //3s内没有检测到治具则进入正常获取步骤get_fix
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
