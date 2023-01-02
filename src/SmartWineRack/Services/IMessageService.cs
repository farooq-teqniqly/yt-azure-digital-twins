using System.Dynamic;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SmartWineRack.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(dynamic body, MessageTypes messageType);
        Task SendBottleMessageAsync(int slotNumber, string upcCode, MessageTypes messageType);
    }

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
            body.deviceName = await repository.GetConfig("DeviceName");
            body.wrsno = await repository.GetConfig("WineRackSerialNumber");
            body.scsno = await repository.GetConfig("ScannerSerialNumber");

            if (!((IDictionary<string, object>)body).ContainsKey("org"))
            {
                body.org = await repository.GetConfig("Organization");
            }

            var payload = JsonConvert.SerializeObject(body);

            var message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };

            var connectionString = await repository.GetConfig("IotHubConnectionString");
            var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            await deviceClient.SendEventAsync(message);
        }

        public async Task SendBottleMessageAsync(int slotNumber, string upcCode, MessageTypes messageType)
        {
            dynamic body = new ExpandoObject();
            body.slot = slotNumber;
            body.upc = upcCode;

            await SendMessageAsync(body, messageType);
        }
    }
}
