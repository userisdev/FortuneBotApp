using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FortuneBotApp
{
    /// <summary> BloodGenerator class. </summary>
    internal sealed class BloodGenerator
    {
        /// <summary> The map </summary>
        private readonly Dictionary<BloodType, BloodItem> map = new Dictionary<BloodType, BloodItem>();

        /// <summary> The last updated </summary>
        private DateTime lastUpdated = DateTime.MinValue;

        /// <summary> Gets the item. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public async Task<BloodItem> GetItem(BloodType type)
        {
            if (DateTime.Today > lastUpdated || map.Values.Any(e => e.Rank == 0))
            {
                await UpdateAsync();
            }

            return map.TryGetValue(type, out BloodItem tmp) ? tmp : BloodItem.Empty;
        }

        /// <summary> Gets the ranking. </summary>
        /// <returns> </returns>
        public async Task<IEnumerable<BloodType>> GetRankingAsync()
        {
            if (DateTime.Today > lastUpdated|| map.Values.Any(e => e.Rank == 0))
            {
                await UpdateAsync();
            }

            return map.Values.OrderBy(e => e.Rank).Select(e => e.Type).ToArray();
        }

        /// <summary> Downloads the HTML asynchronous. </summary>
        /// <param name="url"> The URL. </param>
        /// <returns> </returns>
        /// <exception cref="System.Net.Http.HttpRequestException"> HTTP request failed with status code {response.StatusCode} </exception>
        private static async Task<byte[]> DownloadHtmlAsync(string url)
        {
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Url/{url}");
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                // エラーの場合は例外などの適切な処理を行う
                throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
            }
        }

        /// <summary> Extracts the blood type links. </summary>
        /// <param name="htmlText"> The HTML text. </param>
        /// <returns> </returns>
        private static Dictionary<string, string> ExtractBloodTypeLinks(string htmlText)
        {
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(htmlText);

            Dictionary<string, string> bloodTypeLinks = document.QuerySelectorAll("ul.bloodtype li a")
                .ToDictionary(
                    link => link.QuerySelector("p")?.TextContent?.Trim(),
                    link => link.GetAttribute("href")
                );

            return bloodTypeLinks;
        }

        /// <summary> Extracts the job. </summary>
        /// <param name="document"> The document. </param>
        /// <returns> </returns>
        private static BloodJobRecord ExtractJob(IHtmlDocument document)
        {
            IElement div = document.QuerySelector(".blue");
            IElement p = div?.QuerySelector("p");
            string content = p?.TextContent ?? "---";

            return new BloodJobRecord(content);
        }

        /// <summary> Extracts the love. </summary>
        /// <param name="document"> The document. </param>
        /// <returns> </returns>
        private static BloodLoveRecord ExtractLove(IHtmlDocument document)
        {
            IElement div = document.QuerySelector(".pink");
            IElement p = div?.QuerySelector("p");
            string content = p?.TextContent ?? "---";

            return new BloodLoveRecord(content);
        }

        /// <summary> Extracts the rank. </summary>
        /// <param name="document"> The document. </param>
        /// <returns> </returns>
        private static int ExtractRank(IHtmlDocument document)
        {
            IElement img = document.QuerySelector("div.mainbloodtype > img");
            string src = img?.GetAttribute("src") ?? string.Empty;

            Match match = Regex.Match(src, @"rank(\d{2})\.png");

            // マッチした部分を表示
            if (!match.Success)
            {
                return 0;
            }

            string result = match.Groups[1].Value;
            return int.TryParse(result, out int tmp) ? tmp : 0;
        }

        /// <summary> Extracts the total. </summary>
        /// <param name="document"> The document. </param>
        /// <returns> </returns>
        private static BloodTotalRecord ExtractTotal(IHtmlDocument document)
        {
            IElement div = document.QuerySelector(".pink");
            IElement p = div?.QuerySelector("p");
            string content = p?.TextContent ?? "---";

            AngleSharp.Dom.IElement[] list = document.QuerySelectorAll("ul > li").ToArray();
            if (list.Length < 2)
            {
                return new BloodTotalRecord(content, "---", "---");
            }

            string color = list[0]?.QuerySelector("p")?.TextContent ?? "---";
            string word = list[1]?.QuerySelector("p")?.TextContent ?? "---";

            return new BloodTotalRecord(content, color, word);
        }

        /// <summary> Gets the blood item asyn. </summary>
        /// <param name="type"> The type. </param>
        /// <param name="path"> The path. </param>
        /// <returns> </returns>
        private static async Task<BloodItem> GetBloodItemAsync(BloodType type, string path)
        {
            string url = $"https://uranai.d-square.co.jp/{path}";
            byte[] data = await DownloadHtmlAsync(url);
            string htmlText = Encoding.UTF8.GetString(data);

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(htmlText);

            int rank = ExtractRank(document);
            BloodTotalRecord total = ExtractTotal(document);
            BloodLoveRecord love = ExtractLove(document);
            BloodJobRecord job = ExtractJob(document);

            return new BloodItem(type, rank, total.Content, total.Color, total.Word, love.Content, job.Content, url);
        }

        /// <summary> Updates this instance. </summary>
        private async Task UpdateAsync()
        {
            try
            {
                DateTime now = DateTime.Today;
                lastUpdated = now;

                byte[] data = await DownloadHtmlAsync("https://uranai.d-square.co.jp/bloodtype_today.html");
                string htmlText = Encoding.UTF8.GetString(data);

                map.Clear();

                Dictionary<string, string> links = ExtractBloodTypeLinks(htmlText);
                foreach (KeyValuePair<string, string> item in links)
                {
                    switch (item.Key)
                    {
                        case "A型":
                            map.Add(BloodType.A, await GetBloodItemAsync(BloodType.A, item.Value));
                            break;

                        case "B型":
                            map.Add(BloodType.B, await GetBloodItemAsync(BloodType.B, item.Value));
                            break;

                        case "AB型":
                            map.Add(BloodType.AB, await GetBloodItemAsync(BloodType.AB, item.Value));
                            break;

                        case "O型":
                            map.Add(BloodType.O, await GetBloodItemAsync(BloodType.O, item.Value));
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                lastUpdated = DateTime.MinValue;
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Exception/{ex}");
            }
        }
    }
}
