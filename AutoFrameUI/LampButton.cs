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
    /// 
    /// </summary>
    public partial class LampButton : Button
    {
        ControlState _state = ControlState.Normal;
        /// <summary>
        /// 
        /// </summary>
        public LampButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ControlState
        {
            /// <summary>
            /// 
            /// </summary>
            Normal,
            /// <summary>
            /// 
            /// </summary>
            Hover,
            /// <summary>
            /// 
            /// </summary>
            Pressed,
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _state = ControlState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            _state = ControlState.Hover;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _state = ControlState.Pressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = ControlState.Hover;
        }
        private void CalculateRect(
        out Rectangle imageRect, out Rectangle textRect, Graphics g)
        {
            if(Image != null)
            {
                imageRect = new Rectangle(0, (ClientRectangle.Height - Image.Size.Height) / 2,
                    Image.Size.Width, Image.Size.Height);
                textRect = new Rectangle(Image.Size.Width, 0,
                    ClientRectangle.Width - Image.Width, ClientRectangle.Height);

            }
            else
            {
                imageRect = new Rectangle(0, 0, 0, 0);
                textRect = ClientRectangle;
            }
        
        }
        
        /// <summary>
        /// 画边框与背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="style"></param>
        /// <param name="roundWidth"></param>
        internal void RenderBackGroundInternal(Graphics g, Rectangle rect, RoundStyle style, int roundWidth)
        {
            //       if (ControlState != ControlState.Normal || AlwaysShowBorder)
            //       {
      //      rect.Width--;
     //       rect.Height--;
          
       
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
         //         base.OnPaint(e);
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            Rectangle imageRect;
            Rectangle textRect;

            CalculateRect(out imageRect, out textRect, g);
            g.SmoothingMode = SmoothingMode.HighQuality;


            //  画边框与背景
            //RenderBackGroundInternal(
            //g,
            //    ClientRectangle,
            //    RoundStyle,
            //    Radius
            //            );
            //   画图像
            if(_state == ControlState.Normal)
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else if(_state == ControlState.Hover)
            {
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.LightGray))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
            }
            else if(_state == ControlState.Pressed)
            {
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Gainsboro))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
            }

            if (Image != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImageUnscaled(Image, imageRect.Left, imageRect.Top);
                //g.DrawImage(
                //    Image,
                //    imageRect,
                //    0,
                //    0,
                //    Image.Width,
                //    Image.Height,
                //    GraphicsUnit.Pixel);
            }
            //画文字      
            if (Text != "")
            {
            //    TextFormatFlags flags = base.TextAlign;
                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                   textRect,
                    //        this.ClientRectangle,
                    ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                //StringFormat sf = new StringFormat();
                //sf.Alignment = StringAlignment.Center;
                //sf.LineAlignment = StringAlignment.Center;
                //g.DrawString(Text, Font, fontBrush, textRect,  sf);、
            }

        }
    }
}

