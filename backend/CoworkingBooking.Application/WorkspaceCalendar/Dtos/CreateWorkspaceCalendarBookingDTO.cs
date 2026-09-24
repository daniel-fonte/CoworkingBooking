namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record CreateWorkspaceCalendarBookingRequestDTO(
        string StartAt,
        string EndAt
    );

    public sealed record CreateWorkspaceCalendarBookingResponseDTO(
        string StartAt,
        string EndAt,
        double TotalPrice
    );
}