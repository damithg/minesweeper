namespace Minesweeper.Core
{
    public interface IPlayer
    {
        int PositionX { get; }
        int PositionY { get; }
        bool OnTheBoard { get; set; }
        void UpdatePosition(int x, int y);
    }
}