// <copyright file="AddBottleCommand.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands.Bottle
{
    using SmartWineRack.Commands.Models;
    using SmartWineRack.Data.Dto;
    using SmartWineRack.Data.Repositories;

    /// <summary>
    /// A command that handles adding a bottle to the wine rack.
    /// </summary>
    public class AddBottleCommand : Command<BottleSnapshot>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBottleCommand"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public AddBottleCommand(IRepository repository)
            : base(repository)
        {
        }

        /// <inheritdoc />
        public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
        {
            parameters.EnsureNotNull(nameof(parameters));
            parameters!.EnsureKey("slot");
            parameters!.EnsureKey("upcCode");

            await this.Repository.AddBottle(new BottleDto { Slot = (int)parameters!["slot"], UpcCode = (string)parameters["upcCode"] });

            var bottleDtos = await this.Repository.GetBottles();
            var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

            return new BottleSnapshot(bottles);
        }
    }
}
