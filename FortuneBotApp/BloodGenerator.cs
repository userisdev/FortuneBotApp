using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

        /// <summary> Returns true if ... is valid. </summary>
        /// <value> <c> true </c> if this instance is valid; otherwise, <c> false </c>. </value>
        public bool IsValid => !map.Values.Any(e => e.Rank == 0);

        /// <summary> Gets a value indicating whether this <see cref="BloodGenerator" /> is updating. </summary>
        /// <value> <c> true </c> if updating; otherwise, <c> false </c>. </value>
        public bool Updating { get; private set; } = false;

        /// <summary> Gets the item. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public BloodItem GetItem(BloodType type)
        {
            return map.TryGetValue(type, out BloodItem tmp) ? tmp : BloodItem.Empty;
        }

        /// <summary> Gets the ranking. </summary>
        /// <returns> </returns>
        public IEnumerable<BloodType> GetRanking()
        {
            return map.Values.OrderBy(e => e.Rank).Select(e => e.Type).ToArray();
        }

        /// <summary> Updates this instance. </summary>
        public async Task UpdateAsync()
        {
            if (lastUpdated >= DateTime.Today && !map.Values.Any(e => e.Rank == 0))
            {
                return;
            }

            try
            {
                Updating = true;
                DateTime now = DateTime.Today;
                lastUpdated = now;

                byte[] data = await GetRequestAsync("https://uranai.d-square.co.jp/bloodtype_today.html");
                string htmlText = ConvertHtml(data);

                if (lastUpdated < DateTime.Today)
                {
                    map.Clear();
                }

                Dictionary<string, string> links = ExtractBloodTypeLinks(htmlText);
                foreach (KeyValuePair<string, string> item in links)
                {
                    switch (item.Key)
                    {
                        case "A型":
                            if (!map.ContainsKey(BloodType.A) || !map[BloodType.A].IsValid)
                            {
                                map[BloodType.A] = await GetBloodItemAsync(BloodType.A, item.Value);
                            }
                            break;

                        case "B型":
                            if (!map.ContainsKey(BloodType.B) || !map[BloodType.B].IsValid)
                            {
                                map[BloodType.B] = await GetBloodItemAsync(BloodType.B, item.Value);
                            }
                            break;

                        case "AB型":
                            if (!map.ContainsKey(BloodType.AB) || !map[BloodType.AB].IsValid)
                            {
                                map[BloodType.AB] = await GetBloodItemAsync(BloodType.AB, item.Value);
                            }
                            break;

                        case "O型":
                            if (!map.ContainsKey(BloodType.O) || !map[BloodType.O].IsValid)
                            {
                                map[BloodType.O] = await GetBloodItemAsync(BloodType.O, item.Value);
                            }
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
            finally
            {
                Updating = false;
            }
        }

        /// <summary> Converts the HTML. </summary>
        /// <param name="data"> The data. </param>
        /// <returns> </returns>
        private static string ConvertHtml(byte[] data)
        {
            try
            {
                if (data[0] == 0x1f && data[1] == 0x8b)
                {
                    using (MemoryStream compressedStream = new MemoryStream(data))
                    using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        decompressionStream.CopyTo(decompressedStream);
                        return Encoding.UTF8.GetString(decompressedStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Exception/{ex}");
            }

            return Encoding.UTF8.GetString(data);
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
            string content = p?.TextContent ?? string.Empty;

            return new BloodJobRecord(content);
        }

        /// <summary> Extracts the love. </summary>
        /// <param name="document"> The document. </param>
        /// <returns> </returns>
        private static BloodLoveRecord ExtractLove(IHtmlDocument document)
        {
            IElement div = document.QuerySelector(".pink");
            IElement p = div?.QuerySelector("p");
            string content = p?.TextContent ?? string.Empty;

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
            string content = p?.TextContent ?? string.Empty;

            IElement[] list = document.QuerySelectorAll("ul > li").ToArray();
            if (list.Length < 2)
            {
                return new BloodTotalRecord(content, string.Empty, string.Empty);
            }

            string color = list[0]?.QuerySelector("p")?.TextContent ?? string.Empty;
            string word = list[1]?.QuerySelector("p")?.TextContent ?? string.Empty;

            return new BloodTotalRecord(content, color, word);
        }

        /// <summary> Gets the blood item asyn. </summary>
        /// <param name="type"> The type. </param>
        /// <param name="path"> The path. </param>
        /// <returns> </returns>
        private static async Task<BloodItem> GetBloodItemAsync(BloodType type, string path)
        {
            string url = $"https://uranai.d-square.co.jp/{path}";
            byte[] data = await GetRequestAsync(url);
            string htmlText = ConvertHtml(data);

            File.WriteAllBytes($"{type}.dmp", data);

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(htmlText);

            int rank = ExtractRank(document);
            BloodTotalRecord total = ExtractTotal(document);
            BloodLoveRecord love = ExtractLove(document);
            BloodJobRecord job = ExtractJob(document);

            return new BloodItem(type, rank, total.Content, total.Color, total.Word, love.Content, job.Content, url);
        }

        /// <summary> Gets the request. </summary>
        /// <param name="url"> The URL. </param>
        /// <returns> </returns>
        /// <exception cref="System.Net.Http.HttpRequestException"> Failed to fetch {endpoint} </exception>
        private static async Task<byte[]> GetRequestAsync(string url)
        {
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Url/{url}");

            HttpClient httpClient = HttpClientFactory.Create();
            HttpResponseMessage response = await httpClient.GetAsync(url);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : throw new HttpRequestException($"Failed to fetch {url}");
        }
    }
}
