namespace ModpackCalculator
{
    /// <summary>
    /// This enum represents a mod's status in regards to the modpack and current installation status.
    /// </summary>
    [Flags]
    internal enum ModStatus
    {
        /// <summary>
        /// Empty default value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Denotes the mod is in the current modpack.
        /// </summary>
        CurrentMod = 1,

        /// <summary>
        /// Denotes the mod is in the previous comparison modpack.
        /// </summary>
        PreviousMod = 2,

        /// <summary>
        /// Denotes that the mod is a dependency for an other mod.
        /// </summary>
        Dependency = 4,

        /// <summary>
        /// Denotes that the mod is currently installed in the Arma directory.
        /// </summary>
        Installed = 8,

        /// <summary>
        /// Denotes that the mod is not installed in the Arma directory.
        /// </summary>
        NotInstalled = 16,
    }
}
