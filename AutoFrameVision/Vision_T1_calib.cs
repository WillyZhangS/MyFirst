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
    /// 视觉处理步骤样板类2
    /// </summary>
    public class Vision_T1_calib:VisionBase
    {
        HDevelopExport hde = new HDevelopExport();
        private CaliTranslate m_caliTrans = new CaliTranslate();

        HTuple ModelId;
        HTuple ModelData;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strName"></param>
        public Vision_T1_calib(string strName):base(strName)
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

            string strTemp = IniOperation.GetStringValue(strFile, "T1_calib", "ExposureTime", null);
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

            IniOperation.WriteValue(strFile, "T1_calib", "ExposureTime", m_ExposureTime.ToString());

            return true;
        }

        /// <summary>
        /// 更新显示控件
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

                    //HObject model;

                    //HOperatorSet.GetShapeModelContours(out model, ModelId, 1);
                    //HOperatorSet.DispObj(model, ctl.GetHalconWindow());

                    //hde.disp_message(ctl.GetHalconWindow(), "test", "window", 100, 100, "red", "true");
                    //    HOperatorSet.DispObj(ModelContour, ctl.GetHalconWindow());
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

        /// <summary>
        /// 处理当前图像并显示在指定控件上
        /// </summary>
        /// <param name="vc"></param>
        /// <returns></returns>
        public override bool ProcessImage(VisionControl vc)
        {
            if (vc != null)
            {
                HDevWindowStack.Push(vc.GetHalconWindow());
                vc.LockDisplay();
                vc.DispImageFull(imgSrc);
            }
            HTuple data=0;
            try
            {
                 hde.T1_calib(imgSrc, m_strDir, ModelId, ModelData, out data);
                if (data[0] == 1)
                {
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_X, data[1], false);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_Y, data[2], false);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_A, data[3], true);
              //      CalcDetlaData(data[1], data[2], data[3]);
                    return true;
                }
                else
                {
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_X, data[0], false);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_Y, data[0], false);
                    SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_A, data[0], true);
                    return false;
                }
            }
            catch (Exception e)
            {
                WarningMgr.GetInstance().Info(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.ToString());
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_X, VisionException, false);
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_Y, VisionException, false);
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T1Calib_A, VisionException, true);
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
