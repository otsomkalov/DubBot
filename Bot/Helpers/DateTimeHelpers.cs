using System;

namespace Bot.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTime GetLastSunday(this DateTime dateTime)
        {
            dateTime = dateTime.Date;

            var daysSinceSunday = dateTime.DayOfWeek - DayOfWeek.Sunday;
            var sundayDate = dateTime.AddDays(-daysSinceSunday);

            return sundayDate;
        }
    }
}
