namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record GetWorkspaceCalendarRecurrenceResponseDTO(
        string StartAt,
        string EndAt,
        bool IsFull
    );
}