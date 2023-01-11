// <copyright file="OnboardCommandTestParams.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRackTests.CommandTests
{
    internal class OnboardCommandTestParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardCommandTestParams"/> class.
        /// </summary>
        /// <param name="slotCount">The slot count.</param>
        /// <param name="ownerName">The owner name.</param>
        /// <param name="deviceName">The wine rack name.</param>
        /// <param name="iotProviderConnectionString">The IoT provider connection string.</param>
        internal OnboardCommandTestParams(
            int slotCount,
            string ownerName,
            string deviceName,
            string iotProviderConnectionString)
        {
            this.SlotCount = slotCount;
            this.OwnerName = ownerName;
            this.DeviceName = deviceName;
            this.IotProviderConnectionString = iotProviderConnectionString;
        }

        /// <summary>
        /// Gets the slot count.
        /// </summary>
        internal int SlotCount { get; }

        /// <summary>
        /// Gets the owner name.
        /// </summary>
        internal string OwnerName { get;  }

        /// <summary>
        /// Gets the wine rack name.
        /// </summary>
        internal string DeviceName { get;  }

        /// <summary>
        /// Gets the IoT provider connection string.
        /// </summary>
        internal string IotProviderConnectionString { get;  }
    }
}