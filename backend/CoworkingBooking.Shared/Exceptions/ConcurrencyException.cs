namespace CoworkingBooking.Shared.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() : base("Operation already executed")
        {
            
        }
    }
}