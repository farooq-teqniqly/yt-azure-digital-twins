using System.CommandLine;
using System.CommandLine.Binding;
using System.Dynamic;
using System.Reflection;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RandomStringCreator;
using SmartWineRack.Db;
using Message = Microsoft.Azure.Devices.Client.Message;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace SmartWineRack
{
    public static class RootCommandExtensions
    {
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

            onboardIoTHubCommand.SetHandler(async (cs, dn, deps) =>
            {
                var db = deps.WineRackDbContext;
                var idCreator = deps.IdCreator;

                await db.Configs.AddAsync(new WineRackConfig {Id = idCreator.Get(10), Name = "DeviceName", Value = dn });
                await db.SaveChangesAsync();

                var registryManager = RegistryManager.CreateFromConnectionString(cs);
                var device = new Device(dn);

                await registryManager.AddDeviceAsync(device);

            }, connectionStringArg, deviceNameArg, new DependenicesBinder());

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

            onboardTwinCommand.SetHandler(async (on, sc, deps) =>
            {
                var db = deps.WineRackDbContext;
                var idCreator = deps.IdCreator;

                await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = "Organization", Value = on });
                await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = "SlotCount", Value = sc.ToString() });
                await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = "WineRackSerialNumber", Value = idCreator.Get(10) });
                await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = "ScannerSerialNumber", Value = idCreator.Get(10) });

                var slots = new List<WineRackSlot>(sc);

                for (var i = 0; i < sc; i++)
                {
                    slots.Add(new WineRackSlot {Id = idCreator.Get(10), SlotNumber = i + 1});
                }


                var wineRack = new WineRack
                {
                    Id = idCreator.Get(10),
                    Scanner = new Scanner { Id = idCreator.Get(10) },
                    WineRackSlots = slots
                };

                await db.WineRacks.AddAsync(wineRack);
                
                await db.SaveChangesAsync();

                dynamic expando = new ExpandoObject();
                expando.org = on;
                expando.slotcount = sc;
                
                await SendMessageAsync(expando, MessageTypes.OnboardTwin, db);

            }, orgNameArg, slotCountArg, new DependenicesBinder());

            onboardRootCommand.AddCommand(onboardTwinCommand);

            rootCommand.AddCommand(onboardRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupConfigIoTHubCommand(this RootCommand rootCommand)
        {
            var configRootCommand = new Command("config", "Manage the wine rack's comnfiguration.");
            var addCommand = new Command("add", "Add a wine rack configuration item.");


            var addSettingArg = new Argument<string>(
                name: "setting",
                description: "The name of the setting to add.");

            var addSettingValueArg = new Argument<string>(
                name: "value",
                description: "The setting's value.");

            addCommand.AddArgument(addSettingArg);
            addCommand.AddArgument(addSettingValueArg);

            addCommand.SetHandler(async (sn, sv, deps) =>
            {
                var db = deps.WineRackDbContext;
                var idCreator = deps.IdCreator;
                
                await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = sn, Value = sv });
                await db.SaveChangesAsync();
                
            }, addSettingArg, addSettingValueArg, new DependenicesBinder());

            configRootCommand.AddCommand(addCommand);

            var showCommand = new Command("show", "Show the wine rack's configuration.");

            showCommand.SetHandler(async (deps) =>
            {
                var db = deps.WineRackDbContext;
                var configs = await db.Configs.ToListAsync();

                foreach (var config in configs)
                {
                    Console.WriteLine($"{config.Name}={config.Value}");
                }

            }, new DependenicesBinder());

            configRootCommand.AddCommand(showCommand);

            var deleteCommand = new Command("delete", "Delete a configuration setting.");

            var deleteSettingNameArg = new Argument<string>(
                name: "setting",
                description: "The name of the setting to delete.");

            deleteCommand.AddArgument(deleteSettingNameArg);

            deleteCommand.SetHandler(async (sn, deps) =>
            {
                var db = deps.WineRackDbContext;
                var config = await db.Configs.SingleAsync(c => c.Name == sn);
                db.Configs.Remove(config);
                await db.SaveChangesAsync();

            }, deleteSettingNameArg, new DependenicesBinder());

            configRootCommand.AddCommand(deleteCommand);

            rootCommand.AddCommand(configRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupBottleCommand(this RootCommand rootCommand)
        {
            var bottleRootCommand = new Command("bottle", "Manage the wine rack's bottles.");
            var showCommand = new Command("list", "List the wine rack's bottles.");

            showCommand.SetHandler(async (deps) =>
            {
                await PrintBottles(deps.WineRackDbContext);
            }, new DependenicesBinder());

            bottleRootCommand.AddCommand(showCommand);

            var addCommand = new Command("add", "Add a bottle to the wine rack.");

            var slotNumberArg = new Argument<int>(
                name: "slotNumber",
                description: "The slot number the bottle belongs to.");

            var upcCodeArg = new Argument<string>(
                name: "upcCode",
                description: "The bottle's UPC code.");

            addCommand.AddArgument(slotNumberArg);
            addCommand.AddArgument(upcCodeArg);

            addCommand.SetHandler(async (sn, upc, deps) =>
            {
                var db = deps.WineRackDbContext;
                var wineRack = await db.WineRacks.Include(r => r.WineRackSlots).SingleAsync();
                var slot = wineRack.WineRackSlots.Single(s => s.SlotNumber == sn);

                slot.Bottle = new Bottle { UpcCode = upc, BottleState = BottleState.InPlace};
                await db.SaveChangesAsync();

                //await SendBottleMessage(sn, upc, MessageTypes.BottleAdded, db);
                await PrintBottles(db);

            }, slotNumberArg, upcCodeArg, new DependenicesBinder());

            bottleRootCommand.AddCommand(addCommand);

            var removeCommand = new Command("remove", "Remove a bottle from the wine rack.");
            
            removeCommand.AddArgument(slotNumberArg);

            removeCommand.SetHandler(async (sn, deps) =>
            {
                var db = deps.WineRackDbContext;
                var wineRack = await db.WineRacks
                    .Include(r => r.WineRackSlots)
                    .ThenInclude(s => s.Bottle)
                    .SingleAsync();

                wineRack.WineRackSlots.Single(s => s.SlotNumber == sn).Bottle.BottleState = BottleState.RemoveNotScanned;
                await db.SaveChangesAsync();
                
                await PrintBottles(db);

            }, slotNumberArg, new DependenicesBinder());

            bottleRootCommand.AddCommand(removeCommand);


            var returnCommand = new Command("return", "Return a bottle that has not been scanned to the wine rack.");

            returnCommand.AddArgument(slotNumberArg);

            returnCommand.SetHandler(async (sn, deps) =>
            {
                var db = deps.WineRackDbContext;
                var wineRack = await db.WineRacks
                    .Include(r => r.WineRackSlots)
                    .ThenInclude(s => s.Bottle)
                    .SingleAsync();

                wineRack.WineRackSlots.Single(s => s.SlotNumber == sn).Bottle.BottleState = BottleState.InPlace;
                await db.SaveChangesAsync();

                await PrintBottles(db);

            }, slotNumberArg, new DependenicesBinder());

            bottleRootCommand.AddCommand(returnCommand);
            
            var scanCommand = new Command("scan", "Scan a bottle.");

            scanCommand.AddArgument(slotNumberArg);

            scanCommand.SetHandler(async (sn, deps) =>
            {
                //var db = deps.WineRackDbContext;
                //var slots = await ReadSlotsAsync();
                //var slotCount = await ReadSlotCountAsync();
                //var upc = slots[sn - 1];

                //slots[sn - 1] = null!;

                //await SaveSlotsAsync(slots);
                //await SendBottleMessage(sn, upc, MessageTypes.BottleScanned, db);
                //PrintBottles(slotCount, slots);

            }, slotNumberArg, new DependenicesBinder());

            bottleRootCommand.AddCommand(scanCommand);

            rootCommand.AddCommand(bottleRootCommand);

            return rootCommand;
        }

        private static async Task<int> ReadSlotCountAsync()
        {
            var slotCount = int.Parse(await ReadFileAsync(SlotCountFile));
            return slotCount;
        }

        private static async Task SaveSlotsAsync(string[] slots)
        {
            await WriteFileAsync(SlotsFile, JsonConvert.SerializeObject(slots));
        }

        private static async Task<string[]> ReadSlotsAsync()
        {
            var slotsFileContent = await ReadFileAsync(SlotsFile);

            var slots = JsonConvert.DeserializeObject<string[]>(slotsFileContent)
                        ?? throw new ArgumentNullException("Unable to read slots file!", null as Exception);
            return slots;
        }

        private static async Task PrintBottles(WineRackDbContext db)
        {
            var wineRack = await (db.WineRacks
                .Include(r => r.WineRackSlots)
                .ThenInclude(s => s.Bottle))
                .SingleAsync();

            Console.WriteLine("Slot\tBottle");

            foreach (var slot in wineRack.WineRackSlots.OrderBy(s => s.SlotNumber))
            {
                var bottleText = "Unoccupied";

                if (slot.Bottle != null)
                {
                    bottleText = slot.Bottle is { BottleState: BottleState.RemoveNotScanned } ? 
                        $"{slot.Bottle.UpcCode} (Removed not scanned)" : 
                        $"{slot.Bottle.UpcCode}";
                }

                Console.WriteLine($"{slot.SlotNumber}\t{bottleText}");
            }
        }

        private static async Task<string> ReadFileAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        private static async Task WriteFileAsync(string path, string text)
        {
            await File.WriteAllTextAsync(path, text);
        }

        private static async Task SendMessageAsync(dynamic body, MessageTypes messageType, WineRackDbContext db)
        {
            body.mtype = messageType.ToString().ToLower();
            body.deviceName = (await db.Configs.SingleAsync(c => c.Name == "DeviceName")).Value;
            body.wrsno = (await db.Configs.SingleAsync(c => c.Name == "WineRackSerialNumber")).Value;
            body.scsno = (await db.Configs.SingleAsync(c => c.Name == "ScannerSerialNumber")).Value;

            var payload = JsonConvert.SerializeObject(body);

            var message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };

            var connectionString = (await db.Configs.SingleAsync(c => c.Name == "IotHubConnectionString")).Value;
            var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            await deviceClient.SendEventAsync(message);
        }

        private static async Task SendBottleMessage(int slotNumber, string upcCode, MessageTypes messageType, WineRackDbContext db)
        {
            dynamic expando = new ExpandoObject();
            expando.slot = slotNumber;
            expando.upc = upcCode;

            await SendMessageAsync(expando, messageType, db);
        }
    }
    
    public class DependenicesBinder : BinderBase<Dependencies>
    {
        protected override Dependencies GetBoundValue(BindingContext bindingContext)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WineRackDbContext>();
            optionsBuilder.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "swr.db")}");

            var dbContext = new WineRackDbContext(optionsBuilder.Options);
            var idCreator = new StringCreator("abcdefghijkmnpqrstuvwxyz23456789");
            
            dbContext.Database.Migrate();

            return new Dependencies
            {
                IdCreator = idCreator,
                WineRackDbContext = dbContext
            };
        }
    }

    public class Dependencies
    {
        public WineRackDbContext WineRackDbContext { get; set; }
        public StringCreator IdCreator { get; set; }
    }
    
}
