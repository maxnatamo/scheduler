using System;
using System.Linq;

namespace Scheduler.Cron
{
    /// <summary>
    /// The Cron expression class.
    /// Contains a simple methods to perform basic Cron expression functions.
    /// </summary>
    public class CronExpression
    {
        public string minute;
        public string hour;
        public string day;
        public string month;

        /// <summary>
        /// Create a CronExpression object from a Cron string.
        /// Cron strings are in the format of "MINUTE HOUR DAY MONTH".
        /// Each segment of the Cron string can be either an asterisk (*), a number (ex. "30") or multiple numbers (ex. "0,30")
        /// Example: The Cron string "0,30 * * *" will execute, when the minute mark hits either 0 or 30. The rest of the segments are redundant.
        /// <param name="cronString">The input Cron string.</param>
        /// </summary>
        public CronExpression ( string cronString )
        {
            if( !CronUtilities.IsExpressionStringValid( cronString ) ) {
                throw new System.Exception("Cron expression string is invalid.");
            }
            string[] expressionSegments = cronString.Split(' ');

            this.minute     = expressionSegments[0];
            this.hour       = expressionSegments[1];
            this.day        = expressionSegments[2];
            this.month      = expressionSegments[3];
        }

        /// <summary>
        /// Create a CronExpression object from segment strings.
        /// <param name="minute">The minute segment.</param>
        /// <param name="hour">The hour segment.</param>
        /// <param name="day">The day segment.</param>
        /// <param name="month">The month segment.</param>
        /// </summary>
        public CronExpression ( string minute, string hour, string day, string month )
        {
            if( String.IsNullOrEmpty( minute ) || String.IsNullOrEmpty( hour ) || String.IsNullOrEmpty( day ) || String.IsNullOrEmpty( month ) ) {
                throw new Exception("Invalid contructor parameters. Cron strings cannot be NULL.");
            }

            this.minute = minute;
            this.hour = hour;
            this.day = day;
            this.month = month;
        }

        /// <summary>
        /// Returns a readable Cron expression string, from the current object.
        /// </summary>
        public string GetCronExpressionString( )
        {
            if( String.IsNullOrEmpty( minute ) || String.IsNullOrEmpty( hour ) || String.IsNullOrEmpty( day ) || String.IsNullOrEmpty( month ) ) {
                throw new Exception("Cron expression object has invalid members.");
            }

            return minute + " " + hour + " " + day + " " + month;
        }

        /// <summary>
        /// Returns a DateTime object, with the next time the Cron is expected to execute.
        /// </summary>
        public DateTime GetNextRunTime()
        {
            string expressionString = GetCronExpressionString();
            DateTime now = DateTime.Now;

            if( !CronUtilities.IsExpressionStringValid( expressionString ) ) {
                throw new System.Exception("Cron expression string is invalid.");
            }

            // Set the Seconds-value to 0, so the Cron will execute when the minute mark gets reached.
            now = now.AddSeconds( -now.Second );

            // If the Cron expression is set to run every minute,
            // return the current time, plus a minute.
            if( expressionString == "* * * *" ) {
                return now.AddMinutes( 1 );
            }

            if( minute != "*" ) {
                while( now.Minute != Convert.ToInt32( minute ) ) {
                    now = now.AddMinutes( 1 );
                }
            }
            if( hour != "*" ) {
                while( now.Hour != Convert.ToInt32( hour ) ) {
                    now = now.AddHours( 1 );
                }
            }
            if( day != "*" ) {
                while( now.Day != Convert.ToInt32( day ) ) {
                    now = now.AddDays( 1 );
                }
            }
            if( month != "*" ) {
                while( now.Month != Convert.ToInt32( month ) ) {
                    now = now.AddMonths( 1 );
                }
            }

            return now;
        }
    }

    /// <summary>
    /// The Cron Utilities class.
    /// Contains methods for Cron expressions, like checking validity of expression strings.
    /// </summary>
    public static class CronUtilities
    {
        /// <summary>
        /// Checks whether the supplied Cron expression string is valid.
        /// </summary>
        public static bool IsExpressionStringValid( string expressionString )
        {
            // TODO: Use Regex to check validity of expression segments.
            string[] expressionSegments = expressionString.Split(' ');

            // If there are not the required amount of segments, the string is invalid.
            if( expressionSegments.Length != 4 ) {
                return false;
            }

            // If any of the expression segments is of zero-length,
            // the expression is invalid.
            foreach( var segment in expressionSegments ) {
                if(segment.Length <= 0) {
                    return false;
                }
            }

            // Check whether all values are in the allowed ranges.
            return  CheckSegment( expressionSegments[0], 0, 59 ) &&
                    CheckSegment( expressionSegments[1], 0, 23 ) &&
                    CheckSegment( expressionSegments[2], 0, 31 ) &&
                    CheckSegment( expressionSegments[3], 0, 12 );
        }

        /// <summary>
        /// Checks whether the supplied expression segment is in the valid range.
        /// <returns>min >= x <= max</returns>
        /// <param name="segment">The segment to check for validity.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value. The maximum is also included.</param>
        /// </summary>
        private static bool CheckSegment( string segment, int min, int max )
        {
            // If the segment is not a number, ignore.
            if( segment == "*" ) {
                return true;
            }

            int value = Convert.ToInt32( segment );
            return value >= min && value <= max;
        }
    }
}