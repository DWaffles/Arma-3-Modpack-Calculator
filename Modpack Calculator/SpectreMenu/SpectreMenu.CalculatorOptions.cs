using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModpackCalculator.SpectreMenu
{
    internal partial class SpectreMenu
    {
        [InterfaceOption("Calculation Options", "Calculate Dependencies")]
        private async Task CalculateDependenciesAsync()
        {
            int modsPopulated = 0, totalDependencies = 0;
            var progress = new Progress<DependencyProgress>();
            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    // Define tasks
                    var task = ctx.AddTask("[green]Calculating Dependencies...[/]");

                    progress.ProgressChanged += (s, e) =>
                    {
                        task.Increment(1);
                        task.MaxValue = e.Total;
                    };
                    (modsPopulated, totalDependencies) = await ModManager.CalculateDependenciesAsync(progress); //catch output
                });

            AnsiConsole.MarkupLine($"{totalDependencies} dependencies were added for {modsPopulated} mods.");
        }
        [InterfaceOption("Calculation Options", "Calculate Size")]
        private void CalculateSize()
        {
            var size = ModManager.CalculateModpackSize();
            AnsiConsole.MarkupLine($"Calculated current modpack size with [green]-[/] matched mods ([red]-[/] unmatched) at [green]{size}[/] MB.");
        }
    }
}
