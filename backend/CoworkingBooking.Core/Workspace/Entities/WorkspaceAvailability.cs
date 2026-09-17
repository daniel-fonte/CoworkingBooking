using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Core.Workspace.Entities
{
    public class WorkSpaceAvailability
    {
        public DateTime StartAt { get; private set; }
        public DateTime EndAt { get; private set; }
        public WorkSpaceAvailabilityRecurrence Recurrence { get; private set; }
        public string Timezone { get; private set; }

        public WorkSpaceAvailability(DateTime startAt, DateTime endAt, WorkSpaceAvailabilityRecurrence recurrence, string timezone)
        {
            this.StartAt = NormalizeDate(startAt);
            this.EndAt = NormalizeDate(endAt);
            this.Recurrence = ValidateRecurrence(recurrence);
            this.Timezone = timezone;
            
            if (this.StartAt >= this.EndAt)
            {
                throw new ArgumentException("StartAt must be less than EndAt.", nameof(StartAt));
            }
        }

        public static WorkSpaceAvailability Rehydrate(DateTime startAt, DateTime endAt, WorkSpaceAvailabilityRecurrence recurrence, string timezone)
        {
            return new WorkSpaceAvailability(startAt, endAt, recurrence, timezone);
        }

        // public void MakeAvailable()
        // {
        //     this.IsAvailable = true;
        // }

        // public void MakeUnavailable()
        // {
        //     this.IsAvailable = false;
        // }

        private static DateTime NormalizeDate(DateTime value)
        {
            return value.ToUniversalTime();
        }

        private static WorkSpaceAvailabilityRecurrence ValidateRecurrence(WorkSpaceAvailabilityRecurrence value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value;
        }
    }

    public class WorkSpaceAvailabilityRecurrence
    {
        public Frequency Frequency { get; private set; }
        public int Interval { get; private set; }
        public DateTime Until { get; private set; }
        public List<DayOfWeek>? ByDay { get; private set; }
        public List<int>? ByMonth { get; private set; }

        public WorkSpaceAvailabilityRecurrence(Frequency frequency, DateTime until, List<DayOfWeek>? byDay, List<int>? byMonth)
        {
            this.Frequency = ValidateFrequency(frequency);
            this.Until = NormalizeDate(until);
            this.ByDay = byDay is null ? byDay : ValidateByDay(byDay);
            this.ByMonth = byMonth is null ? byMonth: ValidateByMonth(byMonth);
            this.Interval = 1;
        }


        private static Frequency ValidateFrequency(Frequency value)
        {
            return Enum.IsDefined(typeof(Frequency), value)
                ? value
                : throw new ArgumentOutOfRangeException(nameof(value), "Invalid Frequency.");
        }

        private static DateTime NormalizeDate(DateTime value)
        {
            return value.ToUniversalTime();
        }

        private static List<DayOfWeek> ValidateByDay(List<DayOfWeek> value)
        {
            ArgumentNullException.ThrowIfNull(value);

            value.ForEach(v =>
            {
                if (!Enum.IsDefined(typeof(DayOfWeek), v))
                {
                    throw new ArgumentOutOfRangeException(nameof(v), "Invalid DayOfWeek.");
                }
            });

            return value;
        }

        private static List<int> ValidateByMonth(List<int> value)
        {
            ArgumentNullException.ThrowIfNull(value);

            value.ForEach(v =>
            {
                if (v > 0 && v > 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(v), "Month must be between 1 and 12.");
                }
            });

            return value;
        }

    }
}