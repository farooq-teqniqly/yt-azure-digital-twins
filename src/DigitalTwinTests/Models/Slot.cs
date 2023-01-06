using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DigitalTwinTests.Models
{
    public class Slot
    {
        [JsonProperty("$dtId")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slotNumber")]
        public int SlotNumber { get; set; }
    }

    public class Organization
    {
        [JsonProperty("$dtId")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
