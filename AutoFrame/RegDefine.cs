using System;
namespace AutoFrame
{
    /// <summary>
    /// 
    /// </summary>
    public enum SysBitReg
    {
        条码更新,
        扫码上料完成,
        标刻完成,
        运行状态,
        启动,
        扫码数据清除,
        是否扫码,
        屏蔽光栅,
        是否RST,
    }

    /// <summary>
    /// 系统整型寄存器索引枚举声明
    /// </summary>
    public enum SysIntReg 
    {
         /// <summary>
         /// 站位运行进度百分比，用于界面显示进度条      
         /// </summary>
        进度百分比,
        工件类型=101,
        控件使能,
        是否扫码,//如果是1则已经被勾选，如果为0则未勾选。
        是否打两次标,
    }
    public struct strSn
    {
        static public string ScanCode_Data;
        static public string[] NetData;
        /// <summary>
        /// 待标刻的二维码数据;      
        /// </summary>
        static public string Code_1;
        static public string Code_2;
        static public string Code_3;
        static public string Code_4;
        static public string Code_5;
        static public string Code_6;
        static public string Code_7;
        static public string Code_8;
        static public string Code_others0;
        static public string Code_others1;
        static public string Code_others2;

    }
}