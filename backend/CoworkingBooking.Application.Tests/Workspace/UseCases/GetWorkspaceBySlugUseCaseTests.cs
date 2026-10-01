using CoworkingBooking.Application.Tests.TestData;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.UseCases;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoworkingBooking.Application.Tests.Workspace.UseCases
{
    [TestFixture]
    public class GetWorkspaceBySlugUseCaseTests
    {
        private static GetWorkspaceBySlugUseCase CreateUseCase(ICacheRepository<WorkspaceEntity> cacheRepository) => new(
            new Mock<IWorkspaceRepository>().Object,
            cacheRepository,
            new WorkspaceMapper(),
            NullLogger<GetWorkspaceBySlugUseCase>.Instance
        );

        [Test]
        public async Task Execute_WorkspaceExists_ReturnsDetails()
        {
            var workspaceId = WorkspaceFactory.NewId();
            var cacheRepository = new Mock<ICacheRepository<WorkspaceEntity>>();
            cacheRepository
                .Setup(c => c.GetByKey($"workspace:{WorkspaceFactory.Slug}", It.IsAny<Func<string, Task<WorkspaceEntity?>>>()))
                .ReturnsAsync(WorkspaceFactory.Create(workspaceId));
            var useCase = CreateUseCase(cacheRepository.Object);

            var result = await useCase.Execute(WorkspaceFactory.Slug);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Slug, Is.EqualTo(WorkspaceFactory.Slug));
            Assert.That(result.Data.Name, Is.EqualTo(WorkspaceFactory.Name));
        }

        [Test]
        public async Task Execute_WorkspaceNotFound_ReturnsNotFound()
        {
            var cacheRepository = new Mock<ICacheRepository<WorkspaceEntity>>();
            cacheRepository
                .Setup(c => c.GetByKey(It.IsAny<string>(), It.IsAny<Func<string, Task<WorkspaceEntity?>>>()))
                .ReturnsAsync((WorkspaceEntity?)null);
            var useCase = CreateUseCase(cacheRepository.Object);

            var result = await useCase.Execute("inexistente");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.Single().Type, Is.EqualTo(ErrorType.NotFound));
        }
    }
}
