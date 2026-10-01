using CoworkingBooking.Application.Tests.TestData;
using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Application.WorkspaceCalendar.UseCases;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Entities;
using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;

namespace CoworkingBooking.Application.Tests.WorkspaceCalendar.UseCases
{
    [TestFixture]
    public class UpdateWorkspaceCalendarRecurrencesUseCaseTests
    {
        [Test]
        public async Task Execute_DailyRecurrence_InsertsOneCalendarPerDayAndCommits()
        {
            var workspaceId = WorkspaceFactory.NewId();
            var workspaceCalendarRepository = new Mock<IWorkspaceCalendarRepository>();
            var transactionManager = new Mock<ITransactionManager>();
            transactionManager.Setup(t => t.Session).Returns(new Mock<IClientSessionHandle>().Object);

            var useCase = new UpdateWorkspaceCalendarRecurrencesUseCase(
                workspaceCalendarRepository.Object,
                transactionManager.Object,
                NullLogger<UpdateWorkspaceCalendarRecurrencesUseCase>.Instance
            );

            var availability = new WorkSpaceAvailability(
                new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                new WorkSpaceAvailabilityRecurrence(Frequency.DAILY, new DateTime(2026, 1, 3, 12, 0, 0, DateTimeKind.Utc), null, null),
                "UTC"
            );
            List<WorkspaceCalendarEntity>? insertedCalendars = null;

            workspaceCalendarRepository
                .Setup(r => r.FindByWorkspaceAvailability(workspaceId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<WorkspaceCalendarEntity>());
            workspaceCalendarRepository
                .Setup(r => r.InsertMany(It.IsAny<List<WorkspaceCalendarEntity>>(), It.IsAny<IClientSessionHandle?>()))
                .Callback((List<WorkspaceCalendarEntity> entities, IClientSessionHandle? _) => insertedCalendars = entities)
                .ReturnsAsync(3);

            var result = await useCase.Execute(new UpdateWorkspaceCalendarRecurrenceRequestDTO(workspaceId, availability));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(insertedCalendars, Is.Not.Null);
            Assert.That(
                insertedCalendars!.Select(c => c.StartAt.Date),
                Is.EqualTo(new[] { new DateTime(2026, 1, 1), new DateTime(2026, 1, 2), new DateTime(2026, 1, 3) })
            );
            workspaceCalendarRepository.Verify(
                r => r.SoftDeleteManyByAvailability(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IClientSessionHandle?>()),
                Times.Never
            );
            transactionManager.Verify(t => t.CommitTransaction(), Times.Once);
        }
    }
}
