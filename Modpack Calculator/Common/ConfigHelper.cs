using Newtonsoft.Json;
using System.Text;

namespace ModpackCalculator
{
    internal static class ConfigHelper
    {
        private static string ConfigName { get; set; } = "config.json";
        public static Config ReadConfig()
        {
            if (!File.Exists(ConfigName))
            {
                string template = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
                File.WriteAllText(ConfigName, template, new UTF8Encoding(false));
                return new Config();
            }

            string json = File.ReadAllText(ConfigName, new UTF8Encoding(false));
            return JsonConvert.DeserializeObject<Config>(json) ?? new Config();
        }
        public static Config CheckConfig(Config config)
        {
            return config;
        }
        public static void OutputConfig(Config config = null)
        {
            string json = JsonConvert.SerializeObject(config ?? new Config(), Formatting.Indented);
            File.WriteAllText(ConfigName, json, new UTF8Encoding(false));
        }
    }
    internal class Config
    {
        [JsonProperty("CurrentModpackPath")]
        public string CurrentModpackPath { get; set; } = String.Empty;

        [JsonProperty("PreviousModpackPath")]
        public string PreviousModpackPath { get; set; } = String.Empty;

        [JsonProperty("ArmaPath")]
        public string ArmaPath { get; set; } = String.Empty;
    }
}
