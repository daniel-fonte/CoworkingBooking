using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Application.Workspace.Cache
{
    public sealed record WorkspaceCache(
        string Id,
        string Name,
        string Description,
        string Slug,
        WorkspaceStatus Status,
        WorkspaceType Type,
        WorkspaceCoordinatesCache Coordinates,
        WorkspaceAvailabilityCache Availability,
        double PricePerHour,
        bool IsInactive,
        List<string> Resources,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );

    public sealed record WorkspaceCoordinatesCache(
        string Type,
        double[] Coordinates
    );

    public sealed record WorkspaceAvailabilityCache(
        DateTime StartAt,
        DateTime EndAt,
        WorkspaceAvailabilityRecurrenceCache Recurrence,
        string Timezone
    );

    public sealed record WorkspaceAvailabilityRecurrenceCache(
        Frequency Frequency,
        int Interval,
        DateTime Until,
        List<DayOfWeek>? ByDay,
        List<int>? ByMonth
    );
}