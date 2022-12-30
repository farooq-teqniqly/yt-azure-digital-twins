using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SmartWineRack.Services
{
    public interface IDeviceRepository
    {
        Task AddDevice(string name);
    }

    public class DeviceRepsitory : IDeviceRepository
    {
        private readonly RegistryManager registryManager;

        public DeviceRepsitory(RegistryManager registryManager)
        {
            this.registryManager = registryManager;
        }

        public async Task AddDevice(string name)
        {
            await registryManager.AddDeviceAsync(new Device(name));
        }
    }
}
