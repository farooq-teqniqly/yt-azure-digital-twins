// <copyright file="ListBottleCommand.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Commands.Bottle
{
    using SmartWineRack.Commands.Models;
    using SmartWineRack.Data.Repositories;

    /// <summary>
    /// A command that handles listing the bottles stored in the wine rack.
    /// </summary>
    public class ListBottleCommand : Command<BottleSnapshot>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBottleCommand"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ListBottleCommand(IRepository repository)
            : base(repository)
        {
        }

        /// <inheritdoc />
        public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
        {
            var bottleDtos = await this.Repository.GetBottles();
            var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

            return new BottleSnapshot(bottles);
        }
    }
}