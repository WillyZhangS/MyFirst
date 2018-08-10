using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AutoFrameUI
{
    /// <summary>
    /// 半圆环良率显示控件
    /// </summary>
    public partial class HalfRing : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HalfRing()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public HalfRing(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        // 圆
        // ===============================================================================================
        private int _CircleRadius = 25;
        /// <summary>
        /// 中心圆半径
        /// </summary>
        [Browsable(true)]
        [Description("中心圆半径")]
        public int setCircleRadius
        {
            get
            {
                return _CircleRadius;
            }
            set
            {
                if (value < 0) { _CircleRadius = 0; }
                else { _CircleRadius = value; }
                base.Refresh();
            }
        }

        private int _InRadius = 70;
        /// <summary>
        /// 内圆环半径
        /// </summary>
        [Browsable(true)]
        [Description("内圆环半径")]
        public int setInRadius
        {
            get
            {
                return _InRadius;
            }
            set
            {
                if (value < 0) { _InRadius = 0; }
                else { _InRadius = value; }
                base.Refresh();
            }
        }

        private int _OutRadius = 100;
        /// <summary>
        /// 外圆环半径
        /// </summary>
        [Browsable(true)]
        [Description("外圆环半径")]
        public int setOutRadius
        {
            get
            {
                return _OutRadius;
            }
            set
            {
                if (value < 0) { _OutRadius = 0; }
                else { _OutRadius = value; }
                base.Refresh();
            }
        }
        private int _Length = 10;
        /// <summary>
        /// 圆环的宽度
        /// </summary>
        [Browsable(true)]
        [Description("圆环的宽度")]
        public int setLength
        {
            get
            {
                return _Length;
            }
            set
            {
                if (value < 0) { _Length = 0; }
                else { _Length = value; }
                base.Refresh();
            }
        }

        private Color _ColorGreen = Color.FromArgb(107, 187, 63);
        /// <summary>
        /// ok的颜色
        /// </summary>
        [Browsable(true)]
        [Description("表示OK部分的颜色")]
        public Color setColorGreen
        {
            get
            {
                return _ColorGreen;
            }
            set
            {
                _ColorGreen = value;
                base.Refresh();
            }
        }

        private Color _ColorRed = Color.FromArgb(200, 37, 6);
        /// <summary>
        /// Fail部分的颜色
        /// </summary>
        [Browsable(true)]
        [Description("表示FAIL部分的颜色")]
        public Color setColorRed
        {
            get
            {
                return _ColorRed;
            }
            set
            {
                _ColorRed = value;
                base.Refresh();
            }
        }
        private float _RateOut = 0.5F;
        /// <summary>
        /// 外环部分的比率
        /// </summary>
        [Browsable(true)]
        [Description("外环部分的比率")]
        public float setRateOut
        {
            get
            {
                return _RateOut;
            }
            set
            {
                _RateOut = value;
                base.Refresh();
            }
        }
        private float _RateIn = 0.5F;
        /// <summary>
        /// 内环部分的比率
        /// </summary>
        [Browsable(true)]
        [Description("内环部分的比率")]
        public float setRateIn
        {
            get
            {
                return _RateIn;
            }
            set
            {
                _RateIn = value;
                base.Refresh();
            }
        }
        private bool _bResult = false;
        /// <summary>
        /// 表示当前的结果
        /// </summary>
        [Browsable(true)]
        [Description("表示当前的结果")]
        public bool setResult
        {
            get
            {
                return _bResult;
            }
            set
            {
                _bResult = value;
                base.Refresh();
            }
        }

        private Font _ResultFont = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
        /// <summary>
        /// 显示结果的字体
        /// </summary>
        [Browsable(true)]
        [Description("显示结果的字体")]
        public Font setResultFont
        {
            get
            {
                return _ResultFont;
            }
            set
            {
                _ResultFont = value;
                base.Refresh();
            }
        }

        private string _strOut = ("外环");
        /// <summary>
        /// 外环的注解
        /// </summary>
        [Browsable(true)]
        [Description("外环的注解")]
        public string setScriptOut
        {
            get
            {
                return _strOut;
            }
            set
            {
                _strOut = value;
                base.Refresh();
            }
        }

        private string _strIn = ("内环");
        /// <summary>
        /// 内环的注解
        /// </summary>
        [Browsable(true)]
        [Description("内环的注解")]
        public string setScriptIn
        {
            get
            {
                return _strIn;
            }
            set
            {
                _strIn = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 绘图，重绘响应
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            base.OnPaint(pe);
            //       Round(this.Region);  // 圆角
            int y = this.ClientRectangle.Height /2;
            int x = this.ClientRectangle.Width / 2;

            pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //       SolidBrush RingBrush = new SolidBrush(Color.FromArgb(0XAE, 0XDA, 0X97));
            SolidBrush GreenBrush = new SolidBrush(_ColorGreen);
            SolidBrush RedBrush = new SolidBrush(_ColorRed);
            SolidBrush BackBrush = new SolidBrush(this.BackColor);


            Rectangle rect = new Rectangle(x - _OutRadius, y - _OutRadius, _OutRadius * 2, _OutRadius * 2);
            pe.Graphics.FillPie(GreenBrush, rect, 0, -180 * _RateOut);
            pe.Graphics.FillPie(RedBrush, rect, -180 * _RateOut, -180 * (1 - _RateOut));

            string str = string.Format("{0:00.0}%", _RateOut * 100);

            TextRenderer.DrawText(
                      pe.Graphics,
                     str,
                     Font,
                     new Rectangle(x - _OutRadius, y - _Length, _Length, _Length),
                     //        this.ClientRectangle,
                     ForeColor,
                     TextFormatFlags.Right | TextFormatFlags.Bottom
                     );


            rect.X += _Length;
            rect.Y += _Length;
            rect.Width -= _Length * 2;
            rect.Height -= _Length * 2;
            pe.Graphics.FillPie(BackBrush, rect, 1, -182);

            rect.X = x - _InRadius;
            rect.Y = y - _InRadius;
            rect.Width = _InRadius * 2;
            rect.Height = _InRadius * 2;
            pe.Graphics.FillPie(GreenBrush, rect, 0, -180 * _RateIn);
            pe.Graphics.FillPie(RedBrush, rect, -180 * _RateIn, -180 * (1 - _RateIn));

            str = string.Format("{0:00.0}%", _RateIn * 100);
            TextRenderer.DrawText(
                      pe.Graphics,
                     str,
                     Font,
                     new Rectangle(x - _InRadius, y - _Length, _Length, _Length),
                     //        this.ClientRectangle,
                     ForeColor,
                     TextFormatFlags.Right | TextFormatFlags.Bottom
                     );

            rect.X += _Length;
            rect.Y += _Length;
            rect.Width -= _Length * 2;
            rect.Height -= _Length * 2;
            pe.Graphics.FillPie(BackBrush, rect, 1, -182);


            rect.X = x - _CircleRadius;
            rect.Y = y - _CircleRadius;
            rect.Width = _CircleRadius * 2;
            rect.Height = _CircleRadius * 2;

            if (_bResult)
            {
                pe.Graphics.FillEllipse(GreenBrush, rect);
                TextRenderer.DrawText(
                         pe.Graphics,
                        "OK",
                        _ResultFont,
                        rect,
                        //        this.ClientRectangle,
                        ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                        );
            }
            else
            {
                pe.Graphics.FillEllipse(RedBrush, rect);
                TextRenderer.DrawText(
                         pe.Graphics,
                        "NG",
                        _ResultFont,
                        rect,
                        ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                        );
            }
            DrawStringAndRotateAt(pe.Graphics, x, y, _OutRadius, _strOut);
            DrawStringAndRotateAt(pe.Graphics, x, y, _InRadius, _strIn);

        }

        /// <summary>
        /// 控件大小变更事件响应
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            base.Refresh();
        }


        private void DrawStringAndRotateAt(Graphics g, int xCenter, int yCenter, int nRadius, string strText)
        {
            //以(100，100)为中心画字符串并旋转   
            SizeF size = g.MeasureString(strText, Font);
            PointF rotatePoint = new PointF(xCenter - nRadius+_Length/2 - size.Height/2 , yCenter + size.Width ); //设定旋转的中心点  
          
            Matrix myMatrix = new Matrix();
            myMatrix.RotateAt(90, rotatePoint, MatrixOrder.Append); //旋转270度  
            g.Transform = myMatrix;
            g.DrawString(strText, Font, new SolidBrush(Color.Black), rotatePoint.X - size.Width, rotatePoint.Y - size.Height); //写字  
        }


    }
}
