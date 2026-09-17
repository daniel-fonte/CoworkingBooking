using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Workers.Utils;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Interfaces;
using CoworkingBooking.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.WorkspaceCalendar.UseCases
{
    public class CreateWorkspaceRecurrencesUseCase : IUseCase<CreateWorkspaceRecurrenceRequestDTO, bool>
    {
        private readonly IWorkspaceCalendarRepository workspaceCalendarRepository;
        private readonly ITransactionManager transactionManager;
        private ILogger<CreateWorkspaceRecurrencesUseCase> logger;

        public CreateWorkspaceRecurrencesUseCase(
            IWorkspaceCalendarRepository workspaceCalendarRepository,
            ITransactionManager transactionManager,
            ILogger<CreateWorkspaceRecurrencesUseCase> logger
        )
        {
            this.workspaceCalendarRepository = workspaceCalendarRepository;
            this.transactionManager = transactionManager;
            this.logger = logger;
        }

        public async Task<Result<bool>> Execute(CreateWorkspaceRecurrenceRequestDTO data)
        {
            List<WorkspaceCalendarEntity> workspaceCalendarList = new List<WorkspaceCalendarEntity>();

            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(data.WorkSpaceAvailability.Timezone);

                var currentStartAt = TimeZoneInfo.ConvertTimeFromUtc(data.WorkSpaceAvailability.StartAt, timeZone);
                var currentEndAt = TimeZoneInfo.ConvertTimeFromUtc(data.WorkSpaceAvailability.EndAt, timeZone);
                var untilOnTimezone = TimeZoneInfo.ConvertTimeFromUtc(data.WorkSpaceAvailability.Recurrence.Until, timeZone);
                
                if (data.WorkSpaceAvailability.Recurrence.Frequency == Frequency.WEEKLY && data.WorkSpaceAvailability.Recurrence.ByDay is not null)
                {
                    data.WorkSpaceAvailability.Recurrence.ByDay.ForEach(day =>
                    {
                        while (DateOnly.FromDateTime(currentStartAt) < DateOnly.FromDateTime(DatesUtils.GetClosestDayOfWeek(untilOnTimezone, day)))
                        {
                            WorkspaceCalendarEntity workspaceCalendarEntity = new WorkspaceCalendarEntity(
                                workspaceId: data.WorkspaceId,
                                startAt: DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentStartAt, timeZone), day),
                                endAt: DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentEndAt, timeZone), day)
                            );

                            workspaceCalendarList.Add(workspaceCalendarEntity);

                            currentStartAt = DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentStartAt, timeZone), day);
                            currentEndAt = DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentEndAt, timeZone), day);
                        }
                    });
                }

                if (data.WorkSpaceAvailability.Recurrence.Frequency == Frequency.DAILY && data.WorkSpaceAvailability.Recurrence.ByDay is null)
                {
                    WorkspaceCalendarEntity firsWorkspaceCalendarEntity = new WorkspaceCalendarEntity(
                        workspaceId: data.WorkspaceId,
                        startAt: data.WorkSpaceAvailability.StartAt,
                        endAt: data.WorkSpaceAvailability.EndAt
                    );

                    workspaceCalendarList.Add(firsWorkspaceCalendarEntity);
                    
                    while (DateOnly.FromDateTime(currentStartAt) < DateOnly.FromDateTime(untilOnTimezone))
                    {
                        WorkspaceCalendarEntity workspaceCalendarEntity = new WorkspaceCalendarEntity(
                            workspaceId: data.WorkspaceId,
                            startAt: DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentStartAt, timeZone), null),
                            endAt: DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentEndAt, timeZone), null)
                        );

                        workspaceCalendarList.Add(workspaceCalendarEntity);

                        currentStartAt = DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentStartAt, timeZone), null);
                        currentEndAt = DatesUtils.GetNextDay(TimeZoneInfo.ConvertTimeFromUtc(currentEndAt, timeZone), null);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            try
            {
                await transactionManager.StartSession();

                await workspaceCalendarRepository.InsertMany(workspaceCalendarList, transactionManager.Session);

                await transactionManager.CommitTransaction();

                return Result<bool>.Success(true);
            }
            catch (BulkWriteException ex)
            {
                var errors = new List<Error>();

                foreach (var error in ex.Errors)
                {
                    logger.LogWarning(error.Message);

                    if (error is DuplicateKeyException)
                    {
                        errors.Add(new Error(error.Message, ErrorType.Conflict));
                    }
                    else
                    {
                        errors.Add(new Error(error.Message, ErrorType.InternalServerError));
                    }
                }

                await transactionManager.AbortTransaction();

                return Result<bool>.Failure(errors);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Unexpted error ocurred: {ex}", ex);

                return Result<bool>.Failure(new List<Error> { new Error(ex.Message, ErrorType.InternalServerError) });
            }
        }
    }
}