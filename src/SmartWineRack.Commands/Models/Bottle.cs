// <copyright file="Bottle.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands.Models
{
    /// <summary>
    /// A bottle stored in a wine rack.
    /// </summary>
    public class Bottle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bottle"/> class.
        /// </summary>
        /// <param name="slot">The slot number the bottle is contained in.</param>
        /// <param name="upcCode">The bottle UPC code.</param>
        public Bottle(int slot, string upcCode)
        {
            this.Slot = slot;
            this.UpcCode = upcCode;
        }

        /// <summary>
        /// Gets the bottle UPC code.
        /// </summary>
        public string UpcCode { get; }

        /// <summary>
        /// Gets the slot number the bottle is contained in.
        /// </summary>
        public int Slot { get; }
    }
}
