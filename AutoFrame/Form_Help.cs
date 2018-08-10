using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFrame
{
    public partial class Form_Help : Form
    {
        public Form_Help()
        {
            InitializeComponent();
        }

        private void Form_Help_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
           // radioButton1.Image = new Image();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
        }
    }
}
