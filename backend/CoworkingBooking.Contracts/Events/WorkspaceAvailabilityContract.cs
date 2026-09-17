using CoworkingBooking.Shared.Enums;

namespace CoworkingBooking.Contracts.Events
{
    public sealed record WorkspaceAvailabilityContract(
        DateTime StartAt,
        DateTime EndAt,
        WorkSpaceAvailabilityRecurrenceContract Recurrence,
        string Timezone
    );

    public sealed record WorkSpaceAvailabilityRecurrenceContract(
        Frequency Frequency,
        int Interval,
        DateTime Until,
        List<DayOfWeek>? ByDay,
        List<int>? ByMonth
    );
}