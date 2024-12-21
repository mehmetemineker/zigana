namespace Coreeple.Zigana.Services.Exceptions;
public class HttpMethodNotAllowedServiceException : Exception
{
    public HttpMethodNotAllowedServiceException() : base("Http method not allowed!") { }
}