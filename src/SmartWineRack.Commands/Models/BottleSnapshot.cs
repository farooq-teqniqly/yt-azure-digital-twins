// <copyright file="BottleSnapshot.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Commands.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A snapshot of the bottles stored in the wine rack.
    /// </summary>
    public class BottleSnapshot
    {
        private readonly IList<Bottle> bottles;

        /// <summary>
        /// Initializes a new instance of the <see cref="BottleSnapshot"/> class.
        /// </summary>
        /// <param name="bottles">The list of bottles contained in the snapshot.</param>
        public BottleSnapshot(IList<Bottle> bottles)
        {
            this.bottles = bottles;
        }

        /// <summary>
        /// Gets the bottles stored in the wine rack.
        /// </summary>
        public IEnumerable<Bottle> Bottles => new ReadOnlyCollection<Bottle>(this.bottles);
    }
}
