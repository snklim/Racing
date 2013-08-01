using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing
{
    class GameField
    {
        public enum PrimitiveType
        {
            NotSet, Empty, CarPart, Wall
        }

        public enum CarMovement
        {
            None = 0,
            Forward = 1,
            Backward = 2,
            Left = 4,
            Right = 8
        }

        public class DrawPrimitive
        {
            public int PosX { get; private set; }
            public int PosY { get; private set; }
            public PrimitiveType DrawPrimitiveType { get; private set; }

            public DrawPrimitive(int pPosX, int pPosY, PrimitiveType pDrawPrimitiveType)
            {
                PosX = pPosX; PosY = pPosY; DrawPrimitiveType = pDrawPrimitiveType;
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private PrimitiveType[,] _mainField;
        private PrimitiveType[,] _shadowField;

        private Random _rnd;

        int _carPosX, _carPosY;

        private List<DrawPrimitive> _drawPrimitiveQueue = new List<DrawPrimitive>();

        private CarMovement CurrentMovement = CarMovement.None;

        public GameField(int pWidth, int pHeight)
        {
            Width = pWidth;
            Height = pHeight;

            _mainField = new PrimitiveType[Width, Height];
            _shadowField = new PrimitiveType[Width, Height];

            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    SetValue(x, y, PrimitiveType.Empty);
                }
            }

            _carPosX = Width / 2;
            _carPosY = Height / 2;

            _rnd = new Random();
        }

        int _numOfTicks = 0;
        public void Tick()
        {
            if (_numOfTicks % 10 == 0)
            {
                _shadowField[_rnd.Next(Width - 2) + 1, 1] = PrimitiveType.Wall;
            }

            if (_numOfTicks % 1 == 0)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        SetValue(x, y + 1, _shadowField[x, y]);
                    }
                }

                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        _shadowField[x, y] = _mainField[x, y];
                    }
                }
            }

            if ((CurrentMovement & CarMovement.Left) == CarMovement.Left)
            {
                DrawCar(0); if (_carPosX > 2) _carPosX--; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Right) == CarMovement.Right)
            {
                DrawCar(0); if (_carPosX < Width - 3) _carPosX++; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Forward) == CarMovement.Forward)
            {
                DrawCar(0); if (_carPosY > 1) _carPosY--; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Backward) == CarMovement.Backward)
            {
                DrawCar(0); if (_carPosY < Height - 5) _carPosY++; else CurrentMovement = CarMovement.None;
            }

            DrawCar(PrimitiveType.CarPart);

            _numOfTicks++;
            if (_numOfTicks == int.MaxValue) _numOfTicks = 0;
        }

        void DrawCar(PrimitiveType val)
        {
            SetValue(_carPosX, _carPosY, val);
            SetValue(_carPosX, _carPosY + 1, val);
            SetValue(_carPosX - 1, _carPosY + 1, val);
            SetValue(_carPosX + 1, _carPosY + 1, val);
            SetValue(_carPosX, _carPosY + 2, val);
            SetValue(_carPosX, _carPosY + 3, val);
            SetValue(_carPosX - 1, _carPosY + 3, val);
            SetValue(_carPosX + 1, _carPosY + 3, val);
        }

        private void SetValue(int x, int y, PrimitiveType primitiveType)
        {
            if (_mainField[x, y] != primitiveType)
                _drawPrimitiveQueue.Add(new DrawPrimitive(x, y, primitiveType));
            _mainField[x, y] = primitiveType;
        }

        public DrawPrimitive[] GetPrimitivesToDraw()
        {
            DrawPrimitive[] primitives = _drawPrimitiveQueue.ToArray();
            _drawPrimitiveQueue.Clear();
            return primitives;
        }

        public void CarMoveLeft(Boolean pressed)
        {
            if (pressed) CurrentMovement |= CarMovement.Left;
            else CurrentMovement ^= CarMovement.Left;
        }

        public void CarMoveRight(Boolean pressed)
        {
            if (pressed) CurrentMovement |= CarMovement.Right;
            else CurrentMovement ^= CarMovement.Right;
        }

        public void CarMoveForward(Boolean pressed)
        {
            if (pressed) CurrentMovement |= CarMovement.Forward;
            else CurrentMovement ^= CarMovement.Forward;
        }

        public void CarMoveBackward(Boolean pressed)
        {
            if (pressed) CurrentMovement |= CarMovement.Backward;
            else CurrentMovement ^= CarMovement.Backward;
        }
    }
}
