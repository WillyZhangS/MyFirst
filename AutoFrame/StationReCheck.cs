using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AutoFrameDll;
using CommonTool;
using AutoFrameVision;

namespace AutoFrame
{
    /// <summary>
    /// 复检站
    /// </summary>
    class StationReCheck : StationBase
    {
        public StationReCheck(string strName) : base(strName)
        {
            io_in = new string[] {  };
            io_out = new string[] { "1.17" };
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
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, true);
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停

        public override void StationDeinit()
        {
        }

        public override void StationInit()
        {
            ShowLog("初始化完成");
        }

        public override void StationProcess()
        {
            WaitRegBit((int)SysBitReg.复检工位产品到位, true);
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检工位产品到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检工位产品到位, false);
            WarningMgr.GetInstance().Info("复检工位:复检工位产品到位");
            while (true)
            {
                CheckContinue();
                //if (VisionMgr.GetInstance().ProcessStep("T4")
                //    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                if (true)
                {
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, true.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, true);
                    WarningMgr.GetInstance().Info("复检完成");
                    break;
                }
                else
                {
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, true.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, true);
                    WarningMgr.GetInstance().Info("复检完成");
                    break;
                    ////复检失败,提示人工取料
                    //if (DialogResult.Yes == ShowMessage("复检失败,\n继续复检请点击YES\n暂不处理请点击No"))
                    ////if (DialogResult.Yes == MessageBox.Show("复检失败,\n继续复检请点击YES\n暂不处理请点击No",
                    ////     "产品复检NG" ,MessageBoxButtons.YesNo))
                    //{
                    //}
                    //else
                    //{
                    //    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, true.ToString()));
                    //    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, true);
                    //    break;
                    //}
                }
            }
        }
    }
}
