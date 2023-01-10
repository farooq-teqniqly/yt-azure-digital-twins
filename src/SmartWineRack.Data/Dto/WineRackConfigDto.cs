namespace SmartWineRack.Data.Dto;

public class WineRackConfigDto
{
    public int SlotCount { get; set; }
    public string OwnerName { get; set; }
    public string DeviceName { get; set; }
    public string IotProviderConnectionString { get; set; }
}