using CStringer.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CStringer.Custom
{
    class GradientButton : CustomButtonBase
    {
        public Color color1 = Color.FromArgb(255, 63, 90, 120);
        public Color color2 = Color.FromArgb(255, 40, 48, 61);
        public Color colorBorder = Color.FromArgb(255, 122, 138, 153);

        private Font font;
        private StringFormat stringFormat;
        private Pen borderPen;
        private float percentage = 0.5f;


        public GradientButton()
        {
            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            font = new Font("Arial", 10);
            borderPen = new Pen(colorBorder, 2f);
        }

        protected override void OnStateChanged(ButtonStateArgs e)
        {
            base.OnStateChanged(e);

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Graphics g = pevent.Graphics;
            Rectangle rect = this.ClientRectangle;

            LinearGradientBrush brush = null;
            
            // Choosing brush for whole button
            switch (State)
            {
                case CustomButtonState.Normal:
                    {
                        brush = new LinearGradientBrush(
                            rect,
                            color1,
                            color2,
                            LinearGradientMode.Vertical);

                        borderPen.Width = 2f;

                        break;
                    }
                case CustomButtonState.Hot:
                    {
                        brush = new LinearGradientBrush(
                            rect,
                            color1,
                            color2,
                            LinearGradientMode.Vertical);

                        borderPen.Width = 2f;

                        break;
                    }
                case CustomButtonState.Pressed:
                    {
                        brush = new LinearGradientBrush(
                            rect,
                            ControlPaint.Light(color1, percentage),
                            ControlPaint.Light(color2, percentage),
                            LinearGradientMode.Vertical);

                        borderPen.Width = 3f;

                        break;
                    }
                case CustomButtonState.Focused:
                    {
                        brush = new LinearGradientBrush(
                            rect,
                            color1,
                            color2,
                            LinearGradientMode.Vertical);

                        borderPen.Width = 2f;

                        break;
                    }
                default:
                    {
                        brush = new LinearGradientBrush(
                            rect,
                            color1,
                            color2,
                            LinearGradientMode.Vertical);

                        borderPen.Width = 2f;

                        break;
                    }
            }

            g.FillRectangle(brush, rect);


            // Drawing border
            g.DrawRectangle(borderPen, rect);            


            // Drawing text on the button
            SolidBrush textBrush = new SolidBrush(this.ForeColor);
            this.Text = this.State.ToString(); // TODO: Remove
            g.DrawString(this.Text, font, textBrush, rect, stringFormat);
        }
    }
}
