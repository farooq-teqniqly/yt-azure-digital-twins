// <copyright file="WineRackConfig.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands.Models
{
    /// <summary>
    /// The wine rack configuration.
    /// </summary>
    public class WineRackConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WineRackConfig"/> class.
        /// </summary>
        /// <param name="slotCount">The number of slots.</param>
        /// <param name="ownerName">The owner name.</param>
        /// <param name="deviceName">The wine rack name.</param>
        /// <param name="ioTProviderConnectionString">The connection string to the IoT provider.</param>
        public WineRackConfig(
            int slotCount,
            string ownerName,
            string deviceName,
            string ioTProviderConnectionString)
        {
            this.SlotCount = slotCount;
            this.OwnerName = ownerName;
            this.DeviceName = deviceName;
            this.IoTProviderConnectionString = ioTProviderConnectionString;
        }

        /// <summary>
        /// Gets the number of slots.
        /// </summary>
        public int SlotCount { get; }

        /// <summary>
        /// Gets the owner name.
        /// </summary>
        public string OwnerName { get; }

        /// <summary>
        /// Gets the wine rack name.
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        /// Gets the connection string to the IoT provider.
        /// </summary>
        public string IoTProviderConnectionString { get; }
    }
}
