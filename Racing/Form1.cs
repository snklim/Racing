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

        public Form1()
        {
            m_GameField = new GameField(30, 60);
            
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
            pictureBox1.Refresh();
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
        }

        void DrawGameField(Graphics g)
        {
            g.Clear(Color.Gray);

            for (int x = 1; x < m_GameField.Width - 1; x++)
            {
                for (int y = 1; y < m_GameField.Height - 1; y++)
                {
                    if (m_GameField.GetValue(x, y) == 0)
                        g.DrawRectangle(new Pen(Color.Silver), m_DeltaX + x * m_CellSize, m_DeltaY + y * m_CellSize, m_CellSize, m_CellSize);
                    else if (m_GameField.GetValue(x, y) == 1)
                        g.FillRectangle(new SolidBrush(Color.Red), m_DeltaX + x * m_CellSize, m_DeltaY + y * m_CellSize, m_CellSize, m_CellSize);
                    else if (m_GameField.GetValue(x, y) == 2)
                        g.FillRectangle(new SolidBrush(Color.Orange), m_DeltaX + x * m_CellSize, m_DeltaY + y * m_CellSize, m_CellSize, m_CellSize);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawGameField(e.Graphics);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            InitGameFieldEnviroment();
            pictureBox1.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 37: m_GameField.CarMoveLeft(); break;
                case 38: m_GameField.CarMoveForward(); break;
                case 39: m_GameField.CarMoveRight(); break;
                case 40: m_GameField.CarMoveBackward(); break;
            }
        }
    }
}
