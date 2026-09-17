using CoworkingBooking.Shared.Interfaces;
using MongoDB.Driver;

namespace CoworkingBooking.Infraestructure.Providers
{
    public class MongodbTransactionManagerService : ITransactionManager
    {
        private readonly MongoClient _client;
        private IClientSessionHandle? _session;

        public MongodbTransactionManagerService(MongodbDatabaseService mongodbDatabaseService)
        {
            _client = mongodbDatabaseService.GetMongoClient();
        }

        public IClientSessionHandle Session =>
            _session ?? throw new InvalidOperationException("Session não foi iniciada.");

        public async Task StartSession()
        {
            _session = await _client.StartSessionAsync();
            _session.StartTransaction();
        }

        public async Task CommitTransaction()
        {
            await _session!.CommitTransactionAsync();
        }

        public async Task AbortTransaction()
        {
            await _session!.AbortTransactionAsync();
        }

        public void Dispose()
        {
            _session?.Dispose();
            _session = null;
        }
    }
}