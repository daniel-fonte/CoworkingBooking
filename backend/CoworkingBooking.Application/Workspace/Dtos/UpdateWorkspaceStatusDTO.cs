using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Application.Workspace.Dtos
{
    public sealed record UpdateWorkspaceStatusRequestDTO(
        WorkspaceStatus status
    );

    public sealed record UpdateWorkspaceStatusResponseDTO(
        string Name,
        string Description,
        string Slug,
        WorkspaceStatus WorkspaceStatus,
        WorkspaceType Type,
        Coordinates Coordinates,
        double PricePerHour,
        bool IsInactive,
        List<string> Resources,
        string CreatedAt,
        string UpdatedAt
    );
}