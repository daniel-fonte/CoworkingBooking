using CoworkingBooking.Infraestructure.Models;
using MongoDB.Driver;
using CoworkingBooking.Infraestructure.Migrations;
using Microsoft.Extensions.Logging;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class MigrationRunner
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<MigrationModel> _collection;
        private readonly ILogger<MigrationRunner> _logger;
        private readonly IEnumerable<AbstractMigration> _migrations;

        public MigrationRunner(MongodbDatabaseService mongodbDatabaseService, ILogger<MigrationRunner> logger, IEnumerable<AbstractMigration> migrations)
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            _collection = _mongodbDatabaseService.GetCollection<MigrationModel>("_migrations");
            _logger = logger;
            _migrations = migrations.OrderBy(x => x.Id).ToList();
        }

        public async Task RunMigrations()
        {
            _logger.LogInformation("Starting migration process...");

            foreach (var migration in _migrations)
            {
                var existingMigration = await _collection.Find(m => m.Name == migration.Name).FirstOrDefaultAsync();

                if (existingMigration == null)
                {
                    _logger.LogInformation($"Running migration: {migration.Name}");
                    await migration.Up();

                    var newMigration = new MigrationModel { Name = migration.Name };
                    await _collection.InsertOneAsync(newMigration);
                    _logger.LogInformation($"Migration completed: {migration.Name}");
                }
            }
        }
    }
}