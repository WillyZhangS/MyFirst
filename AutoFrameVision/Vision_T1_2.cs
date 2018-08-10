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
    public class Vision_T1_2:VisionBase
    {
        HDevelopExport hde = new HDevelopExport();
        HTuple ModelId;
        HTuple ModelData;
        /// <summary>
        /// 构造函数,初始化配置
        /// </summary>
        /// <param name="strName"></param>
        public Vision_T1_2(string strName):base(strName)
        {
        }
        /// <summary>
        /// 初始化得到模板图片中心点和角度
        /// </summary>
        /// <returns></returns>
        public bool InitModel()
        {
            HObject imageModel = new HObject();
            //图片路径需要重新规划
            HOperatorSet.ReadImage(out imageModel, m_strDir + "/Model.bmp");
            HTuple data = 0;
            try
            {
                hde.InitTemplete(m_strDir, out ModelId, out ModelData);
                hde.T1_2(imageModel, m_strDir, ModelId, ModelData, out data);
                if (data[0] == 1)
                {
                    return true;
                }
                else
                {
                    imageModel.Dispose();
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                imageModel.Dispose();
                return false;
            }
            imageModel.Dispose();
            return true;
        }

        /// <summary>
        /// 初始化配置，初始化模板，模板数据寄存
        /// </summary>
        /// <returns></returns>
        public override bool InitConfig()
        {
            LoadParam();
            hde.InitTemplete(m_strDir, out ModelId, out ModelData);
            return true;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <returns></returns>
        public override bool LoadParam()
        {
            string strFile = VisionMgr.GetInstance().ConfigDir + "param.ini";

            string strTemp = IniOperation.GetStringValue(strFile, "T1_2", "ExposureTime", null);
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
            
            IniOperation.WriteValue(strFile, "T1_2", "ExposureTime", m_ExposureTime.ToString());

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

                //第二次拍照要求亮,
                m_Camera.SetGrabParam("ExposureTime", m_ExposureTime);
                if (Snap() )
                {
                     return ProcessImage(m_visionControl);
                }
                else
                {
                    VisionMgr.GetInstance().ShowLog(Name + " process snap0 fail ! ");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
    /// 处理当前图像,显示在指定的控件上
    /// </summary>
    /// <param name="vc"></param>
    /// <returns></returns>
        public override bool ProcessImage( VisionControl vc)
        {
            if(vc != null)
            {
                HDevWindowStack.Push(vc.GetHalconWindow());
                vc.LockDisplay();
                vc.DispImageFull(imgSrc);
            }
            HTuple data= 0;
            try
            {
                hde.T1_2(imgSrc, m_strDir, ModelId, ModelData, out data);
                if (data[0] == 1)
                {
                    return true;
                }
                else
                {               
                    return false;
                }
            }
            catch (Exception e)
            {
                WarningMgr.GetInstance().Info(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.ToString());
                SystemMgr.GetInstance().WriteRegDouble((int)SysFloatReg.T12_A, VisionException);
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
