namespace Bot.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetLastSunday(this DateTime dateTime)
    {
        dateTime = dateTime.Date;

        var daysSinceSunday = dateTime.DayOfWeek - DayOfWeek.Sunday;
        var sundayDate = dateTime.AddDays(-daysSinceSunday);

        return sundayDate;
    }
}