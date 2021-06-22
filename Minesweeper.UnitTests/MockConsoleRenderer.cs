using System;
using Minesweeper.Core;
using Minesweeper.Core.Enums;

namespace Minesweeper.UnitTests
{
    public class MockConsoleRenderer : IOutputRenderer
    {
        public string WindowTitle { get; set; }
        public bool ShowCursor { get; set; }

        public ConsoleKey KeyToRead { get; set; }

        public ConsoleKey ReadInput => KeyToRead;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public bool EmojiSupport => false;

        public void Init(string gameTitle)
        { }

        public void Clear()
        { }

        public void ResetColors()
        { }

        public void SetWindowSize(int width, int height)
        { }

        public void RenderSingle(string value)
        { }

        public void Flush()
        { }

        public void RenderGroupWithFlush(string value)
        { }

        public void SetGameDefaultColors()
        { }

        public void RenderSingle(Glyph value)
        { }

        public string DisplaySquareCharacter(Status square)
        {
            return " ";
        }
    }
}