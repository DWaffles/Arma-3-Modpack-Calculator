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
        private bool CalculateDependencies()
        {
            AnsiConsole.WriteLine(MethodBase.GetCurrentMethod().Name);
            return false;
        }
        private bool CalculateSize()
        {
            AnsiConsole.WriteLine(MethodBase.GetCurrentMethod().Name);
            return false;
        }
    }
}
