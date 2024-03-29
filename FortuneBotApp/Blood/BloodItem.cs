﻿namespace FortuneBotApp.Blood
{
    /// <summary> BloodItem class. </summary>
    internal sealed class BloodItem
    {
        /// <summary> Initializes a new instance of the <see cref="BloodItem" /> class. </summary>
        /// <param name="type"> The type. </param>
        /// <param name="rank"> The rank. </param>
        /// <param name="total"> The total. </param>
        /// <param name="color"> The color. </param>
        /// <param name="word"> The word. </param>
        /// <param name="love"> The love. </param>
        /// <param name="job"> The job. </param>
        /// <param name="url"> The URL. </param>
        public BloodItem(BloodType type, int rank, string total, string color, string word, string love, string job, string url)
        {
            Type = type;
            Rank = rank;
            Total = total;
            Color = color;
            Word = word;
            Love = love;
            Job = job;
            Url = url;
        }

        /// <summary> The empty </summary>
        public static BloodItem Empty => new BloodItem(BloodType.Invalid, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        /// <summary> Gets the color. </summary>
        /// <value> The color. </value>
        public string Color { get; }

        /// <summary> Returns true if ... is valid. </summary>
        /// <value> <c> true </c> if this instance is valid; otherwise, <c> false </c>. </value>
        public bool IsValid => Rank != 0 && !string.IsNullOrEmpty(Total) && !string.IsNullOrEmpty(Color) && !string.IsNullOrEmpty(Word) && !string.IsNullOrEmpty(Love) && !string.IsNullOrEmpty(Job) && !string.IsNullOrEmpty(Url);

        /// <summary> Gets the job. </summary>
        /// <value> The job. </value>
        public string Job { get; }

        /// <summary> Gets the love. </summary>
        /// <value> The love. </value>
        public string Love { get; }

        /// <summary> Gets the rank. </summary>
        /// <value> The rank. </value>
        public int Rank { get; }

        /// <summary> Gets the total. </summary>
        /// <value> The total. </value>
        public string Total { get; }

        /// <summary> Gets the type. </summary>
        /// <value> The type. </value>
        public BloodType Type { get; }

        /// <summary> Gets the URL. </summary>
        /// <value> The URL. </value>
        public string Url { get; }

        /// <summary> Gets the word. </summary>
        /// <value> The word. </value>
        public string Word { get; }
    }
}
