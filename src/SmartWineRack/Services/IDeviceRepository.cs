// <copyright file="IDeviceRepository.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    public interface IDeviceRepository
    {
        Task AddDevice(string name);
    }
}
