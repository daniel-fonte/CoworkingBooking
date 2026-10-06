namespace CoworkingBooking.Shared.Interfaces
{
    public record CursorPaginationRecordResponse<T>(
        string? NextCursor,
        int Limit,
        List<T> Data
    );

    public record CursorPaginationRecordRequest (
        int Limit,
        string? Cursor
    );
}