using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YMH219_FlappyBird;

public partial class Form1 : Form
{
    private Bird bird;
    private List<Pipe> pipes;
    private GameManager gameManager;
    private System.Windows.Forms.Timer gameLoopTimer;

    
    private const int ScreenW = 650;
    private const int ScreenH = 750;
    private const int GroundH = 110;
    private int GroundY => ScreenH - GroundH;

    
    private float cloudX1 = 50;
    private float cloudX2 = 320;
    private float cloudX3 = 540;

    public Form1()
    {
        InitializeComponent();
        this.Controls.Clear(); 

        
        this.Text = "YMH219 - Flappy Bird";
        this.ClientSize = new Size(ScreenW, ScreenH);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.DoubleBuffered = true; 
        this.KeyPreview = true;

        
        gameManager = new GameManager();
        bird = new Bird(120, ScreenH / 2 - 50);

        
        pipes = new List<Pipe>
        {
            new Pipe(ScreenW + 50, ScreenH, GroundH),
            new Pipe(ScreenW + 50 + 360, ScreenH, GroundH)
        };

        
        gameLoopTimer = new System.Windows.Forms.Timer();
        gameLoopTimer.Interval = 20; // 50 FPS
        gameLoopTimer.Tick += GameLoopTimer_Tick;
        gameLoopTimer.Start();

        
        this.KeyDown += Form1_KeyDown;
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            if (gameManager.State == GameState.WaitingToStart)
            {
                gameManager.StartGame();
                bird.Jump();
            }
            else if (gameManager.State == GameState.Playing)
            {
                bird.Jump();
            }
        }
        else if (e.KeyCode == Keys.Enter)
        {
            if (gameManager.State == GameState.GameOver)
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        bird.Reset(120, ScreenH / 2 - 50);

        pipes[0].Reset(ScreenW + 50, ScreenH, GroundH);
        pipes[1].Reset(ScreenW + 50 + 360, ScreenH, GroundH);

        gameManager.StartGame();
        this.Invalidate();
    }

    private void GameLoopTimer_Tick(object? sender, EventArgs e)
    {
        
        cloudX1 -= 0.5f;
        cloudX2 -= 0.5f;
        cloudX3 -= 0.5f;
        if (cloudX1 < -180) cloudX1 = ScreenW + 80;
        if (cloudX2 < -180) cloudX2 = ScreenW + 80;
        if (cloudX3 < -180) cloudX3 = ScreenW + 80;

        if (gameManager.State == GameState.Playing)
        {
            
            bird.Update();

            
            if (bird.Y + bird.Height >= GroundY)
            {
                bird.Y = GroundY - bird.Height;
                gameManager.EndGame();
            }
            else if (bird.Y < 0)
            {
                bird.Y = 0;
                bird.VelocityY = 0;
            }

            
            foreach (var pipe in pipes)
            {
                pipe.Update();

                if (pipe.CollidesWith(bird.Bounds, GroundY))
                {
                    gameManager.EndGame();
                    break;
                }

                
                if (!pipe.Passed && (bird.X > pipe.X + pipe.Width))
                {
                    pipe.Passed = true;
                    gameManager.AddScore();
                }

                
                if (pipe.X + pipe.Width < 0)
                {
                    float maxPipeX = 0;
                    foreach (var otherPipe in pipes)
                    {
                        if (otherPipe.X > maxPipeX) maxPipeX = otherPipe.X;
                    }
                    pipe.Reset(maxPipeX + 360, ScreenH, GroundH);
                }
            }
        }

        
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        
        g.Clear(Color.FromArgb(112, 197, 206));

        
        DrawCloud(g, cloudX1, 90, 1.3f);
        DrawCloud(g, cloudX2, 160, 1.0f);
        DrawCloud(g, cloudX3, 70, 0.9f);

        
        foreach (var pipe in pipes)
        {
            pipe.Draw(g, GroundY);
        }

        
        DrawGround(g);

       
        bird.Draw(g);

        
        DrawScore(g);

        
        if (gameManager.State == GameState.WaitingToStart)
        {
            DrawWaitingScreen(g);
        }
        else if (gameManager.State == GameState.GameOver)
        {
            DrawGameOverScreen(g);
        }
    }

    private void DrawCloud(Graphics g, float x, float y, float scale)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using (Brush cloudBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
        {
            g.FillEllipse(cloudBrush, x, y, 70 * scale, 45 * scale);
            g.FillEllipse(cloudBrush, x + 25 * scale, y - 20 * scale, 60 * scale, 55 * scale);
            g.FillEllipse(cloudBrush, x + 55 * scale, y - 5 * scale, 65 * scale, 45 * scale);
            g.FillEllipse(cloudBrush, x + 15 * scale, y + 10 * scale, 85 * scale, 35 * scale);
        }
    }

    private void DrawGround(Graphics g)
    {
        int grassH = 18;
        
        using (Brush grassBrush = new SolidBrush(Color.FromArgb(115, 191, 46)))
        using (Pen grassBorder = new Pen(Color.FromArgb(80, 150, 30), 2))
        {
            g.FillRectangle(grassBrush, 0, GroundY, ScreenW, grassH);
            g.DrawLine(grassBorder, 0, GroundY, ScreenW, GroundY);
        }

        
        using (Brush grassHighlight = new SolidBrush(Color.FromArgb(158, 222, 60)))
        {
            g.FillRectangle(grassHighlight, 0, GroundY, ScreenW, 5);
        }

       
        using (Brush dirtBrush = new SolidBrush(Color.FromArgb(222, 216, 149)))
        using (Pen dirtLinePen = new Pen(Color.FromArgb(200, 190, 130), 1))
        {
            g.FillRectangle(dirtBrush, 0, GroundY + grassH, ScreenW, GroundH - grassH);

            for (int x = -20; x < ScreenW + 30; x += 30)
            {
                g.DrawLine(dirtLinePen, x, GroundY + grassH, x + 25, ScreenH);
            }
        }
    }

    private void DrawScore(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (Font scoreFont = new Font("Segoe UI", 20, FontStyle.Bold))
        using (Font highFont = new Font("Segoe UI", 12, FontStyle.Bold))
        using (Brush textBrush = new SolidBrush(Color.White))
        using (Brush highBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
        using (Pen borderPen = new Pen(Color.FromArgb(40, 40, 40), 3))
        {
            string scoreStr = $"Skor: {gameManager.Score}";
            string highStr = $"En Yüksek: {gameManager.HighScore}";

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(scoreStr, scoreFont.FontFamily, (int)scoreFont.Style, 28, new Point(20, 15), StringFormat.GenericDefault);
                g.DrawPath(borderPen, path);
                g.FillPath(textBrush, path);
            }

            using (GraphicsPath pathHigh = new GraphicsPath())
            {
                pathHigh.AddString(highStr, highFont.FontFamily, (int)highFont.Style, 18, new Point(21, 52), StringFormat.GenericDefault);
                using (Pen p2 = new Pen(Color.FromArgb(50, 50, 50), 2))
                {
                    g.DrawPath(p2, pathHigh);
                }
                g.FillPath(highBrush, pathHigh);
            }
        }
    }

    private void DrawWaitingScreen(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (Font titleFont = new Font("Segoe UI", 24, FontStyle.Bold))
        using (Font subFont = new Font("Segoe UI", 14, FontStyle.Bold))
        using (Brush bgBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 0)))
        using (Brush titleBrush = new SolidBrush(Color.FromArgb(255, 230, 80)))
        using (Brush subBrush = new SolidBrush(Color.White))
        {
            int boxW = 380;
            int boxH = 140;
            Rectangle box = new Rectangle((ScreenW - boxW) / 2, 240, boxW, boxH);
            g.FillRoundedRectangle(bgBrush, box, 15);

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("FLAPPY BIRD", titleFont, titleBrush, new RectangleF(box.X, box.Y + 20, box.Width, 40), sf);
            g.DrawString("Başlamak için SPACE'e basın", subFont, subBrush, new RectangleF(box.X, box.Y + 75, box.Width, 35), sf);
        }
    }

    private void DrawGameOverScreen(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        
        int boxW = 420;
        int boxH = 240;
        Rectangle box = new Rectangle((ScreenW - boxW) / 2, 200, boxW, boxH);

        using (Brush boxBrush = new SolidBrush(Color.FromArgb(210, 25, 25, 25)))
        using (Pen borderPen = new Pen(Color.FromArgb(250, 208, 44), 3))
        {
            g.FillRoundedRectangle(boxBrush, box, 16);
            g.DrawRoundedRectangle(borderPen, box, 16);
        }

        using (Font overFont = new Font("Segoe UI", 22, FontStyle.Bold))
        using (Font scoreFont = new Font("Segoe UI", 17, FontStyle.Bold))
        using (Font infoFont = new Font("Segoe UI", 13, FontStyle.Bold))
        using (Brush redBrush = new SolidBrush(Color.FromArgb(255, 80, 80)))
        using (Brush whiteBrush = new SolidBrush(Color.White))
        using (Brush restartBrush = new SolidBrush(Color.FromArgb(120, 255, 120)))
        {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            
            g.DrawString("OYUNU KAYBETTİNİZ!", overFont, redBrush, new RectangleF(box.X, box.Y + 20, box.Width, 35), sf);

            
            g.DrawString($"Skor: {gameManager.Score}", scoreFont, whiteBrush, new RectangleF(box.X, box.Y + 70, box.Width, 30), sf);

            
            using (Brush goldBrush = new SolidBrush(Color.Gold))
            {
                g.DrawString($"En Yüksek Skor: {gameManager.HighScore}", infoFont, goldBrush, new RectangleF(box.X, box.Y + 110, box.Width, 25), sf);
            }

            
            g.DrawString("Yeniden başlamak için ENTER'a basın", infoFont, restartBrush, new RectangleF(box.X, box.Y + 165, box.Width, 30), sf);
        }
    }
}

public static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int cornerRadius)
    {
        using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
        {
            g.FillPath(brush, path);
        }
    }

    public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle bounds, int cornerRadius)
    {
        using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
        {
            g.DrawPath(pen, path);
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        Size size = new Size(diameter, diameter);
        Rectangle arc = new Rectangle(bounds.Location, size);
        GraphicsPath path = new GraphicsPath();

        if (radius == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}