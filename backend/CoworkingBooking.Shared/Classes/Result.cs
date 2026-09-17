namespace CoworkingBooking.Shared.Classes
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public IReadOnlyCollection<Error>? Errors { get; set; }

        public static Result<T> Success(T value)
        {
            return new Result<T> { IsSuccess = true, Data = value };
        }

        public static Result<T> Failure(IReadOnlyCollection<Error> errors)
        {
            return new Result<T> { IsSuccess = false, Errors = errors };
        }
    }
}