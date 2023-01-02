// <copyright file="RootCommandExtensions.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack
{
    using System.CommandLine;
    using System.Dynamic;
    using Microsoft.Azure.Devices;
    using SmartWineRack.Db;
    using SmartWineRack.Services;

    public static class RootCommandExtensions
    {
        public static RootCommand SetupOnboardIoTHubCommand(this RootCommand rootCommand)
        {
            var onboardRootCommand = new Command(name: "onboard", description: "Onboard this device.");
            var onboardIoTHubCommand = new Command(name: "iothub", description: "Onboard this device to the Azure IoT Hub.");

            var connectionStringArg = new Argument<string>(
                name: "connection-string",
                description: "The Azure IoT Hub connection string.");

            var deviceNameArg = new Argument<string>(
                name: "name",
                description: "The name of the device.");

            onboardIoTHubCommand.AddArgument(connectionStringArg);
            onboardIoTHubCommand.AddArgument(deviceNameArg);

            onboardIoTHubCommand.SetHandler(
                async (cs, dn, deps) =>
            {
                await deps.Repository.AddConfig("DeviceName", dn);

                var deviceRepository = new DeviceRepsitory(RegistryManager.CreateFromConnectionString(cs));

                await deviceRepository.AddDevice(dn);
            },
                connectionStringArg,
                deviceNameArg,
                new DependenciesBinder());

            onboardRootCommand.AddCommand(onboardIoTHubCommand);

            var onboardTwinCommand = new Command(name: "twin", description: "Onboard this device to Azure Digital Twins.");

            var orgNameArg = new Argument<string>(
                name: "organization",
                description: "The name of the organization owning the device.");

            onboardTwinCommand.AddArgument(orgNameArg);

            var slotCountArg = new Argument<int>(
                name: "slotCount",
                description: "The number of slots this device supports.");

            onboardTwinCommand.AddArgument(slotCountArg);

            onboardTwinCommand.SetHandler(
                async (on, sc, deps) =>
            {
                var idFactory = deps.IdFactory;

                var configs = new List<KeyValuePair<string, string>>
                {
                    new ("Organization", on),
                    new ("SlotCount", sc.ToString()),
                    new ("WineRackSerialNumber", idFactory.CreateId()),
                    new ("ScannerSerialNumber", idFactory.CreateId()),
                };

                await deps.Repository.AddConfigs(configs);
                await deps.Repository.AddWineRack(sc);

                dynamic body = new ExpandoObject();
                body.org = on;
                body.slotcount = sc;

                await deps.MessageService.SendMessageAsync(body, MessageTypes.OnboardTwin);
            },
                orgNameArg,
                slotCountArg,
                new DependenciesBinder());

            onboardRootCommand.AddCommand(onboardTwinCommand);

            rootCommand.AddCommand(onboardRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupConfigCommand(this RootCommand rootCommand)
        {
            var configRootCommand = new Command(name: "config", description: "Manage the wine rack's configuration.");
            var addCommand = new Command(name: "add", description: "Add a wine rack configuration item.");

            var addSettingArg = new Argument<string>(
                name: "setting",
                description: "The name of the setting to add.");

            var addSettingValueArg = new Argument<string>(
                name: "value",
                description: "The setting's value.");

            addCommand.AddArgument(addSettingArg);
            addCommand.AddArgument(addSettingValueArg);

            addCommand.SetHandler(
                async (sn, sv, deps) =>
            {
                await deps.Repository.AddConfig(sn, sv);
            },
                addSettingArg,
                addSettingValueArg,
                new DependenciesBinder());

            configRootCommand.AddCommand(addCommand);

            var listCommand = new Command(name: "list", description: "List the wine rack's configuration.");

            listCommand.SetHandler(
                async (deps) =>
            {
                var configs = await deps.Repository.ListConfigs();

                foreach (var config in configs)
                {
                    Console.WriteLine($"{config.Name}={config.Value}");
                }
            }, new DependenciesBinder());

            configRootCommand.AddCommand(listCommand);

            var deleteCommand = new Command(name: "delete", description: "Delete a configuration setting.");

            var deleteSettingNameArg = new Argument<string>(
                name: "setting",
                description: "The name of the setting to delete.");

            deleteCommand.AddArgument(deleteSettingNameArg);

            deleteCommand.SetHandler(
                async (sn, deps) =>
            {
                await deps.Repository.RemoveConfig(sn);
            },
                deleteSettingNameArg,
                new DependenciesBinder());

            configRootCommand.AddCommand(deleteCommand);

            rootCommand.AddCommand(configRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupDecommissionCommand(this RootCommand rootCommand)
        {
            var decommissionRootCommand = new Command(name: "decom", description: "Decomission the wine rack.");

            var deviceNameArg = new Argument<string>(
                name: "name",
                description: "The name of the device.");

            decommissionRootCommand.AddArgument(deviceNameArg);

            decommissionRootCommand.SetHandler(
                async (deps) =>
            {
                await deps.Repository.DecomissionWineRack();
            }, new DependenciesBinder());

            rootCommand.AddCommand(decommissionRootCommand);

            return rootCommand;
        }

        public static RootCommand SetupBottleCommand(this RootCommand rootCommand)
        {
            var bottleRootCommand = new Command(name: "bottle", description: "Manage the wine rack's bottles.");
            var showCommand = new Command(name: "list", description: "List the wine rack's bottles.");

            showCommand.SetHandler(
                handle: async (deps) =>
            {
                await PrintBottles(repository: deps.Repository);
            }, symbol: new DependenciesBinder());

            bottleRootCommand.AddCommand(command: showCommand);

            var addCommand = new Command(name: "add", description: "Add a bottle to the wine rack.");

            var slotNumberArg = new Argument<int>(
                name: "slotNumber",
                description: "The slot number the bottle belongs to.");

            var upcCodeArg = new Argument<string>(
                name: "upcCode",
                description: "The bottle's UPC code.");

            addCommand.AddArgument(argument: slotNumberArg);
            addCommand.AddArgument(argument: upcCodeArg);

            addCommand.SetHandler(
                handle: async (sn, upc, deps) =>
            {
                await deps.Repository.AddBottle(upcCode: upc, slotNumber: sn);
                await deps.MessageService.SendBottleMessageAsync(slotNumber: sn, upcCode: upc, messageType: MessageTypes.BottleAdded);
                await PrintBottles(repository: deps.Repository);
            },
                slotNumberArg,
                upcCodeArg,
                new DependenciesBinder());

            bottleRootCommand.AddCommand(command: addCommand);

            var removeCommand = new Command(name: "remove", description: "Remove a bottle from the wine rack.");

            removeCommand.AddArgument(argument: slotNumberArg);

            removeCommand.SetHandler(
                handle: async (sn, deps) =>
            {
                var upcCode = (await deps.Repository.GetSlots()).Single(predicate: s => s.SlotNumber == sn).Bottle.UpcCode;
                await deps.Repository.RemoveBottle(slotNumber: sn);
                await deps.MessageService.SendBottleMessageAsync(slotNumber: sn, upcCode: upcCode, messageType: MessageTypes.BottleRemoved);
                await PrintBottles(repository: deps.Repository);
            },
                slotNumberArg,
                new DependenciesBinder());

            bottleRootCommand.AddCommand(command: removeCommand);

            var returnCommand = new Command(name: "return", description: "Return a bottle that has not been scanned to the wine rack.");

            returnCommand.AddArgument(argument: slotNumberArg);

            returnCommand.SetHandler(
                handle: async (sn, deps) =>
            {
                var upcCode = (await deps.Repository.GetSlots()).Single(predicate: s => s.SlotNumber == sn).Bottle.UpcCode;
                await deps.Repository.ReturnBottle(slotNumber: sn);
                await deps.MessageService.SendBottleMessageAsync(slotNumber: sn, upcCode: upcCode, messageType: MessageTypes.BottleReturned);
                await PrintBottles(repository: deps.Repository);
            },
                slotNumberArg,
                new DependenciesBinder());

            bottleRootCommand.AddCommand(command: returnCommand);

            var scanCommand = new Command(name: "scan", description: "Scan a bottle and permanently remove it from the wine rack.");

            scanCommand.AddArgument(argument: slotNumberArg);

            scanCommand.SetHandler(
                handle: async (sn, deps) =>
            {
                var upcCode = (await deps.Repository.GetSlots()).Single(predicate: s => s.SlotNumber == sn).Bottle.UpcCode;
                await deps.Repository.ScanBottle(slotNumber: sn);
                await deps.MessageService.SendBottleMessageAsync(slotNumber: sn, upcCode: upcCode, messageType: MessageTypes.BottleScanned);
                await PrintBottles(repository: deps.Repository);
            },
                slotNumberArg,
                new DependenciesBinder());

            bottleRootCommand.AddCommand(command: scanCommand);

            rootCommand.AddCommand(command: bottleRootCommand);

            return rootCommand;
        }

        private static async Task PrintBottles(IWineRackDbRepository repository)
        {
            var slots = await repository.GetSlots();

            Console.WriteLine("Slot\tBottle");

            foreach (var slot in slots)
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
    }
}
