namespace CoworkingBooking.Core.WorkspaceCalendar.Entities
{
    public class WorkspaceCalendarEntity
    {
        public string Id { get; private set; } = string.Empty;
        public string WorkspaceId { get; private set ; } = string.Empty;
        public DateTime StartAt { get; private set; } = default;
        public DateTime EndAt { get; private set; } = default;
        public bool IsFull { get; private set; }
        // public List<Booking> { get; private set; } = new List<Booking>();
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

    // public class Booking
    // {
        
    // }
}