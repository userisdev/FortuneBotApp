namespace FortuneBotApp
{
    /// <summary> BloodJobRecord class. </summary>
    internal sealed class BloodJobRecord
    {
        /// <summary> Initializes a new instance of the <see cref="BloodJobRecord" /> class. </summary>
        /// <param name="content"> The content. </param>
        public BloodJobRecord(string content)
        {
            Content = content;
        }

        /// <summary> Gets the content. </summary>
        /// <value> The content. </value>
        public string Content { get; }
    }
}
