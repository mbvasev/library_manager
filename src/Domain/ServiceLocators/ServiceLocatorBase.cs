using Movies.Domain.ConfigurationProviders;
using Movies.Domain.SegregatedInterfaces;
using Movies.Domain.Services.ImdbService;
using System.Net.Http;

namespace Movies.Domain.ServiceLocators
{
    internal abstract class ServiceLocatorBase : IHttpMessageHandlerProvider
    {
        public ImdbServiceGateway CreateImdbServiceGateway()
        {
            return CreateImdbServiceGatewayCore();
        }

        public ConfigurationProviderBase CreateConfigurationProvider()
        {
            return CreateConfigurationProviderCore();
        }

        public HttpMessageHandler CreateHttpMessageHandler()
        {
            return CreateHttpMessageHandlerCore();
        }

        protected abstract HttpMessageHandler CreateHttpMessageHandlerCore();
        protected abstract ConfigurationProviderBase CreateConfigurationProviderCore();
        protected abstract ImdbServiceGateway CreateImdbServiceGatewayCore();
    }
}
