using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWineRack.Commands.Models;
using SmartWineRack.Commands.Services;
using SmartWineRack.Data.Dto;
using SmartWineRack.Data.Repositories;

namespace SmartWineRack.Commands.Onboard
{
    public class OnboardCommand : Command<WineRackConfig>
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;

        public OnboardCommand(IRepository repository, IDeviceRegistrationService deviceRegistrationService) : base(repository)
        {
            _deviceRegistrationService = deviceRegistrationService;
        }

        public override async Task<WineRackConfig> Execute(IDictionary<string, object>? parameters = null)
        {
            var deviceName = (string)parameters["deviceName"];

            await this.Repository.SaveConfig(
                new WineRackConfigDto
                {
                    DeviceName = deviceName,
                    OwnerName = (string)parameters["ownerName"],
                    SlotCount = (int)parameters["slotCount"],
                    IotProviderConnectionString = (string)parameters["iotProviderConnectionString"]
                });

            await this._deviceRegistrationService.RegisterDevice(deviceName);

            var configDto = await Repository.GetConfig();

            return new WineRackConfig(configDto.SlotCount, configDto.OwnerName, configDto.DeviceName, configDto.IotProviderConnectionString);
        }
    }
}
