using AngleSharp;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public async Task<int> ReadFromCurrentHTMLAsync(string path)
        {
            var currentMods = await ReadFromHTMLAsync(path, ModStatus.CurrentMod);
            AddReadMods(currentMods, ModStatus.CurrentMod);
            return currentMods.Count;
        }
        public async Task<int> ReadFromPreviousHTMLAsync(string path)
        {
            var previousMods = await ReadFromHTMLAsync(path, ModStatus.PreviousMod);
            AddReadMods(previousMods, ModStatus.PreviousMod);
            return previousMods.Count;
        }
        private async Task<List<ModModel>> ReadFromHTMLAsync(string path, ModStatus status)
        {
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
                        ModLink = RegexHelper.StripSpecialCharacters(modLink).Trim(),
                        ModId = RegexHelper.GetModId(modLink) ?? 0,
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
            List<ModModel> installedMods = new();
            DirectoryInfo armaDir = new(path);

            if (!armaDir.Exists)
                throw new DirectoryNotFoundException($"{path} cannot be found.");

            foreach (var dir in armaDir.GetDirectories())
            {
                string filePath = dir.FullName + Path.DirectorySeparatorChar + "meta.cpp";
                if (File.Exists(filePath))
                {
                    using var reader = new StreamReader(filePath);
                    reader.ReadLine(); // disregarding first line

                    var returnedId = RegexHelper.GetModId(reader.ReadLine() ?? String.Empty); //get second line that has publishedid
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
            AddReadMods(installedMods, ModStatus.Installed);
            return installedMods.Count;
        }
        private async Task<ModModel> ScrapeModDependenciesAsync(ModModel scrapeMod)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(scrapeMod.ModLink);

            var requiredItems = document.QuerySelectorAll("#RequiredItems"); // https://stackoverflow.com/questions/32427674/how-to-extract-data-from-website-using-anglesharp-linq

            if (requiredItems.Any())
            {
                var page = requiredItems.ElementAt(0);
                foreach (var child in page.Children)
                {
                    var modId = RegexHelper.GetModId(child.GetAttribute("Href") ?? String.Empty);
                    var modName = RegexHelper.StripSpecialCharacters(child.TextContent);
                    if (!scrapeMod.Dependencies.Where(x => x.ModId == modId || x.ModName.Equals(modName, StringComparison.OrdinalIgnoreCase)).Any())
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