using CStringer.Animation;
using CStringer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static CStringer.Utils.DrawTools;

namespace CStringer.Custom
{
    class AnimButton: CustomButtonBase
    {
        enum AnimationState { None, Highlight, Ripple, Fading }

        Color backgroundColor = Color.Orange; //Color.FromArgb(255, 242, 242, 242); 
        Color rippleColor = Color.DarkOrange; //Color.FromArgb(255, 230, 230, 230);
        Color borderColor = Color.FromArgb(255, 217, 217, 217);
        Color highlightColor = ControlPaint.LightLight(Color.Orange);

        StringFormat stringFormat;
        Font font;
        Pen borderPen;
        SolidBrush backgroundBrush;
        SolidBrush circleBrush;

        AnimationState animationState;
        IAnimation<float> animator;
        Point clickPoint;

        public AnimButton()
        {
            animator = new FloatAnimator(0, 1, FloatAnimator.InitWith.FpsDuration, 60, 150);
            animator.AnimTick += AnimTickHandle;
            animator.Started += AnimStartedHandle;
            animator.Completed += AnimCompletedHandle;

            animationState = AnimationState.None;

            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            font = new Font("Arial", 10);
            borderPen = new Pen(borderColor, 2f);
            backgroundBrush = new SolidBrush(backgroundColor);
            circleBrush = new SolidBrush(rippleColor);
            this.DoubleBuffered = true;
        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            this.Text = animationState.ToString();

            Graphics g = pevent.Graphics;
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint background
            Rectangle rect = this.ClientRectangle;
            g.FillRectangle(backgroundBrush, rect);
            

            // Paint animated parts
            if (animationState == AnimationState.Highlight)
            {
                SolidBrush brush = new SolidBrush(highlightColor);
                g.FillRectangle(brush, rect);
            }
            else if (animationState == AnimationState.Ripple)
            {
                Point centerPoint = rect.Center(); /*new Point(rect.Width / 2, rect.Height / 2);*/
                Point currentPoint = clickPoint.Towards(centerPoint, animator.AnimValue); // AnimValue = [0..1]

                int maxRadius = (int) DrawTools.Distance(Point.Empty, centerPoint);

                Ellipse circle = new Ellipse(currentPoint, (int) (maxRadius * animator.AnimValue));
                g.FillEllipseAround(circleBrush, circle);
            }    
            else if (animationState == AnimationState.Fading)
            {
                // TODO
                Color currentColor = DrawTools.ColorBetween(rippleColor, backgroundColor, animator.AnimValue);
                SolidBrush brush = new SolidBrush(currentColor);
                g.FillRectangle(brush, rect);
            }        
                        
            // TODO: Paint border
            g.DrawRectangle(borderPen, rect);
            
            // TODO: Paint string
            SolidBrush textBrush = new SolidBrush(this.ForeColor);
            g.DrawString(this.Text, font, textBrush, rect, stringFormat);
        }
                
        
        private void AnimStartedHandle(object obj, float animValue)
        {
            // TODO: ButtonStateArgs may be useful
        }


        private void AnimTickHandle(object obj, float animValue)
        {
            this.Refresh();
        }

        private void AnimCompletedHandle(object obj, float animValue)
        {
            if (this.animationState == AnimationState.Ripple && this.State != CustomButtonState.Pressed)
            {
                this.animationState = AnimationState.Fading;
                animator.Start();
            }                
            else if (this.animationState == AnimationState.Fading)
            {
                if (this.State == CustomButtonState.Hot)
                    this.animationState = AnimationState.Highlight;
                else
                    this.animationState = AnimationState.None;
            }                

            this.Refresh();
        }

        protected override void OnStateChanged(ButtonStateArgs sevent)
        {
            base.OnStateChanged(sevent);

            /*
             * None -> Ripple - только при нажатии левой кнопкой
             * Ripple -> Fading - сразу при отжатии левой кнопки если аниматор не активен,
             * иначе добавить слушатель переводящий состояние при завершении анимации
             * Fading -> None - как только аниматор закончит работу
             */

            CustomButtonState oldState = sevent.PreviousState;
            CustomButtonState newState = sevent.CurrentState;
            
            if (newState == CustomButtonState.Normal && !animator.IsActive)
            {
                this.animationState = AnimationState.None;
            }
            else if (newState == CustomButtonState.Pressed)
            {
                this.animationState = AnimationState.Ripple;
                animator.Start();
            }
            else if (this.animationState == AnimationState.Ripple && !animator.IsActive)
            {   
                // If mouseup and animvalue is 1, move to fading state
                this.animationState = AnimationState.Fading;
                animator.Start();
            }      

            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs mevent) // TODO: Left button only
        {
            clickPoint = mevent.Location;
            base.OnMouseDown(mevent);                
        }
    }    

    
}
