using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public class Board : IBoard
    {
        private readonly IOutputRenderer _output;
        private readonly IPlayer _player;

        private Status[,] _squares;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Board(IOutputRenderer output, IPlayer player)
        {
            _output = output;
            _player = player;
        }

        public int ExplodedMines
        {
            get
            {
                //any square that has a status of debris represents an exploded mine
                return _squares
                    .Cast<Status>()
                    .Count(s => s == Status.Debris);
            }
        }

        public void Init(int width, int height, double difficultyFactor)
        {
            if (width < 1 || width > 26)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be between between 1 and 26 inclusive");
            if (height < 1 || height > 26)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be between between 1 and 26 inclusive");

            Width = width;
            Height = height;

            _squares = new Status[width, height];

            var random = new Random();

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                _squares[x, y] = random.NextDouble() < difficultyFactor
                    ? Status.Mine : Status.Safe;
        }

        public List<List<Glyph>> Render(bool showMines = false)
        {
            var boardLines = new List<List<Glyph>>();

            //construct the top and bottom legends and borders
            var yLegendPad = Height > 9 ? "   " : "  "; //when the height reaches double digits more padding is needed
            var xLegendLine = Helpers.StringToLine(yLegendPad + "  ");
            var xLegendBorderCenter = new List<Glyph>();
            var xLegendBorderTop = Helpers.StringToLine(yLegendPad + "┌──┐", ConsoleColor.Blue);
            var xLegendBorderBottom = Helpers.StringToLine(yLegendPad + "└──┘", ConsoleColor.Blue);

            for (var i = 0; i < Width; i++)
            {
                //generate horizontal A/B/C etc legend markers and border, changing colour if player is on this column
                xLegendLine.AddRange(Helpers.StringToLine(Helpers.GetXLegendValue(i) + " ", _player.PositionX == i ? ConsoleColor.Red : ConsoleColor.White));
                xLegendBorderCenter.AddRange(Helpers.StringToLine("──", ConsoleColor.Blue));
            }

            //2 gets us into the ┌──┐ space
            xLegendBorderTop.InsertRange(yLegendPad.Length + 2, xLegendBorderCenter);
            xLegendBorderBottom.InsertRange(yLegendPad.Length + 2, xLegendBorderCenter);

            //add the top legend lines
            boardLines.Add(xLegendLine);
            boardLines.Add(xLegendBorderTop);

            //generate mine field lines remembering that we loop the rows first since we're printing glyphs left to right one line at a time
            for (var y = Height - 1; y >= 0; y--)
            {
                var lineNum = y + 1;
                var legendPad = Height > 9 && lineNum <= 9 ? " " : ""; //correct padding depending on if this is a double digit row

                //create a new line starting with the the legend, switching colours if the player is on this row
                var line = new List<Glyph>()
                {
                    new Glyph(legendPad + lineNum, _player.PositionY == y ? ConsoleColor.Red : ConsoleColor.White),
                    new Glyph(' '),
                    new Glyph('│', ConsoleColor.Blue),
                    new Glyph(' '),
                };

                //loop the columns
                for (var x = 0; x < Width; x++)
                {
                    //display the player icon or mine field square as necessary
                    if (_player.PositionX == x && _player.PositionY == y)
                        line.Add(new Glyph("|>"));
                    else
                    {
                        //show a mine location only if required (e.g. game is over)
                        var square = GetSquare(x, y);
                        if (square == Status.Mine && !showMines)
                            square = Status.Safe;

                        line.Add(new Glyph(_output.DisplaySquareCharacter(square)));
                    }
                }

                //finish the line with the legend
                line.Add(new Glyph(' '));
                line.Add(new Glyph('│', ConsoleColor.Blue));
                line.Add(new Glyph(' '));
                line.Add(new Glyph(lineNum.ToString(), _player.PositionY == y ? ConsoleColor.Red : ConsoleColor.White));

                boardLines.Add(line);
            }

            //add the bottom legend to the board
            boardLines.Add(xLegendBorderBottom);
            boardLines.Add(xLegendLine);

            return boardLines;
        }

        public Status OccupySquare(int x, int y)
        {
            if (x < 0 || x > Width - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "X must be a valid board column index");
            if (y < 0 || y > Height - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "Y must be a valid board row index");

            var status = GetSquare(x, y);

            //if the square is a mine update it to debris so we know the player hit it
            if (status == Status.Mine)
                _squares[x, y] = Status.Debris;

            return status;
        }

        public Status GetSquare(int x, int y)
        {
            if (x < 0 || x > Width - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "X must be a valid board column index");
            if (y < 0 || y > Height - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "Y must be a valid board row index");

            return _squares[x, y];
        }
    }
}