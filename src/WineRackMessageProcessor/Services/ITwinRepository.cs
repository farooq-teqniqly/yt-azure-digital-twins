using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.DigitalTwins.Core;

namespace WineRackMessageProcessor.Services
{
    public interface ITwinRepository
    {
        string GetTwinId(string query);

        Task<BasicDigitalTwin> CreateOrganizationTwin(string name);
        Task UpdateTwin<T>(string id, string path, T value);
        Task<BasicDigitalTwin> CreateWineRackTwin(string serialNumber, string deviceName, int slotCount);
        Task<BasicDigitalTwin> CreateScannerTwin(string serialNumber, string name);
        Task<BasicDigitalTwin> CreateSlotTwin(int slotNumber)

    }

    public class TwinRepository : ITwinRepository
    {
        private readonly DigitalTwinsClient _dtClient;
        private readonly ITwinIdService _twinIdService;

        public TwinRepository(DigitalTwinsClient dtClient, ITwinIdService twinIdService)
        {
            _dtClient = dtClient;
            _twinIdService = twinIdService;
        }

        public string GetTwinId(string query)
        {
            var queryResult = this._dtClient.QueryAsync<object>(query).SingleAsync().Result;
            return ((System.Text.Json.JsonElement)queryResult).GetProperty("$dtId").GetString();
        }

        public async Task<BasicDigitalTwin> CreateOrganizationTwin(string name)
        {
            return await this.CreateTwin(
                ModelIds.Organization,
                new Dictionary<string, object> { { "name", name } });
        }

        public async Task UpdateTwin<T>(string id, string path, T value)
        {
            var patchDoc = new JsonPatchDocument();
            patchDoc.AppendReplace(path, value);

            await this._dtClient.UpdateDigitalTwinAsync(id, patchDoc);
        }

        public async Task<BasicDigitalTwin> CreateWineRackTwin(string serialNumber, string deviceName, int slotCount)
        {
            return await this.CreateTwin(
                ModelIds.WineRack,
                new Dictionary<string, object>
                {
                    { "serialNo", serialNumber },
                    { "name", deviceName },
                    { "slotCount", slotCount },
                });
        }

        public async Task<BasicDigitalTwin> CreateScannerTwin(string serialNumber, string name)
        {
            return await this.CreateTwin(
                ModelIds.Scanner,
                new Dictionary<string, object>
                {
                    { "serialNo", serialNumber },
                    { "name", name }
                });
        }

        public async Task<BasicDigitalTwin> CreateSlotTwin(int slotNumber)
        {
            return await this.CreateTwin(
                ModelIds.WineRackSlot,
                new Dictionary<string, object>
                {
                    { "slotNumber", slotNumber },
                    {"name", $"Slot-{slotNumber}"},
                    { "occupied", false }
                });
        }

        private async Task<BasicDigitalTwin> CreateTwin(string modelId, IDictionary<string, object> contents)
        {
            var id = this._twinIdService.CreateId();

            var twin = new BasicDigitalTwin
            {
                Id = id,
                Metadata = { ModelId = modelId },
                Contents = contents
            };

            var response = await this._dtClient.CreateOrReplaceDigitalTwinAsync(id, twin);
            
            return response.Value;
        }

    }
}
