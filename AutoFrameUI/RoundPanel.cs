using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFrameUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RoundPanel : Panel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoundPanel()
        {
            InitializeComponent();


            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        }

        // 圆角
        // ===============================================================================================
        private int _Radius = 8;  // 圆角弧度

        /// <summary>圆角弧度(0为不要圆角)</summary>
        [Browsable(true)]
        [Description("圆角弧度(0为不要圆角)")]
        public int _setRoundRadius
        {
            get
            {
                return _Radius;
            }
            set
            {
                if (value < 0) { _Radius = 0; }
                else { _Radius = value; }
                base.Refresh();
            }
        }

        /// <summary>
        /// 圆角代码
        /// </summary>
        /// <param name="region"></param>
        public void Round(System.Drawing.Region region)
        {
            // -----------------------------------------------------------------------------------------------
            // 已经是.net提供给我们的最容易的改窗体的属性了(以前要自己调API)
            System.Drawing.Drawing2D.GraphicsPath oPath = new System.Drawing.Drawing2D.GraphicsPath();
            int x = 0;
            int y = 0;
            int thisWidth = this.Width;
            int thisHeight = this.Height;
            int angle = _Radius;
            if (angle > 0)
            {
                System.Drawing.Graphics g = CreateGraphics();
                oPath.AddArc(x, y, angle, angle, 180, 90);                                 // 左上角
                oPath.AddArc(thisWidth - angle, y, angle, angle, 270, 90);                 // 右上角
                oPath.AddArc(thisWidth - angle, thisHeight - angle, angle, angle, 0, 90);  // 右下角
                oPath.AddArc(x, thisHeight - angle, angle, angle, 90, 90);                 // 左下角
                oPath.CloseAllFigures();
                Region = new System.Drawing.Region(oPath);
            }
            // -----------------------------------------------------------------------------------------------
            else
            {
                oPath.AddLine(x + angle, y, thisWidth - angle, y);                         // 顶端
                oPath.AddLine(thisWidth, y + angle, thisWidth, thisHeight - angle);        // 右边
                oPath.AddLine(thisWidth - angle, thisHeight, x + angle, thisHeight);       // 底边
                oPath.AddLine(x, y + angle, x, thisHeight - angle);                        // 左边
                oPath.CloseAllFigures();
                Region = new System.Drawing.Region(oPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public RoundPanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Round(this.Region);  // 圆角
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            base.Refresh();
        }

    }
}
