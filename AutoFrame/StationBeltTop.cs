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
    /// 上层流水线站
    /// </summary>
    class StationBeltTop : StationBase
    {
        private bool[] m_bCylStatus = new bool[6];
        private int m_nOldData = 0; //
        private bool m_bIdle = false;
        private bool[] m_bBeltStaSts = new bool[6]; //流水线各阻挡位是否开始放料
        private DateTime m_IdleTime2, m_IdleTime4, m_IdleTime6;

        public StationBeltTop(string strName) : base(strName)
        {
            io_in = new string[] { "2.21", "2.22", "2.23", "2.24", "2.25", "2.26" };
            io_out = new string[] { "2.10", "2.11", "2.12", "2.13", "2.14", "2.15", "3.18" };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
        }

        public override void InitSecurityState()
        {
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1产品到位, false.ToString()));
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2产品到位, false.ToString()));
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检工位产品到位, false.ToString()));
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装产品到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1产品到位, false);//顶料工位1产品到位false
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2产品到位, false);//顶料工位2产品到位false
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检工位产品到位, false);//复检工位产品到位false
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装产品到位, false);//上盖组装产品到位false
        }

        public override void EmgStop()
        {
            ShowLog("关闭上层传送带");
            IoMgr.GetInstance().WriteIoBit(3, 18, false); //上层传送皮带开启关闭
        }//站位急停

        public override void StationDeinit()
        {
            ShowLog("关闭上层传送带");
            IoMgr.GetInstance().WriteIoBit(3, 18, false); //上层传送皮带开启关闭
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

        public override void StationInit()
        {
            ShowLog("上层传送阻挡气缸2/4下降/上升");
            IoMgr.GetInstance().WriteIoBit(2, 11, true); //上层传送阻挡气缸2/4
            IoMgr.GetInstance().WriteIoBit(2, 13, true);
            WaitTimeDelay(200);
            IoMgr.GetInstance().WriteIoBit(2, 11, false); //上层传送阻挡气缸2/4
            IoMgr.GetInstance().WriteIoBit(2, 13, false);

            WaitRegBit((int)SysBitReg.上盖组装完成, true);//等待上盖组装气缸抬起,且上盖顶料气缸下降到位
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装完成, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装完成, false);
            WaitRegBit((int)SysBitReg.复检完成, true);//等待复检工位气缸初始化
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, false);
            WaitRegBit((int)SysBitReg.顶料工位1下降到位, true);//等待顶料工位1初始化
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1下降到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1下降到位, false);
            WaitRegBit((int)SysBitReg.顶料工位2下降到位, true);//等待顶料工位2初始化
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2下降到位, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2下降到位, false);
            for (int i = 0; i < m_bCylStatus.Length; i++)
                m_bBeltStaSts[i] = false;
            for (int i = 0; i < m_bCylStatus.Length; i++)
                m_bCylStatus[i] = true;
            ShowLog("上层传送阻挡气缸1-6上升");
            for (int i = 10; i < 16; i++)
                IoMgr.GetInstance().WriteIoBit(2, i, false); //上层传送阻挡气缸1-6
            IoMgr.GetInstance().ReadIoIn(2, ref m_nOldData);
            m_nOldData &= 0x3F00000;
            ShowLog("开启上层传送带");
            IoMgr.GetInstance().WriteIoBit(3, 18, true); //上层传送皮带开启
            m_bIdle = false;
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
            bool bEnable = (nNewData & (1 << (nIndex - 1))) != 0;//true高 false低
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
            IoMgr.GetInstance().ReadIoIn(2, ref nData);
            nData &= 0x3F00000;
            if (nData != m_nOldData)
            {
                for (int i = 21; i < 27; i++)
                {
                    if (IsIoTrigger(m_nOldData, nData, i, false))
                    {
                        ShowLog(string.Format("上层感应{0}下降沿,阻挡气缸{1}上升", i - 20, i - 20));
                        IoMgr.GetInstance().WriteIoBit(2, i - 11, false); //上层传送阻挡气缸1-6
                        m_bCylStatus[i - 21] = true;
                        if (i == 22)
                            m_IdleTime2 = DateTime.Now;
                        //if (i == 24)
                        //    m_IdleTime4 = DateTime.Now;
                        if (i == 26)
                            m_IdleTime6 = DateTime.Now;
                    }
                }
                if (IsIoTrigger(m_nOldData, nData, 22, true))
                {
                    ShowLog("上层感应2上升沿");
                    if (false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1产品到位)
                        && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1下降到位) && true == m_bBeltStaSts[0] //且上一个阻挡位开始放料
                        && IoMgr.GetInstance().GetIoInState(2, 27) && (false == IoMgr.GetInstance().GetIoInState(2, 28)))//顶料工位1顶伸气缸在下边
                    {
                        m_bBeltStaSts[0] = false;
                        ShowLog("顶料工站1待料状态");
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1产品到位, true.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1产品到位, true);
                        WarningMgr.GetInstance().Info("上层感应2上升沿,顶料工位1产品到位");
                        m_IdleTime2 = DateTime.Now;
                    }
                }
                if (IsIoTrigger(m_nOldData, nData, 24, true))
                {
                    ShowLog("上层感应4上升沿");
                    if (false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2产品到位)
                        && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2下降到位) && true == m_bBeltStaSts[2]
                        && IoMgr.GetInstance().GetIoInState(2, 29) && (false == IoMgr.GetInstance().GetIoInState(2, 30)))
                    {
                        m_bBeltStaSts[2] = false;
                        ShowLog("顶料工站2待料状态");
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2产品到位, true);
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2产品到位, true.ToString()));
                        WarningMgr.GetInstance().Info("上层感应4上升沿,顶料工位2产品到位");
                        m_IdleTime4 = DateTime.Now;
                    }
                }
                if (IsIoTrigger(m_nOldData, nData, 25, true))
                {
                    if (false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.复检工位产品到位)
                        && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.复检完成) && true == m_bBeltStaSts[3])
                    {
                        m_bBeltStaSts[3] = false;
                        ShowLog("上层感应5上升沿,给复检工站到位信号");
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检工位产品到位, true.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检工位产品到位, true);
                        WarningMgr.GetInstance().Info("上层感应5上升沿,复检工位产品到位");
                    }
                }
                if (IsIoTrigger(m_nOldData, nData, 26, true))
                {
                    ShowLog("上层感应6上升沿");
                    if (false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装产品到位) &&
                        false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装完成) && true == m_bBeltStaSts[4]
                        && IoMgr.GetInstance().GetIoInState(3, 5) && (false == IoMgr.GetInstance().GetIoInState(3, 6))) //气缸6下降状态
                    {
                        m_bBeltStaSts[4] = false;
                        ShowLog("上盖组装工位待料状态");
                        ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装产品到位, true.ToString()));
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装产品到位, true);
                        WarningMgr.GetInstance().Info("上层感应6上升沿,上盖组装产品到位");
                        m_IdleTime6 = DateTime.Now;
                    }
                }

                m_nOldData = nData;
            }
            if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装完成)) //上盖组装完成
            {
                if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.出料皮带上升到位)) //出料站允许进料
                {
                    //m_bBeltStaSts[5] = true;
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.出料皮带上升到位, false.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.出料皮带上升到位, false);
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.上盖组装完成, false.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.上盖组装完成, false);
                    ShowLog("上层流水线阻挡气缸6下降");
                    IoMgr.GetInstance().WriteIoBit(2, 15, true);
                    m_bCylStatus[5] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸6下降,出料站允许进料");
                }
            }
            if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.复检完成)) //复检
            {
                if (false == IoMgr.GetInstance().GetIoInState(2, 26) && true == IoMgr.GetInstance().GetIoInState(2, 25) //上盖组装位置无料,复检位置有料
                    && false == IoMgr.GetInstance().ReadIoOutBit(3, 5)
                    && true == IoMgr.GetInstance().GetIoInState(3, 5) && false == IoMgr.GetInstance().GetIoInState(3, 6)  //上盖顶伸气缸在下边
                    && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.上盖组装完成)
                    && false == m_bBeltStaSts[4]) //根据皮带阻挡气缸之间的间距不一,有可能前一个没到,后一个就已经到了.所以必须先等待前一个到位 
                {
                    m_bBeltStaSts[4] = true;
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.复检完成, false.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.复检完成, false);
                    ShowLog("上层流水线阻挡气缸5下降");
                    IoMgr.GetInstance().WriteIoBit(2, 14, true);
                    m_bCylStatus[4] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸5下降");
                    double fT3A2 = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T3_OffsetA2);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T3_OffsetA3, fT3A2, false);//获取复检站补偿角度传递到下一个工站
                }
            }
            if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2下降到位))
            {
                if (false == IoMgr.GetInstance().GetIoInState(2, 25) && true == IoMgr.GetInstance().GetIoInState(2, 24) //复检位置无料,顶料2位置有料
                    && false == m_bBeltStaSts[3]) //根据皮带阻挡气缸之间的间距不一,有可能前一个没到,后一个就已经到了.所以必须先等待前一个到位 
                {
                    m_bBeltStaSts[3] = true;
                    ShowLog("复检位置无料");
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位2下降到位, false.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位2下降到位, false);
                    ShowLog("上层流水线阻挡气缸4下降");
                    IoMgr.GetInstance().WriteIoBit(2, 13, true);
                    m_bCylStatus[3] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸4下降");
                    double fT3A1 = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T3_OffsetA1);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T3_OffsetA2, fT3A1, false);//获取补偿角度传递到下一个工站
                }
            }
            if (m_bCylStatus[2] && IoMgr.GetInstance().GetIoInState(2, 23)) //顶料1和顶料2之间缓存位置有料
            {
                if (false == IoMgr.GetInstance().GetIoInState(2, 24) //顶料2位置无料
                    && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2在顶升状态)//顶料2工位在待料状态 
                    && false == m_bBeltStaSts[2])//根据皮带阻挡气缸之间的间距不一,有可能前一个没到,后一个就已经到了.所以必须先等待前一个到位
                {
                    m_bBeltStaSts[2] = true;
                    ShowLog("顶料工位2位置无料,阻挡气缸3下降");
                    IoMgr.GetInstance().WriteIoBit(2, 12, true);
                    m_bCylStatus[2] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸3下降");
                }
            }
            if (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1下降到位))
            {
                if (false == IoMgr.GetInstance().GetIoInState(2, 23) && true == IoMgr.GetInstance().GetIoInState(2, 22)) //顶料1和顶料2之间缓存位置无料,顶料1有料
                {
                    //m_bBeltStaSts[1] = true;
                    ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.顶料工位1下降到位, false.ToString()));
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.顶料工位1下降到位, false);
                    ShowLog("顶料1工位下降状态,顶料1/2之间缓存位置无料,阻挡气缸2下降");
                    IoMgr.GetInstance().WriteIoBit(2, 11, true);
                    m_bCylStatus[1] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸2下降");
                }
            }
            if (m_bCylStatus[0] && IoMgr.GetInstance().GetIoInState(2, 21)) //进料缓存位置有料
            {
                if (false == IoMgr.GetInstance().GetIoInState(2, 22)  //顶料1位置无料
                    && false == SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1在顶升状态) //顶料1工位在待料状态
                    && false == m_bBeltStaSts[0]) //根据皮带阻挡气缸之间的间距不一,有可能前一个没到,后一个就已经到了.所以必须先等待前一个到位
                {
                    m_bBeltStaSts[0] = true;
                    ShowLog("顶料1工位无料,阻挡气缸1下降");
                    IoMgr.GetInstance().WriteIoBit(2, 10, true);
                    m_bCylStatus[0] = false;
                    WarningMgr.GetInstance().Info("阻挡气缸1下降");
                }
            }
            //如果上层感应1有料,且顶料1/2工位和上盖组装工位在工作状态
            if (!m_bIdle
                && (IoMgr.GetInstance().GetIoInState(2, 21) || (!(IoMgr.GetInstance().GetIoInState(2, 15) && !IoMgr.GetInstance().GetIoInState(2, 16))) ) //上层感应1有料或者进料平台没在上方
                && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1在顶升状态) 
                   || (IoMgr.GetInstance().GetIoInState(2, 23) && IoMgr.GetInstance().GetIoInState(2, 22) && !SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1产品到位))
                   || (!IoMgr.GetInstance().GetIoInState(2, 22) && !m_bBeltStaSts[0] && (DateTime.Now - m_IdleTime2).TotalMilliseconds>500) )//感应2没信号,且1没在出料状态
                && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2在顶升状态)
                   || (IoMgr.GetInstance().GetIoInState(2, 25) && IoMgr.GetInstance().GetIoInState(2, 24) && !SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2产品到位))
                   || (!IoMgr.GetInstance().GetIoInState(2, 24) && !m_bBeltStaSts[2] && !m_bBeltStaSts[3] && (DateTime.Now - m_IdleTime4).TotalMilliseconds > 500))//感应4没信号,且3/4没在出料状态,如果刚好是上升沿则停留500ms再关皮带
                && ((!IoMgr.GetInstance().GetIoInState(3, 5) && IoMgr.GetInstance().GetIoInState(3, 6)) //上盖组装气缸为上升状态
                   || (IoMgr.GetInstance().GetIoInState(2, 26) && !(IoMgr.GetInstance().GetIoInState(3, 18) && !IoMgr.GetInstance().GetIoInState(3, 19)))  //有料且等待出料,并且出料端平台没在上方
                   || (!IoMgr.GetInstance().GetIoInState(2, 26) && !m_bBeltStaSts[4] && (DateTime.Now - m_IdleTime6).TotalMilliseconds > 500
                       && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.出料皮带上升到位) || !(IoMgr.GetInstance().GetIoInState(3, 18) && !IoMgr.GetInstance().GetIoInState(3, 19)))))) //6感应位置无料,且5没在出料状态,且出料端等待进料状态为true或者没在上方
            {
                //如果上层感应1有料,且顶料1/2工位和上盖组装工位在工作状态
                //if (IoMgr.GetInstance().GetIoInState(2, 21) && SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1在顶升状态)
                //&& SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2在顶升状态) && !m_bIdle
                //&& !IoMgr.GetInstance().GetIoInState(3, 5) && IoMgr.GetInstance().GetIoInState(3, 6)) //上盖组装气缸为上升状态
                //{
                ShowLog("上层感应1有料,且顶料1/2工位和上盖组装工位在工作状态,关闭上层传送带");
                m_bIdle = true;
                IoMgr.GetInstance().WriteIoBit(3, 18, false);//关闭上层流水线转动
                //WarningMgr.GetInstance().Info("关闭上层流水线转动");
            }
            else
            {
                if (m_bIdle 
                && !( (IoMgr.GetInstance().GetIoInState(2, 21) || (!(IoMgr.GetInstance().GetIoInState(2, 15) && !IoMgr.GetInstance().GetIoInState(2, 16)))) //上层感应1有料或者进料平台没在上方
                && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1在顶升状态)
                   || (IoMgr.GetInstance().GetIoInState(2, 23) && IoMgr.GetInstance().GetIoInState(2, 22) && !SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位1产品到位))
                   || (!IoMgr.GetInstance().GetIoInState(2, 22) && !m_bBeltStaSts[0]))//感应2没信号,且1没在出料状态
                && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2在顶升状态)
                   || (IoMgr.GetInstance().GetIoInState(2, 25) && IoMgr.GetInstance().GetIoInState(2, 24) && !SystemMgr.GetInstance().GetRegBit((int)SysBitReg.顶料工位2产品到位))
                   || (!IoMgr.GetInstance().GetIoInState(2, 24) && !m_bBeltStaSts[2] && !m_bBeltStaSts[3]))//感应4没信号,且3/4没在出料状态
                && ((!IoMgr.GetInstance().GetIoInState(3, 5) && IoMgr.GetInstance().GetIoInState(3, 6)) //上盖组装气缸为上升状态
                   || (IoMgr.GetInstance().GetIoInState(2, 26) && !(IoMgr.GetInstance().GetIoInState(3, 18) && !IoMgr.GetInstance().GetIoInState(3, 19)))  //有料且等待出料,并且出料端平台没在上方
                   || (!IoMgr.GetInstance().GetIoInState(2, 26) && !m_bBeltStaSts[4] 
                       && (SystemMgr.GetInstance().GetRegBit((int)SysBitReg.出料皮带上升到位) || !(IoMgr.GetInstance().GetIoInState(3, 18) && !IoMgr.GetInstance().GetIoInState(3, 19)))) ) ))//6感应位置无料,且5没在出料状态,且出料端等待进料状态为true或者没在上方
                {
                    ShowLog("开启上层传送带");
                    m_bIdle = false;
                    IoMgr.GetInstance().WriteIoBit(3, 18, true);//开启上层流水线转动
                    //WarningMgr.GetInstance().Info("开启上层流水线转动");
                }
            }
            CheckContinue();
            Thread.Sleep(5);
        }

    }
}
