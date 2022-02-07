using ModpackCalculator.SpectreMenu;
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
        
        private void AddMods(IEnumerable<ModModel> mods, ModStatus status)
        {
            foreach (var mod in mods)
            {
                ModModel? foundMod = Mods.SingleOrDefault(m => m.Equals(mod));
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
    }
}
