using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Shared.Interfaces
{
    public interface IPublish
    {
        Task Initialize();
        Task<Result<bool>> EnqueueMessage<T>(T @event);
    }
}