using System.Net.Http;
using Movies.Domain.ConfigurationProviders;
using Movies.Domain.Services.ImdbService;

namespace Movies.Domain.ServiceLocators
{
    internal sealed class ServiceLocator : ServiceLocatorBase
    {
        protected override ConfigurationProviderBase CreateConfigurationProviderCore()
        {
            return new ConfigurationProvider();
        }

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpClientHandler();
        }

        protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
        {
            return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
        }
    }
}
