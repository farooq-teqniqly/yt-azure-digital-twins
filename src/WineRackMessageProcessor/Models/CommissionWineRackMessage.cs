// <copyright file="CommissionWineRackMessage.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace WineRackMessageProcessor.Models
{
    using Newtonsoft.Json;

    public class CommissionWineRackMessage
    {
        [JsonProperty("org")]
        public string OrganizationName { get; set; }

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("wrsno")]
        public string WineRackSerialNumber { get; set; }

        [JsonProperty("scsno")]
        public string ScannerSerialNumber { get; set; }

        [JsonProperty("slotCount")]
        public int SlotCount { get; set; }
    }
}
