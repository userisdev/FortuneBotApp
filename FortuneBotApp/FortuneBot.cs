using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FortuneBotApp
{
    /// <summary> FortuneBot class. </summary>
    internal sealed class FortuneBot
    {
        /// <summary> The blood gen </summary>
        private readonly BloodGenerator bloodGen = new BloodGenerator();

        /// <summary> The client </summary>
        private readonly DiscordSocketClient client;

        /// <summary> The token </summary>
        private readonly string token;

        /// <summary> The zodiac gen </summary>
        private readonly ZodiacGenerator zodiacGen = new ZodiacGenerator();

        /// <summary> Initializes a new instance of the <see cref="FortuneBot" /> class. </summary>
        /// <param name="token"> The token. </param>
        public FortuneBot(string token)
        {
            this.token = token;

            client = new DiscordSocketClient();

            client.Log += OnLog;
            client.Ready += OnReady;
            client.SlashCommandExecuted += SlashCommandHandler;
        }

        /// <summary> Runs the asynchronous. </summary>
        public async Task RunAsync()
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        /// <summary> Updates the asynchronous. </summary>
        public async Task UpdateAsync()
        {
            await zodiacGen.UpdateAsync();
            await bloodGen.UpdateAsync();
        }

        /// <summary> Generates the log text. </summary>
        /// <param name="log"> The log. </param>
        /// <returns> </returns>
        private static string GenerateLogText(LogMessage log)
        {
            return $"{log.Source} {log.Message}";
        }

        /// <summary> Generates the slash commands. </summary>
        /// <returns> </returns>
        private static IEnumerable<SlashCommandProperties> GenerateSlashCommands()
        {
            yield return new SlashCommandBuilder()
                .WithName("fortune")
                .WithDescription("おみくじコマンド")
                .Build();

            yield return new SlashCommandBuilder()
                .WithName("fortune_blood")
                .WithDescription("血液型占いコマンド")
                .AddOption("type", ApplicationCommandOptionType.String, "A, B, AB, O")
                .Build();

            yield return new SlashCommandBuilder()
                .WithName("fortune_zodiac")
                .WithDescription("星座占いコマンド")
                .AddOption("zodiac", ApplicationCommandOptionType.String, "zodiac sign.")
                .AddOption("birthday", ApplicationCommandOptionType.String, "month/day.")
                .Build();
        }

        /// <summary> Gets the date. </summary>
        /// <param name="birthdate"> The birthdate. </param>
        /// <returns> </returns>
        private static (int, int) GetDate(string birthdate)
        {
            try
            {
                string[] s = birthdate.Split('/');
                if (s.Length == 2)
                {
                    int month = int.Parse(s[0]);
                    int day = int.Parse(s[1]);
                    return (month, day);
                }

                return (0, 0);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        /// <summary> Gets the zodiac option. </summary>
        /// <param name="options"> The options. </param>
        /// <returns> </returns>
        private static (string, string) GetZodiacOption(IEnumerable<SocketSlashCommandDataOption> options)
        {
            string zodiac = string.Empty;
            string birthday = string.Empty;

            foreach (SocketSlashCommandDataOption option in options)
            {
                if (option.Name == "zodiac")
                {
                    zodiac = option.Value as string;
                }
                else if (option.Name == "birthday")
                {
                    birthday = option.Value as string;
                }
            }

            return (zodiac, birthday);
        }

        /// <summary> Selects the sign. </summary>
        /// <param name="fromZodiac"> From zodiac. </param>
        /// <param name="fromBirthday"> From birthday. </param>
        /// <returns> </returns>
        private static ZodiacType SelectSign(ZodiacType fromZodiac, ZodiacType fromBirthday)
        {
            return fromZodiac == fromBirthday ? fromZodiac : fromBirthday == ZodiacType.Invalid ? fromZodiac : fromBirthday;
        }

        /// <summary> Fortunes the slash command handler. </summary>
        /// <param name="command"> The command. </param>
        private async Task FortuneBloodSlashCommandHandler(SocketSlashCommand command)
        {
            if (bloodGen.Updating)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Blood Updating");
                await command.RespondAsync("blood now updating.");
                return;
            }

            BloodType type = BloodUtility.GetType((command.Data.Options.FirstOrDefault()?.Value as string) ?? string.Empty);
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Blood/{type}");

            try
            {
                if (type == BloodType.Invalid)
                {
                    if (!bloodGen.IsValid)
                    {
                        Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Blood Error");
                        await command.RespondAsync("blood error.");
                        return;
                    }

                    IEnumerable<BloodType> ranking = bloodGen.GetRanking();
                    EmbedBuilder builder = new EmbedBuilder().WithTitle("血液型占い ランキング");

                    foreach ((int rank, BloodType e) in ranking.Select((e, i) => (i + 1, e)))
                    {
                        _ = builder.AddField($"{rank}位", $"{e}型");
                    }

                    _ = builder.AddField("\u200B", "[占いスクエア 今日の血液型占い](https://uranai.d-square.co.jp/bloodtype_today.html)");
                    Embed embed = builder.Build();

                    await command.RespondAsync(embed: embed);
                }
                else
                {
                    BloodItem item = bloodGen.GetItem(type);
                    EmbedBuilder builder = new EmbedBuilder().WithTitle($"{item.Type}型");
                    _ = builder.AddField($"総合運", $"{item.Total}");
                    _ = builder.AddField($"ラッキーカラー", $"{item.Color}");
                    _ = builder.AddField($"ラッキーワード", $"{item.Word}");
                    _ = builder.AddField($"恋愛運", $"{item.Love}");
                    _ = builder.AddField($"仕事運", $"{item.Job}");
                    _ = builder.AddField("\u200B", $"[占いスクエア 今日の血液型占い]({item.Url})");
                    Embed embed = builder.Build();

                    await command.RespondAsync(embed: embed);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary> Fortunes the slash command handler. </summary>
        /// <param name="command"> The command. </param>
        private async Task FortuneSlashCommandHandler(SocketSlashCommand command)
        {
            string imagePath = FortuneImageFinder.Find();
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune/{imagePath}");
            await command.RespondWithFileAsync(imagePath);
        }

        /// <summary> Fortunes the slash command handler. </summary>
        /// <param name="command"> The command. </param>
        private async Task FortuneZodiacSlashCommandHandler(SocketSlashCommand command)
        {
            if (zodiacGen.Updating)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Zodiac Updating");
                await command.RespondAsync("now updating.");
                return;
            }

            (string zodiac, string birthday) = GetZodiacOption(command.Data.Options);
            (int month, int day) = GetDate(birthday);

            ZodiacType signFromZodiac = ZodiacUtility.GetType(zodiac);
            ZodiacType signFromBirthday = ZodiacUtility.GetType(month, day);

            ZodiacType sign = SelectSign(signFromZodiac, signFromBirthday);

            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Zodiac/[{zodiac}]/[{birthday}]/{sign}");

            if (sign == ZodiacType.Invalid)
            {
                IEnumerable<ZodiacType> ranking = zodiacGen.GetRanking();
                EmbedBuilder builder = new EmbedBuilder().WithTitle("星座占い ランキング");

                foreach ((int rank, ZodiacType e) in ranking.Select((e, i) => (i + 1, e)))
                {
                    string emoji = ZodiacUtility.GetEmoji(e);
                    string name = ZodiacUtility.GetJapaneseName(e);
                    _ = builder.AddField($"{rank}位", $"{emoji}{name}");
                }

                _ = builder.AddField("\u200B", "powerd by [JugemKey](http://jugemkey.jp/api/) /【PR】[原宿占い館 塔里木](http://www.tarim.co.jp/)");
                Embed embed = builder.Build();

                await command.RespondAsync(embed: embed);
            }
            else
            {
                ZodiacItem item = zodiacGen.GetItem(sign);
                string emoji = ZodiacUtility.GetEmoji(item.Type);
                string name = ZodiacUtility.GetJapaneseName(item.Type);
                EmbedBuilder builder = new EmbedBuilder().WithTitle($"{emoji}{name}");
                _ = builder.AddField($"総合運", $"{item.Total}");
                _ = builder.AddField($"金運", $"{item.Money}", inline: true);
                _ = builder.AddField($"仕事運", $"{item.Job}", inline: true);
                _ = builder.AddField($"恋愛運", $"{item.Love}", inline: true);
                _ = builder.AddField($"ラッキーアイテム", $"{item.Item}");
                _ = builder.AddField($"ラッキーカラー", $"{item.Color}");
                _ = builder.AddField("\u200B", $"{item.Content}");
                _ = builder.AddField("\u200B", "powerd by [JugemKey](http://jugemkey.jp/api/) /【PR】[原宿占い館 塔里木](http://www.tarim.co.jp/)");
                Embed embed = builder.Build();

                await command.RespondAsync(embed: embed);
            }
        }

        /// <summary> Called when [log]. </summary>
        /// <param name="log"> The log. </param>
        /// <returns> </returns>
        private Task OnLog(LogMessage log)
        {
            string text = GenerateLogText(log);
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : {text}");
            return Task.CompletedTask;
        }

        /// <summary> Clients the ready. </summary>
        private async Task OnReady()
        {
            await client.SetGameAsync("サーバー", type: ActivityType.Watching);

            try
            {
                IEnumerable<SlashCommandProperties> commands = GenerateSlashCommands();
                foreach (SlashCommandProperties command in commands)
                {
                    _ = await client.CreateGlobalApplicationCommandAsync(command);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Exception/{ex}");
            }
        }

        /// <summary> Slashes the command handler. </summary>
        /// <param name="command"> The command. </param>
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "fortune":
                    await FortuneSlashCommandHandler(command);
                    return;

                case "fortune_blood":
                    await FortuneBloodSlashCommandHandler(command);
                    return;

                case "fortune_zodiac":
                    await FortuneZodiacSlashCommandHandler(command);
                    return;

                default:
                    return;
            }
        }
    }
}
