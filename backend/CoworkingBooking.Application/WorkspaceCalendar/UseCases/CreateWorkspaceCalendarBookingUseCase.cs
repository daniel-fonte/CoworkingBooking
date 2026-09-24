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
    public class CreateWorkspaceCalendarBookingUseCase : IUseCase<(string id, CreateWorkspaceCalendarBookingRequestDTO request), CreateWorkspaceCalendarBookingResponseDTO>
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

        public async Task<Result<CreateWorkspaceCalendarBookingResponseDTO>> Execute((string id, CreateWorkspaceCalendarBookingRequestDTO request) input)
        {
            try
            {
                var (id, request) = input;

                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>.Failure(
                        validationResult.Errors.Select(e => new Error(e.ErrorMessage, ErrorType.ValidationError)).ToList()
                    );
                }

                var workspaceCalendarFound = await workspaceCalendarRepository.FindOneById(id);

                if (workspaceCalendarFound == null)
                {
                    logger.LogWarning("Workspace Calendar {Id} not found", id);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>.Failure([new Error($"Workspace Calendar {id} not found", ErrorType.NotFound)]);
                }

                if (!workspaceCalendarFound.IsAvailable())
                {
                    logger.LogWarning("Workspace Calendar {Id} is fulled", workspaceCalendarFound.Id);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>.Failure([new Error($"Workspace Calendar {workspaceCalendarFound.Id} is fulled", ErrorType.ValidationError)]);
                }

                var workspaceExists = await workspaceExistenceCheckerPort.Execute(workspaceCalendarFound.WorkspaceId);

                if (workspaceExists == null)
                {
                    logger.LogWarning("Workspace {Id} not found", workspaceCalendarFound.WorkspaceId);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>
                        .Failure(new List<Error> { new Error($"Workspace {workspaceCalendarFound.WorkspaceId} not found", ErrorType.NotFound)});
                }

                var workspaceCalendarBooking = workspaceCalendarBookingMapper.ToEntity(request);
                
                workspaceCalendarBooking.CalculateTotalPrice(workspaceExists.PricePerHour);

                if (workspaceExists.Availability == null)
                {
                    logger.LogWarning("Workspace Availability {Id} not exists", workspaceCalendarFound.WorkspaceId);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>
                        .Failure(new List<Error> { new Error($"Workspace Availability {workspaceCalendarFound.WorkspaceId} not exists", ErrorType.NotFound)});
                }

                workspaceCalendarFound.AddBooking(workspaceCalendarBooking);

                if (!workspaceCalendarFound.IsAvailable())
                {
                    workspaceCalendarFound.CloseBookings();
                }

                var workspaceCalendarBookingInserted = await workspaceCalendarRepository
                    .UpdateBooking(workspaceCalendarFound.Id, workspaceCalendarFound.Bookings.ToList());

                if (workspaceCalendarBookingInserted == null)
                {
                    logger.LogError("Workspace Calendar {Id} not updated", id);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>
                        .Failure([ new Error($"Workspace Calendar {id} not updated", ErrorType.InternalServerError) ]);
                }

                await workspaceCalendarRepository.UpdateAvailability(workspaceCalendarFound.Id, workspaceCalendarFound.IsFull);

                var response = workspaceCalendarBookingMapper.ToCreateResponseDTO(workspaceCalendarBookingInserted);

                return Result<CreateWorkspaceCalendarBookingResponseDTO>.Success(response);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException) 
                {
                    logger.LogWarning(ex, ex.Message);
                    return Result<CreateWorkspaceCalendarBookingResponseDTO>
                        .Failure([ new Error(ex.Message, ErrorType.ValidationError) ]);
                }

                logger.LogError(ex, "Occured unexpected error.");
                return Result<CreateWorkspaceCalendarBookingResponseDTO>
                    .Failure([ new Error("An unexpected error occurred.", ErrorType.InternalServerError) ]);
            }
        }
    }
}