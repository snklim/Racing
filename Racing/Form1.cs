using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Racing
{
    public partial class Form1 : Form
    {
        private Timer m_MainTimer;
        private GameField m_GameField;
        private Bitmap m_ShadowCanvas;

        public Form1()
        {
            m_GameField = new GameField();
            
            InitializeComponent();

            InitGameFieldEnviroment();

            m_MainTimer = new Timer();
            m_MainTimer.Interval = 10;
            m_MainTimer.Tick += m_MainTimer_Tick;
            m_MainTimer.Start();
        }

        void m_MainTimer_Tick(object sender, EventArgs e)
        {
            m_GameField.Tick();
            DrawGameField(Graphics.FromImage(m_ShadowCanvas));
        }

        int m_CellSize, m_DeltaX, m_DeltaY, m_CanvasWidth, m_CanvasHeight;
        void InitGameFieldEnviroment()
        {
            m_CanvasWidth = pictureBox1.Width;
            m_CanvasHeight = pictureBox1.Height;

            m_CellSize = m_CanvasWidth / m_GameField.Width;
            m_CellSize = Math.Min(m_CellSize, m_CanvasHeight / m_GameField.Height);

            m_DeltaX = (m_CanvasWidth - m_CellSize * m_GameField.Width) / 2;
            m_DeltaY = (m_CanvasHeight - m_CellSize * m_GameField.Height) / 2;

            m_ShadowCanvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        void DrawGameField(Graphics g)
        {
            GameField.DrawPrimitive[] primitives = m_GameField.GetPrimitivesToDraw();

            foreach (GameField.DrawPrimitive p in primitives)
            {
                g.FillRectangle(new SolidBrush(Color.Gray), m_DeltaX + p.PosX * m_CellSize, m_DeltaY + p.PosY * m_CellSize, m_CellSize, m_CellSize);
                g.DrawRectangle(new Pen(Color.Silver), m_DeltaX + p.PosX * m_CellSize, m_DeltaY + p.PosY * m_CellSize, m_CellSize, m_CellSize);

                if (p.DrawPrimitiveType == GameField.PrimitiveType.Wall)
                    g.FillRectangle(new SolidBrush(Color.Red), m_DeltaX + p.PosX * m_CellSize, m_DeltaY + p.PosY * m_CellSize, m_CellSize, m_CellSize);
                if (p.DrawPrimitiveType == GameField.PrimitiveType.Text)
                    g.FillRectangle(new SolidBrush(Color.White), m_DeltaX + p.PosX * m_CellSize, m_DeltaY + p.PosY * m_CellSize, m_CellSize, m_CellSize);
                else if (p.DrawPrimitiveType == GameField.PrimitiveType.CarPart)
                    g.FillRectangle(new SolidBrush(Color.Orange), m_DeltaX + p.PosX * m_CellSize, m_DeltaY + p.PosY * m_CellSize, m_CellSize, m_CellSize);
            }

            if (primitives.Length > 0) pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(m_ShadowCanvas, 0, 0, m_ShadowCanvas.Width, m_ShadowCanvas.Height);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            InitGameFieldEnviroment();
            m_GameField.Resize();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 37: m_GameField.CarMoveLeft(true); break;
                case 38: m_GameField.CarMoveForward(true); break;
                case 39: m_GameField.CarMoveRight(true); break;
                case 40: m_GameField.CarMoveBackward(true); break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 37: m_GameField.CarMoveLeft(false); break;
                case 38: m_GameField.CarMoveForward(false); break;
                case 39: m_GameField.CarMoveRight(false); break;
                case 40: m_GameField.CarMoveBackward(false); break;
                case 78: m_GameField.NewGame(); break;
            }
        }
    }
}
