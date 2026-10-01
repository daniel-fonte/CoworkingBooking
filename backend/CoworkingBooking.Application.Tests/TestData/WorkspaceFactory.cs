using CoworkingBooking.Core.Workspace.Entities;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Classes;
using CoworkingBooking.Shared.Enums;
using MongoDB.Bson;

namespace CoworkingBooking.Application.Tests.TestData
{
    public static class WorkspaceFactory
    {
        public const string Slug = "sala-1";
        public const string Name = "Sala 1";

        public static string NewId() => ObjectId.GenerateNewId().ToString();

        public static WorkspaceEntity Create(
            string id,
            WorkspaceStatus status = WorkspaceStatus.Draft,
            double pricePerHour = 50,
            WorkSpaceAvailability? availability = null
        )
        {
            var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return WorkspaceEntity.Rehydrate(
                id: id,
                name: Name,
                description: "Sala de testes",
                slug: Slug,
                type: WorkspaceType.MeetingRoom,
                status: status,
                coordinates: new GeoJson { type = "Point", coordinates = new double[] { -46.63, -23.55 } },
                pricePerHour: pricePerHour,
                isInactive: false,
                resources: new List<string> { "wifi" },
                createdAt: createdAt,
                updatedAt: createdAt,
                availability: availability
            );
        }

        public static WorkSpaceAvailability CreateDailyAvailability()
        {
            return new WorkSpaceAvailability(
                new DateTime(2026, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 5, 18, 0, 0, DateTimeKind.Utc),
                new WorkSpaceAvailabilityRecurrence(Frequency.DAILY, new DateTime(2026, 2, 5, 18, 0, 0, DateTimeKind.Utc), null, null),
                "UTC"
            );
        }
    }
}
