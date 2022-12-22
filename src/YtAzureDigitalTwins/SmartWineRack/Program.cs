using System.CommandLine;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Rest;
using IotHubConnectionStringBuilder = Microsoft.Azure.Devices.IotHubConnectionStringBuilder;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace SmartWineRack
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var onboardCommand = new Command("onboard", "Onboard this device.");
            
            var connectionStringArg = new Argument<string>(
                name: "connection-string",
                description: "The Azure IoT Hub connection string.");

            var deviceNameArg = new Argument<string>(
                name: "name",
                description: "The name of the device.");

            onboardCommand.AddArgument(connectionStringArg);
            onboardCommand.AddArgument(deviceNameArg);

            onboardCommand.SetHandler(async (cs, dn) =>
            {
                var registryManager = RegistryManager.CreateFromConnectionString(cs);
                var device = new Device(dn);

                await registryManager.AddDeviceAsync(device);
            }, connectionStringArg, deviceNameArg);

            var rootCommand = new RootCommand("Smart Wine Rack Emulator");
            rootCommand.AddCommand(onboardCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}