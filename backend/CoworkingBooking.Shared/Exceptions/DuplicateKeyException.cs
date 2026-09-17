namespace CoworkingBooking.Shared.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public string Id { get;}
        public DuplicateKeyException(string key, string id, Exception? innerException = null) : base($"Duplicate key to '{key}' - '{id}'.", innerException)
        {
            this.Id = id;
        }
    }
}