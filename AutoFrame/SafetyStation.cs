using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFrameDll;
using CommonTool;
using Communicate;
using AutoFrameVision;
using System.Threading;
using System.Windows.Forms;
using System.IO;
namespace AutoFrame
{
    class SafetyStation : StationBase
    {
        public SafetyStation(string strName) : base(strName)
        {
            io_in = new string[] {"1.6" };
            //io_out = new string[] { "1.1", "1.2", "1.3", "1.6", "1.14", "1.15", "1.16" };
            //bPositiveMove = new bool[] { true, true, true, true };
            //strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
        }
        public override void StationInit()
        {
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.屏蔽光栅, true);
        }
        public override void StationProcess()
        {//此站位用于在按下按钮之后或加工期间不得伸入安全光栅内，在上料或者掀盖期间安全光栅失效。
            if ((SystemMgr.GetInstance().GetRegBit((int)SysBitReg.运行状态))&&(SystemMgr.GetInstance().GetRegBit((int)SysBitReg.屏蔽光栅)))
            {
                if (IoMgr.GetInstance().ReadIoInBit("安全光栅")==false) 
                {
                    //工作中触发安全光栅
                    MotionMgr.GetInstance().StopAxis(AxisX);
                    MotionMgr.GetInstance().StopAxis(AxisY);
                    MotionMgr.GetInstance().StopAxis(AxisZ);
                    ShowLog("工作中触发安全光栅");
                }
            }
            WaitTimeDelay(50);
        }
    }
}
