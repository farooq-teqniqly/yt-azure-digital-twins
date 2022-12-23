using System.CommandLine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Message = Microsoft.Azure.Devices.Client.Message;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace SmartWineRack
{
    public static class RootCommandExtensions
    {
        private static readonly string IotHubConnectionStringFile = Path.Join(Directory.GetCurrentDirectory(), "iothubcs.txt");
        private static readonly string DeviceNameFile = Path.Join(Directory.GetCurrentDirectory(), "devicename.txt");
        private static readonly string OrgNameFile = Path.Join(Directory.GetCurrentDirectory(), "orgname.txt");
        private static readonly string SlotCountFile = Path.Join(Directory.GetCurrentDirectory(), "slotcount.txt");
        private static readonly string SlotsFile = Path.Join(Directory.GetCurrentDirectory(), "slots.json");

        public static RootCommand SetupOnboardIoTHubCommand(this RootCommand rootCommand)
        {
            var onboardRootCommand = new Command("onboard", "Onboard this device.");
            var onboardIoTHubCommand = new Command("iothub", "Onboard this device to the Azure IoT Hub.");
            
            var connectionStringArg = new Argument<string>(
                name: "connection-string",
                description: "The Azure IoT Hub connection string.");

            var deviceNameArg = new Argument<string>(
                name: "name",
                description: "The name of the device.");

            onboardIoTHubCommand.AddArgument(connectionStringArg);
            onboardIoTHubCommand.AddArgument(deviceNameArg);

            onboardIoTHubCommand.SetHandler(async (cs, dn) =>
            {
                var registryManager = RegistryManager.CreateFromConnectionString(cs);
                var device = new Device(dn);

                await registryManager.AddDeviceAsync(device);

                await File.WriteAllTextAsync(DeviceNameFile, dn);
            }, connectionStringArg, deviceNameArg);

            onboardRootCommand.AddCommand(onboardIoTHubCommand);
            
            var onboardTwinCommand = new Command("twin", "Onboard this device to Azure Digital Twins.");

            var orgNameArg = new Argument<string>(
                name: "organization",
                description: "The name of the organization owning the device.");

            onboardTwinCommand.AddArgument(orgNameArg);

            var slotCountArg = new Argument<int>(
                name: "slotCount",
                description: "The number of slots this device supports.");

            onboardTwinCommand.AddArgument(slotCountArg);

            onboardTwinCommand.SetHandler(async (on, sc) =>
            {
                await File.WriteAllTextAsync(OrgNameFile, on);
                await File.WriteAllTextAsync(SlotCountFile, sc.ToString());
                
                await File.WriteAllTextAsync(SlotsFile, JsonConvert.SerializeObject(new string[sc]));

                var deviceName = await File.ReadAllTextAsync(DeviceNameFile);
                var connectionString = await File.ReadAllTextAsync(IotHubConnectionStringFile);

                var payload = JsonConvert.SerializeObject(new { deviceName, org = on, slotcount = sc, mtype = MessageTypes.OnboardTwin });
                
                var message = new Message(Encoding.UTF8.GetBytes(payload))
                {
                    ContentEncoding = "utf-8",
                    ContentType = "application/json"
                };
                
                var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
                await deviceClient.SendEventAsync(message);

            }, orgNameArg, slotCountArg);

            onboardRootCommand.AddCommand(onboardTwinCommand);

            rootCommand.AddCommand(onboardRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupConfigIoTHubCommand(this RootCommand rootCommand)
        {
            var configRootCommand = new Command("config", "Manage the device's Azure IoT Hub local configuration.");
            var setCommand = new Command("set", "Set the device's Azure IoT Hub local configuration.");


            var connectionStringArg = new Argument<string>(
                name: "connection-string",
                description: "The device specific Azure IoT Hub connection string.");

            setCommand.AddArgument(connectionStringArg);

            setCommand.SetHandler((cs) =>
            {
                File.WriteAllText(IotHubConnectionStringFile, cs);
            }, connectionStringArg);

            configRootCommand.AddCommand(setCommand);

            var showCommand = new Command("show", "Show the device's Azure IoT Hub local configuration.");

            showCommand.SetHandler(() =>
            {
                Console.WriteLine(File.ReadAllText(IotHubConnectionStringFile));
            });

            configRootCommand.AddCommand(showCommand);

            rootCommand.AddCommand(configRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupBottleCommand(this RootCommand rootCommand)
        {
            var bottleRootCommand = new Command("bottle", "Manage the wine rack's bottles.");
            var showCommand = new Command("list", "List the wine rack's bottles.");

            showCommand.SetHandler(async () =>
            {
                var slotsFileContent = await File.ReadAllTextAsync(SlotsFile);

                var slots = JsonConvert.DeserializeObject<string[]>(slotsFileContent) 
                            ?? throw new ArgumentNullException("Unable to read slots file!", null as Exception);

                var slotCount = int.Parse(await File.ReadAllTextAsync(SlotCountFile));

                PrintBottles(slotCount, slots);
            });

            bottleRootCommand.AddCommand(showCommand);

            var addCommand = new Command("add", "Add a bottle to the wine rack.");

            var slotNumberArg = new Argument<int>(
                name: "slotNumber",
                description: "The slot number where the bottle will go.");

            var upcCodeArg = new Argument<string>(
                name: "upcCode",
                description: "The bottle's UPC code.");

            addCommand.AddArgument(slotNumberArg);
            addCommand.AddArgument(upcCodeArg);

            addCommand.SetHandler(async (sn, upc) =>
            {
                var slotsFileContent = await File.ReadAllTextAsync(SlotsFile);

                var slots = JsonConvert.DeserializeObject<string[]>(slotsFileContent)
                            ?? throw new ArgumentNullException("Unable to read slots file!", null as Exception);

                var slotCount = int.Parse(await File.ReadAllTextAsync(SlotCountFile));

                slots[sn - 1] = upc;

                await File.WriteAllTextAsync(SlotsFile, JsonConvert.SerializeObject(slots));
                
                PrintBottles(slotCount, slots);

            }, slotNumberArg, upcCodeArg);

            bottleRootCommand.AddCommand(addCommand);

            var removeCommand = new Command("remove", "Remove a bottle from the wine rack.");
            
            removeCommand.AddArgument(slotNumberArg);

            removeCommand.SetHandler(async (sn) =>
            {
                var slotsFileContent = await File.ReadAllTextAsync(SlotsFile);

                var slots = JsonConvert.DeserializeObject<string[]>(slotsFileContent)
                            ?? throw new ArgumentNullException("Unable to read slots file!", null as Exception);

                var slotCount = int.Parse(await File.ReadAllTextAsync(SlotCountFile));

                slots[sn - 1] = $"{slots[sn-1]}(Removed not scanned)";

                await File.WriteAllTextAsync(SlotsFile, JsonConvert.SerializeObject(slots));

                PrintBottles(slotCount, slots);

            }, slotNumberArg);

            bottleRootCommand.AddCommand(removeCommand);

            rootCommand.AddCommand(bottleRootCommand);

            return rootCommand;
        }

        private static void PrintBottles(int slotCount, string[] slots)
        {
            Console.WriteLine("Slot\tBottle");

            for (var slotNumber = 0; slotNumber < slotCount; slotNumber++)
            {
                var bottleText = string.IsNullOrEmpty(slots[slotNumber]) ? "Unoccupied" : slots[slotNumber];
                Console.WriteLine($"{slotNumber + 1}\t{bottleText}");
            }
        }
    }
}
