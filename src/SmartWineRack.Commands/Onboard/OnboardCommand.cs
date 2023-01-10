// <copyright file="OnboardCommand.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Commands.Onboard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SmartWineRack.Commands.Models;
    using SmartWineRack.Commands.Services;
    using SmartWineRack.Data.Dto;
    using SmartWineRack.Data.Repositories;

    /// <summary>
    /// A command that handles onboarding a wine rack to the IoT provider.
    /// </summary>
    public class OnboardCommand : Command<WineRackConfig>
    {
        private readonly IDeviceRegistrationService deviceRegistrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardCommand"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="deviceRegistrationService">The device registration service.</param>
        public OnboardCommand(IRepository repository, IDeviceRegistrationService deviceRegistrationService)
            : base(repository)
        {
            this.deviceRegistrationService = deviceRegistrationService;
        }

        /// <inheritdoc />
        public override async Task<WineRackConfig> Execute(IDictionary<string, object>? parameters = null)
        {
            var deviceName = (string)parameters["deviceName"];

            await this.Repository.SaveConfig(
                new WineRackConfigDto
                {
                    DeviceName = deviceName,
                    OwnerName = (string)parameters["ownerName"],
                    SlotCount = (int)parameters["slotCount"],
                    IotProviderConnectionString = (string)parameters["iotProviderConnectionString"],
                });

            await this.deviceRegistrationService.RegisterDevice(deviceName);

            var configDto = await this.Repository.GetConfig();

            return new WineRackConfig(configDto.SlotCount, configDto.OwnerName, configDto.DeviceName, configDto.IotProviderConnectionString);
        }
    }
}
