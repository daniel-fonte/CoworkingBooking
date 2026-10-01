using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Application.Workspace.Mappers;
using CoworkingBooking.Application.Workspace.UseCases;
using CoworkingBooking.Application.Workspace.Validators;
using CoworkingBooking.Core.Workspace.Constraints;
using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Core.Workspace.Repositories;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoworkingBooking.Application.Tests.Workspace.UseCases
{
    [TestFixture]
    public class CreateWorkspaceUseCaseTests
    {
        private static CreateWorkspaceUseCase CreateUseCase(IWorkspaceRepository workspaceRepository) => new(
            NullLogger<CreateWorkspaceUseCase>.Instance,
            workspaceRepository,
            new CreateWorkspaceValidator(),
            new WorkspaceMapper()
        );

        private static CreateWorkspaceRequestDTO BuildRequest() => new(
            Name: "Sala de Reuniao",
            Description: "Sala com projetor",
            Type: WorkspaceType.MeetingRoom,
            Coordinates: new Coordinates { lat = -23.55, lgn = -46.63 },
            PricePerHour: 50,
            Resources: new List<string> { "projetor", "wifi" }
        );

        [Test]
        public async Task Execute_ValidRequest_ReturnsSuccessWithGeneratedSlug()
        {
            var workspaceRepository = new Mock<IWorkspaceRepository>();
            workspaceRepository
                .Setup(r => r.InsertOne(It.IsAny<WorkspaceEntity>()))
                .ReturnsAsync((WorkspaceEntity entity) => entity);
            var useCase = CreateUseCase(workspaceRepository.Object);

            var result = await useCase.Execute(BuildRequest());

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Slug, Is.EqualTo("sala-de-reuniao"));
            Assert.That(result.Data.Status, Is.EqualTo(WorkspaceStatus.Draft));
            workspaceRepository.Verify(r => r.InsertOne(It.Is<WorkspaceEntity>(w => w.Slug == "sala-de-reuniao")), Times.Once);
        }

        [Test]
        public async Task Execute_DuplicateName_ReturnsConflict()
        {
            var workspaceRepository = new Mock<IWorkspaceRepository>();
            workspaceRepository
                .Setup(r => r.InsertOne(It.IsAny<WorkspaceEntity>()))
                .ThrowsAsync(new DuplicateKeyException(new List<string> { WorkspaceConstraints.Slug }, "sala-de-reuniao"));
            var useCase = CreateUseCase(workspaceRepository.Object);

            var result = await useCase.Execute(BuildRequest());

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors!.Single().Type, Is.EqualTo(ErrorType.Conflict));
        }
    }
}
