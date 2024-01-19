using System;

namespace FortuneBotApp
{
    /// <summary> BloodUtility class. </summary>
    internal static class BloodUtility
    {
        /// <summary> Gets the emoji. </summary>
        /// <param name="type"> The type. </param>
        /// <returns> </returns>
        public static string GetEmoji(BloodType type)
        {
            switch (type)
            {
                case BloodType.A:
                    return "🅰";

                case BloodType.B:
                    return "🅱";

                case BloodType.AB:
                    return "🆎";

                case BloodType.O:
                    return "🅾";

                default:
                    return string.Empty;
            }
        }

        /// <summary> Gets the type. </summary>
        /// <param name="name"> The name. </param>
        /// <returns> </returns>
        public static BloodType GetType(string name)
        {
            return name.Equals("a", StringComparison.OrdinalIgnoreCase)
                ? BloodType.A
                : name.Equals("b", StringComparison.OrdinalIgnoreCase)
                ? BloodType.B
                : name.Equals("ab", StringComparison.OrdinalIgnoreCase)
                ? BloodType.AB
                : name.Equals("o", StringComparison.OrdinalIgnoreCase) ? BloodType.O : BloodType.Invalid;
        }
    }
}
