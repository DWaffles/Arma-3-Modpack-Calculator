using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spectre.Console;

namespace ModpackCalculator
{
    internal partial class SpectreMenu
    {
        private bool DisplayMenu { get; set; } = true;
        private Config Config { get; set; } = new();
        public void Start()
        {
            ClearConsole();

            Config = ConfigHelper.ReadConfig();

            // Get Values From Config // or clear values upon user confirmation
            // Check for user confirmation
            // Update Config
            // Menu Driver

            MenuDriver();
        }
        private void MenuDriver()
        {
            Dictionary<string, Dictionary<string, Func<bool>>> choiceGroups = new();
            choiceGroups.Add("View Options", new Dictionary<string, Func<bool>>
            {
                { "View Mod Overview", PrintOverview },
                { "View Matched Mods", null},
                { "View Paths", null}
            });
            choiceGroups.Add("Select Options", new Dictionary<string, Func<bool>>
            {
                { "Select Current Modpack HTML", SelectCurrentPack },
                { "Select Previous Modpack HTML", SelectPreviousPack},
                { "Select Arma Directory", SelectDirectory }
            });
            choiceGroups.Add("Calculation Options", new Dictionary<string, Func<bool>>
            {
                { "Calculate Dependencies", CalculateDependencies },
                { "Calculate Size", CalculateSize},
            });
            choiceGroups.Add("Export Options", new Dictionary<string, Func<bool>>
            {
                { "Export to CSV", null },
            });
            choiceGroups.Add("Menu Options", new Dictionary<string, Func<bool>>
            {
                { $"Clear Console", ClearConsole },
                { $"Quit Program", QuitProgram}
            });

            while (DisplayMenu)
            {
                ClearConsole();

                var prompt = new SelectionPrompt<string>()
                    .Title("\nSelect your [green]option[/].")
                    .PageSize(20)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");

                foreach (var kvp in choiceGroups)
                {
                    prompt.AddChoiceGroup(kvp.Key, kvp.Value.Select(x => x.Key));
                }

                var input = AnsiConsole.Prompt(prompt);

                foreach (var choiceGroup in choiceGroups.Values)
                {
                    if (choiceGroup.ContainsKey(input))
                    {
                        try
                        {
                            choiceGroup[input]();
                        }
                        catch (NotImplementedException ex) when (choiceGroup[input] == null)
                        {
                            WriteException(ex);
                        }
                        catch (Exception ex)
                        {
                            WriteException(ex);
                        }
                    }
                }
            }
        }
        static void WriteException(Exception ex)
        {
            AnsiConsole.MarkupLine("\n[red]Exception[/] occured within the program.");
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }
}
