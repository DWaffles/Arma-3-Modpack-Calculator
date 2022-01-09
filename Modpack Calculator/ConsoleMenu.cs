namespace ModpackCalculator
{
    internal class ConsoleMenu
    {
        private Config Config { get; set; } = new();
        private ModManager ModManager { get; set; } = new();
        public async Task StartAsync()
        {
            Console.WriteLine("Starting Arma 3 Modpack Size Calculator");

            Config = ConfigHelper.ReadConfig();

            if (!string.IsNullOrEmpty(Config.CurrentModpackPath))
            {
                Console.WriteLine($"Automatically importing previous modpack from: {Config.CurrentModpackPath}");
                await ModManager.ReadFromCurrentHTMLAsync(Config.CurrentModpackPath);
            }
            if (!string.IsNullOrEmpty(Config.ArmaPath))
            {
                Console.WriteLine($"Automatically using previous Arma directory: {Config.ArmaPath}");
                ModManager.ReadFromInstalled(Config.ArmaPath);
            }
            await DebugAsync();
        }
        public async Task DebugAsync()
        {
            //var num3 = ModManager.ReadFromPreviousHTMLAsync();

            await ModManager.PopulateDependenciesAsync();
            ModManager.CalculateModpackSize();
            ModManager.PrintModOverview();
            Console.WriteLine("Exporting");
            ModManager.ExportAsCSV();
            Console.WriteLine("Exported");
        }
        private void MenuDriver()
        {

        }
    }
}
