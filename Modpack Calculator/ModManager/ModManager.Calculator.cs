using ByteSizeLib;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        public double CalculateModpackSize()
        {
            long modpackSize = 0;
            var list = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));
            foreach (var mod in list)
            {
                if (mod.Directory?.Exists ?? false)
                {
                    mod.Size = GetDirectorySize(mod.Directory);
                    modpackSize += mod.Size;
                }
            }
            return ByteSize.FromBytes(modpackSize).MegaBytes;
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
