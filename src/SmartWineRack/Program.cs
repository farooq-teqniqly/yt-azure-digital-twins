using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SmartWineRack
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Smart Wine Rack Emulator");
            rootCommand.SetupOnboardIoTHubCommand();
            rootCommand.SetupConfigCommand();
            rootCommand.SetupBottleCommand();
            rootCommand.SetupDecommissionCommand();
 
            return await rootCommand.InvokeAsync(args);
        }
    }
}