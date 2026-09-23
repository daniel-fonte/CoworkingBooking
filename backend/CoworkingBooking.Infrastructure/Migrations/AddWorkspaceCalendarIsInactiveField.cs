using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Migrations
{
    public class AddWorkspaceCalendarIsInactiveField : AbstractMigration
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceCalendarModel> collection;
        private readonly ILogger<ChangeWorkspaceCalendarIndexes> logger;


        public AddWorkspaceCalendarIsInactiveField(
            MongodbDatabaseService mongodbDatabaseService,
            ILogger<ChangeWorkspaceCalendarIndexes> logger
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            this.collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.logger = logger;
            this.Name = "2026-09-23T19:06:52Z_AddWorkspaceCalendarIsInactiveField";
        }
        
        public override async Task Up()
        {
            logger.LogInformation("Initializing add Workspace IsInactive field");

            var filter = Builders<WorkspaceCalendarModel>.Filter.Exists(wc => wc.IsInactive, false);

            var update = Builders<WorkspaceCalendarModel>.Update.Set(wc => wc.IsInactive, false);

            await this.collection.UpdateManyAsync(filter, update);

            logger.LogInformation("Added Workspace Calendar IsInactive field");
        }

        public override async Task Down()
        {
            logger.LogWarning("Rollbacking Migration - {Name}", Name);
            
            var filter = Builders<WorkspaceCalendarModel>.Filter.Exists(wc => wc.IsInactive, true);

            var update = Builders<WorkspaceCalendarModel>.Update.Unset(wc => wc.IsInactive);

            await this.collection.UpdateManyAsync(filter, update);
        }
    
    }
}