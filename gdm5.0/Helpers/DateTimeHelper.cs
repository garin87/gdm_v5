using System;

namespace gdm5._0.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime DateTimeNowWithOffset()
        {
            return DateTime.UtcNow.AddHours(3);
        }
    }
}
