using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Spectre.Console;

namespace ModpackCalculator.SpectreMenu
{
    internal partial class SpectreMenu
    {
        private bool DisplayMenu { get; set; } = true;
        private bool DisplayOverview { get; set; } = true;
        private Config Config { get; set; } = new();
        private ModManager ModManager { get; set; } = new();
        public async Task StartAsync()
        {
            try
            {
                Config = ConfigHelper.ReadConfig();
                ClearConsole();
                await ConfigPromptsAsync();

                await MenuDriver();
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }
        private async Task MenuDriver()
        {
            var choiceGroups = GetInterfaceOptions();

            while (DisplayMenu)
            {
                if (DisplayOverview)
                {
                    AnsiConsole.WriteLine();
                    PrintOverview();
                }

                var prompt = new SelectionPrompt<string>()
                    .Title("\nSelect your [green]option[/].")
                    .PageSize(20)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");

                foreach (var kvp in choiceGroups.OrderBy(x => ChoiceGroups.GetGroupPriority(x.Key)))
                {
                    prompt.AddChoiceGroup(kvp.Key, kvp.Value.Select(x => x.Key));
                }

                var input = AnsiConsole.Prompt(prompt);

                foreach (var choiceGroup in choiceGroups.Values)
                {
                    if (choiceGroup.ContainsKey(input))
                    {
                        ClearConsole();
                        var result = choiceGroup[input].Invoke(this, null);
                        if(result != null && result is Task task)
                        {
                            await task;
                        }
                    }
                }
            }
        }
        private static Dictionary<string, Dictionary<string, MethodInfo>> GetInterfaceOptions()
        {
            Dictionary<string, Dictionary<string, MethodInfo>> dictionary = new();
            foreach(var option in typeof(SpectreMenu).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = (InterfaceChoiceAttribute?) option.GetCustomAttribute(typeof(InterfaceChoiceAttribute));
                if(attribute != null)
                {
                    if (dictionary.ContainsKey(attribute.ChoiceGroup))
                        dictionary[attribute.ChoiceGroup].Add(attribute.ChoiceName, option);
                    else
                    {
                        dictionary[attribute.ChoiceGroup] = new Dictionary<string, MethodInfo>
                        {
                            { attribute.ChoiceName, option }
                        };
                    }
                }
            }
            return dictionary;
        }
        private async Task ConfigPromptsAsync()
        {
            await HandleModpackPromptsAsync();
            HandleDirectoryPrompt();
            ConfigHelper.OutputConfig(Config);
        }
        private async Task HandleModpackPromptsAsync()
        {
            if (Config.PreviousModpackPath != null && new FileInfo(Config.PreviousModpackPath).Exists)
            {
                var input = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Found previous modpack path: {Config.PreviousModpackPath}")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(new[] {
                            "Use as Previous Modpack Path",
                            "Use as Current Modpack Path",
                            "Discard",
                        }));
                switch (input)
                {
                    case "Use as Previous Modpack Path":
                        await ModManager.ReadFromPreviousHTMLAsync(Config.PreviousModpackPath);
                        await HandleCurrentPromptAsync();
                        break;
                    case "Use as Current Modpack Path":
                        Config.CurrentModpackPath = Config.PreviousModpackPath;
                        Config.PreviousModpackPath = null;
                        await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath);
                        break;
                    case "Discard":
                        Config.PreviousModpackPath = null;
                        await HandleCurrentPromptAsync();
                        break;
                }
            }
            else
            {
                Config.PreviousModpackPath = null;
                await HandleCurrentPromptAsync();
            }
        }
        private async Task HandleCurrentPromptAsync()
        {
            if (Config.CurrentModpackPath != null && new FileInfo(Config.CurrentModpackPath).Exists)
            {
                List<string> choices = new() { "Use as Current Modpack Path", "Discard" };
                if (Config.PreviousModpackPath == null)
                {
                    choices.Add(choices[1]);
                    choices[1] = "Use as Previous Modpack Path";
                }
                var input = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Found current modpack path: {Config.CurrentModpackPath}")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(new[] {
                            "Use as Current Modpack Path",
                            "Discard",
                        }));
                switch (input)
                {
                    case "Use as Current Modpack Path":
                        await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath);
                        break;
                    case "Use as Previous Modpack Path":
                        Config.CurrentModpackPath = Config.CurrentModpackPath;
                        Config.PreviousModpackPath = null;
                        await ModManager.ReadFromPreviousHTMLAsync(Config.CurrentModpackPath);
                        break;
                    case "Discard":
                        Config.CurrentModpackPath = null;
                        break;
                }
            }
            else
            {
                Config.CurrentModpackPath = null;
            }
        }
        private void HandleDirectoryPrompt()
        {
            if (Config.ArmaPath != null && new DirectoryInfo(Config.ArmaPath).Exists)
            {
                var input = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Found previously used Arma 3 directory: {Config.ArmaPath}")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(new[] {
                            "Use as Arma 3 workshop path",
                            "Discard",
                        }));
                switch (input)
                {
                    case "Use as Arma 3 workshop path":
                        ModManager.ReadFromInstalled(Config.ArmaPath);
                        break;
                    case "Discard":
                        Config.ArmaPath = null;
                        break;
                }
            }
            else
            {
                Config.ArmaPath = null;
            }
        }
        private static void WriteException(Exception ex)
        {
            AnsiConsole.MarkupLine("\n[red]Exception[/] occured within the program.");
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }
}
