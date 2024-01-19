using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public BloodItem GetItem(BloodType type)
        {
            if (DateTime.Today > lastUpdated)
            {
                Update();
            }

            return map.TryGetValue(type, out BloodItem tmp) ? tmp : BloodItem.Empty;
        }

        /// <summary> Gets the ranking. </summary>
        /// <returns> </returns>
        public IEnumerable<BloodType> GetRanking()
        {
            if (DateTime.Today > lastUpdated)
            {
                Update();
            }

            return map.Values.OrderBy(e => e.Rank).Select(e => e.Type).ToArray();
        }

        /// <summary> Updates this instance. </summary>
        private void Update()
        {
            try
            {
                DateTime now = DateTime.Today;
                lastUpdated = now;

                // https://uranai.d-square.co.jp/bloodtype_today.html

                map.Clear();
                map.Add(BloodType.A, new BloodItem(BloodType.A, 1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                map.Add(BloodType.B, new BloodItem(BloodType.B, 2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                map.Add(BloodType.AB, new BloodItem(BloodType.AB, 3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                map.Add(BloodType.O, new BloodItem(BloodType.O, 4, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                lastUpdated = DateTime.MinValue;
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Exception/{ex}");
            }
        }
    }
}
