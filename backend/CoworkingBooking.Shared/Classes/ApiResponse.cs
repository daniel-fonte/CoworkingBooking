namespace CoworkingBooking.Shared.Classes
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<Error> Errors { get; set; }

        public ApiResponse(bool success, T data, List<Error> errors)
        {
            Success = success;
            Data = data;
            Errors = errors;
        }
    }
}