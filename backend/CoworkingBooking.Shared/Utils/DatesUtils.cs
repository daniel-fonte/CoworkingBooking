using System.Globalization;

namespace CoworkingBooking.Shared.Utils
{
    public class DatesUtils
    {
        public static DateTime GetNextDay(DateTime dateTime, DayOfWeek? targetDay)
        {
            int daysUntil = 1;

            if (targetDay is not null)
            {
                daysUntil = ((int)targetDay - (int)dateTime.DayOfWeek + 7) % 7;
            }

            if (daysUntil == 0)
            {
                daysUntil = 7;
            }

            return dateTime.AddDays(daysUntil);
        }

        public static DateTime GetClosestDayOfWeek(DateTime date, DayOfWeek targetDay)
        {
            int currentDay = (int)date.DayOfWeek;
            int target = (int)targetDay;

            int daysForward = (target - currentDay + 7) % 7;
            int daysBackward = (currentDay - target + 7) % 7;

            if (daysBackward < daysForward)
                return date.AddDays(-daysBackward);

            return date.AddDays(daysForward);
        }

        public static bool BeAValidIsoString(string? dateString)
        {
            return DateTime.TryParse(
                dateString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out _
            );
        }

        public static string ToISOString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }
}