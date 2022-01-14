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
    }
}
