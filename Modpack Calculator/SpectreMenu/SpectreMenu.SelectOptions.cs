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
        [InterfaceOption("Select Options", "Select Current Modpack HTML")]
        private async Task<bool> SelectCurrentPackAsync()
        {
            FileInfo? file = null;
            AnsiConsole.Prompt(
                new TextPrompt<string>("\nEnter the path [green]current[/] modpack HTML file:")
                    .PromptStyle("green")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        return ValidateHTMLPath(input, out file);
                    }));
            if (file != null && AnsiConsole.Confirm($"Use [green]{file.FullName}[/] for current modpack?"))
            {
                Config.CurrentModpackPath = file.FullName;
                await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath);
                return true;
            }
            else
                return false;
        }
        [InterfaceOption("Select Options", "Select Previous Modpack HTML")]
        private async Task<bool> SelectPreviousPackAsync()
        {
            FileInfo? file = null;
            AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the path [red]previous[/] modpack HTML file:")
                    .PromptStyle("palevioletred1")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        return ValidateHTMLPath(input, out file);
                    }));

            if (file != null && AnsiConsole.Confirm($"Use [red]{file.FullName}[/] for previous modpack?"))
            {
                Config.PreviousModpackPath = file.FullName;
                await ModManager.ReadFromPreviousHTMLAsync(Config.PreviousModpackPath);
                return true;
            }
            else
                return false;
        }
        [InterfaceOption("Select Options", "Select Arma Directory")]
        private bool SelectDirectory()
        {
            DirectoryInfo? directory = null;
            AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the path to the [darkorange]Arma 3 directory[/]:")
                    .PromptStyle("darkorange")
                    .AllowEmpty()
                    .Validate(input =>
                    {
                        return ValidateArmaPath(input, out directory);
                    }));

            if (directory != null && AnsiConsole.Confirm($"Use [red]{directory.FullName}[/] for Arma mod directory?"))
            {
                Config.ArmaPath = directory.FullName;
                ModManager.ReadFromInstalled(Config.ArmaPath);
                return true;
            }
            else
                return false;
        }
        private static ValidationResult ValidateHTMLPath(string path, out FileInfo? file)
        {
            if (String.IsNullOrEmpty(path) || path.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                file = null;
                return ValidationResult.Success();
            }

            file = new(path.Replace("\"", "")); // replacing " due to "" being added around a path when dragging a file into the console
            if (file.Exists)
            {
                if (file.Extension.Equals(".html", StringComparison.OrdinalIgnoreCase))
                    return ValidationResult.Success();
                else
                    return ValidationResult.Error($"The given file, {Path.GetFileNameWithoutExtension(file.Name)}.[red]{file.Extension[1..]}[/] is not an HTML file.");
            }
            else
                return ValidationResult.Error($"[red]{path}[/] does not exist, enter a correct path or \"quit\" to exit");
        }
        private static ValidationResult ValidateArmaPath(string path, out DirectoryInfo? directory)
        {
            if (String.IsNullOrEmpty(path) || path.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                directory = null;
                return ValidationResult.Success();
            }

            directory = new(path.Replace("\"", ""));
            if (directory.Exists)
            {
                directory = new(Path.Combine(path, "!Workshop"));
                if (directory.Exists)
                {
                    return ValidationResult.Success();
                }
                else
                    return ValidationResult.Error($"The given path, {path}, does not have a [darkorange]!Workshop[/] folder.");
            }
            else
                return ValidationResult.Error($"[red]{path}[/] does not exist, enter a correct path or \"quit\" to exit");
        }
    }
}
