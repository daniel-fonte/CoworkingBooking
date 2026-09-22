using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using MongoDB.Driver;

namespace CoworkingBooking.Core.WorkspaceCalendar.Repositories
{
    public interface IWorkspaceCalendarRepository
    {
        Task<long> InsertMany(List<WorkspaceCalendarEntity> workspaceCalendars, IClientSessionHandle? session = null);
        Task<List<WorkspaceCalendarEntity>> FindByWorkspaceId(string workspaceId);
        Task<List<WorkspaceCalendarEntity>> FindByWorkspaceAvailability(string workspaceId, DateTime startAt, DateTime until);
        Task<WorkspaceCalendarEntity?> FindOneById(string id);
        Task<WorkspaceCalendarBooking?> UpdateBooking(string id, List<WorkspaceCalendarBooking> workspaceCalendarBooking);
    }
}