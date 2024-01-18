using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FortuneBotApp
{
    /// <summary> FortuneImageFinder </summary>
    internal static class FortuneImageFinder
    {
        /// <summary> The random </summary>
        private static readonly Random rnd = new Random();

        /// <summary> Finds this instance. </summary>
        /// <returns> </returns>
        public static string Find()
        {
            string dirPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            int index = GenerateCustomRandomNumber();
            string imagePath = Path.Combine(dirPath, "fortune_images", $"{index:00}.png");
            return File.Exists(imagePath) ? imagePath : string.Empty;
        }

        /// <summary> Generates the custom random number. </summary>
        /// <returns> </returns>
        private static int GenerateCustomRandomNumber()
        {
            // 画像は00～06の7枚、各数字の割合
            int[] weights = new int[7] { 10, 15, 20, 20, 20, 10, 5 };

            // 確率の総和
            int totalWeight = weights.Sum();

            int randomNumber = rnd.Next(totalWeight);
            int cumulativeWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulativeWeight += weights[i];
                if (randomNumber < cumulativeWeight)
                {
                    // iがランダムな数字となる
                    return i;
                }
            }

            // 通常はここに到達しないはずですが、万が一のために weights 配列の長さを返します。
            return weights.Length - 1;
        }
    }
}
