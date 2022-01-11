using ModpackCalculator;
using Spectre.Console;

namespace ModPackCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which interface to use for the [underline darkorange]Arma 3 Modpack Calculator[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {
                        "Spectre.Console", "System Console", "Quit"
                    }));
            switch(selection)
            {
                case "Spectre.Console":
                    SpectreMenu spectreMenu = new();
                    spectreMenu.Start();
                    break;
                case "System Console":
                    ConsoleMenu consoleMenu = new();
                    await consoleMenu.StartAsync();
                    break;
                default:
                    return;
            }
            _ = Main(args);
        }
    }
}