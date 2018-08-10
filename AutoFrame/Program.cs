using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoFrameDll;

namespace AutoFrame
{


    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (AutoTool.ConfigAll())
            {
                AutoTool.InitSystem();
                Application.Run(new Form_Main());

                AutoTool.DeinitSystem();
            }
                        
        }
    }
}
