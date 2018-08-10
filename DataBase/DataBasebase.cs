using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace DataBase
{
    /// <summary>
    /// 数据库处理步骤基类
    /// </summary>
    public abstract class DataBasebase
    {
        public string m_strConnectDescribe;
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool m_IsOpen;
        /// <summary>
        /// 数据库种类的名称
        /// </summary>
        public string m_strEvidence;
        /// <summary>
        /// 数据接收区
        /// </summary>
        public string[] m_strArrayData = new string[10];
        public DataBasebase(string strName)
        {
            m_strEvidence = strName;
        }
        /// <summary>
        /// 连接打开数据库
        /// </summary>
        /// <returns></returns>
        public abstract bool Open();
        /// <summary>
        /// 数据库是否连接
        /// </summary>
        /// <returns></returns>
        public abstract bool IsOpen();
        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <returns></returns>
        public abstract bool Close();
        /// <summary>
        /// Sql新增命令
        /// </summary>
        /// <returns></returns>
        public abstract bool SqlInsert(string SqlCmd);
        /// <summary>
        /// Sql删除命令
        /// </summary>
        /// <returns></returns>
        public abstract bool SqlDelete(string SqlCmd);
        /// <summary>
        /// Sql更新命令
        /// </summary>
        /// <returns></returns>
        public abstract bool SqlUpdate(string SqlCmd);
        /// <summary>
        /// Sql查询命令
        /// </summary>
        /// <returns></returns>
        public abstract bool SqlSelect(string SqlCmd, out string[] SelectData);
        /// <summary>
        /// 动态创建数组
        /// </summary>
        /// <param name="origArray"></param>
        /// <param name="desiredSize"></param>
        /// <returns></returns>
        public static Array Redim(Array origArray, int desiredSize)
        {
            //determine the type of element
            Type t = origArray.GetType().GetElementType();
            //create a number of elements with a new array of expectations
            //new array type must match the type of the original array
            Array newArray = Array.CreateInstance(t, desiredSize);
            //copy the original elements of the array to the new array
            Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, desiredSize));
            //return new array
            return newArray;
        }
    }
}
