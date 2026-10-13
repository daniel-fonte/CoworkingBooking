using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Migrations
{
    public class AddWorkspaceCalendarVersionField : AbstractMigration
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceCalendarModel> collection;
        private readonly ILogger<ChangeWorkspaceCalendarIndexes> logger;

        public AddWorkspaceCalendarVersionField(
            MongodbDatabaseService mongodbDatabaseService,
            ILogger<ChangeWorkspaceCalendarIndexes> logger
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.logger = logger;
            Name = "2026-10-12T19:06:52Z_AddWorkspaceCalendarVersionField";
        }

        public override async Task Up()
        {
            logger.LogInformation("Initializing add Workspace Calendar Version field");

            var filter = Builders<WorkspaceCalendarModel>.Filter.Exists(wc => wc.Version, false);

            var update = Builders<WorkspaceCalendarModel>.Update.Set(wc => wc.Version, 0);

            await collection.UpdateManyAsync(filter, update);

            logger.LogInformation("Added Workspace Calendar Version field");
        }

        public override async Task Down()
        {
            logger.LogWarning("Rollbacking Migration - {Name}", Name);
            
            var filter = Builders<WorkspaceCalendarModel>.Filter.Exists(wc => wc.Version, true);

            var update = Builders<WorkspaceCalendarModel>.Update.Unset(wc => wc.Version);

            await collection.UpdateManyAsync(filter, update);
        }
    }
}