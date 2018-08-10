using System;
using System.Threading;
using AutoFrameDll;
using CommonTool;
using Communicate;
using AutoFrameVision;
using System.Windows.Forms;

namespace AutoFrame
{
    /// <summary>
    /// 工位1机器人站
    /// </summary>
    class StationRobot1 : StationBase
    {
        private TcpLink m_tcpRobotCtl;
        private TcpLink m_tcpRobotComm;
        private bool bFlagFirstRun = false;

        public StationRobot1(string strName) : base(strName)
        {
            io_in = new string[] { "1.5", "1.7" };
            io_out = new string[] { };
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
            m_tcpRobotCtl = TcpMgr.GetInstance().GetTcpLink(0);
            m_tcpRobotComm = TcpMgr.GetInstance().GetTcpLink(1);
        }

        public override void InitSecurityState()
        {
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.机器人1初始化完成, false.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1初始化完成, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, false);
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

        public override void EmgStop()
        {
            if (m_tcpRobotCtl.IsOpen())
                m_tcpRobotCtl.WriteLine("SP");//机器人停止命令
        }//站位急停

        public override void StationDeinit()
        {
            ShowLog("关闭机器人1控制和通讯网口");
            if (m_tcpRobotCtl.IsOpen())
                m_tcpRobotCtl.WriteLine("SP");
            m_tcpRobotComm.Close();
            m_tcpRobotCtl.Close();
        }

        /// <summary>
        /// 机器人启动初始化
        /// </summary>
        /// <returns></returns>
        private bool RobotInit()
        {
            ShowLog("打开机器人1控制网口");
            byte[] bytes = new byte[1024];
            m_tcpRobotCtl.Open();
            Thread.Sleep(200);
            if (m_tcpRobotCtl.IsOpen())
            {
                m_tcpRobotCtl.WriteLine("SP");
                Thread.Sleep(500);
                m_tcpRobotCtl.ReadData(bytes, 1024);
                m_tcpRobotCtl.WriteLine("RS,ERR");
                WaitTimeDelay(700);
                m_tcpRobotCtl.WriteLine("SL,robot1");
                WaitTimeDelay(700);
                m_tcpRobotCtl.WriteLine("SO");
                WaitTimeDelay(3000);
                m_tcpRobotCtl.WriteLine("RS,PRG");
                WaitTimeDelay(700);
                m_tcpRobotCtl.WriteLine("RN");
                WaitTimeDelay(700);
                m_tcpRobotCtl.ReadData(bytes, 1024);
                return true;
            }
            return false;
        }

        public override void StationInit()
        {
            m_tcpRobotComm.Open();
            if (RobotInit())
            {
                ShowLog("打开机器人1通讯网口");
                string strTypeName = ProductInfo.GetInstance().ProductName;
                if (strTypeName == "0291_0376")
                {
                    strTypeName = "0291";
                }
                m_tcpRobotComm.WriteLine(string.Format("1,{0},{1},0",SystemMgr.GetInstance().GetParamDouble("SystemSpeed")/10, Convert.ToDouble(strTypeName)));
                wait_recevie_cmd(m_tcpRobotComm, "BACKHOME/OK", 20000);
            }
            else
            {
                throw new StationException("机器人1初始化启动失败");
            }
            ShowLog(string.Format("{0}系统位寄存器{1},写入有效值{2}", Name, (int)SysBitReg.机器人1初始化完成, true.ToString()));
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1初始化完成, true); //机器人已经初始化完成
            ShowLog("初始化完成");
            bFlagFirstRun = true;
            //SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, false);
        }

        //public override bool IsReady()
        //{
        //    return true; //全自动不需要按开始按钮
        //}

        public void CaliProcess()
        {
            string[] strCalib = new string[11];//机器人的坐标
            double[] fpix_x = new double[11];//像素x坐标
            double[] fpix_y = new double[11];//像素y坐标
            double[] fpix_a = new double[11];//像素u坐标
            double[] faxis_x = new double[11];//机器人x坐标
            double[] faxis_y = new double[11];//机器人y坐标
            double[] faxis_a = new double[11];//机器人u坐标
            m_tcpRobotComm.WriteLine("8,0,0,0");//机器人标定
            Thread.Sleep(200);
            //wait_recevie_cmd(m_tcpRobotComm, "ROBOT_CATCH/OK");
            //IoMgr.GetInstance().WriteIoBit(4, 7, false);//关闭fpc翻转真空1
            ////WaitIo(1, 5, true);//等待机器人1取料OK
            ////wait_recevie_cmd(m_tcpRobotComm, "ROBOT_JUMP_PHOTO/OK");//机器人到拍照点
            wait_recevie_cmd(m_tcpRobotComm, "CALIB/READY");//机器人到拍照点
            while(true)
            {
                if (DialogResult.Yes == MessageBox.Show("机器人1到标定点,请人工让吸嘴吸料?\n放弃此次标定点击否!", "标定提示", MessageBoxButtons.YesNo)) 
                {
                    break;
                }
                else
                {
                    return;
                }
            }
            m_tcpRobotComm.WriteLine("18,0,0,0");
            int i = 0;
            CaliTranslate caliTrans = new CaliTranslate();
            caliTrans.ClearAllData();
            while (true)
            {
                m_tcpRobotComm.ReadLine(out strCalib[i]);
                if (strCalib[i].Length > 0)
                {
                    string[] strRead = strCalib[i].Split(',');
                    faxis_x[i] = Convert.ToDouble(strRead[0].Trim());
                    faxis_y[i] = Convert.ToDouble(strRead[1].Trim());
                    faxis_a[i] = Convert.ToDouble(strRead[2].Trim());
                    if (VisionMgr.GetInstance().ProcessStep("T2_calib"))  //拍照
                    {
                        fpix_x[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T22_X);
                        fpix_y[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T22_Y);
         //               fpix_a[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T22_A);
                        i++;//拍到一次增加一次
                        m_tcpRobotComm.WriteLine("80,0,0,0");
                        if (i == 11)
                        {
                            for (int k = 0; k < 9; ++k)
                                caliTrans.AppendPointData(fpix_x[k], fpix_y[k], faxis_x[k], faxis_y[k]);

                            caliTrans.AppendRotateData(fpix_x[9], fpix_y[9], fpix_x[10], fpix_y[10], 60);//计算机器人旋转中心点
                            if (caliTrans.CalcCalib())
                            {
                                caliTrans.SaveCaliData(VisionMgr.GetInstance().ConfigDir + "T2_2\\Calib.ini");
                                MessageBox.Show("机器人1标定完成!");
                            }
                            else
                            {
                                MessageBox.Show("机器人1标定失败!请检查确认流程是否正确！");
                            }
                            break;
                        }

                    }
                    else
                    {
                        if (DialogResult.Yes == ShowMessage("机器人1标定图像处理失败，是否重试"))
                        {
                            m_tcpRobotComm.WriteLine("81,0,0,0");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                CheckContinue();
            }
        }

        public override void StationProcess()
        {
            //第一次时需要使用启动按钮启动
        //    WaitBegin();
            if (SystemMgr.GetInstance().Mode == SystemMode.Calib_Run_Mode) //标定
            {
                CaliProcess();
                SwitchState(StationState.STATE_PAUSE);
                CheckContinue();
            }
            else
            {
                if(bFlagFirstRun)
                {
                    WaitRegBit((int)SysBitReg.Tray1盘XYHome完成, true); //第一次的时候等待XY Home完成
                    bFlagFirstRun = false;
                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.Tray1盘XYHome完成, false); //清掉Tray1 XY轴home
                }
                
                m_tcpRobotComm.WriteLine("2,0,0,0");
                Thread.Sleep(200);          
                int cnts = 0;
                while (true)
                {
                    wait_recevie_cmd(m_tcpRobotComm, "ROBOT_CATCH/OK", -1, false, false);
                    IoMgr.GetInstance().WriteIoBit(4, 7, false);//关闭fpc翻转真空1
                    WaitIo(1, 5, true, -1, false, true);//等待机器人1取料OK
                    wait_recevie_cmd(m_tcpRobotComm, "ROBOT_JUMP_PHOTO/OK");//机器人到拍照点
                    
                    while (true)
                    {
                        if (VisionMgr.GetInstance().ProcessStep("T2_2")
                            || SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode) //空跑模式屏蔽拍照
                        {
                            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, false);
                            double t2x = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T2_Delta_X);
                            double t2y = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T2_Delta_Y);
                            double t2a = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T2_Delta_A);
                            if (SystemMgr.GetInstance().Mode == SystemMode.Dry_Run_Mode)
                            {
                                t2x = 0;
                                t2y = 0;
                                t2a = 0;
                            }

                            if (Math.Abs(t2x) < 0.5 && Math.Abs(t2y) < 0.5 && Math.Abs(t2a) < 0.3)
                            {
                                string str = string.Format("6,{0:0.000},{1:0.000},{2:0.000}", t2x, t2y, t2a);
                                m_tcpRobotComm.WriteLine(str);
                                System.Diagnostics.Debug.WriteLine(str);
                            }
                            else
                            {
                               string str =  string.Format("7,{0:0.000},{1:0.000},{2:0.000}", t2x, t2y, t2a);
                                m_tcpRobotComm.WriteLine(str);
                                System.Diagnostics.Debug.WriteLine(str);
                            }
                            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.机器人1组装数, 1, true); //机器人1组装数加1
                            WaitIo(1, 7, true, 50000, false, true); //等待机器人组装完成
                            cnts = 0; //正常组装则计数清零
                            break;
                        }
                        else
                        {
                            //if (DialogResult.Yes == ShowMessage("T2_2拍照失败，是否继续？"))
                            //{

                            //}
                            //else 
                            //{
                            cnts++;
                            if (cnts >= 3)
                            {
                                IoMgr.GetInstance().AlarmLight(LightState.红灯开);
                                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣开);
                                if (DialogResult.Yes == ShowMessage("机器人1连续抛料超过3次，请检查！\n继续抓取点击<继续运行>？放弃此次组装点击<退出运行>？"))
                                {
                                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, true);
                                }
                                else
                                {
                                    SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, false);
                                }
                                IoMgr.GetInstance().AlarmLight(LightState.绿灯闪);
                                IoMgr.GetInstance().AlarmLight(LightState.蜂鸣关);
                            }
                            else
                            {
                                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.机器人1抛料, true);
                            }
                            m_tcpRobotComm.WriteLine("11,0,0,0"); //发送机器人仍料命令
                            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.机器人1抛料数, 1, true);//机器人1抛料数加1
                            WaitIo(1, 7, true, 50000, false, true); //等待机器人组装完成
                            break;
                            //}
                        }

                    }
                   
                    //if (++n > 30)
                    //{
                    //    m_tcpRobotCtl.WriteLine("VR");
                    //    string strData = "";
                    //    m_tcpRobotCtl.ReadLine(out strData);
                    //    WaitCommunicate(m_tcpRobotCtl);
                    //}
                }

            }
        }
    }
}