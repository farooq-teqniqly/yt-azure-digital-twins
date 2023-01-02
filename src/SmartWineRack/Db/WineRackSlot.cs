// <copyright file="WineRackSlot.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db
{
    public class WineRackSlot
    {
        public string Id { get; set; }

        public int SlotNumber { get; set; }

        public Bottle Bottle { get; set; }
    }
}