using ByteSizeLib;

namespace ModpackCalculator
{
    internal partial class ModManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A named tuple, the first value being the count of mods whose dependencies were populated, and the second value being the count of dependencies.</returns>
        public async Task<(int ModsPopulated, int Dependencies)> CalculateDependenciesAsync(IProgress<DependencyProgress>? progress = null)
        {
            int count = 0;
            var list = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));

            var tasks = list.Select(async mod =>
            {
                var response = await ScrapeModDependenciesAsync(mod);
                if (progress != null)
                {
                    var arg = new DependencyProgress()
                    {
                        Calculated = Interlocked.Add(ref count, 1),
                        Total = list.Count(),
                    };
                    progress.Report(arg);
                }
            });
            await Task.WhenAll(tasks);

            return (list.Count(), list.Sum(x => x.Dependencies.Count));
        }
        public double CalculateModpackSize() //return num mods calculated
        {
            long modpackSize = 0;
            var list = Mods.Where(x => x.Status.HasFlag(ModStatus.CurrentMod));
            foreach (var mod in list)
            {
                if (mod.Directory?.Exists ?? false)
                {
                    mod.Bytes = GetDirectorySize(mod.Directory);
                    modpackSize += mod.Bytes;
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
