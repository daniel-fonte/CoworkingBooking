using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Application.Workspace.Dtos
{
    public sealed record CreateWorkspaceRequestDTO(
        string Name,
        string Description,
        WorkspaceType Type,
        Coordinates Coordinates,
        double PricePerHour,
        List<string> Resources
    );

    public sealed record CreateWorkspaceResponseDTO(
        string Slug,
        string Name,
        string Description,
        WorkspaceStatus Status,
        WorkspaceType Type,
        Coordinates Coordinates,
        double PricePerHour,
        List<string> Resources
    );

}