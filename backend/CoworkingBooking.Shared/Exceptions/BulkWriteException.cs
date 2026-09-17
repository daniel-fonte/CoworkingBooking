namespace CoworkingBooking.Shared.Exceptions
{
    public class BulkWriteException : Exception
    {
        public IReadOnlyCollection<Exception> Errors { get; }

        public BulkWriteException(
            IReadOnlyCollection<Exception> errors,
            Exception innerException)
            : base("One or more errors occurred during bulk insert.", innerException)
        {
            Errors = errors;
        }
    }
}