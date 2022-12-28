using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            builder.Services.AddSingleton<ITwinIdService>(_ => new TwinIdService());
        }
    }
}
