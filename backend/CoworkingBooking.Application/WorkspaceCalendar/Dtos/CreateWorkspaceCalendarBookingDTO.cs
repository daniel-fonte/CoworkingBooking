namespace CoworkingBooking.Application.WorkspaceCalendar.Dtos
{
    public sealed record CreateWorkspaceCalendarBookingRequestDTO(
        string StartAt,
        string EndAt,
        Guid? UserId
    );

    public sealed record CreateWorkspaceCalendarBookingResponseDTO(
        string StartAt,
        string EndAt,
        double TotalPrice
    );
}