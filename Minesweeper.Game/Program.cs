using Minesweeper.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Minesweeper.Game
{
    class Program
    {
        static void Main(string[] args)
        {
			//setup our services
            var serviceProvider = ConfigureServices()
                .BuildServiceProvider();

            //call into the main application class to begin
            serviceProvider.GetService<Initializer>()?.Runner();
		}

        private static IServiceCollection ConfigureServices()
        {
            //register our services and the main application class
            return new ServiceCollection()
                .AddSingleton<IOutputRenderer, ConsoleRenderer>()
                .AddSingleton<IPlayer, Player>()
                .AddSingleton<IBoard, Board>()
                .AddSingleton<IGameEngine, GameEngine>()
                .AddTransient<Initializer>();
        }
    }
}