using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFrameDll
{
    public class FormBase : Form
    {
        public FormBase()
        {
            m_nCurSecurity = SECURITY_LEVEL.OPERATOR;
        }
        //public new void Show()
        //{

        //}

        public new void ShowDialog()
        {

        }

        public SECURITY_LEVEL m_nCurSecurity;
    }
}
