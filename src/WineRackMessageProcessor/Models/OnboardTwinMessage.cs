using Newtonsoft.Json;

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

    public class BottleAddedMessage
    {
        [JsonProperty("deviceName")] 
        public string DeviceName { get; set; }

        [JsonProperty("org")]
        public string Organization { get; set; }

        [JsonProperty("slot")]
        public int Slot { get; set; }

        [JsonProperty("upc")]
        public string UpcCode { get; set; }

        [JsonProperty("wrsno")]
        public string WineRackSerialNumber { get; set; }
    }
}
