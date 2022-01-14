using ByteSizeLib;

namespace ModpackCalculator
{
    [Obsolete]
    internal class ConsoleMenu
    {
        private Config Config { get; set; } = new();
        private ModManager ModManager { get; set; } = new();
        public async Task StartAsync()
        {
            Console.WriteLine("Starting Arma 3 Modpack Size Calculator");

            Config = ConfigHelper.ReadConfig();

            if (!string.IsNullOrEmpty(Config.CurrentModpackPath))
            {
                // Check for file existence
                Console.WriteLine($"\nAutomatically importing previous modpack from: {Config.CurrentModpackPath}");
                await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath); //add confirmation
            }
            if (!string.IsNullOrEmpty(Config.PreviousModpackPath))
            {
                // Check for file existence
                Console.WriteLine($"\nAutomatically importing previous modpack from: {Config.PreviousModpackPath}");
                await ModManager.ReadFromPreviousHTMLAsync(Config.PreviousModpackPath); //add confirmation
            }
            if (!string.IsNullOrEmpty(Config.ArmaPath))
            {
                // Check for Arma path existence
                Console.WriteLine($"\nAutomatically using previous Arma directory: {Config.ArmaPath}");
                ModManager.ReadFromInstalled(Config.ArmaPath);
            }
            await MenuDriverAsync();
        }

        private async Task MenuDriverAsync()
        {
            bool display = true;
            while (display)
            {
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine($"[1]: Print Overview");
                Console.WriteLine($"[2]: Select Current Modpack HTML");
                Console.WriteLine($"[3]: Select Previous Modpack HTML");
                Console.WriteLine($"[4]: Select Arma Directory");
                Console.WriteLine($"[5]: Calculate Dependencies");
                Console.WriteLine($"[6]: Calculate Size");
                Console.WriteLine($"[7]: Output Data (CSV)");
                Console.WriteLine($"[Q]: Quit Program");

                Console.WriteLine();
                Console.Write("Selection: ");
                string input = Console.ReadLine();
                Console.WriteLine();

                int count;
                string path;
                switch (input)
                {
                    case "1":
                        DisplayOverview(ModManager.GetMods());
                        break;
                    case "2":
                        path = GetHtmlPath();  // get path from user

                        if (path == null)
                            break;

                        Config.CurrentModpackPath = path;
                        ConfigHelper.OutputConfig(Config);

                        count = await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath);
                        Console.WriteLine($"{count} mods read from the given HTML file.");
                        break;
                    case "3":
                        path = GetHtmlPath();  // get path from user

                        if (path == null)
                            break;
                        
                        Config.PreviousModpackPath = path; 
                        ConfigHelper.OutputConfig(Config);

                        count = await ModManager.ReadFromPreviousHTMLAsync(Config.PreviousModpackPath);
                        Console.WriteLine($"{count} mods read from the given HTML file.");
                        break;
                    case "4":
                        path = GetArmaPath(); // get path from user

                        if (path == null)
                            break;

                        Config.PreviousModpackPath = path;
                        ConfigHelper.OutputConfig(Config);

                        count = ModManager.ReadFromInstalled(GetWorkshopPath(Config.ArmaPath));
                        Console.WriteLine($"{count} installed mods found and recognized.");
                        break;
                    case "5":
                        Console.WriteLine("Populating dependencies...");
                        var (ModsPopulated, Dependencies) = await ModManager.CalculateDependenciesAsync();
                        Console.WriteLine($"{Dependencies} dependencies added for {ModsPopulated} mods.");
                        break;
                    case "6":
                        Console.WriteLine("Calculating modpack size...");
                        var size = ModManager.CalculateModpackSize();
                        Console.WriteLine($"Modpack size from matched mods: {size} MB");
                        break;
                    case "7":
                        ModManager.ExportAsCSV();
                        break;
                    case "Q":
                        display = false;
                        break;
                    case "q":
                        display = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        Console.WriteLine();
                        break;
                }
            }
        }
        private static string GetHtmlPath()
        {
            Console.Write("Drag pack HTML: ");
            string path = Console.ReadLine().Replace("\"", "");

            FileInfo fileInfo = new(path);
            if (fileInfo.Exists)
            {
                if (String.Equals(fileInfo.Extension, ".html", StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
                else
                {
                    Console.WriteLine($"{fileInfo.Name} does not have an .html extension, returning to menu selection.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"Given path is not a valid file, returning to menu selection.");
                return null;
            }
        }
        private static string GetArmaPath() //do proper checking
        {
            Console.Write("Enter path to Arma 3 installation: ");
            string path = Console.ReadLine().Replace("\"", "");
            return path;
        }
        private static string GetWorkshopPath(string path)
        {
            return Path.Combine(path, "!Workshop");
        }
        private static void DisplayOverview(IEnumerable<ModModel> mods)
        {
            int textPad = 40;
            int intPad = 10;

            var currentMods = mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));
            var installedMods = mods.Where(x => x.Status.HasFlag(ModStatus.Installed));
            var previousMods = mods.Where(x => x.Status.HasFlag(ModStatus.PreviousMod));

            var carryOverMods = currentMods.Intersect(previousMods); //Carry Over Mods
            var newMods = currentMods.Where(x => !x.Status.HasFlag(ModStatus.PreviousMod)); //New Mods
            var removedMods = previousMods.Where(x => !x.Status.HasFlag(ModStatus.CurrentMod)); //Removed Mods
            var invalidIDMods = mods.Where(x => x.ModId == 0);

            var unmatchedMods = currentMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)); //Unmatched Mods
            var matchedMods = currentMods.Intersect(installedMods); //matched Mods

            Console.WriteLine($"{mods.Count()} total mods in data (excluding dependencies). {mods.Sum(x => x.Dependencies.Count)} dependencies counted.");
            Console.WriteLine();

            Console.WriteLine($"Current Mods: {currentMods.Count()}");
            if (carryOverMods.Any())
            {
                Console.WriteLine($"    • Carry Over Mods: {carryOverMods.Count()}");

                Console.Write($"        • Matched: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(carryOverMods.Intersect(installedMods).Count());
                Console.ResetColor();

                Console.Write($"        • Unmatched: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(carryOverMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)).Count());
                Console.ResetColor();
            }
            if (newMods.Any())
            {
                Console.WriteLine($"    + New Mods: {newMods.Count()}");
                Console.WriteLine($"        • Matched: {newMods.Intersect(installedMods).Count()}");
                Console.WriteLine($"        • Unmatched: {newMods.Where(x => !x.Status.HasFlag(ModStatus.Installed)).Count()}");
            }
            if (removedMods.Any())
                Console.WriteLine($"    - Removed Mods: {removedMods.Count()}");

            Console.WriteLine($"Previous Mods: {previousMods.Count()}");

            Console.WriteLine($"Installed Mods: {installedMods.Count()}");
            Console.Write($"    • Invalid ID Mods: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(invalidIDMods.Count());
            Console.ResetColor();

            if (invalidIDMods.Any())
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"Installed Mods With ID Zero ({invalidIDMods.Count()})*".PadRight(textPad));
                Console.ResetColor();
                Console.WriteLine(" | " + "Folder Name");
                foreach (var mod in invalidIDMods)
                {
                    Console.WriteLine(string.Concat(mod.ModName.PadRight(textPad).AsSpan(0, textPad), " | ", mod.Directory?.Name.PadRight(textPad).Substring(0, textPad)));
                }
                Console.WriteLine("* denotes valid mods that exist in the mod directory, but do not have a Workshop ID that was found and have not been matched.");
            }

            if (installedMods.Any() && currentMods.Any())
            {
                if (unmatchedMods.Any())
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"Mods Not Matched ({unmatchedMods.Count()})*".PadRight(textPad));
                    Console.ResetColor();
                    Console.WriteLine(" | " + "ID".PadRight(intPad) + " | " + "Link");

                    foreach (var mod in unmatchedMods)
                    {
                        Console.Write(string.Concat(mod.ModName.PadRight(textPad).AsSpan(0, textPad), " | "));
                        Console.Write(mod.ModId.ToString().PadRight(intPad) + " | ");
                        Console.WriteLine(mod.ModLink);
                    }
                    Console.WriteLine("* denotes that these mods are *in* the current modpack, but cannot be located in the mod directory.");
                }
            }
            else if (currentMods.Any())
                Console.WriteLine($"\nNone of the {currentMods.Count()} current mods are installed.");

            if (matchedMods.Any()) // Only here while testing
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"Mods Matched ({matchedMods.Count()})".PadRight(textPad));
                Console.ResetColor();
                Console.Write(" | " + "ID".PadRight(intPad) + " | ");
                Console.Write("Size [MB]".PadRight(intPad) + " | ");
                Console.Write("Dependencies".PadRight(textPad) + " | ");
                Console.Write("Link");
                Console.WriteLine();

                foreach (var mod in matchedMods)
                {
                    Console.Write(string.Concat(mod.ModName.PadRight(textPad).AsSpan(0, textPad), " | "));
                    Console.Write(mod.ModId.ToString().PadRight(intPad) + " | ");
                    Console.Write(string.Concat(ByteSize.FromBytes(mod.Bytes).MegaBytes.ToString().PadRight(intPad).AsSpan(0, intPad), " | "));
                    Console.Write(string.Concat($"[{mod.Dependencies.Count}] {String.Join(", ", mod.Dependencies.Select(x => x.ModName))}".PadRight(textPad).AsSpan(0, textPad), " | "));
                    Console.Write(mod.ModLink);
                    Console.WriteLine();
                }
            }
        }
        
    }
}
