// <copyright file="WineRack.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db
{
    public class WineRack
    {
        public WineRack()
        {
            this.WineRackSlots = new HashSet<WineRackSlot>();
        }

        public string Id { get; set; }

        public ICollection<WineRackSlot> WineRackSlots { get; set; }

        public Scanner Scanner { get; set; }
    }
}