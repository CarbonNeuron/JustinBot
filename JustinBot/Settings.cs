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
        [Option(Alias = "Polly.AccessKey")]
        public string AWSAccessKey { get; set; }
        
        [Option(Alias = "Polly.SecretKey")]
        public string AWSAccessKeyID { get; set; }
        
        [Option(Alias = "BotToken")]
        public string BotToken { get; set; }
        
        [Option(Alias = "LavaLink", DefaultValue = "")]
        string lavalinkConnectionString { get; }
        
        [Option(Alias = "LavalinkPassword", DefaultValue = "")]
        string lavalinkPassword { get; }
    }
}