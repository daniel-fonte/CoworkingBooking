using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoworkingBooking.Infraestructure.Providers
{

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }

    public class MongodbDatabaseService
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongodbDatabaseService(
            IOptions<MongoDbSettings> mongoDbSettings,
            ILogger<MongodbDatabaseService> logger)
        {
            var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Value.ConnectionString);

            settings.MaxConnectionPoolSize = 10;
            settings.MinConnectionPoolSize = 1;
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(20);

            _client = new MongoClient(settings);

            _database = _client.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));

            logger.LogInformation(
                "MongoDB connection established successfully for database {DatabaseName}",
                mongoDbSettings.Value.DatabaseName
            );
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public MongoClient GetMongoClient()
        {
            return _client;
        }
    }
}