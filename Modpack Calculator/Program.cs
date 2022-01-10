using ModpackCalculator;

namespace ModPackCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleMenu menu = new();
            await menu.StartAsync();
        }
    }
}