using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.DigitalTwins.Core;
using Microsoft.Extensions.Logging;
using WineRackMessageProcessor.Models;

namespace WineRackMessageProcessor.Services
{
    public interface ITwinRepository
    {
        string GetTwinId(string query);

        string GetOrganizationTwinId(string name);
        string GetSlotTwinId(string organizationId, string deviceName, int slotNumber);
        TwinRepository.Slot GetSlotTwin(string organizationId, string deviceName, int slotNumber);

        Task<BasicDigitalTwin> CreateOrganizationTwin(string name);
        Task UpdateTwin<T>(string id, string path, T value);
        Task<BasicDigitalTwin> CreateWineRackTwin(string serialNumber, string deviceName, int slotCount);
        Task<BasicDigitalTwin> CreateScannerTwin(string serialNumber, string name);
        Task<BasicDigitalTwin> CreateSlotTwin(int slotNumber);

        Task<BasicRelationship> CreateRelationship(
            string name,
            string sourceTwinId,
            string targetTwinId,
            IDictionary<string, object> properties = null);

        Task<BasicDigitalTwin> CreateBottleTwin(string upcCode);
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

        public string GetOrganizationTwinId(string name)
        {
            return this.GetTwinId(
                $"SELECT T.$dtId FROM DIGITALTWINS T WHERE IS_OF_MODEL('dtmi:com:thewineshoppe:Organization;1') AND T.name = '{name}'");
        }

        public string GetSlotTwinId(string organizationId, string deviceName, int slotNumber)
        {
            return this.GetTwinId(
                $"SELECT slot.$dtId from DIGITALTWINS MATCH (slot)-[:partOf]->(winerack)-[:ownedBy]->(org) WHERE org.$dtId = '{organizationId}' and winerack.name = '{deviceName}' and slot.slotNumber = {slotNumber}");
        }

        public Slot GetSlotTwin(string organizationId, string deviceName, int slotNumber)
        {
            var query = $"SELECT slot.$dtId AS slotTwinId, " +
                        $"winerack.$dtId AS wineRackTwinId, " +
                        $"org.$dtId AS organizationTwinId " +
                        $"FROM DIGITALTWINS " +
                        $"MATCH (slot)-[:partOf]->(winerack)-[:ownedBy]->(org) " +
                        $"WHERE org.$dtId = '{organizationId}' " +
                        $"AND winerack.name = '{deviceName}' " +
                        $"AND slot.slotNumber = {slotNumber}";
            
            var queryResult = this._dtClient.QueryAsync<object>(query).SingleAsync().Result;
            var jsonElement = (System.Text.Json.JsonElement)queryResult;

            return new Slot
            {
                SlotTwinId = jsonElement.GetProperty("slotTwinId").GetString(),
                WineRackTwinId = jsonElement.GetProperty("wineRackTwinId").GetString(),
                OrganizationTwinId = jsonElement.GetProperty("organizationTwinId").GetString(),
            };
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

        public async Task<BasicRelationship> CreateRelationship(
            string name, 
            string sourceTwinId, 
            string targetTwinId,
            IDictionary<string, object> properties = null)
        {
            var id = this._twinIdService.CreateId();

            var relationship = new BasicRelationship
            {
                Id = id,
                SourceId = sourceTwinId,
                TargetId = targetTwinId,
                Name = name,
                Properties = properties
            };

            var response = await this._dtClient.CreateOrReplaceRelationshipAsync(
                relationship.SourceId,
                relationship.Id,
                relationship);

            return response.Value;
        }

        public async Task<BasicDigitalTwin> CreateBottleTwin(string upcCode)
        {
            return await this.CreateTwin(
                ModelIds.Bottle,
                new Dictionary<string, object>
                {
                    { "upcCode", upcCode }
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

        public class Slot
        {
            public string SlotTwinId { get; set; }
            public string WineRackTwinId { get; set; }
            public string OrganizationTwinId { get; set; }
        }

    }
}
