namespace FortuneBotApp
{
    /// <summary> Date class. </summary>
    internal sealed class Date
    {
        /// <summary> Initializes a new instance of the <see cref="Date" /> class. </summary>
        /// <param name="month"> The month. </param>
        /// <param name="day"> The day. </param>
        public Date(int month, int day)
        {
            Month = month;
            Day = day;
        }

        /// <summary> Gets the code. </summary>
        /// <value> The code. </value>
        public int Code => (Month * 100) + Day;

        /// <summary> Gets the day. </summary>
        /// <value> The day. </value>
        public int Day { get; }

        /// <summary> Gets the month. </summary>
        /// <value> The month. </value>
        public int Month { get; }
    }
}
