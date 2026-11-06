using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public class Bird
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; } = 36;
        public int Height { get; } = 28;

        public float VelocityY { get; set; }
        public float Gravity { get; set; } = 0.55f;
        public float JumpStrength { get; set; } = -8.5f;

        public Rectangle Bounds => new Rectangle((int)X, (int)Y, Width, Height);

        public Bird(float startX, float startY)
        {
            Reset(startX, startY);
        }

        public void Jump()
        {
            VelocityY = JumpStrength;
        }

        public void Update()
        {
            VelocityY += Gravity;
            Y += VelocityY;
        }

        public void Reset(float startX, float startY)
        {
            X = startX;
            Y = startY;
            VelocityY = 0;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Kuşun düşüş ve zıplama açısı
            float angle = Math.Clamp(VelocityY * 3.5f, -30f, 60f);

            GraphicsState state = g.Save();
            g.TranslateTransform(X + Width / 2f, Y + Height / 2f);
            g.RotateTransform(angle);

            float halfW = Width / 2f;
            float halfH = Height / 2f;

            // 1. Gövde (Sarı Kuş)
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(250, 208, 44)))
            using (Pen borderPen = new Pen(Color.FromArgb(50, 50, 50), 2))
            {
                g.FillEllipse(bodyBrush, -halfW, -halfH, Width, Height);
                g.DrawEllipse(borderPen, -halfW, -halfH, Width, Height);
            }

            // 2. Kanat
            using (Brush wingBrush = new SolidBrush(Color.FromArgb(255, 235, 130)))
            using (Pen borderPen = new Pen(Color.FromArgb(50, 50, 50), 1.5f))
            {
                g.FillEllipse(wingBrush, -halfW + 4, -2, 16, 12);
                g.DrawEllipse(borderPen, -halfW + 4, -2, 16, 12);
            }

            // 3. Büyük Göz ve Gözbebeği
            using (Brush eyeWhite = new SolidBrush(Color.White))
            using (Brush pupilBlack = new SolidBrush(Color.Black))
            using (Pen eyePen = new Pen(Color.FromArgb(50, 50, 50), 1.5f))
            {
                g.FillEllipse(eyeWhite, 2, -halfH + 3, 14, 14);
                g.DrawEllipse(eyePen, 2, -halfH + 3, 14, 14);

                g.FillEllipse(pupilBlack, 8, -halfH + 6, 6, 7);
                g.FillEllipse(eyeWhite, 10, -halfH + 7, 2, 2);
            }

            // 4. Gaga (Turuncu)
            PointF[] beakPoints = new PointF[]
            {
                new PointF(halfW - 2, -2),
                new PointF(halfW + 10, 3),
                new PointF(halfW - 2, 8)
            };
            using (Brush beakBrush = new SolidBrush(Color.FromArgb(245, 110, 32)))
            using (Pen beakPen = new Pen(Color.FromArgb(50, 50, 50), 1.5f))
            {
                g.FillPolygon(beakBrush, beakPoints);
                g.DrawPolygon(beakPen, beakPoints);
            }

            // 5. Yanak Allığı
            using (Brush blushBrush = new SolidBrush(Color.FromArgb(120, 255, 120, 120)))
            {
                g.FillEllipse(blushBrush, -2, 2, 7, 7);
            }

            g.Restore(state);
        }
    }
}
