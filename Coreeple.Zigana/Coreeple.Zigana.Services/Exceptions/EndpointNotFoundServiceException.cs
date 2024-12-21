namespace Coreeple.Zigana.Services.Exceptions;
public class EndpointNotFoundServiceException : Exception
{
    public EndpointNotFoundServiceException() : base("Endpoint not found!") { }
}