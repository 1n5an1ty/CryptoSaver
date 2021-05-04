using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSaver.Core.ExtensionMethods
{
    public static class ListExtensions
    {
        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="list">Array to shuffle.</param>
        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null) return;
            var n = list.Count;

            for (var i = 0; i < n; i++)
            {
                var r = i + Random.Next(n - i);//*(n - i));

                var t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
        }

        /// <summary>
        /// Joins IEnumerable strings to a comma separated string.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns>System.String.</returns>
        public static string ToCsvLine(this IEnumerable<string> strings)
        {
            return string.Join(",", strings);
        }

        /// <summary>
        /// Converts a comma separated string to a list of strings
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.Collections.Generic.List&lt;System.String&gt;.</returns>
        public static List<string> FromCsvLine(this string input)
        {
            return input.Split(',').ToList();
        }
    }
}
