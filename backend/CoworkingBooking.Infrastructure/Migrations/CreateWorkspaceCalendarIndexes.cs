using CoworkingBooking.Core.WorkspaceCalendar.Repositories;
using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Migrations
{
    public class CreateWorkspaceCalendarIndexes : AbstractMigration
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceCalendarModel> collection;
        private readonly ILogger<CreateWorkspaceCalendarIndexes> logger;


        public CreateWorkspaceCalendarIndexes(
            MongodbDatabaseService mongodbDatabaseService,
            ILogger<CreateWorkspaceCalendarIndexes> logger
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            this.collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.logger = logger;
            this.Name = "2026-09-12T00:00:00Z_CreateWorkspaceCalendarIndexes";
        }

        public override async Task Up()
        {
            logger.LogInformation("Initializing create Workspace Calendar indexes");
            
            var compoundIndex = Builders<WorkspaceCalendarModel>.IndexKeys.Combine(
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.WorkspaceId),
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.StartAt),
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.EndAt)
            );
            var compoundIndexOptions = new CreateIndexOptions { Unique = true };
            var compoundIndexMondel = new CreateIndexModel<WorkspaceCalendarModel>(compoundIndex, compoundIndexOptions);

            await collection.Indexes.CreateOneAsync(compoundIndexMondel);

            logger.LogInformation("Created Workspace Indexes");
        }

        public override async Task Down()
        {
            logger.LogWarning("Rollbacking Migration - {Name}", Name);
            await collection.Indexes.DropOneAsync("workspaceId_1_starAt_1_endAt_1");
        }
    }
}