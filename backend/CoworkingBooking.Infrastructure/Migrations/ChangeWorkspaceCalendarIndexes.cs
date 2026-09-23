using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Migrations
{
    public class ChangeWorkspaceCalendarIndexes : AbstractMigration
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceCalendarModel> collection;
        private readonly ILogger<ChangeWorkspaceCalendarIndexes> logger;


        public ChangeWorkspaceCalendarIndexes(
            MongodbDatabaseService mongodbDatabaseService,
            ILogger<ChangeWorkspaceCalendarIndexes> logger
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            this.collection = _mongodbDatabaseService.GetCollection<WorkspaceCalendarModel>("workspaces_calendar");
            this.logger = logger;
            this.Name = "2026-09-23T00:00:00Z_ChangeWorkspaceCalendarIndexes";
        }

        public async override Task Up()
        {
            logger.LogInformation("Initializing change Workspace Calendar indexes");

            await collection.Indexes.DropOneAsync("workspaceId_1_starAt_1_endAt_1");

            var compoundIndex = Builders<WorkspaceCalendarModel>.IndexKeys.Combine(
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.WorkspaceId),
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.StartAt),
                Builders<WorkspaceCalendarModel>.IndexKeys.Ascending(wc => wc.EndAt)
            );
            
            var compoundIndexOptions = new CreateIndexOptions<WorkspaceCalendarModel> { 
                Unique = true,
                PartialFilterExpression = Builders<WorkspaceCalendarModel>.Filter.Eq(wc => wc.IsInactive, false)
            };

            var compoundIndexMondel = new CreateIndexModel<WorkspaceCalendarModel>(compoundIndex, compoundIndexOptions);

            await collection.Indexes.CreateOneAsync(compoundIndexMondel);

            logger.LogInformation("Changed Workspace Calendar Indexes");
        }

        public override Task Down()
        {
            logger.LogWarning("Rollbacking Migration - {Name}", Name);
            throw new NotImplementedException();
        }

        
    }
}