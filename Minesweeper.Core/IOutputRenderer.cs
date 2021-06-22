using System;
using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public interface IOutputRenderer
    {
        string WindowTitle { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        bool ShowCursor { get; set; }
        ConsoleKey ReadInput { get; }
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        string DisplaySquareCharacter(Status square);
        void Init(string gameTitle);
        void ResetColors();
        void SetGameDefaultColors();
        void RenderSingle(Glyph value);
        void Flush();
        void RenderGroupWithFlush(string value);
        void SetWindowSize(int width, int height);
        void Clear();
    }
}