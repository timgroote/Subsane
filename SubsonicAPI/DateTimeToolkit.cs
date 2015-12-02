using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubsonicAPI
{
    public static class DateTimeToolkit
    {
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = TimeSpan.FromSeconds(timestamp);
            return origin.Add(ts);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
