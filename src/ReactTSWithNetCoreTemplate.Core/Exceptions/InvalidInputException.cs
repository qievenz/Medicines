namespace ReactTSWithNetCoreTemplate.Core.Exceptions
{
    public class InvalidInputException : ArgumentException
    {
        public InvalidInputException(string message) : base(message) { }
        public InvalidInputException(string message, string paramName) : base(message, paramName) { }
        public InvalidInputException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }
        public InvalidInputException() : base("The input provided is invalid.") { }
    }
}
