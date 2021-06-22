using System;
using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public class Initializer
    {
        private const string Title = "JUST FOR FUN :)";
        private const double Difficultyfactor = 0.1; //bigger number mean more mines
        private const int LivesCount = 5;
        private const int PositionInput = 1;

        private readonly IOutputRenderer _output;
        private readonly IGameEngine _gameEngine;

        public Initializer(IOutputRenderer output, IGameEngine gameEngine)
        {
            _output = output;
            _gameEngine = gameEngine;
        }

        public void Runner()
        {
            //init the game engine
            _gameEngine.Init(Title, Difficultyfactor, LivesCount, PositionInput);

            //listen for some user input
            var keyPress = _output.ReadInput;
            _gameEngine.ConfirmDesiredStartPosition();

            while (keyPress != ConsoleKey.Escape)
            {
                switch (keyPress)
                { 
                    case ConsoleKey.UpArrow:
                        _gameEngine.TryMove(Direction.North);
                        break;
                    case ConsoleKey.RightArrow:
                        _gameEngine.TryMove(Direction.East);
                        break;
                    case ConsoleKey.DownArrow:
                        _gameEngine.TryMove(Direction.South);
                        break;
                    case ConsoleKey.LeftArrow:
                        _gameEngine.TryMove(Direction.West);
                        break;
                }

                keyPress = _output.ReadInput;
            }

            _output.ResetColors();
            _output.ShowCursor = true;
            _output.Clear();
        }
    }
}