using System;

namespace Minesweeper.Core
{
    public class Player : IPlayer
    {
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public bool OnTheBoard { get; set; }

        public void UpdatePosition(int x, int y)
        {
            //TODO : fail if drop outside the grid

            PositionX = x;
            PositionY = y;
        }
    }
}