using Newtonsoft.Json;

namespace WineRackMessageProcessor.Models;

public class BottleScannedMessage
{
    [JsonProperty("deviceName")]
    public string DeviceName { get; set; }

    [JsonProperty("org")]
    public string Organization { get; set; }

    [JsonProperty("slot")]
    public int Slot { get; set; }

    [JsonProperty("upc")]
    public string UpcCode { get; set; }
}