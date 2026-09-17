namespace CoworkingBooking.Shared.Exceptions
{
    public class NotFoundException : Exception
    {

        public string Id { get; }
        public NotFoundException(string message, string id, Exception? innerException = null) : base(message, innerException)
        {
            this.Id = id;
        }
    }
}