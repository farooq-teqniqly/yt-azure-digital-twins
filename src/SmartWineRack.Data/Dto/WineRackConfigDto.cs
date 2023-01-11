// <copyright file="WineRackConfigDto.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Data.Dto
{
    /// <summary>
    /// The wine rack configuration DTO.
    /// </summary>
    public class WineRackConfigDto
    {
        private string? ownerName;
        private string? deviceName;
        private string? iotProviderConnectionString;

        /// <summary>
        /// Gets or sets the number of slots.
        /// </summary>
        public int SlotCount { get; set; }

        /// <summary>
        /// Gets or sets the owner name.
        /// </summary>
        public string OwnerName
        {
            get => this.ownerName ?? throw new ArgumentNullException(nameof(this.OwnerName));

            set => this.ownerName = value ?? throw new ArgumentNullException(nameof(this.OwnerName));
        }

        /// <summary>
        /// Gets or sets the wine rack name.
        /// </summary>
        public string DeviceName
        {
            get => this.deviceName ?? throw new ArgumentNullException(nameof(this.DeviceName));

            set => this.deviceName = value ?? throw new ArgumentNullException(nameof(this.DeviceName));
        }

        /// <summary>
        /// Gets or sets the connection string to the IoT provider.
        /// </summary>
        public string IotProviderConnectionString
        {
            get => this.iotProviderConnectionString ?? throw new ArgumentNullException(nameof(this.IotProviderConnectionString));

            set => this.iotProviderConnectionString = value ?? throw new ArgumentNullException(nameof(this.IotProviderConnectionString));
        }
    }
}