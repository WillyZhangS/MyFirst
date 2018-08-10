using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    class 项目使用说明
    {
        //此项目包含mysql和Oracle的数据库封装，使用简介如下文：
        //    表：stdtable
        //    字段：ID(主键),Name,Score,

        //准备:
        //1. 首先添加数据库将数据库名称和数据库进行绑定。
        //    例：  //DataBaseMgr.GetInstance().AddDataBase("数据库自定义名称",new DataBase_Mysql("数据库连接凭据"));
                    //DataBaseMgr.GetInstance().AddDataBase("mysql", new DataBase_Mysql("server=localhost; user id=root; password=12345; port=3306; database=test; charset=utf8"));
                    //DataBaseMgr.GetInstance().AddDataBase("mysql2", new DataBase_Mysql("server=localhost; user id=root; password=12345; port=3306; database=test; charset=utf8"));
        //使用:
        //1. 定义Sql命令,执行命令。
        //例：使用增删改命令
        //增命令
        //string sql = "insert into stdtable(ID,Name, Score) values(1, 'zs',90)";
        //if (DataBaseMgr.GetInstance().ProcessInstDelUpdate("mysql", sql, true))
        //删命令
        //    string sql = "delete from Name where ID = '3'";
        //if (DataBaseMgr.GetInstance().ProcessInstDelUpdate("mysql", sql, false,true,false))
        //改命令
        //    string sql = "update stdtable set Name='wangwu', Score='60' where ID='3'";
        //if (DataBaseMgr.GetInstance().ProcessInstDelUpdate("mysql", sql, false,false,true))
        //查命令
        //string sql = "select Name from stdtable where Score=60";
        //string[] data = new string[1];//数组大小任意定义，输出时可自动调整其大小。
        //if (DataBaseMgr.GetInstance().ProcessSelect("mysql", sql, ref data))
    }
}
