// <copyright file="IDeviceService.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for IoT device services.
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Registers a device in the IoT provider.
        /// </summary>
        /// <param name="deviceName">The unique device name.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RegisterDevice(string deviceName);
    }
}
