using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public class Pipe
    {
        public float X { get; set; }
        public int Width { get; } = 64;
        public int TopHeight { get; set; }
        public int Gap { get; set; } 
        public float Speed { get; set; } = 4f;
        public bool Passed { get; set; } = false;

        private static readonly Random rng = new Random();
        private const int CapHeight = 24;
        private const int CapExtraWidth = 6;

        public Rectangle TopBounds => new Rectangle((int)X, 0, Width, TopHeight);

        public Rectangle BottomBounds(int groundY)
        {
            int bottomY = TopHeight + Gap;
            int bottomHeight = Math.Max(0, groundY - bottomY);
            return new Rectangle((int)X, bottomY, Width, bottomHeight);
        }

        public Pipe(float startX, int screenHeight, int groundHeight)
        {
            Reset(startX, screenHeight, groundHeight);
        }

        public void Reset(float newX, int screenHeight, int groundHeight)
        {
            X = newX;
            Passed = false;

            
            Gap = rng.Next(135, 180);

            
            int availableHeight = screenHeight - groundHeight - Gap;
            int minTopHeight = 60;
            int maxTopHeight = Math.Max(minTopHeight + 10, availableHeight - 50);

            TopHeight = rng.Next(minTopHeight, maxTopHeight);
        }

        public void Update()
        {
            X -= Speed;
        }

        public bool CollidesWith(Rectangle playerBounds, int groundY)
        {
            if (playerBounds.IntersectsWith(TopBounds)) return true;
            if (playerBounds.IntersectsWith(BottomBounds(groundY))) return true;
            return false;
        }

        public void Draw(Graphics g, int groundY)
        {
            g.SmoothingMode = SmoothingMode.None;

            Color pipeGreen = Color.FromArgb(115, 191, 46);
            Color pipeHighlight = Color.FromArgb(158, 222, 60);
            Color pipeShadow = Color.FromArgb(88, 148, 35);
            Color borderColor = Color.FromArgb(40, 60, 20);

            using (Brush bodyBrush = new SolidBrush(pipeGreen))
            using (Brush hiBrush = new SolidBrush(pipeHighlight))
            using (Brush shBrush = new SolidBrush(pipeShadow))
            using (Pen borderPen = new Pen(borderColor, 2))
            {
                
                if (TopHeight > 0)
                {
                    int bodyTop = 0;
                    int bodyH = Math.Max(0, TopHeight - CapHeight);
                    if (bodyH > 0)
                    {
                        Rectangle bodyRect = new Rectangle((int)X, bodyTop, Width, bodyH);
                        g.FillRectangle(bodyBrush, bodyRect);
                        g.FillRectangle(hiBrush, (int)X + 4, bodyTop, 6, bodyH);
                        g.FillRectangle(shBrush, (int)X + Width - 10, bodyTop, 8, bodyH);
                        g.DrawRectangle(borderPen, bodyRect);
                    }

                    Rectangle capRect = new Rectangle((int)X - CapExtraWidth / 2, TopHeight - CapHeight, Width + CapExtraWidth, CapHeight);
                    g.FillRectangle(bodyBrush, capRect);
                    g.FillRectangle(hiBrush, capRect.X + 4, capRect.Y, 7, capRect.Height);
                    g.FillRectangle(shBrush, capRect.Right - 12, capRect.Y, 10, capRect.Height);
                    g.DrawRectangle(borderPen, capRect);
                }

                
                int bottomY = TopHeight + Gap;
                int bottomH = Math.Max(0, groundY - bottomY);

                if (bottomH > 0)
                {
                    Rectangle capRect = new Rectangle((int)X - CapExtraWidth / 2, bottomY, Width + CapExtraWidth, CapHeight);
                    g.FillRectangle(bodyBrush, capRect);
                    g.FillRectangle(hiBrush, capRect.X + 4, capRect.Y, 7, capRect.Height);
                    g.FillRectangle(shBrush, capRect.Right - 12, capRect.Y, 10, capRect.Height);
                    g.DrawRectangle(borderPen, capRect);

                    int bodyY = bottomY + CapHeight;
                    int bodyH = Math.Max(0, bottomH - CapHeight);
                    if (bodyH > 0)
                    {
                        Rectangle bodyRect = new Rectangle((int)X, bodyY, Width, bodyH);
                        g.FillRectangle(bodyBrush, bodyRect);
                        g.FillRectangle(hiBrush, (int)X + 4, bodyY, 6, bodyH);
                        g.FillRectangle(shBrush, (int)X + Width - 10, bodyY, 8, bodyH);
                        g.DrawRectangle(borderPen, bodyRect);
                    }
                }
            }
        }
    }
}