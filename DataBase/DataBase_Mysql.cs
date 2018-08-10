using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySql.Data.MySqlClient;
using System.Data;
using CommonTool;
namespace DataBase
{
    /// <summary>
    /// 数据库处理步骤基类
    /// </summary>
    public class DataBase_Mysql: DataBasebase
    {
        MySqlConnection myConn;
        MySqlDataReader reader;
        MySqlCommand cmd;
        /// <summary>
        /// 状态变更委托函数定义
        /// </summary>
        /// <param name="tcp"></param>
        public delegate void StateChangedHandler(DataBase_Mysql DbsHandle);
        /// <summary>
        /// 状态变更委托事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;
        public DataBase_Mysql(string strName):base(strName)
        {
            // 数据库连接对象
            myConn = new MySqlConnection(m_strEvidence);
        }
        /// <summary>
        /// 连接打开数据库
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (myConn.State == ConnectionState.Closed)
                {
                    // 打开数据库连接
                    myConn.Open();
                    m_IsOpen = true;
                }
            }
            catch (Exception e)
            {
                m_IsOpen = false;
                if (SystemMgr.GetInstance().IsSimulateRunMode() == false)
                    WarningMgr.GetInstance().Error(string.Format("80000,ERR-SSW,{0}:{1}", "Mysq打开失败", e.Message));
                if (StateChangedEvent != null)
                    StateChangedEvent(this);
            }
            return m_IsOpen;
        }
        /// <summary>
        /// 数据库是否连接
        /// </summary>
        /// <returns></returns>
        public override bool IsOpen()
        {
            return m_IsOpen;
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            try
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                    m_IsOpen = false;
                }
            }
            catch
            {

            }
            return m_IsOpen;
        }
        /// <summary>
        /// 增
        /// </summary>
        /// <returns></returns>
        public override bool SqlInsert(string SqlCmd)
        {
            //throw new NotImplementedException();
            try
            {
                cmd = new MySqlCommand(SqlCmd, myConn);
                int num = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
        /// <summary>
        /// 删
        /// </summary>
        /// <returns></returns>
        public override bool SqlDelete(string SqlCmd)
        {
            //throw new NotImplementedException();
            try
            {
                cmd = new MySqlCommand(SqlCmd, myConn);
                int num = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
        /// <summary>
        /// 改
        /// </summary>
        /// <returns></returns>
        public override bool SqlUpdate(string SqlCmd)
        {
            //throw new NotImplementedException();
            try
            {
                cmd = new MySqlCommand(SqlCmd, myConn);
                int num = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
        /// <summary>
        /// 查
        /// </summary>
        /// <returns></returns>
        public override bool SqlSelect(string SqlCmd, out string[] SelectData/*大小自动设定*/)
        {
            try
            {
                // 数据库指令对象
                cmd = new MySqlCommand(SqlCmd, myConn);
                // 执行sql语句，并获取结果
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                // 循环遍历结果
                int i = 0;
                //listNew = new List<string>();
                int ArrayCount = reader.FieldCount;
                m_strArrayData = (string[])Redim(m_strArrayData, ArrayCount);
                reader.Read();
                for (int j=0; j< ArrayCount; j++)
                {
                    m_strArrayData[j] = reader[j].ToString();
                }
                SelectData = m_strArrayData;
                return true;
            }
            catch(Exception e)
            {
                SelectData = null;
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return false;
        }

    }
}
