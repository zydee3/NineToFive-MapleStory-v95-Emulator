using System;

namespace NineToFive.Util {
    public class Time {
        /// <summary>
        /// Equivalent function of System.currentTimeMilliseconds() in java.
        /// </summary>
        /// <returns>Current time in milliseconds</returns>
        public static long GetCurrent() {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static string CurrentTimestamp => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Calculates a timestamp for after a specified duration has passed.
        /// For example, 5 seconds in the future for an if statement if(CurrentTime > LastTime) to check if a duration has passed, where LastTime = Time.GetFuture(x in seconds).
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <returns>Future time in milliseconds</returns>
        public static long GetFuture(int timeInSeconds) {
            return GetCurrent() + timeInSeconds * 1000;
        }

        public static int ConvertToSeconds(long milliseconds) {
            return (int)(milliseconds / 1000);
        }

        public static long ConvertToMilliseconds(int seconds) {
            return seconds * 1000;
        }
    }
}