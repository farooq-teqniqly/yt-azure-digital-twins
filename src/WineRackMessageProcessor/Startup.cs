using System;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WineRackMessageProcessor.Services;

[assembly: FunctionsStartup(typeof(WineRackMessageProcessor.Startup))]
namespace WineRackMessageProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            var credential = new EnvironmentCredential();
            var client = new DigitalTwinsClient(new Uri(config["AdtEndpoint"]), credential);
            builder.Services.AddSingleton(client);

            var twinIdService = new TwinIdService();

            builder.Services.AddSingleton<ITwinIdService>(_ => twinIdService);

            builder.Services.AddSingleton<ITwinRepository>(_ => new TwinRepository(client, twinIdService));
        }
    }
}
