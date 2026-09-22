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
            List<WorkspaceCalendarBooking> bookings,
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

            entity._bookings = NormalizeBookings(bookings);
            entity.Bookings = entity._bookings.AsReadOnly();

            return entity;
        }

        public void AddBooking(WorkspaceCalendarBooking workspaceCalendarBooking)
        {
            ArgumentNullException.ThrowIfNull(workspaceCalendarBooking, nameof(workspaceCalendarBooking));

            if (IsFull)
            {
                throw new InvalidOperationException("Workspace Calendar is full");
            }

            if (workspaceCalendarBooking.StartAt < StartAt || workspaceCalendarBooking.EndAt > EndAt)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workspaceCalendarBooking),
                    $"Booking {workspaceCalendarBooking.StartAt:O} - {workspaceCalendarBooking.EndAt:O} is outside Workspace Calendar window {StartAt:O} - {EndAt:O}."
                );
            }

            var overlapping = _bookings.Find(b => Overlaps(b, workspaceCalendarBooking));

            if (overlapping is not null)
            {
                throw new InvalidOperationException(
                    $"Booking {workspaceCalendarBooking.StartAt:O} - {workspaceCalendarBooking.EndAt:O} overlaps existing booking {overlapping.StartAt:O} - {overlapping.EndAt:O}."
                );
            }

            _bookings.Add(workspaceCalendarBooking);
            Bookings = _bookings.AsReadOnly();
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasBooking()
        {
            return _bookings.Count > 0;
        }

        private static bool Overlaps(WorkspaceCalendarBooking existsBooking, WorkspaceCalendarBooking newBooking)
        {
            return existsBooking.StartAt < newBooking.EndAt && newBooking.StartAt < existsBooking.EndAt;
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

        private static List<WorkspaceCalendarBooking> NormalizeBookings(List<WorkspaceCalendarBooking> value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return new List<WorkspaceCalendarBooking>(value);
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

            if (StartAt >= EndAt)
            {
                throw new ArgumentException("StartAt must be less than EndAt.", nameof(startAt));
            }
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
            TimeSpan duration = EndAt - StartAt;

            TotalPrice = duration.TotalHours * pricePerHour;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}