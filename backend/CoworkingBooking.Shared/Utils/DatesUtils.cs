namespace CoworkingBooking.Workers.Utils
{
    public class DatesUtils
    {
        public static DateTime GetNextDay(DateTime dateTime, DayOfWeek? targetDay)
        {
            int daysUntil = 7;

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
    }
}