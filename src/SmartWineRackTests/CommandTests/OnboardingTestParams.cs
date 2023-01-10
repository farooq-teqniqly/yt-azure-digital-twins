using Castle.Core;

namespace SmartWineRackTests.CommandTests;

public class OnboardingTestParams
{
    public OnboardingTestParams(
        int slotCount, 
        string ownerName, 
        string deviceName, 
        string iotProviderConnectionString)
    {
        SlotCount = slotCount;
        OwnerName = ownerName;
        DeviceName = deviceName;
        IotProviderConnectionString = iotProviderConnectionString;
    }

    public int SlotCount { get; }
    public string OwnerName { get;  }
    public string DeviceName { get;  }
    public string IotProviderConnectionString { get;  }
}