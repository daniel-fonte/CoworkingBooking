using CoworkingBooking.Infraestructure.Models;
using MongoDB.Driver;
using CoworkingBooking.Infraestructure.Migrations;
using Microsoft.Extensions.Logging;
using CoworkingBooking.Shared.Interfaces;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class MigrationRunner
    {
        private readonly MongodbDatabaseService _mongodbDatabaseService;
        private readonly IMongoCollection<MigrationModel> _collection;
        private readonly ILogger<MigrationRunner> _logger;
        private readonly IEnumerable<AbstractMigration> _migrations;
        private readonly IDistribuedLock _distribuedLock;

        public MigrationRunner(
            MongodbDatabaseService mongodbDatabaseService, 
            ILogger<MigrationRunner> logger, 
            IEnumerable<AbstractMigration> migrations,
            IDistribuedLock distribuedLock
        )
        {
            _mongodbDatabaseService = mongodbDatabaseService;
            _collection = _mongodbDatabaseService.GetCollection<MigrationModel>("_migrations");
            _logger = logger;
            _migrations = migrations.OrderBy(x => x.Id).ToList();
            _distribuedLock = distribuedLock;
        }

        public async Task RunMigrations()
        {
            _logger.LogInformation("Starting migration process...");

            var lockToken = await _distribuedLock.AcquireLock("migrations", TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(0));

            if (lockToken is not null)
            {

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

                await _distribuedLock.ReleaseAsync("migrations", lockToken);
            }
        }
    }
}