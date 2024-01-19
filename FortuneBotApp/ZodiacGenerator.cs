using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace FortuneBotApp
{
    /// <summary> ZodiacGenerator class. </summary>
    internal sealed class ZodiacGenerator
    {
        /// <summary> The map </summary>
        private readonly Dictionary<ZodiacType, ZodiacItem> map = new Dictionary<ZodiacType, ZodiacItem>();

        /// <summary> The last updated </summary>
        private DateTime lastUpdated = DateTime.MinValue;

        /// <summary> Gets the item. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public ZodiacItem GetItem(ZodiacType type)
        {
            if (DateTime.Today > lastUpdated)
            {
                Update();
            }

            return map.TryGetValue(type, out ZodiacItem tmp) ? tmp : ZodiacItem.Empty;
        }

        /// <summary> Gets the ranking. </summary>
        /// <returns> </returns>
        public IEnumerable<ZodiacType> GetRanking()
        {
            if (DateTime.Today > lastUpdated)
            {
                Update();
            }

            return map.Values.OrderBy(e => e.Rank).Select(e => e.Type).ToArray();
        }

        /// <summary> Gets the request. </summary>
        /// <param name="endpoint"> The endpoint. </param>
        /// <returns> </returns>
        /// <exception cref="System.Net.Http.HttpRequestException"> Failed to fetch {endpoint} </exception>
        private static string GetRequest(string endpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(endpoint).Result;

                return response.IsSuccessStatusCode
                    ? response.Content.ReadAsStringAsync().Result
                    : throw new HttpRequestException($"Failed to fetch {endpoint}");
            }
        }

        /// <summary> Updates this instance. </summary>
        private void Update()
        {
            try
            {
                DateTime now = DateTime.Today;
                lastUpdated = now;

                string todayText = $"{now:yyyy}/{now:MM}/{now:dd}";
                string endpoint = $"http://api.jugemkey.jp/api/horoscope/free/{todayText}";
                string jsonText = GetRequest(endpoint);

                JObject json = JObject.Parse(jsonText);
                JArray records = json["horoscope"][todayText] as JArray;

                map.Clear();
                foreach (JToken record in records)
                {
                    string content = record["content"].ToString();
                    string item = record["item"].ToString();
                    int money = Convert.ToInt32(record["money"]);
                    int total = Convert.ToInt32(record["total"]);
                    int job = Convert.ToInt32(record["job"]);
                    string color = record["color"].ToString();
                    int love = Convert.ToInt32(record["love"]);
                    int rank = Convert.ToInt32(record["rank"]);
                    string sign = record["sign"].ToString();

                    ZodiacItem zi = new ZodiacItem(content, item, money, total, job, color, love, rank, sign);
                    map.Add(zi.Type, zi);
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
