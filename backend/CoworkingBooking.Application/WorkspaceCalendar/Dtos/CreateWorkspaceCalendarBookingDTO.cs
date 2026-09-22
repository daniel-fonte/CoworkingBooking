namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record CreateWorkspaceCalendarBookingRequestDTO(
        string StartAt,
        string EndAt
    );
}