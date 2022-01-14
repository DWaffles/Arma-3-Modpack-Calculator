using ByteSizeLib;
using System.Text;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public void ExportAsCSV(string fileName = "output.csv") // optional value/overload to export all/certain conditions?
        {
            if (!fileName.EndsWith(".csv"))
                fileName += ".csv";

            var carryOverMods = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod) && x.Status.HasFlag(ModStatus.PreviousMod)); //Carry Over Mods //includes unmatched mods! //inaccuracy
            var newMods = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod) && !x.Status.HasFlag(ModStatus.PreviousMod)); //New Mods //includes unmatched mods! //inaccuracy
            var removedMods = Mods.Where(x => x.Status.HasFlag(ModStatus.PreviousMod) && !x.Status.HasFlag(ModStatus.CurrentMod)); //Removed Mods //includes unmatched mods //inaccuracy?
            var unmatchedMods = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod) && !x.Status.HasFlag(ModStatus.Installed)); //Unmatched Mods

            StringBuilder builder = new();
            if (carryOverMods.Any()) //includes unmatched mods! //inaccuracy
            {
                builder.AppendLine($"Carry Over Mods ({carryOverMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", carryOverMods.Select(x => ModExportCSV(x))));
            }
            if (newMods.Any()) //includes unmatched mods! //inaccuracy
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"New Mods ({newMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", newMods.Select(x => ModExportCSV(x))));
            }
            if (removedMods.Any())  //includes unmatched mods //inaccuracy?
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"Removed Mods ({removedMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", removedMods.Select(x => ModExportCSV(x))));
            }
            if (unmatchedMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append('\n');
                builder.AppendLine($"Unmatched Mods ({unmatchedMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", unmatchedMods.Select(x => ModExportCSV(x))));
            }
            File.WriteAllText(fileName, builder.ToString(), new UTF8Encoding(false));
        }
        private string ModExportCSV(ModModel mod)
        {
            // TITLE - SIZE [MB] - EMPTY - EMPTY - EMPTY - EMPTY - DEPENDENCIES - EMPTY - EMPTY - LINK - ID - EMPTY - CURRENT DATE

            List<string> list = new();
            list.Add(EscapeCSVString(mod.ModName)); // Title
            list.Add(ByteSize.FromBytes(mod.Bytes).MegaBytes.ToString()); // Size
            list.Add(String.Empty); //EMPTY
            list.Add(String.Empty); //EMPTY
            list.Add(String.Empty); //EMPTY
            list.Add(String.Empty); //EMPTY
            list.Add(String.Join(" | ", mod.Dependencies.Select(x => x.ModName))); //DEPENDENCIES
            list.Add(String.Empty); //EMPTY
            list.Add(String.Empty); //EMPTY
            list.Add(mod.ModLink); //LINK
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
