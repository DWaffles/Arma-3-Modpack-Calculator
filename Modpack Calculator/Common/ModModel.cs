namespace ModpackCalculator
{
    internal class ModModel
    {
        public ulong ModId { get; set; } = 0;
        public string ModName { get; set; } = null;
        public string ModLink { get; set; } = null;
        public long Size { get; set; } = 0; //bytes
        public ModpackStatus Status { get; set; } = ModpackStatus.Default;
        public bool NewlyAdded
            => Status.HasFlag(ModpackStatus.CurrentMod) && !Status.HasFlag(ModpackStatus.PreviousMod);
        public bool Removed
            => Status.HasFlag(ModpackStatus.PreviousMod) && !Status.HasFlag(ModpackStatus.CurrentMod);
        public DirectoryInfo Directory { get; set; } = null;
        public List<ModModel> Dependencies { get; set; } = new();
        public void UpdateValues(ModModel updateMod, ModpackStatus addStatus = ModpackStatus.Default)
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
