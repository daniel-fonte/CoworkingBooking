using CoworkingBooking.Infraestructure.Models;
using CoworkingBooking.Infraestructure.Providers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Migrations
{
    public class CreateWorkspaceIndexes : AbstractMigration
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<WorkspaceModel> collection;
        private readonly ILogger<CreateWorkspaceCalendarIndexes> logger;

        public CreateWorkspaceIndexes(
            MongodbDatabaseService mongodbDatabaseService,
            ILogger<CreateWorkspaceCalendarIndexes> logger
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            collection = _mongodbDatabaseService.GetCollection<WorkspaceModel>("workspaces");
            this.logger = logger;
            Name = "2026-09-01T00:00:00Z_CreateWorkspaceIndexes";
        }

        public override async Task Up()
        {
            logger.LogInformation("Initializing create Workspace Indexes");

            var slugIndex = Builders<WorkspaceModel>.IndexKeys.Ascending(w => w.Slug);
            var slugIndexOptions = new CreateIndexOptions { Unique = true };
            var slugIndexModel = new CreateIndexModel<WorkspaceModel>(slugIndex, slugIndexOptions);

            var coordinatesIndex = Builders<WorkspaceModel>.IndexKeys.Geo2DSphere(w => w.Coordinates);
            var coordinatesIndexModel = new CreateIndexModel<WorkspaceModel>(coordinatesIndex);

            var indexes = new List<CreateIndexModel<WorkspaceModel>> { slugIndexModel, coordinatesIndexModel };

            await collection.Indexes.CreateManyAsync(indexes);

            logger.LogInformation("Created Workspace Indexes");
        }

        public override async Task Down()
        {
            logger.LogWarning("Rollbacking Migration - {Name}", Name);
            await collection.Indexes.DropOneAsync("slug_1");
            await collection.Indexes.DropOneAsync("coordinates_2dsphere");
        }
    }
}