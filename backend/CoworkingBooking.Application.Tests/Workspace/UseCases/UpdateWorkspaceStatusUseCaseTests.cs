using CoworkingBooking.Application.Tests.TestData;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.UseCases;
using CoworkingBooking.Application.Workspace.Validators;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoworkingBooking.Application.Tests.Workspace.UseCases
{
    [TestFixture]
    public class UpdateWorkspaceStatusUseCaseTests
    {
        private static UpdateWorkspaceStatusUseCase CreateUseCase(IWorkspaceRepository workspaceRepository) => new(
            NullLogger<UpdateWorkspaceStatusUseCase>.Instance,
            new UpdateWorkspaceStatusValidator(),
            workspaceRepository,
            new WorkspaceMapper()
        );

        [Test]
        public async Task Execute_ValidStatus_ReturnsUpdatedWorkspace()
        {
            var workspaceId = WorkspaceFactory.NewId();
            var workspaceRepository = new Mock<IWorkspaceRepository>();
            workspaceRepository
                .Setup(r => r.FindOneBySlug(WorkspaceFactory.Slug))
                .ReturnsAsync(WorkspaceFactory.Create(workspaceId));
            workspaceRepository
                .Setup(r => r.UpdateOneById(workspaceId, It.IsAny<WorkspaceEntity>()))
                .ReturnsAsync((string _, WorkspaceEntity entity) => entity);
            var useCase = CreateUseCase(workspaceRepository.Object);

            var result = await useCase.Execute((WorkspaceFactory.Slug, new UpdateWorkspaceStatusRequestDTO(WorkspaceStatus.Maintenance)));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.WorkspaceStatus, Is.EqualTo(WorkspaceStatus.Maintenance));
            workspaceRepository.Verify(
                r => r.UpdateOneById(workspaceId, It.Is<WorkspaceEntity>(w => w.Status == WorkspaceStatus.Maintenance)),
                Times.Once
            );
        }

        [Test]
        public async Task Execute_WorkspaceNotFound_ReturnsNotFoundAndDoesNotUpdate()
        {
            var workspaceRepository = new Mock<IWorkspaceRepository>();
            workspaceRepository
                .Setup(r => r.FindOneBySlug(It.IsAny<string>()))
                .ReturnsAsync((WorkspaceEntity?)null);
            var useCase = CreateUseCase(workspaceRepository.Object);

            var result = await useCase.Execute(("inexistente", new UpdateWorkspaceStatusRequestDTO(WorkspaceStatus.Maintenance)));

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.Single().Type, Is.EqualTo(ErrorType.NotFound));
            workspaceRepository.Verify(r => r.UpdateOneById(It.IsAny<string>(), It.IsAny<WorkspaceEntity>()), Times.Never);
        }
    }
}
