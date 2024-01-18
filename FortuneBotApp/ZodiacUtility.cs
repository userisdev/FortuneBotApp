using System;
using System.Collections.Generic;
using System.Linq;

namespace FortuneBotApp
{
    /// <summary> ZodiacUtility class. </summary>
    internal static class ZodiacUtility
    {
        /// <summary> The zodiac table </summary>
        private static readonly IEnumerable<ZodiacRecord> zodiacTable;

        /// <summary> Initializes the <see cref="ZodiacUtility" /> class. </summary>
        static ZodiacUtility()
        {
            zodiacTable = new[]
            {
                new ZodiacRecord(ZodiacType.Aries,nameof(ZodiacType.Aries), "おひつじ座", new Date(3,21),new Date(4,19)),
                new ZodiacRecord(ZodiacType.Taurus,nameof(ZodiacType.Taurus), "おうし座", new Date(4,20),new Date(5,20)),
                new ZodiacRecord(ZodiacType.Gemini,nameof(ZodiacType.Gemini), "ふたご座", new Date(5,21),new Date(6,21)),
                new ZodiacRecord(ZodiacType.Cancer,nameof(ZodiacType.Cancer), "かに座", new Date(6,22),new Date(7,22)),
                new ZodiacRecord(ZodiacType.Leo,nameof(ZodiacType.Leo), "しし座", new Date(7,23),new Date(8,22)),
                new ZodiacRecord(ZodiacType.Virgo,nameof(ZodiacType.Virgo), "おとめ座", new Date(8,23),new Date(9,22)),
                new ZodiacRecord(ZodiacType.Libra,nameof(ZodiacType.Libra), "てんびん座", new Date(9,23),new Date(10,23)),
                new ZodiacRecord(ZodiacType.Scorpius,nameof(ZodiacType.Scorpius), "さそり座", new Date(10,24),new Date(11,22)),
                new ZodiacRecord(ZodiacType.Sagittarius,nameof(ZodiacType.Sagittarius), "いて座", new Date(11,23),new Date(12,21)),
                new ZodiacRecord(ZodiacType.Capricornus,nameof(ZodiacType.Capricornus), "やぎ座", new Date(12,22),new Date(1,19)),
                new ZodiacRecord(ZodiacType.Aquarius,nameof(ZodiacType.Aquarius), "みずがめ座", new Date(1,20),new Date(2,18)),
                new ZodiacRecord(ZodiacType.Pisces,nameof(ZodiacType.Pisces), "うお座", new Date(2,19),new Date(3,20)),
            };
        }

        /// <summary> Gets the description. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public static string GetDescription(ZodiacType type)
        {
            return zodiacTable.FirstOrDefault(e => e.Type == type)?.Description ?? string.Empty;
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
    }
}
