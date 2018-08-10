using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using CommonTool;
using System.Threading;

namespace Communicate
{
    /// <summary>
    /// 网络连接封装类
    /// </summary>
    public class TcpLink :LogView
    {
        /// <summary>
        ///网口号 
        /// </summary>
        public int m_nIndex;         
        /// <summary>
        ///网口定义 
        /// </summary>
        public string m_strName;     
        /// <summary>
        ///对方IP地址 
        /// </summary>
        public string m_strIP;        
        /// <summary>
        ///端口号 
        /// </summary>
        public int m_nPort;           
        /// <summary>
        ///超时时间 
        /// </summary>
        public int m_nTime;           
        /// <summary>
        ///命令分隔 
        /// </summary>
        public string m_strLineFlag;  

        /// <summary>
        ///命令分隔符 
        /// </summary>
        private string m_strLine;

        private TcpClient m_client = null;
        private bool m_bTimeOut = false;

        /// <summary>
        /// 状态变更委托函数定义
        /// </summary>
        /// <param name="tcp"></param>
        public delegate void StateChangedHandler(TcpLink tcp);
        /// <summary>
        /// 状态变更委托事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;

        private static bool m_bConnectSuccess = false;
        private static Exception socketException;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="strName"></param>
        /// <param name="strIP"></param>
        /// <param name="nPort"></param>
        /// <param name="nTime"></param>
        /// <param name="strLine"></param>
        public TcpLink(int nIndex, string strName, string strIP, int nPort, int nTime, string strLine)
        {
            m_nIndex = nIndex;
            m_strName = strName;
            m_strIP = strIP;
            m_nPort = nPort;
            m_nTime = nTime;

            m_strLineFlag = strLine;
            if (strLine == "CRLF")
            {
                m_strLine = "\r\n";
            }
            else if(strLine == "CR" )
            {
                m_strLine = "\r";
            }
            else if(strLine == "LF")
            {
                m_strLine = "\n";
            }
            else if(strLine == "无")
            {
                m_strLine = "";
            }
        }

        /// <summary>
        /// 判断是否超时
        /// </summary>
        /// <returns></returns>
        public bool IsTimeOut()
        {
            return m_bTimeOut;
        }

        /// <summary>
        ///网口打开时通过回调检测是否连接超时。 5秒种 
        /// </summary>
        /// <param name="asyncResult"></param>
        private static void CallBackMethod(IAsyncResult asyncResult)
        {
            try
            {
                m_bConnectSuccess = false;
                TcpClient tcpClient = asyncResult.AsyncState as TcpClient;
                if(tcpClient.Client != null)
                {
                    tcpClient.EndConnect(asyncResult);
                    m_bConnectSuccess = true;
                }
            }
            catch(Exception ex)
            {
                m_bConnectSuccess = false;
                socketException = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        /// <summary>
        ///打开网口 
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (m_client == null)
            {
                m_client = new TcpClient();
            }
            if(m_client.Connected == false)
            {
                m_client.SendBufferSize = 4096;
                m_client.SendTimeout = m_nTime;
                m_client.ReceiveTimeout = m_nTime;
                m_client.ReceiveBufferSize = 4096;

                try
                {
                    TimeoutObject.Reset();
                    socketException = null;
                    m_client.BeginConnect(m_strIP, m_nPort, new AsyncCallback(CallBackMethod), m_client);
                    if(TimeoutObject.WaitOne(5000, false))
                    {
                        if (m_bConnectSuccess)
                        {
                            if (StateChangedEvent != null)
                                StateChangedEvent(this);
                            return m_client.Connected;
                        }
                        else
                            throw socketException;                     
                    }
                    else
                    {
     //                   m_client.Close();
                        throw new TimeoutException("TimeOut Exception");
                    }
                    
               //     m_client.Connect(m_strIP, m_nPort);
                }
                catch(Exception e)
                {
                    m_bTimeOut = true;
                    Debug.WriteLine(string.Format("{0}:{1}{2}\r\n", m_strIP, m_nPort, e.Message));
                    if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                        WarningMgr.GetInstance().Error(string.Format("51210,ERR-SSW,{0}:{1}{2}",m_strIP, m_nPort,e.Message ));
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }          
            }
            return m_client.Connected;
        }

        /// <summary>
        /// 判断网口是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return m_client != null && m_client.Connected;
        }

        /// <summary>
        ///向网口写入数据 
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        public bool WriteData(byte[] sendBytes, int nLen)
        {
            if (m_client.Connected)
            {
                NetworkStream netStream = m_client.GetStream();
                if (netStream.CanWrite)
                {
                    netStream.Write(sendBytes, 0, nLen);
                    ShowLog(System.Text.Encoding.Default.GetString(sendBytes));
                }
                //netStream.Close();
                return true;
            }
            return false;
        }

        /// <summary>
        ///向网口写入字符串 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool WriteString(string strData)
        {
            if(m_client.Connected)
            {
                NetworkStream netStream = m_client.GetStream();
                if (netStream.CanWrite)
                {
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(strData);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    ShowLog(strData);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///向网口写入一行字符 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool WriteLine(string strData)
        {
            if (m_client.Connected)
            {
                NetworkStream netStream = m_client.GetStream();
                if (netStream.CanWrite)
                {
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(strData + m_strLine);
                    netStream.Write(sendBytes, 0, sendBytes.Length);
                    ShowLog(strData);
                }
                //netStream.Close();
                return true;
            }
            return false;
        }

        /// <summary>
        ///从网口读取数据 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        public int ReadData(byte[] bytes, int nLen)
        {
            m_bTimeOut = false;
            int n = 0;
            if (m_client.Connected)
            {
                try
                {
                    NetworkStream netStream = m_client.GetStream();
                    if (netStream.CanRead)
                    {
                        n = netStream.Read(bytes, 0, nLen);
                        if (n > 0)
                        {
                           ShowLog(System.Text.Encoding.Default.GetString(bytes));
                        }
                     }                   
                }
                catch/*(TimeoutException e)*/
                {
                    m_bTimeOut = true;
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
            }
            return n;
        }

        /// <summary>
        ///从网口读取一行数据 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public int ReadLine(out string strData)
        {
            m_bTimeOut = false;
            strData = "";
            if (m_client.Connected)
            {
                try
                {
                    NetworkStream netStream = m_client.GetStream();
                    if (netStream.CanRead)
                    {
                        byte[] bytes = new byte[m_client.ReceiveBufferSize];
                       int n = netStream.Read(bytes, 0, (int)m_client.ReceiveBufferSize);
                        strData = Encoding.UTF8.GetString(bytes, 0, n);
                        if (strData.Length > 0)
                        {
                            ShowLog(strData);
                        }
                    }
                }
                catch /*(TimeoutException e)*/
                {
                    m_bTimeOut = true;
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
            }
            return strData.Length;
        }

        /// <summary>
        ///关闭网口 
        /// </summary>
        public void Close()
        {
            if (m_client != null)
            {
                if (m_client.Connected)
                {
                    NetworkStream netStream = m_client.GetStream();
                    netStream.Close();
                }
                m_client.Close();

                m_client = null;
                m_bTimeOut = false;
                if (StateChangedEvent != null)
                    StateChangedEvent(this);
            }
        }

        /// <summary>
        /// 清除缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            if(m_client != null)
            {
                NetworkStream netStream = m_client.GetStream();
                m_client.GetStream().Flush();
                netStream.Close();
            }
        }             
    }
}

