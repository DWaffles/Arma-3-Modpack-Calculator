using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModpackCalculator.SpectreMenu
{
    internal partial class SpectreMenu
    {
        [InterfaceChoice(ChoiceGroups.ExportGroup, "Export to CSV")]
        public void ExportToCSV()
        {
            ModManager.ExportAsCSV();
        }
    }
}
