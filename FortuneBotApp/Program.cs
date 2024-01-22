using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace FortuneBotApp
{
    /// <summary> Program class. </summary>
    internal static class Program
    {
        /// <summary> Defines the entry point of the application. </summary>
        /// <param name="args"> The arguments. </param>
        private static async Task Main(string[] args)
        {
            string token = args.FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("token error.");
                Environment.Exit(1);
            }

            string execPath = Assembly.GetExecutingAssembly().Location;
            string dirPath = Path.GetDirectoryName(execPath);
            string execNameWithoutExtension = Path.GetFileNameWithoutExtension(execPath);
            string logFilePath = Path.Combine(dirPath, $"{execNameWithoutExtension}.log");
            Trace.Listeners.Clear();
            _ = Trace.Listeners.Add(new TextWriterTraceListener(logFilePath, "LogFile"));
            _ = Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.AutoFlush = true;

            HttpClient httpClient = HttpClientFactory.Create();
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0";
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            FortuneBot bot = new FortuneBot(token);

            // NOTE:初回更新
            await bot.UpdateAsync();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await bot.UpdateAsync();
                }
            });

            await bot.RunAsync();
            Environment.Exit(0);
        }
    }
}
