namespace CoworkingBooking.Shared.Classes
{
    public class Error
    {
        public string Message { get; set; }
        public ErrorType Type { get; set; }

        public Error(string message, ErrorType type)
        {
            Message = message;
            Type = type;
        }
    }

    public enum ErrorType
    {
        NotFound,
        ValidationError,
        InternalServerError,
        Conflict,
        Forbidden
    }
}