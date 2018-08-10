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
using System.Drawing;

namespace AutoFrame
{
    /// <summary>
    /// 工位2Tray盘站
    /// </summary>
    class StationTray2 : StationBase
    {
        private int m_nDistance = 120000; //一个tray盘的高度175555
        private int m_nAxisSpeed = 5500000;
        /// <summary>
        /// 奇数行个数
        /// </summary>
        private int m_nTrayOddNum = ProductInfo.GetInstance().m_nTrayOddNum;
        /// <summary>
        /// 偶数行个数
        /// </summary>
        private int m_nTrayEvenNum = ProductInfo.GetInstance().m_nTrayEvenNum;
        /// <summary>
        /// 总行数
        /// </summary>
        private int m_nTrayRows = ProductInfo.GetInstance().m_nTrayRows;
        /// <summary>
        /// 总个数
        /// </summary>
        private int m_nTrayItemCount = ProductInfo.GetInstance().m_nTrayItemCount;
        /// <summary>
        /// 
        /// </summary>
        private TrayPointTranslate m_PointTrans = new TrayPointTranslate();

        List<ProductInfo.POS_DIR> m_listPosDir = new List<ProductInfo.POS_DIR>();
        List<ProductInfo.POS_DIR> m_listVertex = new List<ProductInfo.POS_DIR>();
		
		private bool m_bUpTray2Point = false;
		
        public StationTray2(string strName):base(strName)
        {
            io_in = new string[] { "1.9", "1.10", "1.11", "1.12", "1.13","2.32",
                "4.19", "4.20","4.21", "4.22", "4.23", "4.24", "4.25", "4.26", "4.27", "4.28", "4.29", "4.30", "4.31"};
            io_out = new string[] { "1.10", "1.11", "1.12", "1.13", "4.8", "4.9", "4.10", "4.11", "4.14", "4.15", "4.17" };
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
            trans_point();
            ShowLog("Tray2取料OK信号关闭");
            IoMgr.GetInstance().WriteIoBit(1, 10, false);  //Tray2取料OK,信号关闭
        }

        //轴急停已经由StationBase实现
        //public override void EmgStop()
        //{
        //}//站位急停

        public override void StationDeinit()
        {
            ShowLog("XYZ轴断伺服");
            MotionMgr.GetInstance().ServoOff(AxisX);
            MotionMgr.GetInstance().ServoOff(AxisY);
            MotionMgr.GetInstance().ServoOff(AxisZ);
        }

        public override void StationInit()
        {
            m_nTrayOddNum = ProductInfo.GetInstance().m_nTrayOddNum; //奇数行
            m_nTrayEvenNum = ProductInfo.GetInstance().m_nTrayEvenNum;//偶数行
            m_nTrayRows = ProductInfo.GetInstance().m_nTrayRows; //总行数
            m_nTrayItemCount = ProductInfo.GetInstance().m_nTrayItemCount;//总个数

            ShowLog("XYZ轴上伺服");
            MotionMgr.GetInstance().ServoOn(AxisX);
            MotionMgr.GetInstance().ServoOn(AxisY);
            MotionMgr.GetInstance().ServoOn(AxisZ);
            WaitRegBit((int)SysBitReg.机器人2初始化完成, true);
            z_up();
            rotate_down();
            ShowLog("关真空");
            IoMgr.GetInstance().WriteIoBit(1, 13, false);//关真空
            loosen_tray();
            tray_left();
            MotionMgr.GetInstance().Home(AxisX, 1);//以负极限home方式
            MotionMgr.GetInstance().Home(AxisY, 1);
            MotionMgr.GetInstance().Home(AxisZ, 1);
            WaitHome(AxisX, 500000);
            WaitHome(AxisY, 500000);
            WaitHome(AxisZ, 500000);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.Tray2盘XYHome完成, true); //Tray1 XY轴home OK
            WaitIo(4, 25, true, 5); //等待抽屉到位感应
            lock_leftTray();
            WaitIo(4, 29, true, 5); //等待空盘仓抽屉到位感应
            lock_rightTray();
            //以home后的自动偏移量作为初始位置
            //MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[1].x, 10000); //X轴到初始位
            //MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[1].y, 10000); //Y轴到初始位
            //MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[1].z, 10000); //Z轴到初始位
            //WaitMotion(AxisX, -1);
            //WaitMotion(AxisY, -1);
            //WaitMotion(AxisZ, -1);
            IoMgr.GetInstance().WriteIoBit(4, 17, false); //旋转气缸回原始状态
            WaitTimeDelay(200);
            ShowLog("初始化完成");
            m_bUpTray2Point = true;
        }

        /// <summary>
        /// 锁住左边抽屉气缸
        /// </summary>
        private void unlock_leftTray()
        {
            ShowLog("打开左边抽屉气缸");
            IoMgr.GetInstance().WriteIoBit(4, 10, true);
            WaitIo(4, 26, true, 10000);
            WaitIo(4, 27, false, 10000);
        }

        /// <summary>
        /// 打开左边抽屉气缸
        /// </summary>
        private void lock_leftTray()
        {
            ShowLog("锁住左边抽屉气缸");
            IoMgr.GetInstance().WriteIoBit(4, 10, false);
            WaitIo(4, 26, true, 10000);
            WaitIo(4, 27, false, 10000);
        }

        /// <summary>
        /// 打开右边抽屉气缸
        /// </summary>
        private void unlock_rightTray()
        {
            ShowLog("打开右边抽屉气缸");
            IoMgr.GetInstance().WriteIoBit(4, 11, true);
            WaitIo(4, 30, false, 10000);
            WaitIo(4, 31, true, 10000); 
        }

        /// <summary>
        /// 关闭右边抽屉气缸
        /// </summary>
        private void lock_rightTray()
        {
            ShowLog("关闭右边抽屉气缸");
            IoMgr.GetInstance().WriteIoBit(4, 11, false);
            WaitIo(4, 30, true, 10000);
            WaitIo(4, 31, false, 10000);
        }

        /// <summary>
        /// 搬运气缸到左边
        /// </summary>
        /// <param name="nPassWait">-1:  1:超时跳出等待</param>
        private void tray_left(int nPassWait = -1)
        {
            ShowLog("搬运气缸到左边");
            IoMgr.GetInstance().WriteIoBit(4, 15, false);
            IoMgr.GetInstance().WriteIoBit(4, 9, true);
            if (-1 == nPassWait)
            {
                WaitIo(4, 21, true, 10000);
                WaitIo(4, 22, false, 10000);
            }
            else
            {
                WaitIo(4, 21, true, 5, true);
                WaitIo(4, 22, false, 5, true);
            }
        }

        /// <summary>
        /// 搬运气缸到右边
        /// </summary>
        /// <param name="nPassWait">-1:  1:超时跳出等待</param>
        private void tray_right(int nPassWait = -1)
        {
            ShowLog("搬运气缸到右边");
            IoMgr.GetInstance().WriteIoBit(4, 9, false);
            IoMgr.GetInstance().WriteIoBit(4, 15, true);
            if (-1==nPassWait)
            {
                WaitIo(4, 21, false, 10000);
                WaitIo(4, 22, true, 10000);
            }
            else
            {
                WaitIo(4, 21, false, 5, true);
                WaitIo(4, 22, true, 5, true);
            }
        }

        /// <summary>
        /// 分离气缸松开tray盘
        /// </summary>
        private void loosen_tray()
        {
            ShowLog("分离气缸松开tray盘");
            IoMgr.GetInstance().WriteIoBit(4, 8, true);
            IoMgr.GetInstance().WriteIoBit(4, 14, false);
            WaitIo(4, 19, true, 10000);
            WaitIo(4, 20, false, 10000);
        }

        /// <summary>
        /// 分离气缸夹住tray盘
        /// </summary>
        private void clamp_tray()
        {
            ShowLog("分离气缸夹住tray盘");
            IoMgr.GetInstance().WriteIoBit(4, 8, false);
            IoMgr.GetInstance().WriteIoBit(4, 14, true);
            WaitIo(4, 19, false, 10000);
            WaitIo(4, 20, true, 10000);
        }

        /// <summary>
        /// 旋转气缸向下
        /// </summary>
        private void rotate_up()
        {
            ShowLog("旋转气缸向下");
            IoMgr.GetInstance().WriteIoBit(1, 12, true);
            WaitIo(1, 11, false);
            WaitIo(1, 12, true);
        }

        /// <summary>
        /// 旋转气缸向上
        /// </summary>
        private void rotate_down()
        {
            ShowLog("旋转气缸向上");
            IoMgr.GetInstance().WriteIoBit(1, 12, false);
            WaitIo(1, 11, true, 10000);
            WaitIo(1, 12, false, 10000);
        }

        /// <summary>
        /// Z轴气缸向上
        /// </summary>
        private void z_up()
        {
            ShowLog("Z轴气缸向上");
            IoMgr.GetInstance().WriteIoBit(1, 11, false);
            WaitIo(1, 9, true, 10000);
            WaitIo(1, 10, false, 10000);
        }

        /// <summary>
        /// Z轴气缸向下
        /// </summary>
        private void z_down()
        {
            ShowLog("Z轴气缸向下");
            IoMgr.GetInstance().WriteIoBit(1, 11, true);
            WaitIo(1, 9, false, 10000);
            WaitIo(1, 10, true, 10000);
        }

        /// <summary>
        /// 得到tray盘点位坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        //private Point get_point(int index)
        //{
        //    //int nDisOr = m_dicPoint[7].y - m_dicPoint[4].y;//行间距
        //    //int nDisOc = m_dicPoint[5].x - m_dicPoint[4].x;//列间距
        //    int nDisOr = (m_dicPoint[6].y - m_dicPoint[4].y) / (m_nTrayRows % 2 == 0 ? m_nTrayRows / 2 - 1 : m_nTrayRows / 2);//行间距
        //    int nDisOc = (m_dicPoint[5].x - m_dicPoint[4].x) / (m_nTrayOddNum - 1);//列间距

        //    //以奇数行和偶数行合并算作一大行
        //    int xPlus = 0, yPlus = 0;
        //    int nCol = 1; //列号
        //    int nRow = index / (m_nTrayOddNum + m_nTrayEvenNum);//行号
        //    int nRemainder = index % (m_nTrayOddNum + m_nTrayEvenNum);//余数
        //    if (nRemainder < m_nTrayOddNum) //小于奇数行个数,行号为奇数
        //    {
        //        nCol = nRemainder;
        //        xPlus = m_dicPoint[4].x + nCol * nDisOc;
        //        yPlus = m_dicPoint[4].y + nRow * nDisOr;
        //    }
        //    else  //行号为偶数
        //    {
        //        nCol = nRemainder - m_nTrayOddNum;
        //        xPlus = m_dicPoint[8].x + nCol * nDisOc;
        //        yPlus = m_dicPoint[8].y + nRow * nDisOr;
        //    }
        //    return new Point(xPlus, yPlus);
        //}

        /// <summary>
        /// 得到tray盘点位坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Point get_point(int index)
        {
            double xPlus = 0, yPlus = 0;
            m_PointTrans.Translate(m_listPosDir[index].x, m_listPosDir[index].y, out xPlus, out yPlus);
            return new Point((int)xPlus, (int)yPlus);
        }

        /// <summary>
        /// 形成tray盘点位映射
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private void trans_point()
        {
            m_listPosDir = ProductInfo.GetInstance().m_listPosDir;
            m_listVertex = ProductInfo.GetInstance().m_listVertex;
            m_PointTrans.ClearAllData();
            m_PointTrans.AppendPointData(m_listVertex[0].x, m_listVertex[0].y, m_dicPoint[4].x, m_dicPoint[4].y);
            m_PointTrans.AppendPointData(m_listVertex[1].x, m_listVertex[1].y, m_dicPoint[5].x, m_dicPoint[5].y);
            m_PointTrans.AppendPointData(m_listVertex[2].x, m_listVertex[2].y, m_dicPoint[6].x, m_dicPoint[6].y);
            m_PointTrans.AppendPointData(m_listVertex[3].x, m_listVertex[3].y, m_dicPoint[7].x, m_dicPoint[7].y);
            m_PointTrans.CalcCalib();
        }

        /// <summary>
        /// 送出fpc
        /// </summary>
        private void send_fpc(int i)
        {
            if (IoMgr.GetInstance().GetIoInState(1, 13) //fpc翻转真空2检测
                || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑屏蔽真空检测
            {
                IoMgr.GetInstance().WriteIoBit(1, 12, true);
                //rotate_up();//先翻转，防止撞到机械手

                if (i >= 0 && i <= 20) //第一排的后面几个点位先回X轴，防止撞到微动开关
                {
                    rotate_up();//先翻转，防止撞到机械手
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, m_nAxisSpeed); //固定给机器人取料点
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, m_nAxisSpeed);
                    WaitMotion(AxisX, -1);
                    WaitMotion(AxisY, -1);
                }
                else
                { 
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, m_nAxisSpeed); //固定给机器人取料点
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, m_nAxisSpeed);
                    WaitMotion(AxisX, -1);
                    WaitMotion(AxisY, -1);
                    rotate_up();
                }
                if (IoMgr.GetInstance().GetIoInState(1, 13)//fpc翻转真空2检测
                    || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑屏蔽真空检测
                {
                    if (m_listPosDir[i].nDir == -1)
                    {
                        IoMgr.GetInstance().WriteIoBit(4, 17, true); //旋转气缸旋转
                        WaitTimeDelay(200);
                    }
                    ShowLog("Tray2取料OK信号开启,并等待机器人2取料OK信号");
                    IoMgr.GetInstance().WriteIoBit(1, 10, true);  //Tray2取料OK,信号开启
                    WaitIo(1, 6, false, -1, false, true);
                    WaitIo(1, 6, true, -1, false, true);//用上升沿检测等待机器人2取料OK信号,关真空由机器人站控制
                    if (m_listPosDir[i].nDir == -1)
                    {
                        IoMgr.GetInstance().WriteIoBit(4, 17, false); //旋转气缸回原始状态
                        WaitTimeDelay(200);
                    }
                    IoMgr.GetInstance().WriteIoBit(1, 10, false);  //Tray2取料OK,信号关闭
                }
                else
                {
                    ShowLog("关真空");
                    IoMgr.GetInstance().WriteIoBit(1, 13, false); //关真空
                }
            }
            else
            {
                ShowLog("关真空");
                IoMgr.GetInstance().WriteIoBit(1, 13, false); //关真空
            }
        }

        /// <summary>
        /// 搬运tray
        /// </summary>
        private void change_tray()
        {
            WarningMgr.GetInstance().Info("tray盘2工位开始换料");
            int bZDown2 = 0;
            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[1].x, m_nAxisSpeed); //X轴到分离位
            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[1].y, m_nAxisSpeed); //Y轴到分离位
            //先夹紧Z轴再下降
            clamp_tray();

            long lZPos = MotionMgr.GetInstance().GetAixsPos(AxisZ);
            if (lZPos > 2000000)
            {
                bZDown2 = 1;
                MotionMgr.GetInstance().RelativeMove(AxisZ, -2000000/*5*m_nDistance*/, 2000000); //Z轴下降一段距离
            }
            else if (lZPos > 1000000)
            {
                bZDown2 = 2;
                MotionMgr.GetInstance().RelativeMove(AxisZ, -1000000/*5*m_nDistance*/, 2000000); //Z轴下降一段距离
            }
            else if (lZPos > 500000)
            {
                bZDown2 = 3;
                MotionMgr.GetInstance().RelativeMove(AxisZ, -500000/*5*m_nDistance*/, 2000000); //Z轴下降一段距离
            }
            else
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[1].z, 2000000); //Z轴到换料位置
            }
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            rotate_down();
            z_down();
            WaitTimeDelay(200);
            z_up();
            z_down();
            WaitTimeDelay(200);
            z_up();
            if (IoMgr.GetInstance().GetIoInState(4, 28)) //检测空盘仓是否已满
            {
                unlock_rightTray();
                while (true)
                {
                    IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                    IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                    if (DialogResult.Yes == ShowMessage("第二工位空盘仓已满,请清料"))
                    //if (DialogResult.Yes == MessageBox.Show( "第二工位空盘仓已满,请清料","清料提示", MessageBoxButtons.YesNo))
                    {
                        WaitIo(4, 29, false);
                        WaitIo(4, 29, true);
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        lock_rightTray();
                        break;
                    }
                    else
                    {
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        WaitTimeDelay(3000);
                    }
                }
            }
            tray_right(1);
            WaitTimeDelay(50);
            if (IoMgr.GetInstance().GetIoInState(4, 21) || !IoMgr.GetInstance().GetIoInState(4, 22)) //tray盘2右移没到位
            {
                IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                while (true)
                {
                    if (DialogResult.No == ShowMessage("tray2换盘超时,请检查!\n处理完毕点击<退出运行>？"))
                    {
                        if (!IoMgr.GetInstance().GetIoInState(4, 21) || IoMgr.GetInstance().GetIoInState(4, 22)) //tray盘2右移到位
                        {
                            break;
                        }
                    }
                    else
                    {
                        WaitTimeDelay(10);
                    }
                }
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
            }
            loosen_tray();
            tray_left(1);
            WaitTimeDelay(50);
            if (!IoMgr.GetInstance().GetIoInState(4, 21) || IoMgr.GetInstance().GetIoInState(4, 22)) //tray盘2左移没到位
            {
                IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                while (true)
                {
                    if (DialogResult.No == ShowMessage("tray2换盘超时,请检查!\n处理完毕点击<退出运行>？"))
                    {
                        if (IoMgr.GetInstance().GetIoInState(4, 21) || !IoMgr.GetInstance().GetIoInState(4, 22)) //tray盘2左移到位
                        {
                            break;
                        }
                    }
                    else
                    {
                        WaitTimeDelay(10);
                    }
                }
                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
            }
            if (1==bZDown2)
            {
                MotionMgr.GetInstance().RelativeMove(AxisZ, 2000000/*5 * m_nDistance*/, 2000000); //Z轴上升一段距离
            }
            else if (2==bZDown2)
            {
                MotionMgr.GetInstance().RelativeMove(AxisZ, 1000000/*5 * m_nDistance*/, 2000000); //Z轴上升一段距离
            }
            else if (3 == bZDown2)
            {
                MotionMgr.GetInstance().RelativeMove(AxisZ, 500000/*5 * m_nDistance*/, 2000000); //Z轴上升一段距离
            }
            else
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, (int)lZPos, 2000000); //Z轴上升到之前位置
            }
            WaitMotion(AxisZ, -1);
            WarningMgr.GetInstance().Info("tray盘2工位换料结束");
        }

        /// <summary>
        /// tray盘上升
        /// </summary>
        private void tray_up()
        {
            bool bFlag = true;
            if (m_bUpTray2Point)
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[1].z, 2000000); //移动到该点位
                WaitMotion(AxisZ, -1);
                m_bUpTray2Point = false;
            }
            if (Math.Abs(MotionMgr.GetInstance().GetAixsPos(AxisZ) + m_nDistance) < Math.Abs(m_dicPoint[3].z))
            {
                MotionMgr.GetInstance().RelativeMove(AxisZ, m_nDistance, 2000000);
                WaitMotion(AxisZ, -1);

                WaitTimeDelay(50);
                if (IoMgr.GetInstance().GetIoInState(2, 32)
                    || SystemMgr.GetInstance().Mode == SystemMode.Simulate_Run_Mode)
                {
                    while (true)
                    {
                        CheckContinue();
                        WaitTimeDelay(50);
                        if (IoMgr.GetInstance().GetIoInState(2, 32)
                            || SystemMgr.GetInstance().Mode == SystemMode.Simulate_Run_Mode || bFlag)
                        {
                            bFlag = false;
                            MotionMgr.GetInstance().RelativeMove(AxisZ, -m_nDistance / 5, 2000000);
                            WaitMotion(AxisZ, -1);
                        }
                        else
                        {
                            MotionMgr.GetInstance().RelativeMove(AxisZ, m_nDistance / 5+3000, 2000000);
                            WaitMotion(AxisZ, -1);
                            break;
                        }
                    }
                }
            }
            else if (Math.Abs(Math.Abs(m_dicPoint[3].z) - Math.Abs(MotionMgr.GetInstance().GetAixsPos(AxisZ))) >= 5000/*m_nDistance/4*/)
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[3].z, 2000000);
                WaitMotion(AxisZ, -1);

                WaitTimeDelay(50);
                if (IoMgr.GetInstance().GetIoInState(2, 32)
                    || SystemMgr.GetInstance().Mode == SystemMode.Simulate_Run_Mode)
                {
                    while (true)
                    {
                        CheckContinue();
                        WaitTimeDelay(50);
                        if (IoMgr.GetInstance().GetIoInState(2, 32)
                            || SystemMgr.GetInstance().Mode == SystemMode.Simulate_Run_Mode || bFlag)
                        {
                            bFlag = false;
                            MotionMgr.GetInstance().RelativeMove(AxisZ, -m_nDistance / 5, 2000000);
                            WaitMotion(AxisZ, -1);
                        }
                        else
                        {
                            MotionMgr.GetInstance().RelativeMove(AxisZ, m_nDistance / 5 + 3000, 2000000);
                            WaitMotion(AxisZ, -1);
                            break;
                        }
                    }
                }
            }
            else
            {
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[1].z, 2000000);
                WaitMotion(AxisZ, -1);
                unlock_leftTray();
                unlock_rightTray();
                m_bUpTray2Point = true;//需要移动该点位
                while (true)
                {
                    IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                    IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                    if (DialogResult.Yes == ShowMessage("第二工位tray盘已空请上料"))
                    //if (DialogResult.Yes == MessageBox.Show( "第二工位tray盘已空请上料","换料提示", MessageBoxButtons.YesNo))
                    {
                        WaitIo(4, 25, false);
                        WaitIo(4, 25, true);
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        lock_leftTray();
                        break;
                    }
                    else
                    {
                        IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                        IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                        WaitTimeDelay(3000);
                    }
                }
            }
        }

        public override void StationProcess()
        {
            WaitTimeDelay(50);
            if (IoMgr.GetInstance().GetIoInState(2, 32)
                || SystemMgr.GetInstance().Mode == SystemMode.Simulate_Run_Mode) //平整对射感应检测有料
            {
                //clamp_tray();
                WarningMgr.GetInstance().Info("tray盘2工位取料开始");
                WaitTimeDelay(500);
                for (int i = 0; i < m_nTrayItemCount; i++)
                {
                    Point pt = get_point(i);
                    MotionMgr.GetInstance().AbsMove(AxisX, pt.X, m_nAxisSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, pt.Y, m_nAxisSpeed);
                    IoMgr.GetInstance().WriteIoBit(1, 12, false);
                    WaitMotion(AxisX, -1);
                    WaitMotion(AxisY, -1);
                    rotate_down();
                    
                    ShowLog("开真空");
                    IoMgr.GetInstance().WriteIoBit(1, 13, true); //开真空
                    z_down();
                    
                    WaitTimeDelay(50);
                    z_up();
                    //WaitTimeDelay(50);
                    send_fpc(i);
                }
                change_tray();
            }
            tray_up();
        }
    }
}
