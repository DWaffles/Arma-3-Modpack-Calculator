using ByteSizeLib;

namespace ModpackCalculator
{
    /// <summary>
    /// Represents a Steam Workshop and/or a locally installed mod.
    /// </summary>
    internal class ModModel
    {
        /// <summary>
        /// Gets the Workshop ID of the mod. Under certain circumstances, this can be 0.
        /// </summary>
        public ulong ModId { get; set; } = 0;

        /// <summary>
        /// Gets the name of the mod. Under certain circumstances, this can be different from the name the mod is listed under.
        /// </summary>
        public string? ModName { get; set; } = null;

        /// <summary>
        /// Gets the URL of the mod's Steam Workshop page.
        /// </summary>
        public string? ModLink { get; set; } = null;

        /// <summary>
        /// Gets the mod's size in bytes.
        /// </summary>
        public long Bytes { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public ModStatus Status { get; set; } = ModStatus.Default;

        /// <summary>
        /// Gets the <see cref="ByteSize"/> representation of the mod's size.
        /// </summary>
        public ByteSize Size
            => ByteSize.FromBytes(Bytes);

        /// <summary>
        /// Gets whether or not the mod is newly added to the current modpack.
        /// </summary>
        public bool NewlyAdded
            => Status.HasFlag(ModStatus.CurrentMod) && !Status.HasFlag(ModStatus.PreviousMod);

        /// <summary>
        /// Gets whether or not the mod was removed in the current modpack.
        /// </summary>
        public bool Removed
            => Status.HasFlag(ModStatus.PreviousMod) && !Status.HasFlag(ModStatus.CurrentMod);

        // Steam Metadeta // Date Published/Updated
        /// <summary>
        /// Gets the <see cref="DirectoryInfo"/> pertaining to the mod folder.
        /// </summary>
        public DirectoryInfo? Directory { get; set; } = null;

        /// <summary>
        /// Gets a list of partial <see cref="ModModel"/>'s that the mod depends on.
        /// </summary>
        public List<ModModel> Dependencies { get; set; } = new();

        /// <summary>
        /// Updates the mod's values with the provided <see cref="ModModel"/>.
        /// </summary>
        /// <param name="updateMod"></param>
        /// <param name="addStatus">If provided, it will add the <see cref="ModStatus"/> to the mod's <see cref="ModModel.Status"/>.</param>
        public void UpdateValues(ModModel updateMod, ModStatus addStatus = ModStatus.Default)
        {
            if (ModId == 0)
                ModId = updateMod.ModId;
            if (String.IsNullOrEmpty(ModLink))
                ModLink = updateMod.ModLink;
            if (Directory == null)
                Directory = updateMod.Directory;
            if (!Dependencies.Any())
                Dependencies = updateMod.Dependencies;

            Status |= addStatus;
        }
        public override bool Equals(object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                ModModel compare = (ModModel)obj;
                if ((this.ModId != 0 && this.ModId == compare!.ModId) || this.ModName!.Equals(compare.ModName, StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ModId, ModName);
        }
    }
}
