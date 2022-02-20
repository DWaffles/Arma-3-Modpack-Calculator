namespace ModpackCalculator.SpectreMenu
{
    internal partial class SpectreMenu
    {
        [InterfaceChoice(ChoiceGroups.ExportGroup, "Export to CSV")]
        public void ExportToCSV()
        {
            ModManager.ExportAsFormattedCSV();
        }
    }
}
