﻿// <copyright file="OnboardCommandTests.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRackTests.CommandTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using SmartWineRack.Commands.Models;
    using SmartWineRack.Commands.Onboard;
    using SmartWineRack.Commands.Services;
    using SmartWineRack.Data.Dto;
    using SmartWineRack.Data.Repositories;
    using Xunit;

    public class OnboardCommandTests
    {
        private readonly OnboardCommandTestParams defaultTestParams = new (
            4,
            "The Wine Place",
            "winerack001",
            "iot_hub_conn_str");

        [Fact]
        public async Task OnboardCommand_Returns_WineRack_Config()
        {
            await this.ExecuteOnboardingCommand(
                this.defaultTestParams,
                (config, _, _) =>
                {
                    config.SlotCount.Should().Be(this.defaultTestParams.SlotCount);
                    config.OwnerName.Should().Be(this.defaultTestParams.OwnerName);
                    config.DeviceName.Should().Be(this.defaultTestParams.DeviceName);
                    config.IoTProviderConnectionString.Should().Be(this.defaultTestParams.IotProviderConnectionString);
                });
        }

        [Fact]
        public async Task OnboardCommand_Registers_WineRack_In_IotProvider()
        {
            await this.ExecuteOnboardingCommand(
                this.defaultTestParams, (config, _, mockDeviceRegistrationService) =>
                {
                    mockDeviceRegistrationService.Verify(m => m.RegisterDevice(config.DeviceName), Times.Once);
                });
        }

        private async Task ExecuteOnboardingCommand(
            OnboardCommandTestParams testParams,
            Action<WineRackConfig, Mock<IRepository>, Mock<IDeviceRegistrationService>>? verifyAction = null)
        {
            var commandParams = new Dictionary<string, object>
            {
                { "slotCount", testParams.SlotCount },
                { "ownerName", testParams.OwnerName },
                { "deviceName", testParams.DeviceName },
                { "iotProviderConnectionString", testParams.IotProviderConnectionString },
            };

            var mockRepository = new Mock<IRepository>();

            mockRepository.Setup(m => m.GetConfig()).ReturnsAsync(() => new WineRackConfigDto
            {
                DeviceName = testParams.DeviceName,
                OwnerName = testParams.OwnerName,
                SlotCount = testParams.SlotCount,
                IotProviderConnectionString = testParams.IotProviderConnectionString,
            });

            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();

            var command = new OnboardCommand(mockRepository.Object, mockDeviceRegistrationService.Object);

            var config = await command.Execute(commandParams);

            verifyAction?.Invoke(config, mockRepository, mockDeviceRegistrationService);
        }
    }
}