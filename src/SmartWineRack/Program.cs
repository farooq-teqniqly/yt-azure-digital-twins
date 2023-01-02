// <copyright file="Program.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack
{
    using System.CommandLine;

    internal class Program
    {
        public static async Task<int> Main(string[] args)
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