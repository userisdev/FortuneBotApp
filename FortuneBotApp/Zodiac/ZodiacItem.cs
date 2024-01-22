namespace FortuneBotApp.Zodiac
{
    /// <summary> ZodiacItem class. </summary>
    internal sealed class ZodiacItem
    {
        /// <summary> Initializes a new instance of the <see cref="ZodiacItem" /> class. </summary>
        /// <param name="content"> The content. </param>
        /// <param name="item"> The item. </param>
        /// <param name="money"> The money. </param>
        /// <param name="total"> The total. </param>
        /// <param name="job"> The job. </param>
        /// <param name="color"> The color. </param>
        /// <param name="love"> The love. </param>
        /// <param name="rank"> The rank. </param>
        /// <param name="sign"> The sign. </param>
        public ZodiacItem(string content, string item, int money, int total, int job, string color, int love, int rank, string sign)
        {
            Content = content;
            Item = item;
            Money = money;
            Total = total;
            Job = job;
            Color = color;
            Love = love;
            Rank = rank;
            Sign = sign;

            Type = ZodiacUtility.GetTypeFromJapanese(sign);
        }

        /// <summary> Gets the empty. </summary>
        /// <value> The empty. </value>
        public static ZodiacItem Empty => new ZodiacItem(string.Empty, string.Empty, 0, 0, 0, string.Empty, 0, 0, string.Empty);

        /// <summary> Gets the color. </summary>
        /// <value> The color. </value>
        public string Color { get; }

        /// <summary> Gets the content. </summary>
        /// <value> The content. </value>
        public string Content { get; }

        /// <summary> Gets the item. </summary>
        /// <value> The item. </value>
        public string Item { get; }

        /// <summary> Gets the job. </summary>
        /// <value> The job. </value>
        public int Job { get; }

        /// <summary> Gets the love. </summary>
        /// <value> The love. </value>
        public int Love { get; }

        /// <summary> Gets the money. </summary>
        /// <value> The money. </value>
        public int Money { get; }

        /// <summary> Gets the rank. </summary>
        /// <value> The rank. </value>
        public int Rank { get; }

        /// <summary> Gets the sign. </summary>
        /// <value> The sign. </value>
        public string Sign { get; }

        /// <summary> Gets the total. </summary>
        /// <value> The total. </value>
        public int Total { get; }

        /// <summary> Gets the type. </summary>
        /// <value> The type. </value>
        public ZodiacType Type { get; }
    }
}
