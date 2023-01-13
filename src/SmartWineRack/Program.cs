// <copyright file="Program.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace SmartWineRack
{
    using System.Threading.Tasks;

    public class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Smart Wine Rack Emulator");

            var configCommand = new Command("config", "Manage the wine rack's configuration");

            var addCommand = new Command("add", "Add a configuration.");
            var keyArg = new Argument<string>("key", "The configuration setting name.");
            var valArg = new Argument<string>("value", "The configuration setting value.");

            addCommand.AddArgument(keyArg);
            addCommand.AddArgument(valArg);
            
            addCommand.SetHandler(
                async (arg1, arg2, deps) =>
            {
                Console.WriteLine($"Added {arg1}:{arg2}");
                Console.WriteLine($"Dependency function returned {deps.DependencyFunc()}");
            }, keyArg,
                valArg,
                new DependenciesBinder());

            configCommand.AddCommand(addCommand);
            rootCommand.AddCommand(configCommand);

            return await configCommand.InvokeAsync(args);
        }
    }

    public class Dependencies
    {
        public Func<int> DependencyFunc { get; set; }
    }

    public class DependenciesBinder : BinderBase<Dependencies>
    {
        protected override Dependencies GetBoundValue(BindingContext bindingContext)
        {
            var func = new Func<int>(() => 7);

            return new Dependencies { DependencyFunc = func };
        }
    }
}
