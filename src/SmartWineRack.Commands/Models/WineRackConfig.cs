using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWineRack.Commands.Models
{
    public class WineRackConfig
    {
        public int SlotCount { get; }
        public string OwnerName { get; }
        public string DeviceName { get; }
        public string IoTProviderConnectionString { get; }

        public WineRackConfig(int slotCount, string ownerName, string deviceName, string ioTProviderConnectionString)
        {
            SlotCount = slotCount;
            OwnerName = ownerName;
            DeviceName = deviceName;
            IoTProviderConnectionString = ioTProviderConnectionString;
        }
    }
}
