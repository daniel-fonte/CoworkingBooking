using System.Text.Json.Serialization;

namespace CoworkingBooking.Core.Workspace.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<WorkspaceType>))]
    public enum WorkspaceType
    {
        OpenDesk = 1,
        PrivateOffice = 2,
        MeetingRoom = 3,
        EventSpace = 4
    }

    [JsonConverter(typeof(JsonStringEnumConverter<WorkspaceStatus>))]
    public enum WorkspaceStatus
    {
        Available = 1,
        Reserved = 2,
        Maintenance = 3,
        Unavailable = 4,
        Draft = 5
    }
}