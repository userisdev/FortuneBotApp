namespace FortuneBotApp
{
    /// <summary> ZodiacRecord class. </summary>
    internal sealed class ZodiacRecord
    {
        /// <summary> Initializes a new instance of the <see cref="ZodiacRecord" /> class. </summary>
        /// <param name="type"> The type. </param>
        /// <param name="name"> The name. </param>
        /// <param name="beginDate"> The begin date. </param>
        /// <param name="endDate"> The end date. </param>
        public ZodiacRecord(ZodiacType type, string name, Date beginDate, Date endDate)
        {
            Type = type;
            Name = name;
            BeginDate = beginDate;
            EndDate = endDate;
        }

        /// <summary> Gets the begin date. </summary>
        /// <value> The begin date. </value>
        public Date BeginDate { get; }

        /// <summary> Gets the end date. </summary>
        /// <value> The end date. </value>
        public Date EndDate { get; }

        /// <summary> Gets the name. </summary>
        /// <value> The name. </value>
        public string Name { get; }

        /// <summary> Gets the type. </summary>
        /// <value> The type. </value>
        public ZodiacType Type { get; }
    }
}
