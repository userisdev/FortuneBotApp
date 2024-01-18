﻿using Discord;
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
        /// <summary> The client </summary>
        private readonly DiscordSocketClient client;

        /// <summary> The token </summary>
        private readonly string token;

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
                .AddOption("type", ApplicationCommandOptionType.String, "A, B, AB, O", isRequired: true)
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
            string type = (command.Data.Options.FirstOrDefault()?.Value as string) ?? string.Empty;
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Blood/{type}");
            await command.RespondAsync(type);
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
            (string zodiac, string birthday) = GetZodiacOption(command.Data.Options);
            (int month, int day) = GetDate(birthday);

            ZodiacType signFromZodiac = ZodiacUtility.GetType(zodiac);
            ZodiacType signFromBirthday = ZodiacUtility.GetType(month, day);

            ZodiacType sign = SelectSign(signFromZodiac, signFromBirthday);
            string description = ZodiacUtility.GetDescription(sign);

            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Fortune Zodiac/[{zodiac}]/[{birthday}]/{sign}");

            await command.RespondAsync($"{sign}/{description}");
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
