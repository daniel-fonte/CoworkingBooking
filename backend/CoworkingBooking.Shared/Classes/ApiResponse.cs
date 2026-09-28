namespace CoworkingBooking.Shared.Classes
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public List<Error> Errors { get; set; }

        public ApiResponse(bool success, List<Error>? errors = null)
        {
            Success = success;
            Errors = errors ?? [];
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponse(
            bool success,
            T? data = default,
            List<Error>? errors = null
        ) : base(success, errors)
        {
            Data = data;
        }
    }
}