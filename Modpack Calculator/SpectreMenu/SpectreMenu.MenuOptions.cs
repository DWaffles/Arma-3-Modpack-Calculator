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
        [InterfaceChoice(ChoiceGroups.MenuGroup, "Clear Console")]
        private bool ClearConsole()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[underline darkorange]Arma 3 Modpack Size Calculator[/]").LeftAligned().RuleStyle(Style.Parse("darkorange")));
            return true;
        }
        [InterfaceChoice(ChoiceGroups.MenuGroup, "Quit Menu")]
        private bool QuitProgram()
        {
            Console.Clear();
            DisplayMenu = false;
            return true;
        }
    }
}
