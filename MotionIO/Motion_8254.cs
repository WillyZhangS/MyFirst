/********************************************************************
	created:	2013/11/12
	filename: 	MOTION_8254
	file ext:	h
	author:		ShengZhang
	purpose:	8254运动控制卡的封装类
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using CommonTool;
using Adlink;


namespace MotionIO
{
    /// <summary>
    /// 8254运动控制卡封装,类名必须以"Motion_"前导，否则加载不到
    /// </summary>
    public class Motion_8254 : Motion
    {

        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
       public Motion_8254(int nCardIndex, string strName,int nMinAxisNo, int nMaxAxisNo)
            :base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            m_bEnable = false;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {           
            //TRACE("init card\r\n");
            int boardID_InBits = 0;
            int mode = (Int32)APS_Define.INIT_AUTO_CARD_ID | (Int32)APS_Define.INIT_PARAM_LOAD_FLASH;

            try
            {
                int ret = APS168.APS_initial(ref boardID_InBits, mode);
                if ((Int32)APS_Define.ERR_NoError == ret)
                {
                    ret = APS168.APS_load_param_from_file("param.xml");
                    if ((Int32)APS_Define.ERR_NoError == ret)
                    {
                        m_bEnable = true;
                        return true;
                    }
                    else
                    {
                        m_bEnable = false;
                        WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,运动控制卡8254读取配置文件失败, result = {0}", ret));
                        return false;
                    }
                }
                else
                {
                    m_bEnable = false;
                    WarningMgr.GetInstance().Error(string.Format("30100,ERR-XYT, 运动控制卡8254初始化失败! result = {0}", ret));
                    return false;
                }
            }
            catch(Exception e)
            {
                m_bEnable = false;
                System.Windows.Forms.MessageBox.Show(e.Message, "控制卡8254初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }                    
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            int ret = APS168.APS_close();
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,8254板卡库文件关闭出错! result = {0}", ret));
                return false;
            }
        }

        /// <summary>
        /// 给予使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            int ret = APS168.APS_set_servo_on(nAxisNo, 1);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,8254 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }
        
        /// <summary>
        /// 断开使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            int ret = APS168.APS_set_servo_on(nAxisNo, 0);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30104,ERR-XYT,8254 Card Axis {0} servo off Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }
        
        /// <summary>
        /// 读取伺服使能状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {            
            if ((GetMotionIoState(nAxisNo) & (0x01 << 7)) == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点参数, 对于8254，此参数代表回原点的方向</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam)
        {
            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_DIR, nParam);
            Thread.Sleep(50);
            int ret = APS168.APS_home_move(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,8254 Axis {0} Home Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }
        
        /// <summary>
        /// 以绝对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            int ret = APS168.APS_absolute_move(nAxisNo, nPos, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,8254 Axis {0} abs move Error,result = {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            int ret = APS168.APS_relative_move(nAxisNo, nPos, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,8254 Axis {0} relative move Error,result is {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        /// JOG运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="bPositive">方向</param>
        /// <param name="bStart">开始标志</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool JogMove(int axis, bool bPositive, int bStart, int nSpeed)
        {
            APS168.APS_set_axis_param(axis, (Int32)APS_Define.PRA_JG_MODE, 0);
            APS168.APS_set_axis_param(axis, (Int32)APS_Define.PRA_JG_DIR, bPositive ? 1 : 0);
            APS168.APS_set_axis_param_f(axis, (Int32)APS_Define.PRA_JG_VM, nSpeed);
            int ret = APS168.APS_jog_start(axis, bStart);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,8254 Axis {0} relative move Error,result = {1}", axis, ret));
                return false;
            }
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            int ret = APS168.APS_stop_move(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;

            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,8254 Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));
                return false;
            }
               
        }
        
        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            int ret = APS168.APS_emg_stop(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30110,ERR-XYT,8254 Card Emergency stop axis {0} Error, result = {1}", nAxisNo, ret));
                return false;
            }
        }
        
        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            return APS168.APS_motion_status(nAxisNo);
        }
        
        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {

       //     Random rnd1 = new Random();

       //     return rnd1.Next();

     
            return APS168.APS_motion_io_status(nAxisNo);
        }
        
        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetAixsPos(int nAxisNo)
        {

            int nAxisPos = 0;
            APS168.APS_get_position(nAxisNo, ref nAxisPos);
            return nAxisPos;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常信止, -1:未知异常 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            int nRet = APS168.APS_motion_status(nAxisNo);
            //if (nRet == (Int32)APS_Define.ERR_NoError)//函数调用正常返回
                if ((nRet & (Int32)(APS_Define.MTS_NSTP_V)) != 0)//运动完成
                {
                    if ((nRet & (Int32)(APS_Define.MTS_ASTP_V)) != 0)//确定错误信息
                    {
                        int stop_code = 0;
                        APS168.APS_get_stop_code(nAxisNo, ref stop_code);
                        if (1 == stop_code)  //急停
                        {
                            Debug.WriteLine("Axis {0} have Emg single \r\n", nAxisNo);
                            return stop_code;
                        }
                        else if (2 == stop_code)  //报警
                        {
                            Debug.WriteLine("Axis {0} have Alm single \r\n", nAxisNo);
                            return stop_code;
                        }
                        else if (3 == stop_code)  //未servo on
                        {
                            Debug.WriteLine("Axis {0} have servo single \r\n", nAxisNo);
                            return stop_code;
                        }
                        else if (4 == stop_code) //正限位   
                        {
                            Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                            return stop_code;
                        }
                        else if (5 == stop_code) //负限位
                        {
                            Debug.WriteLine("Axis {0} have MEL single \r\n", nAxisNo);
                            return stop_code;
                        }
                        return stop_code;

                    }
                    else
                        return 0;//正常运动完成
                }
                else
                    return -1;//未完成
            //}
            //return -1;//调用异常
        }
          
        /// <summary>
        /// 判断轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo)
        {
            int nRet = IsAxisNormalStop(nAxisNo);
            if (nRet == 0)
            {
                int nTargetPos = 0;
                int nPos = 0;
                APS168.APS_get_target_position(nAxisNo, ref nTargetPos);
                APS168.APS_get_position(nAxisNo, ref nPos);

                if (Math.Abs(nPos - nTargetPos) > 100)
                    return 6;  //轴停止后位置超限
            }
            return nRet;
        }

        /// <summary>
        /// 位置清零
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool SetPosZero(int nAxisNo)
        {
            return APS168.APS_set_position(nAxisNo, 0) == (Int32)APS_Define.ERR_NoError;
        }
        
        /// <summary>
        /// 速度模式旋转轴
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {
            int ret = APS168.APS_velocity_move(nAxisNo, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                    WarningMgr.GetInstance().Error(string.Format("30111,ERR-XYT,8254 Card axis {0} VelocityMove Error, result = {1}", nAxisNo, ret));
                return false;
            }
        }

        /// <summary>
        ///此函数8254板卡不提供不使用,回原点内部已经封装好过程处理 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo);
        }

    }

}
