using System;
using System.Collections.Generic;
using System.Linq;

namespace FortuneBotApp
{
    /// <summary> ZodiacUtility class. </summary>
    internal static class ZodiacUtility
    {
        /// <summary> The emoji map </summary>
        private static readonly IReadOnlyDictionary<ZodiacType, string> emojiMap;

        /// <summary> The JPN name map </summary>
        private static readonly IReadOnlyDictionary<ZodiacType, string> jpnNameMap;

        /// <summary> The zodiac table </summary>
        private static readonly IEnumerable<ZodiacRecord> zodiacTable;

        /// <summary> The zodiac type map </summary>
        private static readonly IReadOnlyDictionary<string, ZodiacType> zodiacTypeMap;

        /// <summary> Initializes the <see cref="ZodiacUtility" /> class. </summary>
        static ZodiacUtility()
        {
            zodiacTable = new[]
            {
                new ZodiacRecord(ZodiacType.Aries,nameof(ZodiacType.Aries), new Date(3,21), new Date(4,19)),
                new ZodiacRecord(ZodiacType.Taurus,nameof(ZodiacType.Taurus), new Date(4,20), new Date(5,20)),
                new ZodiacRecord(ZodiacType.Gemini,nameof(ZodiacType.Gemini), new Date(5,21), new Date(6,21)),
                new ZodiacRecord(ZodiacType.Cancer,nameof(ZodiacType.Cancer), new Date(6,22), new Date(7,22)),
                new ZodiacRecord(ZodiacType.Leo,nameof(ZodiacType.Leo), new Date(7,23), new Date(8,22)),
                new ZodiacRecord(ZodiacType.Virgo,nameof(ZodiacType.Virgo), new Date(8,23), new Date(9,22)),
                new ZodiacRecord(ZodiacType.Libra,nameof(ZodiacType.Libra), new Date(9,23), new Date(10,23)),
                new ZodiacRecord(ZodiacType.Scorpius,nameof(ZodiacType.Scorpius), new Date(10,24), new Date(11,22)),
                new ZodiacRecord(ZodiacType.Sagittarius,nameof(ZodiacType.Sagittarius), new Date(11,23), new Date(12,21)),
                new ZodiacRecord(ZodiacType.Capricornus,nameof(ZodiacType.Capricornus), new Date(12,22), new Date(1,19)),
                new ZodiacRecord(ZodiacType.Aquarius,nameof(ZodiacType.Aquarius), new Date(1,20), new Date(2,18)),
                new ZodiacRecord(ZodiacType.Pisces,nameof(ZodiacType.Pisces), new Date(2,19), new Date(3,20)),
            };

            zodiacTypeMap = new Dictionary<string, ZodiacType>
            {
                {"おひつじ座",ZodiacType.Aries },
                {"牡羊座",ZodiacType.Aries },
                {"おうし座",ZodiacType.Taurus},
                {"牡牛座",ZodiacType.Taurus},
                {"ふたご座",ZodiacType.Gemini},
                {"双子座",ZodiacType.Gemini },
                {"かに座",ZodiacType.Cancer},
                {"蟹座",ZodiacType.Cancer},
                {"しし座",ZodiacType.Leo },
                {"獅子座",ZodiacType.Leo },
                {"おとめ座",ZodiacType.Virgo},
                {"乙女座",ZodiacType.Virgo},
                {"てんびん座",ZodiacType.Libra},
                {"天秤座",ZodiacType.Libra },
                {"さそり座",ZodiacType.Scorpius },
                {"蠍座",ZodiacType.Scorpius },
                {"いて座",ZodiacType.Sagittarius },
                {"射手座",ZodiacType.Sagittarius },
                {"やぎ座",ZodiacType.Capricornus },
                {"山羊座",ZodiacType.Capricornus },
                {"みずがめ座",ZodiacType.Aquarius },
                {"水瓶座",ZodiacType.Aquarius },
                {"うお座",ZodiacType.Pisces },
                {"魚座",ZodiacType.Pisces },
            };

            jpnNameMap = new Dictionary<ZodiacType, string>
            {
                {ZodiacType.Aries, "おひつじ座" },
                {ZodiacType.Taurus, "おうし座" },
                {ZodiacType.Gemini, "ふたご座" },
                {ZodiacType.Cancer, "かに座" },
                {ZodiacType.Leo, "しし座" },
                {ZodiacType.Virgo, "おとめ座" },
                {ZodiacType.Libra, "てんびん座" },
                {ZodiacType.Scorpius, "さそり座" },
                {ZodiacType.Sagittarius, "いて座" },
                {ZodiacType.Capricornus, "やぎ座" },
                {ZodiacType.Aquarius, "みずがめ座" },
                {ZodiacType.Pisces, "うお座" },
            };

            emojiMap = new Dictionary<ZodiacType, string>
            {
                {ZodiacType.Aries, "♈" },
                {ZodiacType.Taurus, "♉" },
                {ZodiacType.Gemini, "♊" },
                {ZodiacType.Cancer, "♋" },
                {ZodiacType.Leo, "♌" },
                {ZodiacType.Virgo, "♍" },
                {ZodiacType.Libra, "♎" },
                {ZodiacType.Scorpius, "♏" },
                {ZodiacType.Sagittarius, "♐" },
                {ZodiacType.Capricornus, "♑" },
                {ZodiacType.Aquarius, "♒" },
                {ZodiacType.Pisces, "♓" },
            };
        }

        /// <summary> Gets the emoji. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public static string GetEmoji(ZodiacType type)
        {
            return emojiMap.TryGetValue(type, out string tmp) ? tmp : string.Empty;
        }

        /// <summary> Gets the name of the japanese. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public static string GetJapaneseName(ZodiacType type)
        {
            return jpnNameMap.TryGetValue(type, out string tmp) ? tmp : string.Empty;
        }

        /// <summary> Gets the type. </summary>
        /// <param name="name"> The name. </param>
        /// <returns> </returns>
        public static ZodiacType GetType(string name)
        {
            foreach (ZodiacRecord item in zodiacTable)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Type;
                }
            }

            return ZodiacType.Invalid;
        }

        /// <summary> Gets the type. </summary>
        /// <param name="month"> The month. </param>
        /// <param name="day"> The day. </param>
        /// <returns> </returns>
        public static ZodiacType GetType(int month, int day)
        {
            int code = (month * 100) + day;

            IOrderedEnumerable<ZodiacRecord> sorted = zodiacTable.OrderBy(e => e.BeginDate.Code);

            ZodiacRecord cap = zodiacTable.First(e => e.Type == ZodiacType.Capricornus);

            // やぎ座のみ年をまたぐため先に判定する
            if ((cap.BeginDate.Code <= code && 1231 >= code) || (0101 <= code && cap.EndDate.Code >= code))
            {
                return cap.Type;
            }

            foreach (ZodiacRecord item in sorted)
            {
                if (item.BeginDate.Code <= code && item.EndDate.Code >= code)
                {
                    return item.Type;
                }
            }

            return ZodiacType.Invalid;
        }

        /// <summary> Gets the type from japanese. </summary>
        /// <param name="name"> The name. </param>
        /// <returns> </returns>
        public static ZodiacType GetTypeFromJapanese(string name)
        {
            return zodiacTypeMap.TryGetValue(name, out ZodiacType tmp) ? tmp : ZodiacType.Invalid;
        }
    }
}
