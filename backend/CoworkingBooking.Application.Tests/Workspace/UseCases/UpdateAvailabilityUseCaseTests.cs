using CoworkingBooking.Application.Tests.TestData;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.Ports;
using CoworkingBooking.Application.Workspace.UseCases;
using CoworkingBooking.Application.Workspace.Validators;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.Workspace.Events;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Publishers;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoworkingBooking.Application.Tests.Workspace.UseCases
{
    [TestFixture]
    public class UpdateAvailabilityUseCaseTests
    {
        [Test]
        public async Task Execute_ValidAvailability_UpdatesStatusAndPublishesEvent()
        {
            var workspaceId = WorkspaceFactory.NewId();
            var workspaceRepository = new Mock<IWorkspaceRepository>();
            var publisher = new Mock<IUpdateWorkspaceAvailabilityPublisher>();
            var calendarExistenceChecker = new Mock<IWorkspaceCalendarExistenceCheckerPort>();
            var recurrenceMapper = new WorkspaceAvailabilityRecurrenceMapper();

            var useCase = new UpdateAvailabilityUseCase(
                workspaceRepository.Object,
                new UpdateWorkspaceAvailabilityValidator(),
                new WorkspaceMapper(),
                new WorkspaceAvailabilityMapper(recurrenceMapper),
                recurrenceMapper,
                NullLogger<UpdateAvailabilityUseCase>.Instance,
                publisher.Object,
                calendarExistenceChecker.Object
            );

            var request = new UpdateWorkspaceAvailabilityRequestDTO(
                StartAt: "2026-01-05T08:00:00Z",
                EndAt: "2026-01-05T18:00:00Z",
                Until: "2026-02-05T18:00:00Z",
                Frequency: Frequency.DAILY,
                ByDay: null,
                ByMonth: null,
                Timezone: "UTC"
            );
            var workspace = WorkspaceFactory.Create(workspaceId);
            var updatedWorkspace = WorkspaceFactory.Create(workspaceId, WorkspaceStatus.Available, availability: WorkspaceFactory.CreateDailyAvailability());

            workspaceRepository.Setup(r => r.FindOneBySlug(WorkspaceFactory.Slug)).ReturnsAsync(workspace);
            calendarExistenceChecker
                .Setup(c => c.Execute(workspaceId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<WorkspaceCalendarEntity>());
            workspaceRepository
                .Setup(r => r.UpdateAvailability(workspaceId, It.IsAny<WorkSpaceAvailability>()))
                .ReturnsAsync(workspace);
            workspaceRepository
                .Setup(r => r.UpdateStatusById(workspaceId, WorkspaceStatus.Available))
                .ReturnsAsync(updatedWorkspace);
            publisher
                .Setup(p => p.EnqueueMessage(It.IsAny<UpdatedWorkspaceAvailabilityEvent>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await useCase.Execute((WorkspaceFactory.Slug, request));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Frequency, Is.EqualTo(Frequency.DAILY));
            Assert.That(result.Data.Timezone, Is.EqualTo("UTC"));
            workspaceRepository.Verify(r => r.UpdateStatusById(workspaceId, WorkspaceStatus.Available), Times.Once);
            publisher.Verify(
                p => p.EnqueueMessage(It.Is<UpdatedWorkspaceAvailabilityEvent>(e => e.WorkspaceId == workspaceId)),
                Times.Once
            );
        }
    }
}
