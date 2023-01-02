// <copyright file="DeviceRepsitory.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using Microsoft.Azure.Devices;

    public class DeviceRepsitory : IDeviceRepository
    {
        private readonly RegistryManager registryManager;

        public DeviceRepsitory(RegistryManager registryManager)
        {
            this.registryManager = registryManager;
        }

        public async Task AddDevice(string name)
        {
            await this.registryManager.AddDeviceAsync(new Device(name));
        }
    }
}