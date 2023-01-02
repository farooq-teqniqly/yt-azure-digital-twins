// <copyright file="BottleRemovedMessage.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace WineRackMessageProcessor.Models
{
    using Newtonsoft.Json;

    public class BottleRemovedMessage
    {
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("org")]
        public string Organization { get; set; }

        [JsonProperty("slot")]
        public int Slot { get; set; }
    }
}