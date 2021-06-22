using Minesweeper.Core.Enums;

namespace Minesweeper.Core
{
    public interface IGameEngine
    {
        State State { get; }
        int LivesRemaining { get; }
        int MoveCounter { get; }
        void Reset();
        void Init(string gameTitle, double difficultyFactor, int startLives, int? positionInput);
        void ConfirmDesiredStartPosition();
        void RenderToOutput();
        bool TryMove(Direction direction);
    }
}