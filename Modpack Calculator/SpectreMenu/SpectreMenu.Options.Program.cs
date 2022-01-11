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
        private bool ClearConsole()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[underline darkorange]Arma 3 Modpack Size Calculator[/]").LeftAligned().RuleStyle(Style.Parse("darkorange")));
            return true;
        }
        private bool QuitProgram()
        {
            Console.Clear();
            DisplayMenu = false;
            return true;
        }
    }
}
