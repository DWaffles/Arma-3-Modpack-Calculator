using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModpackCalculator
{
    internal partial class SpectreMenu
    {
        private bool SelectCurrentPack()
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
                return true;
            }
            else
                return false;
        }
        private bool SelectPreviousPack()
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
                Config.CurrentModpackPath = file.FullName;
                return true;
            }
            else
                return false;
        }
        private static ValidationResult ValidateHTMLPath(string input, out FileInfo? file)
        {
            if (String.IsNullOrEmpty(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                file = null;
                return ValidationResult.Success();
            }

            file = new(input.Replace("\"", "")); // replacing " due to "" being added around a path when dragging a file into the console
            if (file.Exists)
            {
                if (file.Extension.Equals(".html", StringComparison.OrdinalIgnoreCase))
                    return ValidationResult.Success();
                else
                    return ValidationResult.Error($"The given file, {Path.GetFileNameWithoutExtension(file.Name)}.[red]{file.Extension[1..]}[/] is not an HTML file.");
            }
            else
                return ValidationResult.Error($"[red]{input}[/] does not exist, enter a correct path or \"quit\" to exit");
        }
        private bool SelectDirectory()
        {
            AnsiConsole.WriteLine(MethodBase.GetCurrentMethod().Name);
            return false;
        }
    }
}
