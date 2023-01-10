// <copyright file="IDeviceRegistrationService.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Commands.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for device registration services.
    /// </summary>
    public interface IDeviceRegistrationService
    {
        /// <summary>
        /// Registers a device in the IoT provider.
        /// </summary>
        /// <param name="deviceName">The unique device name.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RegisterDevice(string deviceName);
    }
}
