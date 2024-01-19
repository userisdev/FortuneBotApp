namespace FortuneBotApp
{
    /// <summary> BloodTotalRecord class. </summary>
    internal sealed class BloodTotalRecord
    {
        /// <summary> Initializes a new instance of the <see cref="BloodTotalRecord" /> class. </summary>
        /// <param name="content"> The content. </param>
        /// <param name="color"> The color. </param>
        /// <param name="word"> The word. </param>
        public BloodTotalRecord(string content, string color, string word)
        {
            Content = content;
            Color = color;
            Word = word;
        }

        /// <summary> Gets the color. </summary>
        /// <value> The color. </value>
        public string Color { get; }

        /// <summary> Gets the content. </summary>
        /// <value> The content. </value>
        public string Content { get; }

        /// <summary> Gets the word. </summary>
        /// <value> The word. </value>
        public string Word { get; }
    }
}
