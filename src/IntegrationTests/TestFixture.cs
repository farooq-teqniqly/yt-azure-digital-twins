using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace IntegrationTests
{
    public class TestFixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        {
            var credential = new ClientSecretCredential(
                configuration["AZURE_TENANT_ID"],
                configuration["AZURE_CLIENT_ID"],
                configuration["AZURE_CLIENT_SECRET"]);

            var client = new DigitalTwinsClient(new Uri(configuration["AdtEndpoint"]), credential);
            services.AddSingleton(client);

        }

        protected override IEnumerable<TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "local.settings.json", IsOptional = false };
        }

        protected override ValueTask DisposeAsyncCore() => new();
    }
}