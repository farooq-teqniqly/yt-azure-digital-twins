// <copyright file="SellBottleCommand.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands.Bottle
{
    using SmartWineRack.Commands.Models;
    using SmartWineRack.Data.Repositories;

    /// <summary>
    /// A command that handles selling a bottle.
    /// </summary>
    public class SellBottleCommand : Command<BottleSnapshot>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SellBottleCommand"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public SellBottleCommand(IRepository repository)
            : base(repository)
        {
        }

        /// <inheritdoc />
        public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
        {
            parameters.EnsureNotNull(nameof(parameters));
            parameters!.EnsureKey("slot");

            await this.Repository.RemoveBottle((int)parameters!["slot"]);

            var bottleDtos = await this.Repository.GetBottles();
            var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

            return new BottleSnapshot(bottles);
        }
    }
}