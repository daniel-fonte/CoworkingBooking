using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.Mappers;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.WorkspaceCalendar.UseCases
{
    public class GetWorkspaceCalendarRecurrencesUseCase : IUseCase<(string, CursorPaginationRecordRequest, CancellationToken), CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>>
    {
        private readonly IWorkspaceCalendarRepository workspaceCalendarRepository;
        private readonly ILogger<GetWorkspaceCalendarRecurrencesUseCase> logger;
        private readonly WorkspaceCalendarMapper workspaceCalendarMapper;

        public GetWorkspaceCalendarRecurrencesUseCase(
            IWorkspaceCalendarRepository workspaceCalendarRepository,
            WorkspaceCalendarMapper workspaceCalendarMapper,
            ILogger<GetWorkspaceCalendarRecurrencesUseCase> logger
        )
        {
            this.workspaceCalendarRepository = workspaceCalendarRepository;
            this.workspaceCalendarMapper = workspaceCalendarMapper;
            this.logger = logger;
        }

        public async Task<Result<CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>>> Execute((
            string, 
            CursorPaginationRecordRequest, 
            CancellationToken
        ) input)
        {
            var (workspaceId, cursorPaginationRecordRequest, cancellationToken) = input;

            var recurrencesFound = await workspaceCalendarRepository
                .CursorPagination(wc => 
                    wc.WorkspaceId, workspaceId, 
                    cursorPaginationRecordRequest.Cursor, 
                    cursorPaginationRecordRequest.Limit, 
                    cancellationToken
                );

            if (recurrencesFound.Data.Count == 0)
            {
                logger.LogWarning("Not found Workspace Calendar Recurrences to Workspace - {ID}", workspaceId);
                return Result<CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>>
                    .Failure([ new Error($"Not found Workspace Calendar Recurrences to Workspace - {workspaceId}", ErrorType.NotFound) ]);
            }

            var workspaceCalendarRecurrences = recurrencesFound.Data
                .Select(workspaceCalendarMapper.ToGetWorkspaceCalendarRecurrenceResponseDTO)
                .ToList();

            return Result<CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>>
                .Success(new CursorPaginationRecordResponse<GetWorkspaceCalendarRecurrenceResponseDTO>(
                    recurrencesFound.NextCursor,
                    cursorPaginationRecordRequest.Limit,
                    workspaceCalendarRecurrences
                ));
        }
    }
}