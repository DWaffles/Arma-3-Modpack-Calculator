using Spectre.Console;

namespace ModpackCalculator.SpectreMenu
{
    internal partial class SpectreMenu
    {
        [InterfaceChoice(ChoiceGroups.CalculateGroup, "Calculate Dependencies")]
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
        [InterfaceChoice(ChoiceGroups.CalculateGroup, "Calculate Size")]
        private void CalculateSize()
        {
            var (count, size) = ModManager.CalculateModpackSize();
            //AnsiConsole.MarkupLine($"Calculated current modpack size with [green]{count}[/] matched mods ([red]-[/] unmatched) at [green]{size.MegaBytes}[/] MB.");

            var tree = new Tree($"[yellow]Calculated Mods[/]: {count}");
            tree.AddNode($"GB: {size.GigaBytes.ToString("##,00.00")}");
            tree.AddNode($"MB: {size.MegaBytes.ToString("##,00.00")}");
            tree.AddNode($"B:  {size.Bytes.ToString("##,00.00")}");
            AnsiConsole.Write(tree);
        }
    }
}
