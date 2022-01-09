namespace ModpackCalculator
{
    internal partial class ModManager
    {
        private List<ModModel> Mods { get; set; } = new();
        //private List<ModModel> InstalledMods { get; set; } = new();
        //private List<ModModel> CurrentMods { get; set; } = new();
        //private List<ModModel> PreviousMods { get; set; } = new();
        public async Task PopulateDependenciesAsync()
        {
            var list = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod));

            var tasks = list.Select(async mod =>
            {
                var response = await ScrapeModDependenciesAsync(mod);
            });
            await Task.WhenAll(tasks);
        }

        private void AddReadMods(IEnumerable<ModModel> mods, ModpackStatus status)
        {
            foreach (var mod in mods)
            {
                ModModel foundMod;
                if (mod.ModId == 0)
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    foundMod = Mods.SingleOrDefault(m => m.ModName.Equals(mod.ModName, StringComparison.OrdinalIgnoreCase));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
                else
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    foundMod = Mods.SingleOrDefault(m => m.ModId == mod.ModId ||
                                        m.ModName.Equals(mod.ModName, StringComparison.OrdinalIgnoreCase));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
                if (foundMod == null)
                {
                    Mods.Add(mod);
                }
                else
                {
                    foundMod.UpdateValues(mod, status);
                }
            }
        }
        private string GetWorkshopPath(string path)
        {
            return Path.Combine(path, "!Workshop");
        }
        public void PrintModOverview()
        {
            Console.WriteLine($"{Mods.Count} total mods in data (excluding dependencies). {Mods.Sum(x => x.Dependencies.Count())} dependencies counted.");
            Console.WriteLine($"Current Mods: {Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod)).Count()}");
            Console.WriteLine($"Previous Mods: {Mods.Where(x => x.Status.HasFlag(ModpackStatus.PreviousMod)).Count()}");
            Console.WriteLine($"Installed Mods: {Mods.Where(x => x.Status.HasFlag(ModpackStatus.Installed)).Count()}");

            var invalidIDMods = Mods.Where(x => x.ModId == 0);
            if (invalidIDMods.Any())
            {
                Console.WriteLine($"\nMods with ID of 0: {invalidIDMods.Count()}\n");
                foreach (var mod in invalidIDMods)
                {
                    PrintMod(mod);
                }
            }

            var unmatchedMods = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod) && !x.Status.HasFlag(ModpackStatus.Installed));
            if (unmatchedMods.Any())
            {
                Console.WriteLine($"\nCurrent Mods Which Are Not Installed: {unmatchedMods.Count()}\n");
                foreach (var mod in unmatchedMods)
                {
                    PrintMod(mod);
                }
            }
        }
        public void PrintMods()
        {
            Console.WriteLine($"Count: {Mods.Count}");
            foreach (ModModel mod in Mods)
            {
                Console.WriteLine($"Name: {mod.ModName}");
                Console.WriteLine($"Link: {mod.ModLink}");
                Console.WriteLine($"Id: {mod.ModId}");
                Console.WriteLine($"Size: {mod.Size}");
                Console.WriteLine($"Status: {mod.Status}");
                Console.WriteLine($"Directory Present: {mod.Directory?.Exists ?? false}");
                Console.WriteLine($"Dependencies: {String.Join(" | ", mod.Dependencies.Select(x => x.ModName))}");
                Console.WriteLine();
            }
        }
        public void PrintMod(ModModel mod)
        {
            Console.WriteLine($"Name: {mod.ModName}");
            Console.WriteLine($"Link: {mod.ModLink}");
            Console.WriteLine($"Id: {mod.ModId}");
            Console.WriteLine($"Size: {mod.Size}");
            Console.WriteLine($"Status: {mod.Status}");
            Console.WriteLine($"Directory Present: {mod.Directory?.Exists ?? false}");
            Console.WriteLine($"Dependencies: {String.Join(" | ", mod.Dependencies.Select(x => x.ModName))}");
            Console.WriteLine();
        }
    }
}
