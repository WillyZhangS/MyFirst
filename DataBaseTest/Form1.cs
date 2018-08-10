using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataBase;
using System.Threading;
namespace DataBaseTest
{
    public partial class Form1 : Form
    {
        DataBaseMgr mgr;
        public Form1()
        {
            InitializeComponent();
            mgr = new DataBaseMgr();
            mgr.AddDataBase("mysql", new DataBase_Mysql("server=localhost; user id=root; password=12345; port=3306; database=test; charset=utf8"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Action<object> action = (object obj) =>
            {
                //延迟一秒初始化硬件，便于窗口接收初始化异常
                this.BeginInvoke((MethodInvoker)delegate
                {
                    textBox1.Text = "开始延时";
                });
                System.Diagnostics.Debug.WriteLine("开始延时");
                //Task.Delay(TimeSpan.FromSeconds(15));
                Thread.Sleep(1000);
                this.BeginInvoke((MethodInvoker)delegate
                {
                    textBox1.Text = "延时结束";
                });
                System.Diagnostics.Debug.WriteLine("结束延时");
                //如果需要程序一启动就给轴上电，可以在此处添加代码哦
                //todo: tcp, com, vision
            };
            Task t1 = new Task(action, "");
            t1.Start();
            t1.Wait();
        }
    }
}
