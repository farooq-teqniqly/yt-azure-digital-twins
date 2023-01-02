// <copyright file="Bottle.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db
{
    public class Bottle
    {
        public string Id { get; set; }

        public string UpcCode { get; set; }

        public WineRackSlot WineRackSlot { get; set; }

        public BottleState BottleState { get; set; }
    }
}