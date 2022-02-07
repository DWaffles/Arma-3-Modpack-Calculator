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
        [InterfaceChoice(ChoiceGroups.View, "Toggle Overview")]
        private void ToggleOverview()
        {
            DisplayOverview = !DisplayOverview;
        }
        //[InterfaceOption("View Options", "View Mod Overview")]
        private void PrintOverview()
        {
            var mods = ModManager.GetMods();

            var currentMods = mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));
            var previousMods = mods.Where(x => x.Status.HasFlag(ModStatus.PreviousMod));
            var installedMods = mods.Where(x => x.Status.HasFlag(ModStatus.Installed));

            var carryOverMods = currentMods.Intersect(previousMods); //Carry Over Mods
            var addedMods = currentMods.Where(x => !x.Status.HasFlag(ModStatus.PreviousMod)); //added Mods
            var removedMods = previousMods.Where(x => !x.Status.HasFlag(ModStatus.CurrentMod)); //Removed Mods
            var invalidIDMods = mods.Where(x => x.ModId == 0);

            var tree = new Tree($"[yellow]All Mods[/]: {mods.Count}");

            var current = tree.AddNode($"[yellow]Current Mods[/]: {currentMods.Count()}");
            if(carryOverMods.Any())
            {
                var carryOver = current.AddNode($"[yellow]Carry Over Mods[/]: {carryOverMods.Count()}");
                carryOver.AddNode($"[green]Matched[/]: {carryOverMods.Intersect(installedMods).Count()}");
                carryOver.AddNode($"[red]Not Matched[/]: {carryOverMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)).Count()}");
            }
            if(addedMods.Any())
            {
                var added = current.AddNode($"[yellow]Added Mods[/]: {addedMods.Count()}");
                added.AddNode($"[green]Matched[/]: {addedMods.Intersect(installedMods).Count()}");
                added.AddNode($"[red]Not Matched[/]: {addedMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)).Count()}");
            }
            if (removedMods.Any())
                current.AddNode($"[yellow]Removed Mods[/]: {removedMods.Count()}");

            tree.AddNode($"[yellow]Previous Mods[/]: {previousMods.Count()}");

            var installed = tree.AddNode($"[yellow]Installed Mods[/]: {installedMods.Count()}");
            if(invalidIDMods.Any())
            {
                installed.AddNode($"[red]Invalid ID Mods[/]: {invalidIDMods.Count()}");
            }

            AnsiConsole.Write(tree);
        }
        [InterfaceChoice(ChoiceGroups.View, "List Mods")]
        private void ListMods()
        {
            var mods = ModManager.GetMods();

            var currentMods = mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));
            var previousMods = mods.Where(x => x.Status.HasFlag(ModStatus.PreviousMod));
            var installedMods = mods.Where(x => x.Status.HasFlag(ModStatus.Installed));

            var carryOverMods = currentMods.Intersect(previousMods); //Carry Over Mods
            var addedMods = currentMods.Where(x => !x.Status.HasFlag(ModStatus.PreviousMod)); //added Mods
            var removedMods = previousMods.Where(x => !x.Status.HasFlag(ModStatus.CurrentMod)); //Removed Mods
            var invalidIDMods = mods.Where(x => x.ModId == 0);

            var unmatchedMods = currentMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)); //Unmatched Mods
            var matchedMods = currentMods.Intersect(installedMods); //matched Mods

            Table table;

            PrintOverview();
            if (invalidIDMods.Any()) //ID Zero Mods
            {
                table = new Table() //Name //Folder
                    .Border(TableBorder.MinimalHeavyHead);
                table.AddColumns($"[yellow]Installed Mods with ID of Zero[/] ({invalidIDMods.Count()})", "[yellow]Folder Name[/]");
                foreach (var mod in invalidIDMods.OrderBy(x => x.ModName))
                {
                    table.AddRow(mod.ModName, mod.Directory?.Name ?? String.Empty);
                }
                AnsiConsole.Write(table);
            }
            
            if (unmatchedMods.Any()) //Not Matched Mods
            {
                table = new Table() //Name //ID //Dependencies //Link
                    .Border(TableBorder.MinimalHeavyHead);
                table.AddColumns($"[yellow]Mods Not Matched[/] ({unmatchedMods.Count()})", "[yellow]ID[/]", "[yellow]Dependencies[/]", "[yellow]Link[/]");
                foreach (var mod in unmatchedMods.OrderBy(x => x.ModName))
                {
                    string dependencies = string.Concat($"({mod.Dependencies.Count}) {String.Join(", ", mod.Dependencies.Select(x => x.ModName))}");
                    table.AddRow(mod.ModName, mod.ModId.ToString(), dependencies, mod.ModLink);
                }
                AnsiConsole.Write(table);
            }

            if(matchedMods.Any()) //Matched Mods
            {
                table = new Table() //Name //ID // Size // Dependencies //Link
                    .Border(TableBorder.MinimalHeavyHead);
                var dependencyCount = matchedMods.Sum(x => x.Dependencies.Count());
                table.AddColumns($"[yellow]Matched Mods[/] ({matchedMods.Count()})", "[yellow]ID[/]", "[yellow]Size [[MB]][/]", $"[yellow]Dependencies ({dependencyCount})[/]", "[yellow]Link[/]");
                foreach (var mod in matchedMods.OrderBy(x => x.ModName))
                {
                    string dependencies = string.Concat($"({mod.Dependencies.Count}) {String.Join(", ", mod.Dependencies.Select(x => x.ModName))}");
                    table.AddRow(mod.ModName, mod.ModId.ToString(), mod.Size.MegaBytes.ToString(), dependencies, mod.ModLink);
                }
                AnsiConsole.Write(table);
            }

            AnsiConsole.Confirm("Continue?");
            ClearConsole();
        }
        [InterfaceChoice(ChoiceGroups.View, "View Paths")]
        private void ViewPaths()
        {
            AnsiConsole.MarkupLine("[yellow]Current Modpack Path:[/] ");
            AnsiConsole.MarkupLine("[yellow]Current Modpack Path: ");
        }
    }
}
