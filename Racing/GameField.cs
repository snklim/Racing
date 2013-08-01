using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing
{
    class GameField
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private int[,] _mainFiled;
        private int[,] _shadowField;

        private Random _rnd;
        
        int _carPosX, _carPosY;

        public GameField(int pWidth, int pHeight)
        {
            Width = pWidth;
            Height = pHeight;

            _mainFiled = new int[Width, Height];
            _shadowField = new int[Width, Height];

            _carPosX = Width / 2;
            _carPosY = Height / 2;

            _rnd = new Random();
        }

        int _numOfTicks = 0;
        public void Tick()
        {
            if (_numOfTicks % 100 == 0)
            {
                _shadowField[_rnd.Next(Width - 2) + 1, 1] = 1;
            }

            if (_numOfTicks % 10 == 0)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        if (_shadowField[x, y] != 2) _mainFiled[x, y + 1] = _shadowField[x, y];
                    }
                }

                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        _shadowField[x, y] = _mainFiled[x, y];
                    }
                }
            }

            DrawCar(2);

            _numOfTicks++;
            if (_numOfTicks == int.MaxValue) _numOfTicks = 0;
        }

        void DrawCar(int val)
        {
            _mainFiled[_carPosX, _carPosY] = val;
            _mainFiled[_carPosX, _carPosY + 1] = val;
            _mainFiled[_carPosX - 1, _carPosY + 1] = val;
            _mainFiled[_carPosX + 1, _carPosY + 1] = val;
            _mainFiled[_carPosX, _carPosY + 2] = val;
            _mainFiled[_carPosX, _carPosY + 3] = val;
            _mainFiled[_carPosX - 1, _carPosY + 3] = val;
            _mainFiled[_carPosX + 1, _carPosY + 3] = val;
        }

        public int GetValue(int x, int y)
        {
            return _mainFiled[x, y];
        }

        public void CarMoveLeft()
        {
            DrawCar(0);
            if (_carPosX > 2) _carPosX--;
        }

        public void CarMoveRight()
        {
            DrawCar(0);
            if (_carPosX < Width - 3) _carPosX++;
        }

        public void CarMoveForward()
        {
            DrawCar(0);
            if (_carPosY > 1) _carPosY--;
        }

        public void CarMoveBackward()
        {
            DrawCar(0);
            if (_carPosY < Height - 5) _carPosY++;
        }
    }
}
