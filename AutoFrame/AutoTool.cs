using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonTool;
using AutoFrameDll;
using AutoFrameVision;
using Communicate;


namespace AutoFrame
{
    class AutoTool
    {
        public AutoTool()
        {
           
        }

        public static  void OnProductChanged()
        {
            if (ProductInfo.GetInstance().GetProductTypeCount() == 1)
            {
                StationMgr.GetInstance().LoadPointFile("Point.xml");
            }
            else
            {
                StationMgr.GetInstance().LoadPointFile(string.Format("Point_{0}.xml", ProductInfo.GetInstance().ProductName));
            }
        }
        public static bool ConfigAll()
        {
            try
            {
                ProductInfo.GetInstance().StateChangedEvent += OnProductChanged;
                //先初始化报警类,避免后续其它类初始化时异常,造成循环调用.
                WarningMgr.GetInstance();
                //初始化设置为OP权限
           //     Security.ChangeOpMode();
                //最先在systemCfg.xml文件中得到系统参数配置文件名
                if (!SystemMgr.GetInstance().GetSystemParamName("systemCfg.xml"))
                {
                    MessageBox.Show("系统参数配置文件名获取失败\n将启用默认文件systemParam", "系统初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SystemMgr.GetInstance().AppendSystemParamName("systemCfg.xml");
                }
                //开始读取配置
                if (ConfigMgr.GetInstance().LoadCfgFile("systemCfg.xml"))
                {
                    //读取系统参数配置, 站位点位文件
                    SystemMgr.GetInstance().LoadParamFile(/*"systemParam.xml"*/SystemMgr.GetInstance().m_strSystemParamName);   
                    //站位点位文件的加载需要放到产品信息加载之后,通过AutoTool的响应事件加载               
               //     StationMgr.GetInstance().LoadPointFile("Point.xml");
               
                    //读取日志系统配置
                    WarningMgr.GetInstance().ReadXmlConfig("Log4Net.config");
                    //读取产品配置信息
                    ProductInfo.GetInstance().LoadProductInfo();
                    //加入自定义站位类和视觉流程类
                    AddStation();
                    AddVisionStep();

                    return true;                   
                }
                else
                {
                    MessageBox.Show("系统配置文件systemCfg.xml加载失败", "系统初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }   
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString() ,"系统初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        ///向站位管理器添加自定议的站位,同时为站位类关联手动调试窗口界面类 
        /// </summary>
        public static void AddStation()
        {   
			StationMgr.GetInstance().AddStation(new Form_sta_templete(), new StationMarkMachine("打标站"));
            StationMgr.GetInstance().AddStation(new Form_sta_templete(), new SafetyStation("安全站"));
        }

        /// <summary>
        /// 向视觉管理器添加自定义的步骤,同时为其绑定指定的相机采集类 
        /// </summary>
        public static void AddVisionStep()
        {
            //加入一个相机采集类, 加入步骤前必须先添加相机采集
            VisionMgr.GetInstance().AddCamera(new CameraGige("Cam_0"));
            //加入视觉步骤类, 并将其绑定到指定的相机采集类上
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_1("T1_1"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_2("T1_2"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_3("T1_3"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_4("T1_4"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_10("T1_10"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_11("T1_11"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_12("T1_12"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_13("T1_13"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_14("T1_14"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_15("T1_15"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_16("T1_16"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_17("T1_17"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_18("T1_18"));
 
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_calib("T1_calib"));
            VisionMgr.GetInstance().AddVisionStep("Cam_0", new Vision_T1_calib2("T1_calib2"));
        }

        /// <summary>
        /// 初始化硬件
        /// </summary>
        public static void InitSystem()
        {
            Action<object> action = (object obj) =>
            {
                //延迟一秒初始化硬件，便于窗口接收初始化异常
                Task.Delay(1000);
                MotionMgr.GetInstance().InitAllCard();
                IoMgr.GetInstance().InitAllCard();


                //系统管理器线程，负责清除过期文件数据，检查系统是否空闲无操作
                SystemMgr.GetInstance().StartMonitor();

                //如果需要程序一启动就给轴上电，可以在此处添加代码
                //todo: tcp, com, vision
            };
            Task t1 = new Task(action, "");            
            t1.Start();            
        }

        public static void DeinitSystem()
        {
            MotionMgr.GetInstance().DeinitAllCard();
            IoMgr.GetInstance().DeinitAllCard();

            SystemMgr.GetInstance().StopMonitor();
            //todo: tcp, com, vision
        }

        public static void SaveLifeTime(TimeSpan tsMachine, TimeSpan tsSoftware)
        {
            string strFile = AppDomain.CurrentDomain.BaseDirectory + "Autoframe.ini";//获取当前路径

            string value = IniOperation.GetStringValue(strFile, "Time", "MachineTime", null);
            int nSecond = 0;
            if (value != null)
            {
                nSecond = (int)tsMachine.TotalSeconds + Convert.ToInt32(value);
            }
            else
                nSecond = (int)tsMachine.TotalSeconds;
            IniOperation.WriteValue(strFile, "Time", "MachineTime", nSecond.ToString());

            value = IniOperation.GetStringValue(strFile, "Time", "SoftwareTime", null);
            nSecond = 0;
            if (value != null)
            {
                nSecond = (int)tsSoftware.TotalSeconds + Convert.ToInt32(value);
            }
            else
                nSecond = (int)tsSoftware.TotalSeconds;
            IniOperation.WriteValue(strFile, "Time", "SoftwareTime", nSecond.ToString());
        }

        public static void ReadLifeTime(out int nMachineSecond, out int nSoftwareSecond)
        {
            nMachineSecond = 0;
            nSoftwareSecond = 0;

            string strFile = AppDomain.CurrentDomain.BaseDirectory + "Autoframe.ini";//获取当前路径
            string value = IniOperation.GetStringValue(strFile, "Time", "MachineTime", null);
            if (value != null)
            {
                nMachineSecond = Convert.ToInt32(value);
            }
            else
                nMachineSecond = 0;

            value = IniOperation.GetStringValue(strFile, "Time", "SoftwareTime", null);
            if (value != null)
            {
                nSoftwareSecond =  Convert.ToInt32(value);
            }
            else
                nSoftwareSecond =0;
        }


            /// <summary>
            /// INI文件操作样板函数
            /// </summary>
            private void TestIniOperation()
        {

            string file = "F:\\TestIni.ini";


            //写入/更新键值
            IniOperation.WriteValue(file, "Desktop", "Color", "Red");
            IniOperation.WriteValue(file, "Desktop", "Width", "3270");

            IniOperation.WriteValue(file, "Toolbar", "Items", "Save,Delete,Open");
            IniOperation.WriteValue(file, "Toolbar", "Dock", "True");

            //写入一批键值
            IniOperation.WriteItems(file, "Menu", "File=文件\0View=视图\0Edit=编辑");

            //获取文件中所有的节点
            string[] sections = IniOperation.GetAllSectionNames(file);

            //获取指定节点中的所有项
            string[] items = IniOperation.GetAllItems(file, "Menu");

            //获取指定节点中所有的键
            string[] keys = IniOperation.GetAllItemKeys(file, "Menu");

            //获取指定KEY的值
            string value = IniOperation.GetStringValue(file, "Desktop", "color", null);

            //删除指定的KEY
            IniOperation.DeleteKey(file, "desktop", "color");

            //删除指定的节点
            IniOperation.DeleteSection(file, "desktop");

            //清空指定的节点
            IniOperation.EmptySection(file, "toolbar");

        }

    }
}
