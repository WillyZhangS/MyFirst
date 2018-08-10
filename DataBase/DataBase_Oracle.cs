using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataBase
{
    /// <summary>
    /// 数据库处理步骤类
    /// </summary>
    public class DataBase_Oracle: DataBasebase
    {
        /// <summary>
        /// Oracle数据库链接信息描述定义
        /// </summary>
        public string m_strOracleDescribe = "";
        public DataBase_Oracle(string strName) : base(strName)
        {

        }
        public override bool Open()
        {
            return false;
        }
        public override bool IsOpen()
        {
            return false;
        }
        public override bool Close()
        {
            return false;
        }
        /// <summary>
        /// 增
        /// </summary>
        /// <returns></returns>
        public override bool SqlInsert(string SqlCmd)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 删
        /// </summary>
        /// <returns></returns>
        public override bool SqlDelete(string SqlCmd)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 改
        /// </summary>
        /// <returns></returns>
        public override bool SqlUpdate(string SqlCmd)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 查
        /// </summary>
        /// <returns></returns>
        public override bool SqlSelect(string SqlCmd, out string[] SelectData/*List<string> listNew*/)
        {
            SelectData = null;
            return true;
        }
    }
}
