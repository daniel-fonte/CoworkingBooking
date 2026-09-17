namespace CoworkingBooking.Shared.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public string Value { get;}

        public DuplicateKeyException(
            List<string> constraints, 
            string value, 
            Exception? innerException = null
        ) : base($"Duplicate key to Index: '{string.Join("-", constraints)}' Value: '{value}'.", innerException)
        {
            this.Value = value;
        }
    }
}