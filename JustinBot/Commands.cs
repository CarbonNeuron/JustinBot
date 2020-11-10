using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon.Polly;
using Amazon.Polly.Model;
using Castle.Core.Internal;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Lavalink4NET.Player;

namespace JustinBot
{
    public class Commands : BaseCommandModule
    {
        [Command("leave")]
        [RequireRolesAttribute(RoleCheckMode.Any, new[] {"Justin Access"})]
        public async Task leave(CommandContext ctx, [RemainingText] string textToSpeak)
        {
            var player = Program.audioService.GetPlayer<QueuedLavalinkPlayer>(ctx.Guild.Id) 
                         ?? await Program.audioService.JoinAsync<QueuedLavalinkPlayer>(ctx.Guild.Id,ctx.Member.VoiceState.Channel.Id);
            await player.DisconnectAsync();
            await player.DestroyAsync();
        }

        [Command("speak")]
        [RequireRolesAttribute(RoleCheckMode.Any, new []{"Justin Access"})]
        public async Task speak(CommandContext ctx, [RemainingText] string textToSpeak)
        {
            var player = Program.audioService.GetPlayer<QueuedLavalinkPlayer>(ctx.Guild.Id) 
                         ?? await Program.audioService.JoinAsync<QueuedLavalinkPlayer>(ctx.Guild.Id,ctx.Member.VoiceState.Channel.Id);

            var ActualVoice = VoiceId.Justin;
            foreach (var user in ctx.Message.MentionedUsers)
            {
                Console.WriteLine(user.Mention.ToString());
                var DisMem = await ctx.Guild.GetMemberAsync(user.Id);
                var callout = DisMem.Nickname.IsNullOrEmpty() ? DisMem.DisplayName : DisMem.Nickname;
                textToSpeak = textToSpeak.Replace(user.Mention.ToString(), callout);
            }
            var SpeechResponse = await Program.Polly.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
            {
                Engine = Engine.Neural,
                LanguageCode = LanguageCode.EnUS,
                OutputFormat = OutputFormat.Mp3,
                SampleRate = "24000",
                TextType = TextType.Text,
                Text = textToSpeak,
                VoiceId = ActualVoice
            });
                
            var g = Guid.NewGuid();
            string path = $@"{Settings.PersistentSettings.VoicePath}{g}.Mp3";
            FileStream f = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite);
            await SpeechResponse.AudioStream.CopyToAsync(f);
            f.Flush();
            f.Close();
            var track = await Program.audioService.GetTrackAsync(HttpUtility.UrlEncode(path));
            // play track
            await player.PlayAsync(track);
            await ctx.Message.DeleteAsync();
            //await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
        }
        
        [Command("speakdump")]
        [Aliases("s2f")]
        [RequireRolesAttribute(RoleCheckMode.Any, new []{"Justin Access"})]
        public async Task speakdump(CommandContext ctx, [RemainingText] string textToSpeak)
        {
            try
            {
                var ActualVoice = VoiceId.Justin;
                foreach (var user in ctx.Message.MentionedUsers)
                {
                    Console.WriteLine(user.Mention.ToString());
                    var DisMem = await ctx.Guild.GetMemberAsync(user.Id);
                    var callout = DisMem.Nickname.IsNullOrEmpty() ? DisMem.DisplayName : DisMem.Nickname;
                    textToSpeak = textToSpeak.Replace(user.Mention.ToString(), callout);
                }
                var SpeechResponse = await Program.Polly.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
                {
                    Engine = Engine.Neural,
                    LanguageCode = LanguageCode.EnUS,
                    OutputFormat = OutputFormat.Mp3,
                    SampleRate = "24000",
                    TextType = TextType.Text,
                    Text = textToSpeak,
                    VoiceId = ActualVoice
                });
                
                var g = Guid.NewGuid();
                string path = $@"{g}.Mp3";
                await ctx.RespondWithFileAsync($"{textToSpeak.Truncate(200)}.Mp3", SpeechResponse.AudioStream);
                await ctx.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        
    }
}