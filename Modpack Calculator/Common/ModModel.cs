﻿using ByteSizeLib;

namespace ModpackCalculator
{
    internal class ModModel
    {
        public ulong ModId { get; set; } = 0;
        public string ModName { get; set; } = null;
        public string ModLink { get; set; } = null;
        public long Bytes { get; set; } = 0; //bytes
        public ModStatus Status { get; set; } = ModStatus.Default;
        public ByteSize Size
            => ByteSize.FromBytes(Bytes);
        public bool NewlyAdded
            => Status.HasFlag(ModStatus.CurrentMod) && !Status.HasFlag(ModStatus.PreviousMod);
        public bool Removed
            => Status.HasFlag(ModStatus.PreviousMod) && !Status.HasFlag(ModStatus.CurrentMod);
        // Steam Metadeta // Date Published/Updated
        public DirectoryInfo Directory { get; set; } = null;
        public List<ModModel> Dependencies { get; set; } = new();
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
    }
}
