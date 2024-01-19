namespace FortuneBotApp
{
    /// <summary> BloodLoveRecord class. </summary>
    internal sealed class BloodLoveRecord
    {
        /// <summary> Initializes a new instance of the <see cref="BloodLoveRecord" /> class. </summary>
        /// <param name="content"> The content. </param>
        public BloodLoveRecord(string content)
        {
            Content = content;
        }

        /// <summary> Gets the content. </summary>
        /// <value> The content. </value>
        public string Content { get; }
    }
}
