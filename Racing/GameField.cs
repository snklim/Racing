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
            NotSet, Empty, CarPart, Wall, Text
        }

        public enum CarMovement
        {
            None = 0,
            Forward = 1,
            Backward = 2,
            Left = 4,
            Right = 8
        }

        public enum GameState
        {
            Play, GameOver, YouWin, GameCompleted
        }

        public class Cell
        {
            public int PosX { get; set; }
            public int PosY { get; set; }
            public Cell(int pPosX, int pPosY){
                PosX = pPosX; 
                PosY=pPosY;}
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
        public GameState CurrentGameState { get; private set; }

        public GameField(int pWidth, int pHeight)
        {
            Width = pWidth;
            Height = pHeight;

            StartNewGame();

            _carPosX = Width / 2;
            _carPosY = Height / 2;

            _rnd = new Random();
        }

        void StartNewGame()
        {
            CurrentGameState = GameState.Play;

            _numOfPeaceOfWall = 5;
            _numOfTextLeftToPrint = 5;

            _mainField = new PrimitiveType[Width, Height];
            _shadowField = new PrimitiveType[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetValue(x, y, PrimitiveType.Empty);
                }
            }
        }

        int _numOfTicks = 0;
        int _numOfPeaceOfWall = 5;
        int _numOfMovesLeftToWin = 0;
        int _numOfTextLeftToPrint = 5;
        int _gameLavel = 0;

        List<int[]> _levelTxt = new List<int[]>
        {
            new int[]{1,0,0,0,1,1,1,0,1,0,0,0,1,0,1,1,1,0,1,0,0,0,0,0,0,0,1},
            new int[]{1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,1,1},
            new int[]{1,0,0,0,1,1,0,0,0,1,0,1,0,0,1,1,0,0,1,0,0,0,0,0,0,0,1},
            new int[]{1,0,0,0,1,0,0,0,0,1,0,1,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1},
            new int[]{1,1,1,0,1,1,1,0,0,0,1,0,0,0,1,1,1,0,1,1,1,0,0,0,0,0,1}
        };
        public void Tick()
        {
            if (CurrentGameState != GameState.Play) return;

            if (_numOfTicks % (100 - 10 * _gameLavel) == 0 && _numOfPeaceOfWall > 0 && _numOfTextLeftToPrint == 0)
            {
                _numOfPeaceOfWall--;
                _shadowField[_rnd.Next(Width - 1), 0] = PrimitiveType.Wall;
                _numOfMovesLeftToWin = Height;
            }

            if (_numOfTicks % (10 - _gameLavel) == 0)
            {
                if (_numOfTextLeftToPrint > 0)
                {
                    //foreach (int txtPos in _levelTxt[_numOfTextLeftToPrint - 1])
                    for (int i = 0; i < _levelTxt[_numOfTextLeftToPrint - 1].Length; i++)
                    {
                        if (_levelTxt[_numOfTextLeftToPrint - 1][i] == 1)
                            _shadowField[i, 0] = PrimitiveType.Text;
                    }
                        
                    _numOfTextLeftToPrint--;
                }

                // check if there isn't road accident
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height - 1; y++)
                    {
                        if (IsCarCrashed(x, y + 1, _shadowField[x, y]))
                            CurrentGameState = GameState.GameOver;
                    }
                }

                // emitate road moving
                if (CurrentGameState == GameState.Play)
                {
                    // make move in shadow field
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height - 1; y++)
                        {
                            SetValue(x, y + 1, _shadowField[x, y]);
                        }
                    }

                    // copy result fro shadow field to main
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            // do not copy car from main field to shadow field
                            if (_mainField[x, y] == PrimitiveType.CarPart)
                                _shadowField[x, y] = PrimitiveType.Empty;
                            else
                                _shadowField[x, y] = _mainField[x, y];
                        }
                    }
                }

                if (_numOfPeaceOfWall == 0)
                {
                    _numOfMovesLeftToWin--;
                    if (_numOfMovesLeftToWin == 0)
                    {
                        CurrentGameState = GameState.YouWin;
                        NextLevel();
                    }
                }
            }

            int newCarPosX, newCarPosY;
            newCarPosX = _carPosX; newCarPosY = _carPosY;

            if ((CurrentMovement & CarMovement.Left) == CarMovement.Left)
            {
                if (newCarPosX > 1) newCarPosX--; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Right) == CarMovement.Right)
            {
                if (newCarPosX < Width - 2) newCarPosX++; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Forward) == CarMovement.Forward)
            {
                if (_carPosY > 0) newCarPosY--; else CurrentMovement = CarMovement.None;
            }
            if ((CurrentMovement & CarMovement.Backward) == CarMovement.Backward)
            {
                if (_carPosY < Height - 4) newCarPosY++; else CurrentMovement = CarMovement.None;
            }

            if (IsCarCrashed2(newCarPosX, newCarPosY))
            {
                CurrentGameState = GameState.GameOver;
            }
            else
            {
                DrawCar(PrimitiveType.Empty);
                _carPosX = newCarPosX; _carPosY = newCarPosY;
                DrawCar(PrimitiveType.CarPart);
            }

            _numOfTicks++;
            if (_numOfTicks == int.MaxValue) _numOfTicks = 0;
        }

        List<Cell> _carShape = new List<Cell>(8) 
        { 
            new Cell(0, 0), new Cell(0, 1), new Cell(-1, 1), 
            new Cell(1, 1), new Cell(0, 2), new Cell(0, 3), 
            new Cell(-1, 3), new Cell(1, 3) 
        };

        void NextLevel()
        {
            _gameLavel++;
            if (_gameLavel < 10)
                StartNewGame();
            else
                CurrentGameState = GameState.GameCompleted;
        }

        void DrawCar(PrimitiveType val)
        {
            foreach (Cell c in _carShape)
                SetValue(_carPosX + c.PosX, _carPosY + c.PosY, val);
        }

        bool IsCarCrashed(int x, int y, PrimitiveType primitive)
        {
            if (primitive != PrimitiveType.Wall) return false;

            foreach (Cell c in _carShape)
                if (_carPosX + c.PosX == x && _carPosY + c.PosY == y) return true;

            return false;
        }

        bool IsCarCrashed2(int carPosX, int carPosY)
        {
            foreach (Cell c in _carShape)
                if (_mainField[carPosX + c.PosX, carPosY + c.PosY] == PrimitiveType.Wall) return true;

            return false;
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

        public void NewGame()
        {
            StartNewGame();
        }
    }
}
