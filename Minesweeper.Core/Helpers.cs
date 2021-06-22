using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Core
{
    public static class Helpers
    {
        public static List<Glyph> StringToLine(string chars, ConsoleColor? consoleColor = null)
        {
            return chars.Select(c => new Glyph(c, consoleColor)).ToList();
        }

        public static string GetXLegendValue(int index)
        {
            var utfA = 65;
            return char.ConvertFromUtf32(utfA + index);
        }
    }
}