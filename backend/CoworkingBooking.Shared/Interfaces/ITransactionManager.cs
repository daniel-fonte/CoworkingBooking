using MongoDB.Driver;

namespace CoworkingBooking.Shared.Interfaces
{
    public interface ITransactionManager
    {
        Task StartSession();
        Task CommitTransaction();
        Task AbortTransaction();
        IClientSessionHandle Session { get; }
        void Dispose();
    }
}