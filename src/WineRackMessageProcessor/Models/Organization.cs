using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WineRackMessageProcessor.Models
{
    public class Organization
    {
        [JsonProperty("org")]
        public string Name { get; set; }
    }
}
