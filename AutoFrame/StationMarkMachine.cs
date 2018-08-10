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
    class StationMarkMachine : StationBase
    {
        private string constring = "User ID=MES_PUBLIC;Password=MES_PUBLIC;Data Source=SUHCMES;";

        object data0;
        object data1;
        object data2;
        object ss;
        int GoSnapSpeed = SystemMgr.GetInstance().GetParamInt("GoSnapSpeed");
        int GoMarkingSpeed = SystemMgr.GetInstance().GetParamInt("GoMarkingSpeed");
        int GoPickSpeed = SystemMgr.GetInstance().GetParamInt("GoPickSpeed");
        public int m_nType = 0;//产品类型
        public string[] m_strStep = {"T1_4","T1_3", "T1_4",
            "T1_1", "T1_10", "T1_11", "T1_12", "T1_13", "T1_14","T1_15","T1_16",
            "T1_17","T1_18","T1_19","T1_20","T1_21","T1_22","T1_23","T1_24","T1_25" };//T1_2被单独处理
        double m_fx, m_fy;
        public int isscan;
        CaliTranslate Calib = new CaliTranslate();
        
        public StationMarkMachine(string strName) : base(strName)
        {
            io_in = new string[] {  "1.3","1.5", "1.6" };
            io_out = new string[] { "1.1", "1.2","1.3", "1.6" , "1.14","1.15","1.16","1.8"};
            bPositiveMove = new bool[] { true, true, true, true };
            strAxisName = new string[] { "X轴", "Y轴", "Z轴", "U轴" };
        }

        //public override bool IsReady()
        //{
        //    return true; //全自动不需要按开始按钮
        //}

        public override void InitSecurityState()
        {
            //激光控制初始化
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00000
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", false);
            IoMgr.GetInstance().WriteIoBit("打标程序4", false);
            MotionMgr.GetInstance().StopAxis(AxisX);
            MotionMgr.GetInstance().StopAxis(AxisY);
            MotionMgr.GetInstance().StopAxis(AxisZ);
        }

        public override void StationInit()
        {
            MotionMgr.GetInstance().ServoOn(AxisX);
            MotionMgr.GetInstance().ServoOn(AxisY);
            MotionMgr.GetInstance().ServoOn(AxisZ);
            WaitTimeDelay(200);

            MotionMgr.GetInstance().Home(AxisZ, 1);//Z轴回原点
            WaitHome(AxisZ, -1);
            
            MotionMgr.GetInstance().Home(AxisX, 0);//XY轴回原点
            MotionMgr.GetInstance().Home(AxisY, 0);//XY轴回原点
            WaitHome(AxisX, -1);
            WaitHome(AxisY, -1);

            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, GoPickSpeed);//XY移动到上料地点
            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, GoPickSpeed);//XY移动到上料地点
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);

            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, GoPickSpeed);//XY移动到上料地点
            WaitMotion(AxisZ, -1);
            //激光控制初始化
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00000
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", false);
            IoMgr.GetInstance().WriteIoBit("打标程序4", false);
            ShowLog("初始化完成");
        }

        public override void StationDeinit()
        {
            MotionMgr.GetInstance().StopAxis(AxisX);
            MotionMgr.GetInstance().StopAxis(AxisY);
            MotionMgr.GetInstance().StopAxis(AxisZ);
            MotionMgr.GetInstance().ServoOff(AxisX);
            MotionMgr.GetInstance().ServoOff(AxisY);
            MotionMgr.GetInstance().ServoOff(AxisZ);
            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.控件使能, 3, true);
        }
        
        /// <summary>
        /// 上料扫码
        /// </summary>
        /// <returns></returns>
        public bool ScanCode()
        {
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.条码更新, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.是否扫码, false);
            isscan = SystemMgr.GetInstance().GetRegInt((int)SysIntReg.是否扫码);//如果等于1就要扫码如果等于2就不需要扫码
            ShowLog("正在查询条码...");
            if (!QueryData())//扫到数据，并且获得服务器数据
                return false;//没有扫到数据
            ShowLog("条码查询成功,请按启动按钮");
            WaitIo("启动", true, -1);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.条码更新, true);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.是否扫码, true);
            m_nType = SystemMgr.GetInstance().GetRegInt((int)SysIntReg.工件类型);
            isscan = SystemMgr.GetInstance().GetRegInt((int)SysIntReg.是否扫码);//如果等于1就要扫码如果等于2就不需要扫码
            if (!QueryData())//扫到数据，并且获得服务器数据
                return false;//没有扫到数据
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, true);//开启安全光栅检测
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.扫码上料完成, true);//自动界面显示上料站完成信号
            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.进度百分比, 25, true);//更新进度条
            return true;
        }

        /// <summary>
        /// 读取服务器数据
        /// </summary>
        /// <returns></returns>
        public bool QueryData()
        {
            m_nType = SystemMgr.GetInstance().GetRegInt((int)SysIntReg.工件类型);
            //如果是上盖模块和上盖模块RST，不需要扫码连接数据库直接过去拍照打标；
            if (m_nType == 3 || m_nType == 1)
            {
                return true;
            }
            //如果是新增模块n且未勾选是否扫码即默认为不需要扫码连接数据库获取数据，直接过去拍照打标；
            else if (m_nType >= 5 && isscan == 2)
            {
                return true;
            }
            //如果是其他新增模块，且勾选扫码需要获取数据库数据，查询数据库。
            else if (strSn.ScanCode_Data != string.Empty && m_nType >= 5)
            {
                //获取服务器数据,数据库查找
                string table0 = SystemMgr.GetInstance().GetParamString("Table0");//表
                string field0 = SystemMgr.GetInstance().GetParamString("Field0");//字段
                string table1 = SystemMgr.GetInstance().GetParamString("Table1");//表
                string field1 = SystemMgr.GetInstance().GetParamString("Field1");//字段
                string table2 = SystemMgr.GetInstance().GetParamString("Table2");//表
                string field2 = SystemMgr.GetInstance().GetParamString("Field2");//字段
                string comstring0 = "select"+" "+ field0 + " " + "from" + " " + table0 + " " + " where prtsn = '" + strSn.ScanCode_Data + "'";
                string comstring1 = "select" + " " + field1 + " " + "from" + " " + table1 + " " + " where prtsn = '" + strSn.ScanCode_Data + "'";
                string comstring2 = "select" + " " + field2 + " " + "from" + " " + table2 + " " + " where prtsn = '" + strSn.ScanCode_Data + "'";
                if (OracleHelper.IsConnected(constring))
                {
                    data0 = OracleHelper.ExecuteScalar(comstring0, constring);
                    WaitTimeDelay(10);
                    data1 = OracleHelper.ExecuteScalar(comstring1, constring);
                    WaitTimeDelay(10);
                    data2 = OracleHelper.ExecuteScalar(comstring2, constring);
                }
                else
                {
                    ShowLog("新增模块数据库连接失败，请检查硬件连接或数据库服务端");
                    return false;
                }
                string DATA0 = Convert.ToString(data0);
                string DATA1 = Convert.ToString(data1);
                string DATA2 = Convert.ToString(data2);
                ////已经接收到服务器发来的数据
                if (DATA0 != string.Empty&& DATA1 != string.Empty && DATA2 != string.Empty)
                {
                    strSn.Code_others0 = data0.ToString();
                    strSn.Code_others1 = data1.ToString();
                    strSn.Code_others2 = data2.ToString();
                    return true;
                }
                //未接收到服务器发来的数据
                else
                {
                    ShowLog("已连接到新增模块数据库，但未找到该条码相应信息或数据无");
                    return false;
                }
            }
            //剩下的是整流和逆变模块且已经接收到数据则需要查询数据库，老版本的查询方式，新版本则只提取数据库字段内容
            else if (strSn.ScanCode_Data != string.Empty)
            {
                //获取服务器数据,数据库查找
                string comstring = "select mattypedsc from printdata where prtsn = '" + strSn.ScanCode_Data + "'";
                if (OracleHelper.IsConnected(constring))
                {
                    ss = OracleHelper.ExecuteScalar(comstring, constring);
                }
                else
                {
                    ShowLog("数据库连接失败，请检查硬件连接或数据库服务端");
                    return false;
                }
                string DATA = Convert.ToString(ss);
                ////已经接收到服务器发来的数据
                if (DATA != string.Empty)
                {
                    //缓存数据
                    switch (m_nType)
                    {
                        case 1://上盖RST模块不需要打动态标
                            break;
                        case 2://逆变模块
                            strSn.Code_4 = ss.ToString().Substring(9, 2);//SV82N2S2C2CIM1;
                            strSn.Code_5 = ss.ToString() + ";" + strSn.ScanCode_Data;
                            break;
                        case 3:
                            //上盖模块不需要打动态标
                            break;
                        case 4://整机模块   //例：SV820N2S2C2C
                            strSn.Code_1 = strSn.ScanCode_Data;//1点，//SN码
                            strSn.Code_2 = ss.ToString().Substring(6, 2);//2点，//2S
                            strSn.Code_3 = ss.ToString().Substring(0, 6);//3点，//SV820N
                            strSn.Code_8 = strSn.ScanCode_Data;//8点
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                //未接收到服务器发来的数据
                else
                {
                    ShowLog("已连接到数据库，但未找到该条码相应信息或数据无");
                    return false;
                }
            }
            //未接收到数据
            else
            {
                ShowLog("无SN扫码数据");
                return false;
            }
        }
        /// <summary>
        /// 打23号点
        /// </summary>
        /// <returns></returns>
        public void Marking23Point()
        {
            ShowLog("标刻就绪，准备标刻整流23模块");
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00010
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", true);
            IoMgr.GetInstance().WriteIoBit("打标程序4", false);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", true);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", false);
            ShowLog("标刻中，等待标刻完成");
            WaitIo("标刻完成", true);
            ShowLog("标刻完成");
        }
        /// <summary>
        /// 打8号点,需要退出人工掀盖
        /// </summary>
        /// <returns></returns> 
        public void Marking8Point()
        {
            ShowLog("标刻就绪，准备标刻整流模块8");
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00011
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", true);
            IoMgr.GetInstance().WriteIoBit("打标程序4", true);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", true);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", false);
            ShowLog("标刻中，等待标刻完成");
            WaitIo("标刻完成", true);
            ShowLog("整流8标刻完成");
        }
        /// <summary>
        /// 镭射整流模块
        /// </summary>
        public void MarkingCurrent()
        {
            //移动到23标刻位置，从标刻点1到标刻点23X往右移动10000；Y往前移动25000;高度为23打标高度
            MotionMgr.GetInstance().RelativeMove(AxisX,10000, GoMarkingSpeed);
            MotionMgr.GetInstance().RelativeMove(AxisY,-(15000+10000), GoMarkingSpeed);
            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[13].z+20000, GoMarkingSpeed);//换到了整机上往上抬20mm
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            WaitTimeDelay(50);
            ShowLog("标刻位置就绪，准备标刻");
            Marking23Point();//2、3位置打标
            //退出到上料点,准备掀盖
            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, GoPickSpeed);
            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, GoPickSpeed);
            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, GoPickSpeed);
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.进度百分比, 75, true);//更新进度条
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, false);//关闭安全光栅检测
            //已经退出到上料点，掀盖重新按启动
            ShowLog("请掀盖，按启动");
            WaitIo("启动", true);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, true);//开启安全光栅检测
            if (CamCheckCover())  //重新拍照检测盖子,并标刻第8点
            {
                ShowLog("8点标刻完成");
            }
            else
            {
                ShowLog("第8点盖子拍照失败,未完成标刻");
            }
        }
        /// <summary>
        /// 拍照失败处理
        /// </summary>
        public void CameraFault()
        {
            ShowLog("相机处理失败，请选择操作");
            IoMgr.GetInstance().WriteIoBit("相机光源", false);
            while (true)
            {
                CheckContinue();
                IoMgr.GetInstance().WriteIoBit("相机光源", true);
                WaitTimeDelay(100);
                if (VisionMgr.GetInstance().ProcessStep(m_strStep[m_nType - 1]))
                {
                    IoMgr.GetInstance().WriteIoBit("相机光源", false);
                    break;
                }
                else
                {
                    if (DialogResult.Yes == ShowMessage("拍照失败,再次拍照点击<继续>，推出重新放置点击<退出运行>?"))
                    {

                    }
                    else
                    {
                        //退出到上料点
                        MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, GoPickSpeed);
                        MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, GoPickSpeed);
                        MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, GoPickSpeed);
                        WaitMotion(AxisX, -1);
                        WaitMotion(AxisY, -1);
                        WaitMotion(AxisZ, -1);
                        ShowLog("重新放置，并按启动");
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, false);//关闭安全光栅检测
                        WaitIo("启动", true, -1);
                        SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, true);//开启安全光栅检测
                        if (Camera())
                            break;
                    }
                }
                IoMgr.GetInstance().WriteIoBit("相机光源", false);
            }
        }
        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns></returns>
        public bool Camera()
        {
            //运行到相机底部，判断工件类型
            switch (m_nType)
            {
                case 1://拍照//上盖RST模块位置(和上盖同位置)
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[7].x, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[7].y, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[7].z, GoSnapSpeed);
                    break;
                case 2://拍照//逆变模块位置
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[5].x, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[5].y, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[5].z, GoSnapSpeed);
                    break;
                case 3://拍照//上盖位置
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[7].x, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[7].y, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[7].z, GoSnapSpeed);
                    break;
                case 4://拍照//整机位置
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[14].x, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[14].y, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[14].z, GoSnapSpeed);
                    break;
                default://拍照//注：新增模块拍照点点位序号比产品类型序号大11；
                    MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[(int)(m_nType+11)].x, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[(int)(m_nType + 11)].y, GoSnapSpeed);
                    MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[(int)(m_nType + 11)].z, GoSnapSpeed);
                    break;
            }
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            ShowLog("拍照位置就绪，准备拍照");
            IoMgr.GetInstance().WriteIoBit("相机光源", true);
            WaitTimeDelay(100);
            if (VisionMgr.GetInstance().ProcessStep(m_strStep[m_nType - 1]))//拍照成功
            {
                ShowLog("拍照完成，开始移动到打标位置");
                IoMgr.GetInstance().WriteIoBit("相机光源", false);
            }
            else//拍照不成功
            {
                CameraFault();//再次拍照或退出重新放置
            }
            return true;
        }
        /// <summary>
        /// 拍照检测盖子是否掀到位
        /// </summary>
        public bool CamCheckCover() 
        {
            //再次到达相机底部
            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[14].x, GoSnapSpeed);
            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[14].y-2000, GoSnapSpeed);//往前走一点，拍照更清晰
            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[14].z, GoSnapSpeed);
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            IoMgr.GetInstance().WriteIoBit("相机光源", true);
            WaitTimeDelay(100);
            if (VisionMgr.GetInstance().ProcessStep("T1_2"))
            {
                //拍照成功
                IoMgr.GetInstance().WriteIoBit("相机光源", false);
                m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_X);
                m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_Y);
                //计算轴偏差量
                double m_Ax0 = (int)(m_dicPoint[6].x - m_fx);
                double m_Ay0 = (int)(m_dicPoint[6].y - m_fy);
                double m_Ax1 = (int)(m_dicPoint[6].x - m_dicPoint[4].x);
                double m_Ay1 = (int)(m_dicPoint[6].y - m_dicPoint[4].y);

                MotionMgr.GetInstance().RelativeMove(AxisX, (int)(m_Ax0 - m_Ax1-60000), GoMarkingSpeed);
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)(m_Ay0 - m_Ay1-25000), GoMarkingSpeed);
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[12].z + 20000, GoMarkingSpeed);//换到了整机上往上抬20mm
                WaitMotion(AxisX, -1);
                WaitMotion(AxisY, -1);
                WaitMotion(AxisZ, -1);
                //打标整流8点
                Marking8Point();
                return true;
            }
            else
            {
                //拍照不成功
                while (true)
                {
                    CheckContinue();
                    if (VisionMgr.GetInstance().ProcessStep("T1_2"))
                    {
                        //拍照成功,标刻8点
                        Marking8Point();
                        return true;
                    }
                    else
                    {
                        if (DialogResult.Yes == ShowMessage("人工掀盖不到位,再次拍照点击<继续>，推出重新掀盖点击<退出运行>?"))
                        {

                        }
                        else
                        {
                            //退出到上料点
                            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, GoPickSpeed);
                            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, GoPickSpeed);
                            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, GoPickSpeed);
                            WaitMotion(AxisX, -1);
                            WaitMotion(AxisY, -1);
                            WaitMotion(AxisZ, -1);
                            ShowLog("重新掀盖，并按启动");
                            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, false);//关闭安全光栅检测
                            WaitIo("启动", true, -1);
                            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, true);//开启安全光栅检测
                            if (CamCheckCover())
                            break;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 拍照完成移动到打标地点
        /// </summary>
        public void GoMarkPoint()
        {
            if (m_nType == 1)//上盖模块RST
            {
                //当前焦点轴位置
                m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_X);
                m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_Y);
                //计算轴偏差量
                double m_Ax0 = (int)(m_dicPoint[8].x - m_fx);
                double m_Ay0 = (int)(m_dicPoint[8].y - m_fy);
                double m_Ax1 = (int)(m_dicPoint[8].x - m_dicPoint[10].x);
                double m_Ay1 = (int)(m_dicPoint[10].y - m_dicPoint[8].y);
                MotionMgr.GetInstance().RelativeMove(AxisX, -(int)(m_Ax1 - m_Ax0), GoMarkingSpeed);//到达上盖打标位置
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)(m_Ay0 + m_Ay1), GoMarkingSpeed);//到达上盖打标位置
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[10].z, GoMarkingSpeed);//上盖高度
            }

            if (m_nType == 2)//逆变模块
            {
                //当前焦点轴位置
                m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_X);
                m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T13_Y);
                ////计算轴偏差量
                double m_Ax0 = (int)(m_fx - m_dicPoint[6].x);
                double m_Ay0 = (int)(m_fy - m_dicPoint[6].y);

                double m_Ax1 = (int)(m_dicPoint[6].x - m_dicPoint[4].x);
                double m_Ay1 = (int)(m_dicPoint[6].y - m_dicPoint[4].y);
                MotionMgr.GetInstance().RelativeMove(AxisX, -(int)(m_Ax0 + m_Ax1), GoMarkingSpeed);//到达逆变1位置
                MotionMgr.GetInstance().RelativeMove(AxisY, -(int)(m_Ay0 + m_Ay1 + 15000), GoMarkingSpeed);//到达逆变1位置
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[11].z, GoMarkingSpeed);
            }

            if (m_nType == 3)//上盖模块
            {
                //当前焦点轴位置
                m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_X);
                m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T14_Y);
                //计算轴偏差量
                double m_Ax0 = (int)(m_dicPoint[8].x - m_fx);
                double m_Ay0 = (int)(m_dicPoint[8].y - m_fy);
                double m_Ax1 = (int)(m_dicPoint[8].x - m_dicPoint[10].x);
                double m_Ay1 = (int)(m_dicPoint[10].y - m_dicPoint[8].y);
                MotionMgr.GetInstance().RelativeMove(AxisX, -(int)(m_Ax1 - m_Ax0), GoMarkingSpeed);//到达上盖打标位置
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)(m_Ay0 + m_Ay1), GoMarkingSpeed);//到达上盖打标位置
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[10].z, GoMarkingSpeed);//上盖高度
            }
            if (m_nType == 4)//整机模块
            {
                //当前焦点轴位置
                m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_X);
                m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T11_Y);
                //计算轴偏差量
                double m_Ax0 = (int)(m_dicPoint[6].x - m_fx);
                double m_Ay0 = (int)(m_dicPoint[6].y - m_fy);
                double m_Ax1 = (int)(m_dicPoint[6].x - m_dicPoint[4].x);
                double m_Ay1 = (int)(m_dicPoint[6].y - m_dicPoint[4].y);

                MotionMgr.GetInstance().RelativeMove(AxisX, (int)(m_Ax0 - m_Ax1) - 10000, GoMarkingSpeed);
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)(m_Ay0 - m_Ay1) + 10000, GoMarkingSpeed);
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[15].z, GoMarkingSpeed);
            }
            if (m_nType >= 5)//其他新增模块
            {
                //当前轴位置
                switch (m_nType)
                {
                    case 5:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_10_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_10_Y);
                        break;
                    case 6:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_11_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_11_Y);
                        break;
                    case 7:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_12_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_12_Y);
                        break;
                    case 8:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_13_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_13_Y);
                        break;
                    case 9:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_14_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_14_Y);
                        break;
                    case 10:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_15_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_15_Y);
                        break;
                    case 11:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_16_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_16_Y);
                        break;
                    case 12:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_17_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_17_Y);
                        break;
                    case 13:
                        m_fx = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_18_X);
                        m_fy = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_18_Y);
                        break;
                }
                //计算轴偏差量
                double m_Ax0 = (int)(m_dicPoint[6].x - m_fx);
                double m_Ay0 = (int)(m_dicPoint[6].y - m_fy);
                double m_Ax1 = (int)(m_dicPoint[6].x - m_dicPoint[4].x);
                double m_Ay1 = (int)(m_dicPoint[6].y - m_dicPoint[4].y);
                //新增模块打标点位序号比产品类型序号大20；
                MotionMgr.GetInstance().RelativeMove(AxisX, (int)(m_Ax0 - m_Ax1), GoMarkingSpeed);
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)(m_Ay0 - m_Ay1), GoMarkingSpeed);
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[m_nType + 20].z, GoMarkingSpeed);
                WaitMotion(AxisX, -1);
                WaitMotion(AxisY, -1);
                WaitMotion(AxisZ, -1);
                //从焦点开始第一次打标偏移
                MotionMgr.GetInstance().RelativeMove(AxisX, (int)m_dicPoint[m_nType + 20].x, GoMarkingSpeed);
                MotionMgr.GetInstance().RelativeMove(AxisY, (int)m_dicPoint[m_nType + 20].y, GoMarkingSpeed);
                MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[m_nType + 20].z, GoMarkingSpeed);
                WaitMotion(AxisX, -1);
                WaitMotion(AxisY, -1);
                WaitMotion(AxisZ, -1);
            }

            WaitTimeDelay(50);
            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.进度百分比, 50, true);//更新进度条
        }
        /// <summary>
        /// 写入文本文件
        /// </summary>
        public void WriteFile() 
        {
            string strFilePath =SystemMgr.GetInstance().GetParamString("LaserSavePath"); //"D:/Print/AutoPrint/";//文本保存路径
            switch (m_nType)
            {
                case 1://不需要TXT文件，固定文本标刻
                    break;
                case 2://写入1个TXT文件
                    StreamWriter sw4 = new StreamWriter(strFilePath + "\\4.txt", false);
                    sw4.Write(strSn.Code_4);
                    sw4.Flush();
                    sw4.Close();
                    StreamWriter sw5 = new StreamWriter(strFilePath + "\\5.txt", false);
                    sw5.Write(strSn.Code_5);
                    sw5.Flush();
                    sw5.Close();
                    break;
                case 3://不需要TXT文件，固定文本标刻
                    break;
                case 4://写入4个TXT文件
                    StreamWriter sw1 = new StreamWriter(strFilePath + "\\1.txt", false);
                    sw1.Write(strSn.Code_1);
                    sw1.Flush();
                    sw1.Close();
                    StreamWriter sw2 = new StreamWriter(strFilePath + "\\2.txt", false);
                    sw2.Write(strSn.Code_2);
                    sw2.Flush();
                    sw2.Close();
                    StreamWriter sw3 = new StreamWriter(strFilePath + "\\3.txt", false);
                    sw3.Write(strSn.Code_3);
                    sw3.Flush();
                    sw3.Close();
                    StreamWriter sw8 = new StreamWriter(strFilePath + "\\8.txt", false);
                    sw8.Write(strSn.Code_8);
                    sw8.Flush();
                    sw8.Close();
                    break;
                default://写入3个TXT文件,注：如果扫码被勾选则写入数据库字符串，如果扫码没勾选则写入NONE字符串。
                    if (isscan == 1)
                    {   
                        StreamWriter swOther0 = new StreamWriter(strFilePath + "\\Data0.txt", false);
                        swOther0.Write(strSn.Code_others0);
                        swOther0.Flush();
                        swOther0.Close();
                        StreamWriter swOther1 = new StreamWriter(strFilePath + "\\Data1.txt", false);
                        swOther1.Write(strSn.Code_others1);
                        swOther1.Flush();
                        swOther1.Close();
                        StreamWriter swOther2 = new StreamWriter(strFilePath + "\\Data2.txt", false);
                        swOther2.Write(strSn.Code_others2);
                        swOther2.Flush();
                        swOther2.Close();
                    }
                    else
                    {
                        StreamWriter swOther0 = new StreamWriter(strFilePath + "\\Data0.txt", false);
                        swOther0.Write("NONE");
                        swOther0.Flush();
                        swOther0.Close();
                        StreamWriter swOther1 = new StreamWriter(strFilePath + "\\Data1.txt", false);
                        swOther1.Write("NONE");
                        swOther1.Flush();
                        swOther1.Close();
                        StreamWriter swOther2 = new StreamWriter(strFilePath + "\\Data2.txt", false);
                        swOther2.Write("NONE");
                        swOther2.Flush();
                        swOther2.Close();
                    }
                    break;
            }
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.扫码数据清除, false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.扫码数据清除, true);
        }
        /// <summary>
        /// 镭射
        /// </summary>
        public void Marking()
        {
            //生成从数据库获取的TXT文本给激光调用。
            WriteFile();
            switch (m_nType)
            {  
                case 1://标刻上盖RST模块*********   注：0000为初始化，0010和0011分别用于标刻8点和23点********
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00110
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 2://逆变模块
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00101
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 3://上盖模块
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00100
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 4://整机模块
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00001
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
//*********************************************************************新增模块，以下模块为第一次打标程序****************
                case 5://新增模块1/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00111 
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 6://新增模块2/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01000
                    IoMgr.GetInstance().WriteIoBit(1,16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 7://新增模块3/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01001
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 8://新增模块4/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01010
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 9://新增模块5/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01011
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 10://新增模块6/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01100
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 11://新增模块7/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01101
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 12://新增模块8/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01110
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 13://新增模块9/1
                    IoMgr.GetInstance().WriteIoBit("打标程序2", false);//01111
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
            }
            WaitTimeDelay(500);//延时长一点，以防继电器机械延迟
            IoMgr.GetInstance().WriteIoBit("打标使能", true);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", false);
            ShowLog("标刻中...，等待标刻完成");
            WaitIo("标刻完成", true);
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00000
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", false);
            IoMgr.GetInstance().WriteIoBit("打标程序4", false);
            ShowLog("第一次标刻完成");
            //如果为新增模块并且需要二次打标，则需要移动到第二个点位进行标刻
            if (m_nType>=5 && SystemMgr.GetInstance().GetRegInt((int)SysIntReg.是否打两次标)==1)
            {
                MarkingSecond();
                ShowLog("第二次标刻完成");
            }
             //如果为整机模块，则继续标刻整流模块。
            if (m_nType == 4)
            {
                MarkingCurrent();
                ShowLog("第二次标刻完成");
            }
            ShowLog("全部标刻完成");
            IoMgr.GetInstance().WriteIoBit("打标程序2", false);//00000
            IoMgr.GetInstance().WriteIoBit(1, 16, false);
            IoMgr.GetInstance().WriteIoBit("打标程序1", false);
            IoMgr.GetInstance().WriteIoBit("打标程序3", false);
            IoMgr.GetInstance().WriteIoBit("打标程序4", false);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.标刻完成, true);
            //退出到上料点
            MotionMgr.GetInstance().AbsMove(AxisX, m_dicPoint[2].x, GoPickSpeed);
            MotionMgr.GetInstance().AbsMove(AxisY, m_dicPoint[2].y, GoPickSpeed);
            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[2].z, GoPickSpeed);
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.运行状态, false);//关闭安全光栅检测
            SystemMgr.GetInstance().WriteRegInt((int)SysIntReg.进度百分比, 100, true);//更新进度条
        }
        /// <summary>
        /// 移动到第二个标刻点打标(功能仅限于新增模块)
        /// </summary>
        private void MarkingSecond()
        {
            //从焦点开始第二次打标偏移
            MotionMgr.GetInstance().RelativeMove(AxisX, (int)m_dicPoint[m_nType + 29].x, GoMarkingSpeed);
            MotionMgr.GetInstance().RelativeMove(AxisY, (int)m_dicPoint[m_nType + 29].y, GoMarkingSpeed);
            MotionMgr.GetInstance().AbsMove(AxisZ, m_dicPoint[m_nType + 29].z, GoMarkingSpeed);
            WaitMotion(AxisX, -1);
            WaitMotion(AxisY, -1);
            WaitMotion(AxisZ, -1);
            switch (m_nType)
            {
                case 5://新增模块1/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//10111 
                    IoMgr.GetInstance().WriteIoBit(1, 16, false);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 6://新增模块2/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11000
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 7://新增模块3/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11001
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 8://新增模块4/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11010
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 9://新增模块5/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11011
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 10://新增模块6/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11100
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 11://新增模块7/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11101
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", false);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
                case 12://新增模块8/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11110
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", false);
                    break;
                case 13://新增模块9/2
                    IoMgr.GetInstance().WriteIoBit("打标程序2", true);//11111
                    IoMgr.GetInstance().WriteIoBit(1, 16, true);
                    IoMgr.GetInstance().WriteIoBit("打标程序1", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序3", true);
                    IoMgr.GetInstance().WriteIoBit("打标程序4", true);
                    break;
            }
            WaitTimeDelay(500);//延时长一点，以防继电器机械延迟
            IoMgr.GetInstance().WriteIoBit("打标使能", true);
            WaitTimeDelay(50);
            IoMgr.GetInstance().WriteIoBit("打标使能", false);
            ShowLog("标刻中...，等待标刻完成");
            WaitIo("标刻完成", true);
        }
        /// <summary>
        /// 标定处理步1
        /// </summary>
        public void CaliProcess()
        {
            IoMgr.GetInstance().WriteIoBit("相机光源", true);
            double[] fpix_x = new double[11];//像素x坐标
            double[] fpix_y = new double[11];//像素y坐标
            //double[] fpix_a = new double[11];//像素u坐标
            double[] faxis_x = new double[11];//轴x坐标
            double[] faxis_y = new double[11];//轴y坐标
            //double[] faxis_a = new double[11];//轴u坐标
            int i = 0;
            long nCalibDis = 1000;//一次走的标定的距离
            CaliTranslate caliTrans = new CaliTranslate();
            caliTrans.ClearAllData();
            while (true)
            {
                int kl = i % 3 - 1;
                int kc = i / 3 - 1;
                MotionMgr.GetInstance().AbsMove(AxisX, (int)(m_dicPoint[3].x + kl * nCalibDis), 100000);
                MotionMgr.GetInstance().AbsMove(AxisY, (int)(m_dicPoint[3].y + kc * nCalibDis), 100000);
                WaitMotion(AxisX, -1);
                WaitMotion(AxisY, -1);
                WaitTimeDelay(500);
                //faxis_x[i] = 0;
                //faxis_y[i] = 0;
                faxis_x[i] = MotionMgr.GetInstance().GetAixsPos(AxisX);
                faxis_y[i] = MotionMgr.GetInstance().GetAixsPos(AxisY);
                if (VisionMgr.GetInstance().ProcessStep("T1_calib"))  //拍照
                {
                    //fpix_x[i] = 0;
                    //fpix_y[i] = 0;
                    fpix_x[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib_X);
                    fpix_y[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib_Y);
                    //fpix_a[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib_A);
                    i++;//拍到一次增加一次
                    if (i == 9)
                    {
                        for (int k = 0; k < 9; ++k)
                            caliTrans.AppendPointData(fpix_x[k], fpix_y[k], faxis_x[k], faxis_y[k]);
                        fpix_x[9] = 0;
                        fpix_y[10] = 0;
                        faxis_x[9] = 0;
                        faxis_y[10] = 0;
                        caliTrans.SaveCaliData(VisionMgr.GetInstance().ConfigDir + "\\Calib.ini");
                        MessageBox.Show("标定完成!");
                        IoMgr.GetInstance().WriteIoBit("相机光源", false);
                        break;
                    }
                }
                else
                {
                    if (DialogResult.Yes == ShowMessage("标定图像处理失败，是否重试"))
                    {
                    }
                    else
                    {
                        break;
                    }
                }
                CheckContinue();
            }
        }
        /// <summary>
        /// 标定处理步2
        /// </summary>
        public void CaliProcess2()
        {
            double[] fpix_x = new double[11];//像素x坐标
            double[] fpix_y = new double[11];//像素y坐标
            //double[] fpix_a = new double[11];//像素u坐标
            double[] faxis_x = new double[11];//轴x坐标
            double[] faxis_y = new double[11];//轴y坐标
            //double[] faxis_a = new double[11];//轴u坐标
            int i = 0;
            long nCalibDis = 1000;//一次走的标定的距离
            CaliTranslate caliTrans = new CaliTranslate();
            caliTrans.ClearAllData();
            while (true)
            {
                int kl = i % 3 - 1;
                int kc = i / 3 - 1;
                MotionMgr.GetInstance().AbsMove(AxisX, (int)(m_dicPoint[7].x + kl * nCalibDis), 50000);
                MotionMgr.GetInstance().AbsMove(AxisY, (int)(m_dicPoint[7].y + kc * nCalibDis), 50000);
                MotionMgr.GetInstance().AbsMove(AxisZ,m_dicPoint[7].z, 50000);
                WaitMotion(AxisX, -1);
                WaitMotion(AxisY, -1);
                WaitMotion(AxisZ, -1);
                WaitTimeDelay(500);
                //faxis_x[i] = 0;
                //faxis_y[i] = 0;
                faxis_x[i] = MotionMgr.GetInstance().GetAixsPos(AxisX);
                faxis_y[i] = MotionMgr.GetInstance().GetAixsPos(AxisY);
                if (VisionMgr.GetInstance().ProcessStep("T1_calib2"))  //拍照
                {
                    //fpix_x[i] = 0;
                    //fpix_y[i] = 0;
                    fpix_x[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib2_X);
                    fpix_y[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib2_Y);
                    //fpix_a[i] = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1Calib_A);
                    i++;//拍到一次增加一次
                    if (i == 9)
                    {
                        for (int k = 0; k < 9; ++k)
                            caliTrans.AppendPointData(fpix_x[k], fpix_y[k], faxis_x[k], faxis_y[k]);
                        fpix_x[9] = 0;
                        fpix_y[10] = 0;
                        faxis_x[9] = 0;
                        faxis_y[10] = 0;
                        caliTrans.SaveCaliData(VisionMgr.GetInstance().ConfigDir + "\\Calib2.ini");
                        MessageBox.Show("标定完成!");
                        break;
                    }
                }
                else
                {
                    if (DialogResult.Yes == ShowMessage("标定图像处理失败，是否重试"))
                    {
                    }
                    else
                    {
                        break;
                    }
                }
                CheckContinue();
            }
        }

        /// <summary>
        /// 程序运行流程
        /// </summary>
        public override void StationProcess()
        {
            if (SystemMgr.GetInstance().Mode == SystemMode.Calib_Run_Mode) //选定标定模式的时候，进行九点标定步骤，
            {
                CaliProcess();
                // CaliProcess2();
            }
            else
            {
                m_fx = 0;
                m_fy = 0;
                m_nType = SystemMgr.GetInstance().GetRegInt((int)SysIntReg.工件类型);
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.扫码上料完成, false);
                SystemMgr.GetInstance().WriteRegBit((int)SysBitReg.标刻完成, false);
                //是否勾选待加工工件的类型
                if (m_nType>0)//如果已经选定加工工件的类型
                {
                    ShowLog("等待初始化");
                    if (ScanCode())//扫码通讯部分
                    {
                        GoSnapSpeed = SystemMgr.GetInstance().GetParamInt("GoSnapSpeed");
                        GoMarkingSpeed = SystemMgr.GetInstance().GetParamInt("GoMarkingSpeed");
                        GoPickSpeed = SystemMgr.GetInstance().GetParamInt("GoPickSpeed");
                        if (Camera())//拍照
                        {
                            GoMarkPoint();//运行到打标位  
                            Marking();//激光打标
                        }
                        else
                        {
                            ShowLog("相机处理失败");
                        }
                    }
                    else
                    {
                        ShowLog("未扫条码!");
                        WaitTimeDelay(500);
                    }
                }
                //没有勾选待加工工件
                else
                {
                    ShowMessage("未勾选工件类型!");
                    WaitTimeDelay(4000);
                }
            }
        }
    }
}
