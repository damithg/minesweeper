using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core;
using Minesweeper.Core.Enums;
using Xunit;

namespace Minesweeper.UnitTests
{
    public class GameTests
    {
        private readonly ServiceProvider _serviceProvider;
        
        public GameTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton<IOutputRenderer, MockConsoleRenderer>()
                .AddSingleton<IPlayer, Player>()
                .AddSingleton<IBoard, Board>()
                .AddSingleton<IGameEngine, GameEngine>()
                .BuildServiceProvider();

            var consoleService = _serviceProvider.GetService<IOutputRenderer>();
            consoleService.WindowWidth = 120;
            consoleService.WindowHeight = 35;
            consoleService.WindowTitle = "JUST FOR FUN :)";
        }

        [Fact(DisplayName = "GIVEN a valid initialization, " +
                            "WHEN the game started, " +
                            "THEN it reset to start on first row")]
        public void Initialize_Game_With_Blank_State()
        {
            var gameService = _serviceProvider.GetService<IGameEngine>();

            gameService.Reset();

            gameService.MoveCounter.Should().Be(0, "Move counter is 0 when game starts");
        }

        [Fact(DisplayName = "GIVEN a valid initialization, " +
                            "WHEN the player move EAST, " +
                            "THEN the move counter is 1")]
        public void Counter_Increment_When_Player_Moves()
        {
            var gameService = _serviceProvider.GetService<IGameEngine>();

            gameService.Reset();

            gameService.TryMove(Direction.East);

            gameService.MoveCounter.Should().Be(1, "Move counter is 1 after moving EAST");
        }
    }
}
