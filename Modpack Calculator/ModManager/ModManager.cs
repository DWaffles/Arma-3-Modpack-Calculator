using System.Collections.ObjectModel;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        private List<ModModel> Mods { get; set; } = new();
        public ReadOnlyCollection<ModModel> GetMods()
        {
            return Mods.AsReadOnly();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A named tuple, the first value being the count of mods whose dependencies were populated, and the second value being the count of dependencies.</returns>
        public async Task<(int ModsPopulated, int Dependencies)> PopulateDependenciesAsync()
        {
            var list = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));

            var tasks = list.Select(async mod =>
            {
                var response = await ScrapeModDependenciesAsync(mod);
            });
            await Task.WhenAll(tasks);
            return (list.Count(), list.Sum(x => x.Dependencies.Count));
        }
        private void AddReadMods(IEnumerable<ModModel> mods, ModStatus status)
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
    }
}
