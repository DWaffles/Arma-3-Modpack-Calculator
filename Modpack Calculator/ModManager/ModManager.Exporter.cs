using ByteSizeLib;
using System.Text;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public void ExportAsFormattedCSV(string fileName = "output.csv") // optional value/overload to export all/certain conditions?
        {
            if (!fileName.EndsWith(".csv"))
                fileName += ".csv";

            //Combine carryOverMods && unmatchedMods, add status colum

            var carryOverMods = Mods.Where(x => x.Status.HasFlags(ModStatus.CurrentMod, ModStatus.PreviousMod, ModStatus.Installed)).OrderBy(x => x.ModName);
            var unmatchedMods = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod) && !x.Status.HasFlag(ModStatus.Installed)).OrderBy(x => x.ModName);
            var newMods = Mods.Where(x => x.NewlyAdded && x.Status.HasFlag(ModStatus.Installed)).OrderBy(x => x.ModName);
            var removedMods = Mods.Where(x => x.Removed).OrderBy(x => x.ModName);

            StringBuilder builder = new();
            if (carryOverMods.Any())
            {
                builder.AppendLine($"Carry Over Mods ({carryOverMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join(Environment.NewLine, carryOverMods.Select(x => ModExportCSV(x))));
            }
            if (unmatchedMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"Unmatched Mods ({unmatchedMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join(Environment.NewLine, unmatchedMods.Select(x => ModExportCSV(x))));
            }
            if (newMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"New Mods ({newMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join(Environment.NewLine, newMods.Select(x => ModExportCSV(x))));
            }
            if (removedMods.Any())  //includes unmatched mods
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"Removed Mods ({removedMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join(Environment.NewLine, removedMods.Select(x => ModExportCSV(x))));
            }
            
            File.WriteAllText(fileName, builder.ToString(), new UTF8Encoding(false));
        }
        private string ModExportCSV(ModModel mod)
        {
            // TITLE - SIZE [MB] - EMPTY - EMPTY - EMPTY - EMPTY - DEPENDENCIES - EMPTY - EMPTY - LINK - ID - EMPTY - CURRENT DATE

            List<string> list = new();
            list.Add(EscapeCSVString(mod.ModName!)); // Title
            list.Add(mod.Size.MegaBytes.ToString()); // Size
            list.AddRange(Enumerable.Repeat(String.Empty, 4));
            list.Add(String.Join(" | ", mod.Dependencies.Select(x => x.ModName))); //DEPENDENCIES
            list.AddRange(Enumerable.Repeat(String.Empty, 2));
            list.Add(mod.ModLink + " "); //LINK
            list.Add(mod.ModId.ToString()); //ID
            list.Add(String.Empty); //EMPTY
            list.Add(DateTime.Now.ToString("MM/dd/yyyy")); //CURRENT DATE

            return String.Join(",", list);
        }
        private static string EscapeCSVString(string str) //move to RegexHelper
        {
            var mustQuote = str.Any(x => x == ',' || x == '\"' || x == '\r' || x == '\n');

            if (!mustQuote)
            {
                return str;
            }

            str = str.Replace("\"", "\"\"");

            return string.Format("\"{0}\"", str);
        }
    }
}
