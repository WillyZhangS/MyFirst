using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommonTool;
using AutoFrameDll;
using System.Diagnostics;

namespace DataBase
{
    /// <summary>
    /// 数据库管理类
    /// </summary>
    public class DataBaseMgr : SingletonTemplate<DataBaseMgr>
    {
        /// <summary>
        /// 数据库处理步骤的集合,以数据库名称为key
        /// </summary>
        public Dictionary<string, DataBasebase> m_dicDBS = new Dictionary<string, DataBasebase>();
        /// <summary>
        /// 状态变更委托函数定义
        /// </summary>
        /// <param name="tcp"></param>
        public delegate void StateChangedHandler(DataBaseMgr DatMgrHandler);
        /// <summary>
        /// 状态变更委托事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;
        /// <summary>
        /// 文件配置路径
        /// </summary>
        public string m_strConfigDir;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataBaseMgr()
        {
            m_dicDBS.Clear();
            m_strConfigDir = AppDomain.CurrentDomain.BaseDirectory + "VisionConfig\\"/* + ProductInfo.GetInstance().ProductName + "\\"*/;
        }
        /// <summary>
        /// 增加数据库操作类
        /// </summary>
        public void AddDataBase(string str_DbsName, DataBasebase DBS)
        {
            m_dicDBS.Add(str_DbsName, DBS);
        }
        /// <summary>
        /// 打开指定数据库
        /// </summary>
        /// <param name="strDbsName"></param>
        /// <returns></returns>
        public bool Open(string strDbsName)
        {
            try
            {
                DataBasebase dbs;
                m_dicDBS.TryGetValue(strDbsName, out dbs);
                dbs.Open();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("数据库:{0}连接失败{1}\r\n", strDbsName, e.Message));
                if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                    WarningMgr.GetInstance().Error(string.Format("80000,ERR-SSW,数据库：{0}连接失败{1}", strDbsName,e.Message));
                if (StateChangedEvent != null)
                    StateChangedEvent(this);
            }
            return false;
        }
        /// <summary>
        /// 获取指定名称的数据库类
        /// </summary>
        /// <param name="strDbsName"></param>
        /// <returns></returns>
        public DataBasebase GetDataBase(string strDbsName)
        {
            return m_dicDBS[strDbsName];
        }
        /// <summary>
        /// 根据指定数据库获取数据
        /// </summary>
        /// <returns></returns>
        public bool ProcessSelect(string strDbsName, string SqlCmd, ref string[] SelectData)
        {
            DataBasebase dbs;
            try
            {
                m_dicDBS.TryGetValue(strDbsName, out dbs);
                if (dbs == null)
                {
                    SelectData = null;
                    if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                        WarningMgr.GetInstance().Error(string.Format("80001,ERR-SSW,数据库{0}: undefine", strDbsName));
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
                else
                {
                    if (!Open(strDbsName))
                    {
                        System.Diagnostics.Debug.WriteLine("数据库连接失败");
                    }
                    dbs.SqlSelect(SqlCmd, out SelectData);
                    dbs.Close();
                    return true;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(string.Format("数据库{0}:接收到命令{1}，但输出异常\r\n", strDbsName, SqlCmd, e.Message));
                SelectData = null;
            }
            return false;
        }
        /// <summary>
        /// 增删改集中处理类
        /// </summary>
        /// <param name="strDbsName"></param>
        /// <param name="SqlCmd"></param>
        /// <param name="_bIsInst"></param>
        /// <param name="_bIsDel"></param>
        /// <param name="_bIsUpdate"></param>
        /// <returns></returns>
        public bool ProcessInstDelUpdate(string strDbsName, string SqlCmd,bool _bIsInst=false,bool _bIsDel=false,bool _bIsUpdate=false)
        {
            DataBasebase dbs;
            try
            {
                m_dicDBS.TryGetValue(strDbsName, out dbs);
                if (dbs == null)
                {
                    if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                        WarningMgr.GetInstance().Error(string.Format("80001,ERR-SSW,数据库{0}: undefine", strDbsName));
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
                else
                {
                    if (!Open(strDbsName))
                    {
                        System.Diagnostics.Debug.WriteLine("数据库连接失败");
                    }
                    if (_bIsInst)
                    {
                        if(!dbs.SqlInsert(SqlCmd)) return false;
                    }
                    else if (_bIsDel)
                    {
                        if (!dbs.SqlDelete(SqlCmd)) return false;
                    }
                    else if (_bIsUpdate)
                    {
                        if (!dbs.SqlUpdate(SqlCmd)) return false;
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("数据库{0}:接收到命令{1}，但未定义命令输出\r\n", strDbsName, SqlCmd));
                        return false;
                    }
                    dbs.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("数据库{0}:接收到命令{1}，但输出异常\r\n", strDbsName, SqlCmd, e.Message));
            }
            return false;
        }
    }
}
