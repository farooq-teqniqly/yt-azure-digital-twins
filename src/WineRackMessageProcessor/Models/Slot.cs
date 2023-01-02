﻿// <copyright file="Slot.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace WineRackMessageProcessor.Models
{
    public class Slot
    {
        public string SlotTwinId { get; set; }

        public string BottleTwinId { get; set; }

        public string WineRackTwinId { get; set; }

        public string OrganizationTwinId { get; set; }
    }
}