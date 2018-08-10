
//#define NO_EXPORT_APP_MAIN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using CommonTool;
using AutoFrameDll;

namespace AutoFrameVision
{
    /// <summary>
    /// 视觉处理步骤样板类
    /// </summary>
    public class Vision_T1_14:VisionBase
    {
        HDevelopExport hde = new HDevelopExport();
        private CaliTranslate m_caliTrans = new CaliTranslate();

        HTuple ModelId;
        HTuple ModelData;

        public HTuple m_RowCenter = 0;
        public HTuple m_ColCenter = 0;
        public HTuple m_FixTool = 0;

        /// <summary>
        /// 构造函数,初始化配置
        /// </summary>
        /// <param name="strName"></param>
        public Vision_T1_14(string strName):base(strName)
        {

        }

        /// <summary>
        /// 初始化配置，初始化模板，模板数据寄存
        /// </summary>
        /// <returns></returns>
        public override bool InitConfig()
        {
            LoadParam();
            hde.InitTemplete(m_strDir, out ModelId, out ModelData);
            m_caliTrans.LoadCaliData(VisionMgr.GetInstance().ConfigDir + "\\calib.ini");
            return true;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <returns></returns>
        public override bool LoadParam()
        {
            string strFile = VisionMgr.GetInstance().ConfigDir + "param.ini";

            string strTemp = IniOperation.GetStringValue(strFile, "T1_14", "ExposureTime", null);
            m_ExposureTime = Convert.ToInt32(strTemp);
            if (0 == m_ExposureTime)
            {
                m_ExposureTime = 1000;
                SaveParam();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SaveParam()
        {
            string strFile = VisionMgr.GetInstance().ConfigDir + "param.ini";

            IniOperation.WriteValue(strFile, "T1_14", "ExposureTime", m_ExposureTime.ToString());

            return true;
        }

        /// <summary>
        /// 在显示控件变化时,用当前内容更新显示
        /// </summary>
        /// <param name="ctl"></param>
        public override void UpdateVisionControl(VisionControl ctl)
        {
            ctl.LockDisplay();
            try
            {
                if (imgSrc != null && imgSrc.IsInitialized() && imgSrc.Key != IntPtr.Zero)
                {

                    HTuple num = 0;
                    HOperatorSet.CountObj(imgSrc, out num);
                    if (num > 0)//&& m_image.IsInitialized() && m_image.Key != IntPtr.Zero)
                    {
                        HOperatorSet.DispImage(imgSrc, ctl.GetHalconWindow());
                    }
                }
            }
            catch (HalconException HDevExpDefaultException1)
            {
                System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.ToString());
            }
            finally
            {
                ctl.UnlockDisplay();
            }
        }

        /// <summary>
        /// 设定曝光值
        /// </summary>
        /// <param name="nExp"></param>
        public override void SetExposureTime(int nExp)
        {
            m_ExposureTime = nExp;
        }

        public override bool Process()
        {
            if (m_Camera != null)
            {
                if (m_visionControl != null)
                    m_visionControl.RegisterUpdateInterface(this);

                //第一次拍照要求暗,
                m_Camera.SetGrabParam("ExposureTime", m_ExposureTime);
                if (Snap())
                {
                     return ProcessImage(m_visionControl);
                }
                else
                {
                    VisionMgr.GetInstance().ShowLog(Name + " process snap1 fail ! ");
                    return false;
                }
            }
            return false;
        }

        private bool CalcDetlaData()
        {
            double x1 = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_14_X);
            double y1 = SystemMgr.GetInstance().GetRegDouble((int)SysFloatReg.T1_14_Y);

            double x1Axis, y1Axis;
            m_caliTrans.Translate(x1, y1, out x1Axis, out y1Axis);

            //TODO:注意行列与机器人X, Y的关系
            SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_X, x1Axis, true);
            SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_Y, y1Axis, true);

            return true;
        }

        /// <summary>
        /// 处理当前图像,显示在指定的控件上
        /// </summary>
        /// <param name="vc"></param>
        /// <returns></returns>
        public override bool ProcessImage( VisionControl vc)
        {
            if (vc != null)
            {
                HDevWindowStack.Push(vc.GetHalconWindow());
                vc.LockDisplay();        
                vc.DispImageFull(imgSrc);
            }
            try
            {
                HTuple data=0;
                hde.T1_14(imgSrc, m_strDir, ModelId, ModelData, out data);
                if (data[0] == 1)
                {
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_X, data[1], true);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_Y, data[2], true);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_A, data[3], false);

                    if (CalcDetlaData())
                        return true;
                    else
                        return false;
                }
                else
                {
                    //处理失败时，必须将无效数值写入数据区，防止使用上一次的数据
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_X, data[0], true);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_Y, data[0], true);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_A, data[0], false);
                    return false;
                }
            }
            catch (Exception e)
            {
                WarningMgr.GetInstance().Info(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.ToString());
                //处理失败时，必须将无效数值写入数据区，防止使用上一次的数据
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_X, VisionException, true);
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_Y, VisionException, true);
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1_14_A, VisionException, false);
                return false;
            }
            finally
            {
                if (vc != null)
                {
                    vc.UnlockDisplay();
                    HDevWindowStack.Pop();
                }
            }
        }
    }
}
