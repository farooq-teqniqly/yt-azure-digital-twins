using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WineRackMessageProcessor.Models
{
    public class OnboardTwinMessage
    {
        [JsonProperty("org")]
        public string Name { get; set; }

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
