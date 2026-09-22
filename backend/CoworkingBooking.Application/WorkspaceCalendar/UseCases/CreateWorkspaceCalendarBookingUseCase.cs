using CoworkingBooking.Application.Interfaces;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.Mappers;
using CoworkingBooking.Application.WorkspaceCalendar.Ports;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Shared.Classes;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Application.WorkspaceCalendar.UseCases
{
    public class CreateWorkspaceCalendarBookingUseCase : IUseCase<(string id, CreateWorkspaceCalendarBookingRequestDTO request), bool>
    {
        private readonly IWorkspaceCalendarRepository workspaceCalendarRepository;
        private readonly ILogger<CreateWorkspaceCalendarBookingUseCase> logger;
        private readonly IWorkspaceExistenceCheckerPort workspaceExistenceCheckerPort;
        private readonly IValidator<CreateWorkspaceCalendarBookingRequestDTO> validator;
        private readonly WorkspaceCalendarBookingMapper workspaceCalendarBookingMapper;
        
        public CreateWorkspaceCalendarBookingUseCase(
            IWorkspaceCalendarRepository workspaceCalendarRepository,
            ILogger<CreateWorkspaceCalendarBookingUseCase> logger,
            IWorkspaceExistenceCheckerPort workspaceExistenceCheckerPort,
            IValidator<CreateWorkspaceCalendarBookingRequestDTO> validator,
            WorkspaceCalendarBookingMapper workspaceCalendarBookingMapper
        )
        {
            this.workspaceCalendarRepository = workspaceCalendarRepository;
            this.logger = logger;
            this.workspaceExistenceCheckerPort = workspaceExistenceCheckerPort;
            this.validator = validator;
            this.workspaceCalendarBookingMapper = workspaceCalendarBookingMapper;
        }

        public async Task<Result<bool>> Execute((string id, CreateWorkspaceCalendarBookingRequestDTO request) input)
        {
            var (id, request) = input;

            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(
                    validationResult.Errors.Select(e => new Error(e.ErrorMessage, ErrorType.ValidationError)).ToList()
                );
            }

            var workspaceCalendarFound = await workspaceCalendarRepository.FindOneById(id);

            if (workspaceCalendarFound == null)
            {
                logger.LogWarning("Workspace Calendar {Id} not found", id);
                return Result<bool>.Failure(new List<Error> { new Error($"Workspace Calendar {id} not found", ErrorType.NotFound)});
            }

            var workspaceExists = await workspaceExistenceCheckerPort.Execute(workspaceCalendarFound.WorkspaceId);

            if (workspaceExists == null)
            {
                logger.LogWarning("Workspace {Id} not found", workspaceCalendarFound.WorkspaceId);
                return Result<bool>
                    .Failure(new List<Error> { new Error($"Workspace {workspaceCalendarFound.WorkspaceId} not found", ErrorType.NotFound)});
            }

            var workspaceCalendarBooking = workspaceCalendarBookingMapper.ToEntity(request);
            
            workspaceCalendarBooking.CalculateTotalPrice(workspaceExists.PricePerHour);

            if (workspaceExists.Availability == null)
            {
                logger.LogWarning("Workspace Availability {Id} not exists", workspaceCalendarFound.WorkspaceId);
                return Result<bool>
                    .Failure(new List<Error> { new Error($"Workspace Availability {workspaceCalendarFound.WorkspaceId} not exists", ErrorType.NotFound)});
            }

            workspaceCalendarFound.AddBooking(workspaceCalendarBooking);

            var workspaceCalendarBookingInserted = await workspaceCalendarRepository
                .UpdateBooking(workspaceCalendarFound.Id, workspaceCalendarFound.Bookings.ToList());

            return Result<bool>.Success(true);
        }
    }
}