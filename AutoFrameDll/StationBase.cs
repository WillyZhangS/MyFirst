using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CommonTool;
using Communicate;
using System.Diagnostics;

namespace AutoFrameDll
{
    /// <summary>
    /// 点位信息
    /// </summary>
    public struct PointInfo
    {
        /// <summary>
        /// 点位名称
        /// </summary>
        public string strName;
        /// <summary>
        /// x轴脉冲位置
        /// </summary>
        public int x;
        /// <summary>
        /// y轴脉冲位置
        /// </summary>
        public int y;
        /// <summary>
        /// z轴脉冲位置
        /// </summary>
        public int z;
        /// <summary>
        /// u轴脉冲位置
        /// </summary>
        public int u;
    }
 
    /// <summary>
    /// 工站基类
    /// </summary>
    public class StationBase: LogView
    {
        /// <summary>
        /// 站位异常类，表示站位流程内出现流程异常
        /// </summary>
        public class StationException : ApplicationException
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message"></param>
            public StationException(string message) : base(message)
            {
            }
        }
        /// <summary>
        /// 站位异常类，表示站位正常退出或因其它站位退出信号退出
        /// </summary>
        public class SafeException : ApplicationException
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message"></param>
            public SafeException(string message) : base(message)
            {
            }
        }
        /// <summary>
        /// 输入io数组
        /// </summary>
        public string[] io_in= { } ;
        /// <summary>
        /// 输出io数组
        /// </summary>
        public string[] io_out = { };
        /// <summary>
        /// 每个轴运动方向,true-正，false-负
        /// </summary>
        public bool[] bPositiveMove = { true, true, true, true };   
        /// <summary>
        /// 轴名字
        /// </summary>
        public string[] strAxisName = { "X轴", "Y轴", "Z轴", "U轴" };
        /// <summary>
        /// 轴号数组
        /// </summary>
        public int []m_nAxisArray =  {0, 0, 0, 0 };	
        /// <summary>
        /// 点位集合
        /// </summary>
        public Dictionary<int, PointInfo> m_dicPoint = new Dictionary<int, PointInfo>();
  
        private StationState m_nCurState;                   //当前站位状态
        /// <summary>
        /// 自动线程是否运行中
        /// </summary>
        private bool m_bRunThread;
        private Thread m_thread = null;   

        /// <summary>
        /// 工站基类构造函数
        /// </summary>
        /// <param name="strName"></param>
        public StationBase(string strName)
        {
            _strName = strName;
        }
        /// <summary>
        /// 站位当前是否全自动运行 
        /// </summary>
        bool _bBeginCycle = false;
        /// <summary>
        /// 是否全自动运行属性
        /// </summary>       
        public bool BeginCycle
        {
            set { _bBeginCycle = value; }
            get { return _bBeginCycle; }
        }
        /// <summary>
        /// 站位名
        /// </summary>
        private string _strName = string.Empty;
        /// <summary>
        /// 站位名属性
        /// </summary>
        public string Name
        {
            get { return _strName; }
        }
        /// <summary>
        /// 站位序号
        /// </summary>
        private int _nIndex = 0; 
        /// <summary>
        /// 站位序号属性
        /// </summary>
        public int Index
        {
            set { _nIndex = value; }
            get { return _nIndex; }
        }

        /// <summary>
        /// 站位是否启用
        /// </summary>
        bool _bStationEnable = true;
        /// <summary>
        /// 得到站位启动状态or设置站位是否启动 属性
        /// </summary>
        public bool StationEnable
        {
            get { return _bStationEnable; }
            set { _bStationEnable = value; }
        }

        /// <summary>
        /// 设置轴号
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nAxisNo"></param>
        public void SetAxisNo(int index, int nAxisNo)
        {
            m_nAxisArray[index] = nAxisNo;
        }

        /// <summary>
        /// 得到轴号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetAxisNo(int index)  
        {
           return  m_nAxisArray[index];
        }

        /// <summary>
        /// 得到X轴号
        /// </summary>
        public int AxisX   {get{ return m_nAxisArray[0]; }}
        
        /// <summary>
        /// 得到Y轴号
        /// </summary>
        public int AxisY  {get{return m_nAxisArray[1]; }}
        
        /// <summary>
        /// 得到Z轴号
        /// </summary>
        public int AxisZ  {get{ return m_nAxisArray[2]; }}
        
        /// <summary>
        /// 得到U轴号
        /// </summary>
        public int AxisU {get{ return m_nAxisArray[3]; }}

        /// <summary>
        /// 开始运行
        /// </summary>
        public void StartRun()  
        {
            if(m_thread == null)
                m_thread = new Thread(ThreadProc);
            if (m_thread.ThreadState != System.Threading.ThreadState.Running)
            {         
                m_bRunThread = true;
                m_thread.Start();
            }
            if(SystemMgr.GetInstance().IsSimulateRunMode())
                ShowLog("当前站位处于模拟运行模式");
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        public void StopRun()
        {
            if (m_thread != null)
            {
                m_bRunThread = false;
                m_nCurState = StationState.STATE_MANUAL;
                
                if(m_thread.Join(5000) == false)
                    m_thread.Abort();
              
                m_thread = null;
            }
        }

        //public bool IsAuto()
        //{

        //}

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="strXml"></param>
        void StartScriptFun(string strXml)
        { }

        /// <summary>
        /// 站位运行状态切换
        /// </summary>
        /// <param name="nState"></param>
        public void SwitchState(StationState nState)
        {
            if(m_nCurState != nState)
            {
                m_nCurState = nState;
                ShowLog("状态切换至" + nState.ToString());
            }
        }

        /// <summary>
        /// 超时提示, 工程师模式时可以忽略超时错误继续运行，其它模式只支持继续等待或停止站位
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public DialogResult ShowMessage(string strText)
        {
            if(Security.IsEngMode())
            {
                Form_Message frm = new Form_Message(this);
                return frm.MessageShow(strText, "超时提示", MessageBoxButtons.YesNoCancel);
            }
            //return MessageBox.Show(strText,"超时提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
            //     MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
            else
            {
                Form_Message frm = new Form_Message(this);
                return frm.MessageShow(strText, "超时提示", MessageBoxButtons.YesNo);
            }
                //return MessageBox.Show(strText, "超时提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                //     MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
        
        /// <summary>
        /// 站位流程延时等待,当写流程需要较长时间等待时使用此函数
        /// </summary>
        /// <param name="nMilliSeconds">毫秒值</param>
        public void WaitTimeDelay(int nMilliSeconds)
        {
            int nTime = 0;
            while(true)
            {
                Thread.Sleep(SystemMgr.GetInstance().ScanTime);                
                CheckContinue();
                nTime += SystemMgr.GetInstance().ScanTime;
                if (nTime > nMilliSeconds)
                    break;
            }
        }

        private int GetMotionTimeOut()
        {
            int n = SystemMgr.GetInstance().GetParamInt("MotionTimeOut");
            if (n == 0)
                return 600;
            else
                return n;
        }
        /// <summary>
        /// 等待回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        public void WaitHome(int nAxisNo, int nTimeOut = 0)
        {       
            string strName = strAxisName[Array.IndexOf(m_nAxisArray, nAxisNo)];
            ShowLog(string.Format("等待{0}回原点",strName));
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }

            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetMotionTimeOut();
            while (true)
            {
                CheckContinue();
                int nRet = MotionMgr.GetInstance().IsHomeNormalStop(nAxisNo);
                if (nRet == -1)   //未到位
                {
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待{1}回原点超时",Name,strName);
                        DialogResult dr = ShowMessage(strInfo + ",是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("30001,ERR-XYT,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }                 
                }
                else if (nRet == 0) //已到位
                {
                    break;
                }
                else //异常停止
                {
                    string[] strError = { "急停", "报警", "未励磁", "正限位", "负限位", "停止位置超限" };
                    int nIndex = Array.IndexOf(m_nAxisArray, nAxisNo);
                    string strInfo = string.Format("{0}出现{1} {2}异常", Name, strAxisName[nIndex], strError[nRet - 1]);
                    throw new StationException(string.Format("30002,ERR-XYT,{0}", strInfo));
                }
            }
        }

        /// <summary>
        /// 等待轴运动完毕
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        public void WaitMotion(int nAxisNo, int nTimeOut = 0)
        {
            string strName = strAxisName[Array.IndexOf(m_nAxisArray, nAxisNo)];
            ShowLog(string.Format("等待{0}轴到位",strName));
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }

            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetMotionTimeOut();
            while (true)
            {
                CheckContinue();
                int nRet = MotionMgr.GetInstance().IsAxisInPos(nAxisNo);
                if (nRet == -1)   //未到位
                {
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待{1}到位超时", Name, strName);
                        DialogResult dr = ShowMessage(strInfo + ",是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("30003,ERR-XYT,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }                  
                }
                else if(nRet == 0) //已到位
                {
                    break;
                }
                else //异常停止
                {
                   string[] strError = { "急停", "报警", "未励磁", "正限位", "负限位","停止后位置超限" };
                    int nIndex = Array.IndexOf(m_nAxisArray, nAxisNo);
                    string strInfo = string.Format("{0}出现{1}{2}异常", Name, strAxisName[nIndex], strError[nRet - 1]);
                    throw new StationException(string.Format("30002,ERR-XYT,{0}", strInfo));
                }                    
            }
        }

        private int GetIoTimeOut()
        {
            int n = SystemMgr.GetInstance().GetParamInt("IoTimeOut");
            if (n == 0)
                return 100;
            else
                return n;
        }

      
        /// <summary>
        /// 等待IO输入点有效
        /// </summary>
        /// <param name="nCardNo">卡号</param>
        /// <param name="nBitIndex">点位索引 从1开始</param>
        /// <param name="bValid">高电平有效或低电平有效</param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">暂停时是否卡在此函数中不跳出， true:等待暂停恢复  false:当流程暂停时跳出此函数不等待 </param>
        public void WaitIo(int nCardNo, int nBitIndex, bool bValid, int nTimeOut=-1 , bool  bShowDialog = true, bool bPause = true )
        {
            string strName = IoMgr.GetInstance().GetIoInName(nCardNo, nBitIndex - 1);
            ShowLog(string.Format("等待IO{0}.{1}{2}，有效电平{3}", nCardNo, nBitIndex, strName, bValid));
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }
            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetIoTimeOut();
            while (true)
            {
                if (bPause)
                    CheckContinue();
                else
                    CheckContinue_NotBreak();
                bool bBit = IoMgr.GetInstance().ReadIoInBit(nCardNo, nBitIndex);
                if (bBit != bValid)   //未到位
                {
                     Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (bShowDialog  && nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待IO点等待IO{1}.{2}{3}为{4}超时", Name, nCardNo, nBitIndex, strName, bValid.ToString());
                        DialogResult dr = ShowMessage(strInfo + ",是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("20001,ERR-XYT,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                    else if (bShowDialog == false && nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                     
                        string strInfo = string.Format("{0}等待IO点IO{1}.{2}{3}有效电平为{4}达到{5}秒钟结束,跳出等待", 
                            Name, nCardNo, nBitIndex, strName, bValid.ToString(),nTimeOut.ToString());
                        ShowLog(strInfo);
                        break;
                    }
                }
                else //已到位
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 等待IO输入点有效
        /// </summary>
        /// <param name="strIoName">点位名称</param>
        /// <param name="bValid">高电平有效或低电平有效</param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">暂停时是否卡在此函数中不跳出， true:等待暂停恢复  false:当流程暂停时跳出此函数不等待 </param>
        public void WaitIo(string strIoName, bool bValid, int nTimeOut = -1, bool bShowDialog = true, bool bPause = true)
        {
            Int64 nData;
            if (IoMgr.GetInstance().m_dicIn.TryGetValue(strIoName, out nData))
            {
                WaitIo((int)(nData >> 8), (int)(nData & 0xff), bValid, nTimeOut, bShowDialog, bPause);
            }
            else
            {
                string strInfo = string.Format("不存在的IO输入点名称 {0}， 请确认配置是否正确", strIoName);
                MessageBox.Show(strInfo, "IO等待输入点出错");
            }
        }

        private int GetRegTimeOut()
        {
            int n = SystemMgr.GetInstance().GetParamInt("RegTimeOut");
            if (n == 0)
                return 600;
            else
                return n;
        }

        /// <summary>
        /// 等待系统寄存器是否有效
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bValid"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        public void WaitRegBit(int nIndex, bool bValid, int nTimeOut = -1)
        {
            ShowLog(string.Format("等待位寄存器{0}，有效值{1}",nIndex, bValid.ToString()));

            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetRegTimeOut();
            while (true)
            {
                CheckContinue();
                bool bBit = SystemMgr.GetInstance().GetRegBit(nIndex);
                if (bBit != bValid)//未到位
                {
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待系统寄存器{1}为{2}超时",Name, nIndex, bValid.ToString());
                        DialogResult dr = ShowMessage(strInfo + "，是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("50001,ERR-SSW,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                }
                else //已到位
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 等待系统整型寄存器
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="nTarget"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        public void WaitRegInt(int nIndex, int nTarget, int nTimeOut = -1)
        {
            ShowLog(string.Format("等待整型寄存器{0}，有效值{1}", nIndex, nTarget));
            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetRegTimeOut();
            while (true)
            {
                CheckContinue();
                int  nRet = SystemMgr.GetInstance().GetRegInt(nIndex);
                if (nRet != nTarget)   //未到位
                {
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待系统值寄存器{1}为{2}超时", Name, nIndex, nTarget);
                        DialogResult dr = ShowMessage(strInfo + "，是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("50002,ERR-SSW,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                }
                else //已到位
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 判定串口数据通讯是否超时报警
        /// </summary>
        /// <param name="comLink"></param>
        public void WaitCommunicate(ComLink comLink)
        {
            if (comLink.IsTimeOut())
            {
                string strInfo = Name + comLink.m_strName + "读取超时";
                if (ShowMessage(strInfo + "，是否继续运行") == DialogResult.No)
                {
                    ShowLog("等待超时，退出流程");
                    throw new StationException(string.Format("20002,ERR-SSW,{0}", strInfo));
                }
                else
                {
                    ShowLog("等待超时，忽略此超时");
                }

            }
            CheckContinue();
        }

        /// <summary>
        /// 判定网口数据通讯是否超时报警
        /// </summary>
        /// <param name="tcpLink"></param>
        public void WaitCommunicate(TcpLink tcpLink)
        {
            if (tcpLink.IsTimeOut())
            {
                string strInfo = Name + tcpLink.m_strName + "读取超时";
                if (ShowMessage(strInfo + "，是否继续运行") == DialogResult.No)
                {
                    ShowLog("等待超时，退出流程");
                    throw new StationException(string.Format("20003,ERR-SSW,{0}", strInfo)); ;
                }
                else
                {
                    ShowLog("等待超时，忽略此超时");
                }
            }
            CheckContinue();
        }

        private int GetCommTimeOut()
        {
            int n = SystemMgr.GetInstance().GetParamInt("CommandTimeOut");
            if (n == 0)
                return 600;
            else
                return n;
        }

        /// <summary>
        ///  在网口上等待指定命令
        /// </summary>
        /// <param name="tcplink"></param>
        /// <param name="strCmd"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">暂停时是否卡在此函数中不跳出， true:等待暂停恢复  false:当流程暂停时跳出此函数不等待 </param> 

        public void wait_recevie_cmd(TcpLink tcplink, string strCmd, int nTimeOut = -1, bool bShowDialog = true, bool bPause = true)
        {
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }
            string strData = "";
            ShowLog(string.Format("等待{0}接收命令:{1}", tcplink.m_strName, strCmd));
            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetCommTimeOut();
            while (true)
            {
                if (bPause)
                    CheckContinue();
                else
                    CheckContinue_NotBreak();
                tcplink.ReadLine(out strData);
                if (strCmd == strData)
                {
                    break;
                }
                else
                {
                    if(strData.Length > 0)
                    {
                        ShowLog("接收到:" + strData + ",继续接收中");
                    }
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待接收命令{1}超时", Name, strCmd);
                        DialogResult dr = ShowMessage(strInfo + "，是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("20004,ERR-SSW,{0}", strInfo));
                        }
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                    else if (bShowDialog == false && nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待接收命令{1}达到{2}秒钟结束,跳出等待",Name, strCmd, nTimeOut);
                        ShowLog(strInfo);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  在串口上等待指定命令
        /// </summary>
        /// <param name="comLink"></param>
        /// <param name="strCmd"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>     
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">暂停时是否卡在此函数中不跳出， true:等待暂停恢复  false:当流程暂停时跳出此函数不等待 </param> 
        private void wait_recevie_cmd(ComLink comLink, string strCmd, int nTimeOut = -1, bool bShowDialog = true, bool bPause = true)
        {
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }
            string strData = "";
            ShowLog(string.Format("等待{0}接收命令:{1}", comLink.m_strName, strCmd));
            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetCommTimeOut();
            while (true)
            {
                if (bPause)
                    CheckContinue();
                else
                    CheckContinue_NotBreak();
                comLink.ReadLine(out strData);
                if (strCmd == strData)
                {
                    break;
                }
                else
                {
                    if (strData.Length > 0)
                    {
                        ShowLog("接收到:" + strData + ",继续接收中");
                    }
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待接收命令{1}超时", Name, strCmd);
                        DialogResult dr = ShowMessage(strInfo + "，是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("20004,ERR-SSW,{0}", strInfo));
                        }                         
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                    else if (bShowDialog == false && nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待接收命令{1}达到{2}秒钟结束,跳出等待", Name, strCmd, nTimeOut);
                        ShowLog(strInfo);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  在串口上等待指定命令
        /// </summary>
        /// <param name="comLink"></param>
        /// <param name="strCmd"></param>
        /// <param name="nTimeOut">超时时间，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bPassPause">是否忽略暂停处理  false:默认等待暂停  true:当流程暂停时跳过此等待 </param>
        private void wait_recevie_cmd(ComLink comLink, string strCmd, int nTimeOut , bool bPassPause = false)
        {
            if (SystemMgr.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return;
            }
            string strData = "";
            ShowLog(string.Format("等待{0}接收命令:{1}", comLink.m_strName, strCmd));
            int nStartCount = System.Environment.TickCount;
            if (nTimeOut == 0)
                nTimeOut = GetCommTimeOut();
            while (true)
            {
                CheckContinue_NotBreak();
                comLink.ReadLine(out strData);
                if (strCmd == strData)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(SystemMgr.GetInstance().ScanTime);
                    if (nTimeOut != -1 && System.Environment.TickCount - nStartCount > nTimeOut * 1000)
                    {
                        string strInfo = string.Format("{0}等待接收命令{1}超时", Name, strCmd);
                        DialogResult dr = ShowMessage(strInfo + "，是否继续等待");
                        if (dr == DialogResult.No)
                        {
                            ShowLog("等待超时，退出流程");
                            throw new StationException(string.Format("20004,ERR-SSW,{0}", strInfo));
                        }                         
                        else if (dr == DialogResult.Yes)
                        {
                            ShowLog("等待超时，重置超时");
                            nStartCount = System.Environment.TickCount;
                        }
                        else
                        {
                            ShowLog("等待超时，忽略此超时");
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 检查系统运行状态
        /// </summary>    
        /// <returns></returns>
        private bool CheckContinue_NotBreak()
        {
            while (true)
            {
                if (m_nCurState == StationState.STATE_AUTO)   //自动运行
                {
                    return true;
                }
                else if (m_nCurState == StationState.STATE_PAUSE )  //暂停
                {
                     return true;
                }
                else if (m_nCurState == StationState.STATE_MANUAL)   //停止
                {
                    throw new SafeException(Name + "手动停止");
                }
                else if (m_nCurState == StationState.STATE_EMG)
                {
                    throw new StationException(Name + "外部异常停止");
                }
                else if(m_nCurState == StationState.STATE_READY)
                {                   
                    return true;
                }
            }
        }

        /// <summary>
        /// 检查系统运行状态
        /// </summary>
        /// <returns></returns>
        public bool CheckContinue(bool bIsDeinit= false)
        {
            while (true)
            {
                if (m_nCurState == StationState.STATE_AUTO)   //自动运行
                {
                    return true;
                }
                else if (m_nCurState == StationState.STATE_PAUSE )  //暂停
                {
                    Thread.Sleep(50);
                }
                else if (m_nCurState == StationState.STATE_MANUAL)   //停止
                {
                    if(bIsDeinit == false)
                        throw new SafeException(Name + "手动停止");
                }
                else if (m_nCurState == StationState.STATE_EMG)
                {
                    if (bIsDeinit == false)
                        throw new StationException(Name + "外部异常停止");
                }
                else if(m_nCurState == StationState.STATE_READY)
                {                   
                    return true;
                }
            }
        }

        /// <summary>
        /// 等待开始
        /// </summary>
        protected void WaitBegin()
        {
            ShowLog("站位就绪，等待开始");
            while (true)
            {
                CheckContinue();//检测是否需要暂停卡住

                if (_bBeginCycle)  //如果自动运行信号已经就绪
                {
           //         m_nCurState = StationState.STATE_AUTO;
                    _bBeginCycle = false;        //置回false, 避免下次其它站未就绪继续自动
                    return;
                }
                else //还未收到自动开始信号
                {
                    if (m_nCurState != StationState.STATE_READY)
                    {
                        m_nCurState = StationState.STATE_READY; //置为就绪态,由站位管理器来确认是否启动                      
                    }                   
                }
                Thread.Sleep(SystemMgr.GetInstance().ScanTime);
            }
        }

        /// <summary>
        ///站位公共处理步骤
        /// </summary>
        public void ThreadProc()
        {         
            while (m_bRunThread)
            {
                //处于自动运行状态时
                if(m_nCurState == StationState.STATE_AUTO)
                {
                    try
                    {
                        if(StationEnable)
                            StationInit();
                        while (true)
                        {
                            if (StationEnable)
                                StationProcess();  //站位异常或状状切换会跳到异常处理
                            else
                                DisableRun();
                        }
                    }
                    catch (StationException e)
                    {
                        if(m_nCurState != StationState.STATE_EMG)
                        {
                            m_nCurState = StationState.STATE_EMG;
                            if (StationMgr.GetInstance().IsEmg() == false)
                            {
                                StationMgr.GetInstance().EmgStopAllStation();
                                WarningMgr.GetInstance().Error(e.Message);
                                ShowLog("本站位出现异常,停止运行");
                            }
                            Debug.WriteLine(e.Message);
                        }
                        else
                        {
                            ShowLog(e.Message);                           
                            Debug.WriteLine(e.Message);
                        }                    
                    }
                    catch (SafeException e)//如果已经切回手动状态，则退出线程
                    {
                        Debug.WriteLine(e.Message);
                        ShowLog("手动停止运行");
                        break;
                    }
                    catch (Exception e)
                    {
                        m_nCurState = StationState.STATE_EMG;
                        StationMgr.GetInstance().EmgStopAllStation();
                        MessageBox.Show(e.ToString(), Name + "出现异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ShowLog( "本站位出现异常,停止运行");
                    }
                }                
                    //当为急停时，等待复位
                Thread.Sleep(50);
            }
            //todo: deinit时调用checkContinue会导致异常，此处可能需要移到循环里
        //    try
            {
                StationDeinit();
            }
        }

        /// <summary>
        /// 站位初始化,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationInit()
        {
        }

        /// <summary>
        /// 站位结束处理过程,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationDeinit()
        {
        }

        /// <summary>
        /// 站位初始化为安全状态,虚函数，重写需要子类继承
        /// </summary>
        public virtual void InitSecurityState()
        {
        }

        /// <summary>
        /// 站位是否已经等待开始
        /// </summary>
        /// <returns></returns>
        public virtual bool IsReady()
        {
            return m_nCurState == StationState.STATE_READY;
        }

        /// <summary>
        /// 响应流程暂停的处理，比如流水线在暂停时需要停止
        /// </summary>
        public virtual void OnPause()
        {

        }
        /// <summary>
        /// 响应流程恢复的处理，比如流水线在恢复时需要继续运行
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// 站位急停,虚函数，重写需要子类继承
        /// </summary>
        public virtual void EmgStop()
        {
            for (int i = 0; i < 4; ++i)
            {
                if (m_nAxisArray[i] > 0)
                    MotionMgr.GetInstance().StopEmg(m_nAxisArray[i]);
            }
        }

        /// <summary>
        /// 站位被禁用时空运行
        /// </summary>
        public void DisableRun()
        {
            WaitBegin();
            ShowLog("此站已禁用，就绪等待中");
            WaitTimeDelay(1000);

        }

        /// <summary>
        /// 站位处理流程,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationProcess()
        {
            WaitBegin();
            ShowLog("流程未编写，默认就绪等待中");
            WaitTimeDelay(1000);

        }
    }
}
