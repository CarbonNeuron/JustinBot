using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Runtime;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;
using NLog;

namespace JustinBot
{
    class Program
    {
        public static AmazonPollyClient Polly = new AmazonPollyClient(new BasicAWSCredentials(Settings.PersistentSettings.AWSAccessKey, Settings.PersistentSettings.AWSAccessKeyID), RegionEndpoint.USEast1);

        public static DiscordClient discord;
        public static LavalinkNode audioService;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static async Task Main(string[] args)
        {
            _logger.Debug("I'm alive!");
            _logger.Debug("Starting discord....");
            discord = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = Settings.PersistentSettings.BotToken,
                TokenType = TokenType.Bot
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] {".."},
                EnableMentionPrefix = true
            });
            var clientWrapper = new DiscordClientWrapper(discord);
            audioService = new LavalinkNode(new LavalinkNodeOptions
            {
                RestUri = $"http://{Settings.PersistentSettings.lavalinkConnectionString}/",
                WebSocketUri = $"ws://{Settings.PersistentSettings.lavalinkConnectionString}/",
                Password = $"{Settings.PersistentSettings.lavalinkPassword}"
            }, clientWrapper);
            var service = new InactivityTrackingService(
                audioService, // The instance of the IAudioService (e.g. LavalinkNode)
                clientWrapper, // The discord client wrapper instance
                new InactivityTrackingOptions
                {
                    DisconnectDelay = TimeSpan.Zero,
                    TrackInactivity = false
                });
            service.RemoveTracker(DefaultInactivityTrackers.ChannelInactivityTracker);
            service.BeginTracking();
            discord.Ready += DiscordOnReady;
            commands.RegisterCommands<Commands>();
            await discord.ConnectAsync();
            Thread.Sleep(5000);
            await audioService.InitializeAsync();
                //var connections = await discord.GetConnectionsAsync();
            
            await Task.Delay(-1); //Run forever
        }

        private static async Task DiscordOnReady(DiscordClient sender, ReadyEventArgs e)
        {
            await audioService.InitializeAsync();
        }
    }
}