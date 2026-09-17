using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Application.Workspace.Dtos
{
    public sealed record UpdateWorkspaceAvailabilityRequestDTO(
        string StartAt,
        string EndAt,
        string Until,
        Frequency Frequency,
        List<DayOfWeek>? ByDay,
        List<int>? ByMonth,
        string Timezone
    );

    public sealed record UpdateWorkspaceAvailabilityResponseDTO(
        string StartAt,
        string EndAt,
        string Until,
        Frequency Frequency,
        List<DayOfWeek>? ByDay,
        List<int>? ByMonth,
        string Timezone
    );
}