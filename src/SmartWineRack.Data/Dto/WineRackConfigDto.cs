namespace SmartWineRack.Data.Dto;

public class WineRackConfigDto
{
    private string? ownerName;
    private string? deviceName;
    private string? iotProviderConnectionString;

    public int SlotCount { get; set; }
    public string OwnerName
    {
        get => ownerName ?? throw new ArgumentNullException(nameof(OwnerName));

        set => ownerName = value ?? throw new ArgumentNullException(nameof(OwnerName));
    }

    public string DeviceName
    {
        get => deviceName ?? throw new ArgumentNullException(nameof(DeviceName));

        set => deviceName = value ?? throw new ArgumentNullException(nameof(DeviceName));
    }

    public string IotProviderConnectionString
    {
        get => iotProviderConnectionString ?? throw new ArgumentNullException(nameof(IotProviderConnectionString));

        set => iotProviderConnectionString = value ?? throw new ArgumentNullException(nameof(IotProviderConnectionString));
    }
}