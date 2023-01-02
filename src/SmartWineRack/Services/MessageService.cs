// <copyright file="MessageService.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using System.Dynamic;
    using System.Text;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;

    public class MessageService : IMessageService
    {
        private readonly IWineRackDbRepository repository;

        public MessageService(IWineRackDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task SendMessageAsync(dynamic body, MessageTypes messageType)
        {
            body.mtype = messageType.ToString().ToLower();
            body.deviceName = await this.repository.GetConfig("DeviceName");
            body.wrsno = await this.repository.GetConfig("WineRackSerialNumber");
            body.scsno = await this.repository.GetConfig("ScannerSerialNumber");

            if (!((IDictionary<string, object>)body).ContainsKey("org"))
            {
                body.org = await this.repository.GetConfig("Organization");
            }

            var payload = JsonConvert.SerializeObject(body);

            var message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json",
            };

            var connectionString = await this.repository.GetConfig("IotHubConnectionString");
            var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            await deviceClient.SendEventAsync(message);
        }

        public async Task SendBottleMessageAsync(int slotNumber, string upcCode, MessageTypes messageType)
        {
            dynamic body = new ExpandoObject();
            body.slot = slotNumber;
            body.upc = upcCode;

            await this.SendMessageAsync(body, messageType);
        }
    }
}