namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public void CalculateModpackSize()
        {
            var list = Mods.Where(x => x.Status.HasFlag(ModpackStatus.CurrentMod));
            foreach (var mod in list)
            {
                if (mod.Directory?.Exists ?? false)
                    mod.Size = GetDirectorySize(mod.Directory);
            }
        }
        private long GetDirectorySize(DirectoryInfo directory)
        {
            long size = 0;

            // Add file sizes.
            FileInfo[] fis = directory.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = directory.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetDirectorySize(di);
            }

            return size;
        }
    }
}
