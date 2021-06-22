using System;

namespace Minesweeper.Core
{
    public class Glyph
    {
        public string Value { get; }

        public ConsoleColor? Color { get; }

        public Glyph(char value, ConsoleColor? color = null)
        {
            Value = value.ToString();
            Color = color;
        }

        public Glyph(string value, ConsoleColor? color = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value must contain a non-empty string");

            Value = value;
            Color = color;
        }
    }
}