namespace CoworkingBooking.Core.WorkspaceCalendar.Entities
{
    public class WorkspaceCalendarEntity
    {
        public string Id { get; private set; } = string.Empty;
        public string WorkspaceId { get; private set ; } = string.Empty;
        public DateTime StartAt { get; private set; } = default;
        public DateTime EndAt { get; private set; } = default;
        public bool IsFull { get; private set; }
        public IReadOnlyList<WorkspaceCalendarBooking> Bookings { get; private set; } = Array.Empty<WorkspaceCalendarBooking>();
        private List<WorkspaceCalendarBooking> _bookings = new List<WorkspaceCalendarBooking>();
        public DateTime CreatedAt { get; private set; } = default;
        public DateTime UpdatedAt { get; private set; } = default;

        public WorkspaceCalendarEntity(
            string workspaceId,
            DateTime startAt,
            DateTime endAt
        ) {
            this.Id = string.Empty;
            this.WorkspaceId = NormalizeRequired(workspaceId, nameof(workspaceId));
            this.StartAt = NormalizeDate(startAt);
            this.EndAt = NormalizeDate(endAt);
            this.CreatedAt = DateTime.UtcNow.ToUniversalTime();
            this.UpdatedAt = DateTime.UtcNow.ToUniversalTime();

            this.IsFull = false;
        }

        public static WorkspaceCalendarEntity Rehydrate(
            string id,
            string workspaceId,
            DateTime startAt,
            DateTime endAt,
            bool isFull,
            DateTime createdAt,
            DateTime updatedAt
        )
        {
            var entity = new WorkspaceCalendarEntity(workspaceId, startAt, endAt)
            {
                Id = id,
                IsFull = isFull,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return entity;
        }

        public void AddBooking(WorkspaceCalendarBooking workspaceCalendarBooking, string workspaceTimezone)
        {
            if (IsFull)
            {
                throw new InvalidOperationException("Workspace Calendar is full");
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(workspaceTimezone);

            var startAtTimezone = TimeZoneInfo.ConvertTimeFromUtc(workspaceCalendarBooking.StartAt, timeZone);
            var currentEndAtTimezone = TimeZoneInfo.ConvertTimeFromUtc(workspaceCalendarBooking.EndAt, timeZone);

            _bookings.ForEach(b =>
            {
                TimeSpan startAtDifference = TimeZoneInfo.ConvertTimeFromUtc(b.StartAt, timeZone) - startAtTimezone;

                if (startAtDifference.Hours <= 0)
                {
                    Console.WriteLine("StartAt já está dentro de uma reserva");
                }
            });

            _bookings.Add(workspaceCalendarBooking);
            Bookings = _bookings;
        }

        private static string NormalizeRequired(string value, string propertyName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException($"{propertyName} is required.", propertyName)
                : value.Trim();
        }

        private static DateTime NormalizeDate(DateTime value)
        {
            return value.ToUniversalTime();
        }
    }

    public class WorkspaceCalendarBooking
    {
        public DateTime StartAt { get; private set; }
        public DateTime EndAt { get; private set; }
        public double TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; } = default;
        public DateTime UpdatedAt { get; private set; } = default;

        public WorkspaceCalendarBooking(DateTime startAt, DateTime endAt)
        {
            StartAt = NormalizeDate(startAt);
            EndAt = NormalizeDate(endAt);
            CreatedAt = DateTime.UtcNow.ToUniversalTime();
            UpdatedAt = DateTime.UtcNow.ToUniversalTime();
        }

        public static WorkspaceCalendarBooking Rehydrate(
            DateTime startAt,
            DateTime endAt,
            double totalPrice,
            DateTime createdAt,
            DateTime updatedAt
        )
        {
            var entity = new WorkspaceCalendarBooking(startAt, endAt)
            {
                TotalPrice = totalPrice,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return entity;
        }

        private static DateTime NormalizeDate(DateTime value)
        {
            return value.ToUniversalTime();
        }

        public void CalculateTotalPrice(double pricePerHour)
        {
            TimeSpan totalHours = EndAt - StartAt;

            TotalPrice = totalHours.Hours * pricePerHour;
        }
    }
}