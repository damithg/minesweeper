using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public class GameEngine : IGameEngine
    {
        private readonly IOutputRenderer _output;
        private readonly IBoard _board;
        private readonly IPlayer _player;

        private string _desiredStartPosition = "";
        private string _gameTitle = "";
        private double _difficultyFactor;
        private int _startLives;

        public State State { get; private set; }
        public int LivesRemaining => _startLives - _board.ExplodedMines;
        public int MoveCounter { get; private set; }

        public GameEngine(IOutputRenderer output, IBoard board, IPlayer player)
        {
            _output = output;
            _board = board;
            _player = player;
        }

        public void Init(string gameTitle, double difficultyFactor, int startLives, int? positionInput)
        {
            _gameTitle = gameTitle;
            _difficultyFactor = difficultyFactor;
            _startLives = startLives;

            _desiredStartPosition += positionInput.Value.ToString();
            _output.Init(gameTitle);

            Reset();
        }

        public void Reset()
        {
            _output.SetGameDefaultColors();
            MoveCounter = 0;

            ConfigureBoard();
            _player.UpdatePosition(0, 0);
            _player.OnTheBoard = false;
            RenderToOutput();
        }
        
        private void ConfigureBoard()
        {
            int boardHeight;
            var boardWidth = boardHeight = 16;

            _board.Init(boardWidth, boardHeight, _difficultyFactor);
        }

        public void ConfirmDesiredStartPosition()
        {
            if (!int.TryParse(_desiredStartPosition, out var position) || position <= 0 || position > _board.Height)
                return;

            State = State.FreeMovement;
            _player.UpdatePosition(-1, position - 1);
            _player.OnTheBoard = TryMove(Direction.East);
        }

        public bool TryMove(Direction direction)
        {
            if (State != State.FreeMovement)
                return false;

            bool MakePermittedMove(int x, int y)
            {
                if (_board.GetSquare(x, y) == Status.Debris && _player.OnTheBoard)
                    return true; //permitted but not counted

                //attempt to move into the desired square
                var status = _board.OccupySquare(x, y);
                bool result;

                if (status == Status.Safe)
                {
                    _player.UpdatePosition(x, y);
                    result = true;
                }
                else if (status == Status.Debris)
                    result = _player.OnTheBoard; //prevents a valid move occuring when the player hits debris from the start position
                else
                    result = false;

                if (status != Status.Debris)
                    MoveCounter += 1;

                return result;
            }

            var permittedMove = false;

            //calculate and move to the new coordinates based on the direction of travel
            switch (direction)
            {
                case Direction.North:
                    if (_player.PositionY < _board.Height - 1)
                        permittedMove = MakePermittedMove(_player.PositionX, _player.PositionY + 1);
                    break;
                case Direction.East:
                    if (_player.PositionX < _board.Width - 1)
                        permittedMove = MakePermittedMove(_player.PositionX + 1, _player.PositionY);
                    break;
                case Direction.South:
                    if (_player.PositionY > 0)
                        permittedMove = MakePermittedMove(_player.PositionX, _player.PositionY - 1);
                    break;
                case Direction.West:
                    if (_player.PositionX > 0)
                        permittedMove = MakePermittedMove(_player.PositionX - 1, _player.PositionY);
                    break;
            }

            //deal with failed move situations
            if (!permittedMove && (LivesRemaining <= 0 || !_player.OnTheBoard))
            {
                if (LivesRemaining <= 0)
                    State = State.Loser;

                _desiredStartPosition = string.Empty;
                _player.UpdatePosition(-1, -1);
            }

            //maybe the player made it all the way across
            if (_player.PositionX == _board.Width - 1)
                State = State.Winner;

            RenderToOutput();
            return permittedMove;
        }

        public void RenderToOutput()
        {
            var gameLines = new List<List<Glyph>>();

            var gameTitle = $"{_output.DisplaySquareCharacter(Status.Mine)} {_gameTitle} {_output.DisplaySquareCharacter(Status.Mine)}";
            var padding = (_output.WindowWidth - gameTitle.Length) / 2;

            //insert the main heading
            gameLines.Insert(0, new List<Glyph>());
            gameLines.Insert(1, Helpers.StringToLine(new StringBuilder().Append(' ', padding).Append(gameTitle).ToString(), ConsoleColor.Red));
            gameLines.Insert(2, Helpers.StringToLine(new StringBuilder().Append(' ', padding - 1).Append('═', gameTitle.Length + 2).ToString(), ConsoleColor.Red));
            gameLines.Insert(3, new List<Glyph>());

            //generate the board
            var boardLines = _board.Render(State == State.Loser || State == State.Winner);

            //generate the status/info lines to sit beside the board
            var info = new List<string>()
            {
                "Navigate from West to East using the arrow keys",
                "Watch out for the mines!",
                "",
                State == State.Loser || State == State.Winner ? "Press ESC to quit" : "",
                "",
                State == State.Loser ? "GAME OVER"
                    : State == State.Winner ? $"CONGRATULATIONS, YOU MADE IT!"
                    : State == State.FreeMovement ? $"Position: {Helpers.GetXLegendValue(_player.PositionX)}{_player.PositionY + 1}" : "",
                State != State.Loser && State != State.Winner ? $"Lives remaining: {LivesRemaining}" : ""
            };

            //board should be positioned to the right of the screen so the lhs has space for status and instructions
            var boardWidth = boardLines.First().Count;
            padding = (_output.WindowWidth - boardWidth) - 10; //10 cols of safety margin

            //create the default padding used for board lines without any info/status text
            var defaultPadder = new StringBuilder().Append(' ', padding).ToString();
            var infoStartLine = (boardLines.Count - info.Count) / 2; //positions the info/status block roughly in the middle of the board height

            //merge our status/info lines with the board lines
            for (var i = 0; i < boardLines.Count - 1; i++)
            {
                var linePrepend = defaultPadder;

                //check we're in an appropriate place in the board lines array and build the status/info padded string
                if (i >= infoStartLine && i - infoStartLine < info.Count)
                    linePrepend = BuildBoardLinePrependString(info[i - infoStartLine], padding);

                //insert the status/info line into the list of chars for this board line
                boardLines[i].InsertRange(0, Helpers.StringToLine(linePrepend));
            }

            //add the board into the overall output array
            gameLines.AddRange(boardLines);


            //clear the display and print out each line of the game
            _output.Clear();

            foreach (var line in gameLines)
            {
                foreach (var chr in line)
                    _output.RenderSingle(chr);

                _output.Flush();
            }

            _output.SetGameDefaultColors();
        }

        
        private string BuildBoardLinePrependString(string text, int overallLength)
        {
            var padding = (overallLength - text.Length) / 2;
            var prependString = new StringBuilder().Append(' ', padding).Append(text).Append(' ', padding).ToString();

            //in case we're off by a character due to int rounding
            while (prependString.Length > overallLength)
                prependString = prependString.Remove(overallLength);
            while (prependString.Length < overallLength)
                prependString += " ";

            return prependString;
        }
    }
}