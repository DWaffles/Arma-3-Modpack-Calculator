using AngleSharp;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public async Task<int> ReadFromCurrentHTMLAsync(string path)
        {
            // Clear current mods first
            var currentMods = await ReadFromHTMLAsync(path, ModStatus.CurrentMod);
            AddMods(currentMods, ModStatus.CurrentMod);
            return currentMods.Count;
        }
        public async Task<int> ReadFromPreviousHTMLAsync(string path)
        {
            // Clear previous mods first
            var previousMods = await ReadFromHTMLAsync(path, ModStatus.PreviousMod);
            AddMods(previousMods, ModStatus.PreviousMod);
            return previousMods.Count;
        }
        private static async Task<List<ModModel>> ReadFromHTMLAsync(string path, ModStatus status)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(File.ReadAllText(path)));

            var table = document.QuerySelectorAll(".mod-list > table:nth-child(1) > tbody:nth-child(1)");

            if (table.Any())
            {
                List<ModModel> readMods = new();
                foreach (var item in table.ElementAt(0).Children)
                {
                    string name = item.Children.ElementAt(0).TextContent;
                    string modLink = item.Children.ElementAt(2).TextContent;

                    ModModel mod = new()
                    {
                        ModName = name,
                        ModLink = modLink.StripSpecialCharacters().Trim(),
                        ModId = modLink.GetModId() ?? 0,
                        Status = status,
                    };
                    readMods.Add(mod);
                }
                document.Close();
                return readMods;
            }
            document.Close();
            return new List<ModModel>();
        }
        public int ReadFromInstalled(string path)
        {
            if (!GetWorkshopDirectory(path, out var armaDir))
                throw new DirectoryNotFoundException($"{path} cannot be found.");

            List<ModModel> installedMods = new();

            foreach (var dir in armaDir.GetDirectories())
            {
                string filePath = dir.FullName + Path.DirectorySeparatorChar + "meta.cpp";
                if (File.Exists(filePath))
                {
                    using var reader = new StreamReader(filePath);
                    reader.ReadLine(); // disregarding first line

                    var returnedId = (reader.ReadLine() ?? String.Empty).GetModId(); //get second line that has publishedid
                    if (returnedId != null)
                    {
                        string name = reader.ReadLine() ?? string.Empty; //meta.cpp name lines are [name = "Mod Name";]
                        if (name.StartsWith("name = ") && name.EndsWith("\";"))
                        {
                            name = name.Remove(name.Length - 2).Substring(8);
                        }
                        ModModel mod = new()
                        {
                            ModName = name,
                            ModId = returnedId.Value,
                            Status = ModStatus.Installed,
                            Directory = dir,
                        };
                        installedMods.Add(mod);
                    }
                }
            }
            AddMods(installedMods, ModStatus.Installed);
            return installedMods.Count;
        }
        private static bool GetWorkshopDirectory(string path, out DirectoryInfo directory)
        {
            if (!String.IsNullOrEmpty(path) && new DirectoryInfo(path).Exists && (directory = new DirectoryInfo(Path.Combine(path, "!Workshop"))).Exists)
                return true;
            else
            {
                directory = null;
                return false;
            }
        }
        private static async Task<ModModel> ScrapeModDependenciesAsync(ModModel scrapeMod)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(scrapeMod.ModLink!);

            var requiredItems = document.QuerySelectorAll("#RequiredItems"); // https://stackoverflow.com/questions/32427674/how-to-extract-data-from-website-using-anglesharp-linq

            if (requiredItems.Any())
            {
                var page = requiredItems.ElementAt(0);
                foreach (var child in page.Children)
                {
                    var modId = (child.GetAttribute("Href") ?? String.Empty).GetModId();
                    var modName = child.TextContent.StripSpecialCharacters();
                    if (!scrapeMod.Dependencies.Where(x => x.ModId == modId || x.ModName!.Equals(modName, StringComparison.OrdinalIgnoreCase)).Any())
                    {
                        ModModel modDependency = new()
                        {
                            ModLink = child.GetAttribute("Href") ?? String.Empty,
                            ModName = modName,
                            ModId = modId ?? 0,
                            Status = ModStatus.Dependency
                        };
                        scrapeMod.Dependencies.Add(modDependency);
                    }
                }
            }
            return scrapeMod;
        }
    }
}