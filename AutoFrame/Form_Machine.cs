using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonTool;
using System.IO;

namespace AutoFrame
{
    public partial class Form_Machine : Form
    {
        public Form_Machine()
        {
            InitializeComponent();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        //    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
        //    {
        //        if (i % 2 == 0)
        //        {
        //            this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Gray;
        ////            this.dataGridViewBase.Rows[i].DefaultCellStyle.Font = this.splitContainer1.Font;
        //        }
        //        else
        //        {
        //            this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
        ////            this.dataGridView1.Rows[i].DefaultCellStyle.Font = this.splitContainer1.Font;
        //        }
        //    }
        }

        private void Form_Machine_Load(object sender, EventArgs e)
        {

            dataGridView1.Rows.Add(3);
            dataGridView1.Rows[0].Cells[0].Value = "M400";
            dataGridView1.Rows[0].Cells[1].Value = "OPT-RING 32mm";
            dataGridView1.Rows[0].Cells[2].Value = "Cognex M34X3";
            dataGridView1.Rows[0].Cells[3].Value = "Cognex - CCD TU833";
            dataGridView1.Rows[1].Cells[1].Value = "OPT-RING 32mm";
            dataGridView1.Rows[1].Cells[3].Value = "Cognex - CCD TU833";

            dataGridView1.Rows[2].Cells[1].Value = "OPT-RING 32mm";
            dataGridView1.Rows[2].Cells[3].Value = "Cognex - CCD TU833";


            label_build_date.Text = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToShortDateString();
            label_version.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            FileInfo fi = new FileInfo(SystemMgr.GetInstance().m_strSystemParamName);
            if (fi.Exists)
            {
                label_last_date.Text =  fi.LastWriteTime.ToShortDateString();
            }

            int nMachineTime = 0;
            int nSoftwareTime = 0;
            AutoTool.ReadLifeTime(out nMachineTime, out nSoftwareTime);

            TimeSpan ts = new TimeSpan(0, 0, nMachineTime);
            label_machine_life.Text = string.Format("{0}天{1}小时{2}分{3}秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);

            ts = new TimeSpan(0, 0, nSoftwareTime);
            label_software_life.Text = string.Format("{0}天{1}小时{2}分{3}秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);


        }
    }
}

