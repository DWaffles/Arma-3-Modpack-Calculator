using ByteSizeLib;
using System.Text;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public void ExportAsCSV(string fileName = "output.csv")
        {
            if (!fileName.EndsWith(".csv"))
                fileName += ".csv";

            var carryOverMods = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod) && x.Status.HasFlag(ModpackStatus.PreviousMod)); //Carry Over Mods
            var newMods = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod) && !x.Status.HasFlag(ModpackStatus.PreviousMod)); //New Mods
            var removedMods = Mods.Where(x => x.Status.HasFlag(ModpackStatus.PreviousMod) && !x.Status.HasFlag(ModpackStatus.CurrentMod)); //Removed Mods
            var unmatchedMods = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod) && !x.Status.HasFlag(ModpackStatus.Installed)); //Unmatched Mods

            StringBuilder builder = new();
            if (carryOverMods.Any())
            {
                builder.AppendLine($"Carry Over Mods ({carryOverMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", carryOverMods.Select(x => ModExportCSV(x))));
            }
            if (newMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append("\n");
                builder.AppendLine($"New Mods ({newMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", newMods.Select(x => ModExportCSV(x))));
            }
            if (removedMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append("\n");
                builder.AppendLine($"Removed Mods ({removedMods.Count()}), Size [MB],Empty,Empty,Empty,Empty,Dependencies,Empty,Empty,Link,ID,Empty,Last Checked");
                builder.AppendLine(String.Join("\n", removedMods.Select(x => ModExportCSV(x))));
            }
            if (unmatchedMods.Any())
            {
                if (builder.Length != 0)
                    builder.Append("\n");
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
            list.Add(ByteSize.FromBytes(mod.Size).MegaBytes.ToString()); // Size
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
