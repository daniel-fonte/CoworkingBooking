using CoworkingBooking.Application.Tests.TestData;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.Mappers;
using CoworkingBooking.Application.WorkspaceCalendar.Ports;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Application.WorkspaceCalendar.Validators;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Shared.Classes;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoworkingBooking.Application.Tests.WorkspaceCalendar.UseCases
{
    [TestFixture]
    public class CreateWorkspaceCalendarBookingUseCaseTests
    {
        private static CreateWorkspaceCalendarBookingUseCase CreateUseCase(
            IWorkspaceCalendarRepository workspaceCalendarRepository,
            IWorkspaceExistenceCheckerPort workspaceExistenceChecker
        ) => new(
            workspaceCalendarRepository,
            NullLogger<CreateWorkspaceCalendarBookingUseCase>.Instance,
            workspaceExistenceChecker,
            new CreateWorkspaceCalendarBookingValidator(),
            new WorkspaceCalendarBookingMapper()
        );

        [Test]
        public async Task Execute_ValidBooking_ReturnsTotalPrice()
        {
            var calendarId = WorkspaceFactory.NewId();
            var workspaceId = WorkspaceFactory.NewId();
            var workspaceCalendarRepository = new Mock<IWorkspaceCalendarRepository>();
            var workspaceExistenceChecker = new Mock<IWorkspaceExistenceCheckerPort>();
            var useCase = CreateUseCase(workspaceCalendarRepository.Object, workspaceExistenceChecker.Object);

            var day = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);
            var existingBooking = new WorkspaceCalendarBooking(day.AddHours(8), day.AddHours(9));
            var calendar = WorkspaceCalendarEntity.Rehydrate(
                id: calendarId,
                workspaceId: workspaceId,
                startAt: day.AddHours(8),
                endAt: day.AddHours(18),
                bookings: new List<WorkspaceCalendarBooking> { existingBooking },
                isFull: false,
                isInactive: false,
                createdAt: day,
                updatedAt: day
            );

            workspaceCalendarRepository.Setup(r => r.FindOneById(calendarId)).ReturnsAsync(calendar);
            workspaceExistenceChecker
                .Setup(w => w.Execute(workspaceId))
                .ReturnsAsync(WorkspaceFactory.Create(workspaceId, pricePerHour: 50, availability: WorkspaceFactory.CreateDailyAvailability()));
            workspaceCalendarRepository
                .Setup(r => r.UpdateBooking(calendarId, It.IsAny<List<WorkspaceCalendarBooking>>()))
                .ReturnsAsync((string _, List<WorkspaceCalendarBooking> bookings) => bookings.Last());

            var request = new CreateWorkspaceCalendarBookingRequestDTO("2026-01-05T10:00:00Z", "2026-01-05T12:00:00Z");

            var result = await useCase.Execute((calendarId, request));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.TotalPrice, Is.EqualTo(100));
            workspaceCalendarRepository.Verify(
                r => r.UpdateBooking(calendarId, It.Is<List<WorkspaceCalendarBooking>>(b => b.Count == 2)),
                Times.Once
            );
            workspaceCalendarRepository.Verify(r => r.UpdateAvailability(calendarId, false), Times.Once);
        }

        [Test]
        public async Task Execute_CalendarNotFound_ReturnsNotFound()
        {
            var calendarId = WorkspaceFactory.NewId();
            var workspaceCalendarRepository = new Mock<IWorkspaceCalendarRepository>();
            var workspaceExistenceChecker = new Mock<IWorkspaceExistenceCheckerPort>();
            var useCase = CreateUseCase(workspaceCalendarRepository.Object, workspaceExistenceChecker.Object);

            workspaceCalendarRepository
                .Setup(r => r.FindOneById(It.IsAny<string>()))
                .ReturnsAsync((WorkspaceCalendarEntity?)null);

            var request = new CreateWorkspaceCalendarBookingRequestDTO("2026-01-05T10:00:00Z", "2026-01-05T12:00:00Z");

            var result = await useCase.Execute((calendarId, request));

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.Single().Type, Is.EqualTo(ErrorType.NotFound));
            workspaceExistenceChecker.Verify(w => w.Execute(It.IsAny<string>()), Times.Never);
        }
    }
}
