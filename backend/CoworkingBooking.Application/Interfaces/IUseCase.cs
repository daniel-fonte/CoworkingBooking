using CoworkingBooking.Shared.Classes;

namespace CoworkingBooking.Application.Interfaces
{
    public interface IUseCase<TRequest, TResponse>
    {
        Task<Result<TResponse>> Execute(TRequest request);
    }
}