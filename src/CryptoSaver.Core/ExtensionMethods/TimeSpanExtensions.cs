using System;

namespace CryptoSaver.Core.ExtensionMethods
{
    /// <summary>
    /// Class TimeSpanExtensions.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Timespan from the specified seconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan Secs(this int seconds)
        {
            if (seconds < 0) new ArgumentOutOfRangeException(nameof(seconds), Messages.RequirePositiveMessage);
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Timespan from the specified seconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan Secs(this double seconds)
        {
            if (seconds < 0) new ArgumentOutOfRangeException(nameof(seconds), Messages.RequirePositiveMessage);
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Timespan from the specified minutes.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan Mins(this int minutes)
        {
            if (minutes < 0) new ArgumentOutOfRangeException(nameof(minutes), Messages.RequirePositiveMessage);
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Timespan from the specified minutes.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan Mins(this double minutes)
        {
            if (minutes < 0) new ArgumentOutOfRangeException(nameof(minutes), Messages.RequirePositiveMessage);
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Class Messages for the TimeSpanExtensions.
        /// </summary>
        private static class Messages
        {
            /// <summary>
            /// The require positive message
            /// </summary>
            internal const string RequirePositiveMessage = "TimeSpans Should be positive";
        }
    }
}