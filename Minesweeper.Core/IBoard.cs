using System.Collections.Generic;
using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public interface IBoard
    {
        int Width { get; }
        int Height { get; }
        int ExplodedMines { get; }
        void Init(int width, int height, double difficultyFactor);
        List<List<Glyph>> Render(bool showMines = false);
        Status OccupySquare(int x, int y);
        Status GetSquare(int x, int y);
    }
}