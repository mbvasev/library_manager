using Test.Acceptance.Doubles.Spies;
using Test.Acceptance.Mediators;
using Movies.Domain.ConfigurationProviders;
using Movies.Domain.ServiceLocators;
using Movies.Domain.Services.ImdbService;
using System.Net.Http;

namespace Test.Acceptance.Domain.ServiceLocators
{
    internal sealed class ServiceLocatorForAcceptanceTesting : ServiceLocatorBase
    {
        private readonly Mediator _testMediator;
        public ServiceLocatorForAcceptanceTesting(Mediator testMediator)
        {
            _testMediator = testMediator;
        }

        protected override ConfigurationProviderBase CreateConfigurationProviderCore() => new ConfigurationProvider();

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpMessageHandlerSpy(_testMediator);
        }

        protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
        {
            return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
        }
    }
}
