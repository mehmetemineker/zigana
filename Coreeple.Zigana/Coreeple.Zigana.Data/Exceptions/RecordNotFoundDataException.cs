namespace Coreeple.Zigana.Data.Exceptions;
public class RecordNotFoundDataException : Exception
{
    public RecordNotFoundDataException() { }
    public RecordNotFoundDataException(string message) : base(message) { }
    public RecordNotFoundDataException(string message, Exception inner) : base(message, inner) { }
}
