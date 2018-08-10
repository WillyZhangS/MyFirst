using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace AutoFrameVision
{
    /// <summary>
    /// GIGE相机,通过halcon接口采集
    /// </summary>
    public class CameraGige:CameraBase
    {
        /// <summary>
        /// 相机采集句柄
        /// </summary>
        HTuple m_hAcqHandle = null;
        /// <summary>
        /// 当前是否处在异步模式
        /// </summary>
        bool m_bIsGrab = false;

        /// <summary>
        /// 以相机名称进行构造
        /// </summary>
        /// <param name="strName"></param>
        public CameraGige(string strName):base(strName)
        {
            m_hAcqHandle = null;
            m_bIsGrab = false;
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            if (m_hAcqHandle == null)
            {
                try
                {
                    //HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "progressive",
                    //    -1, "default", -1, "false", "default", this.Name, 0, -1, out m_hAcqHandle);
                    HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "progressive",
                    -1, "default", -1, "false", "default", "00214900696a_DahengImavision_MER50014GM", 0, -1, out m_hAcqHandle);

                    //open_framegrabber('GigEVision', 0, 0, 0, 0, 0, 0, 'progressive', -1, 'default', -1, 'false', 'default', '00214900696a_DahengImavision_MER50014GM', 0, -1, AcqHandle)
                    //open_framegrabber('GigEVision', 0, 0, 0, 0, 0, 0, 'progressive', -1, 'default',
                    //-1, 'false', 'default', '00214900696a_DahengImavision_MER50014GM', 0, -1, AcqHandle)
                    return m_hAcqHandle != null;
                }
                catch (HalconException HDevExpDefaultException1)
                {
                    System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断是否已经打开相机
        /// </summary>
        /// <returns></returns>
        public override bool isOpen()
        {
            return m_hAcqHandle != null;
        }
        /// <summary>
        /// 关闭相机,同时释放相机缓存图像
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            if (m_hAcqHandle != null)
            {
                try
                {
                    HOperatorSet.CloseFramegrabber(m_hAcqHandle);
                }
                catch (HalconException HDevExpDefaultException1)
                {
                    System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
                    return false;
                }
                m_hAcqHandle = null;
            }
            //if(m_image != null)
            //{
            //    m_image.Dispose();
            //    m_image = null;
            //}
            return true;
        }      

        /// <summary>
        /// 同步采集一张图像
        /// </summary>
        /// <returns>0:失败, 1:成功</returns>
        public override int Snap()
        {
            if (m_bIsGrab)
            {
                m_bIsGrab = false;
            }
            if (m_hAcqHandle == null)
                Open();
            if (m_hAcqHandle != null)
            {
                try
                {
                    m_image.Dispose();
                    HOperatorSet.GrabImage(out m_image, m_hAcqHandle);
                }
                catch (HalconException HDevExpDefaultException1)
                {
                    System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
                    return 0;
                }
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 异步采集一张相机图像
        /// </summary>
        /// <returns></returns>
        public override int Grab()
        {
            if (m_hAcqHandle == null)
                Open();
            if (m_hAcqHandle != null)
            {
                try
                {
                    if(m_bIsGrab == false)
                    {
                        m_bIsGrab = true;
                        HOperatorSet.GrabImageStart(m_hAcqHandle, -1);
                    }
                    m_image.Dispose();
                    HOperatorSet.GrabImageAsync(out m_image, m_hAcqHandle, -1);
                }
                catch (HalconException HDevExpDefaultException1)
                {
                    System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
                    return 0;
                }
                return 1;
            }
            return 0;
        }
     
        /// <summary>
        /// 停止异常采集
        /// </summary>
        /// <returns></returns>
        public override bool StopGrab()
        {
            if (m_hAcqHandle != null)
            {
                try
                {
                    HOperatorSet.SetFramegrabberParam(m_hAcqHandle, "do_abort_grab", 1);
                }
                catch (HalconException HDevExpDefaultException1)
                {
                    System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置参数，不同型号相机不一样。 
        /// balser 曝光 : "ExposureTimeRaw"
        /// 大恒水星曝光："ExposureTime"
        /// </summary>
        public override void SetGrabParam(string strParam ,int nValue)
        {
            try
            {
                if (m_hAcqHandle == null)
                    Open();
                if(m_hAcqHandle != null)
                    HOperatorSet.SetFramegrabberParam(m_hAcqHandle, strParam, nValue);

            }
            catch (HalconException he)
            {
                System.Diagnostics.Debug.WriteLine(he.ToString());
            }
        }
    }
}
