using Config.Net;

namespace JustinBot
{
    public static class Settings
    {
        public static PSettings PersistentSettings =
            new ConfigurationBuilder<PSettings>().UseEnvironmentVariables().UseJsonFile("settings.json").Build();
    }

    public interface PSettings
    {
        [Option(Alias = "AWSToken")]
        public string AWSToken { get; set; }
        
        [Option(Alias = "BotToken")]
        public string BotToken { get; set; }
    }
}