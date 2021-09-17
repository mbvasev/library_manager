using System.Net.Http;

namespace Movies.Domain.SegregatedInterfaces
{
    internal interface IHttpMessageHandlerProvider
    {
        HttpMessageHandler CreateHttpMessageHandler();
    }
}
