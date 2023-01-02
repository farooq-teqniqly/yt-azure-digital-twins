﻿using Newtonsoft.Json;

namespace WineRackMessageProcessor.Models
{
    public class OnboardTwinMessage
    {
        [JsonProperty("org")]
        public string Organization { get; set; }

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
