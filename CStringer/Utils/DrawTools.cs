using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Utils
{
    static class DrawTools
    {        

        public static float Distance(Point p1, Point p2)
        {
            int a = p2.X - p1.X;
            int b = p2.Y - p1.Y;
            return (float) Math.Sqrt(a * a + b * b);
        }

        public static Color ColorBetween(Color color1, Color color2, float colorPos)
        {
            int deltaA = color2.A - color1.A;
            int deltaR = color2.R - color1.R;
            int deltaG = color2.G - color1.G;
            int deltaB = color2.B - color1.B;

            byte newA = (byte)(color1.A + deltaA * colorPos);
            byte newR = (byte)(color1.R + deltaR * colorPos);
            byte newG = (byte)(color1.G + deltaG * colorPos);
            byte newB = (byte)(color1.B + deltaB * colorPos);

            return Color.FromArgb(newA, newR, newG, newB);
        }

        public static int ToBgr(this Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            int bgr = (b << 16) | (g << 8) | r;
            return bgr;
        }

        public static Color ColorFromBgr(int bgr)
        {
            byte r = (byte) (bgr & 0x0000FF);
            byte g = (byte) ((bgr & 0x00FF00) >> 8);
            byte b = (byte) ((bgr & 0xFF0000) >> 16);

            return Color.FromArgb(r, g, b);
        }

        public static byte[] ToArray(this Color color)
        {
            byte[] array = { color.A, color.R, color.G, color.B };
            return array;
        }

        public static void FillEllipseAround(this Graphics g, Brush ellipseBrush, Rectangle rect, Point center)
        {
            Rectangle newRect = new Rectangle(
                new Point(
                    center.X - rect.Width / 2,
                    center.Y - rect.Height / 2),
                rect.Size
                );

            g.FillEllipse(ellipseBrush, newRect);
        }

        public static void FillEllipseAround(this Graphics g, Brush ellipseBrush, Ellipse ellipse)
        {
            g.FillEllipse(ellipseBrush, ellipse.Rectangle);
        }

        public static Point Towards(this Point currentPoint, Point targetPoint, float delta)
        {
            //Point centerPoint = new Point(rect.Width / 2, rect.Height / 2);

            int deltaX = targetPoint.X - currentPoint.X;
            int deltaY = targetPoint.Y - currentPoint.Y;

            int currentX = currentPoint.X + (int)(delta * deltaX);
            int currentY = currentPoint.Y + (int)(delta * deltaY);

            return new Point(currentX, currentY);
        }

        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Width / 2, rect.Height / 2);
        }

        public class Ellipse
        {
            public Rectangle Rectangle { get; private set; }

            public Ellipse(Point center, int radius)
            {
                int x = center.X - radius;
                int y = center.Y - radius;

                this.Rectangle = new Rectangle(x, y, radius << 1, radius << 1);
            }

            public Ellipse(Rectangle rect)
            {
                this.Rectangle = rect;
            }
        }
    }
}
