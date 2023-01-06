// <copyright file="TwinRepository.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

using System;

namespace WineRackMessageProcessor.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure;
    using Azure.DigitalTwins.Core;
    using WineRackMessageProcessor.Models;

    public class TwinRepository : ITwinRepository
    {
        private readonly DigitalTwinsClient dtClient;
        private readonly ITwinIdService twinIdService;

        public TwinRepository(DigitalTwinsClient dtClient, ITwinIdService twinIdService)
        {
            this.dtClient = dtClient;
            this.twinIdService = twinIdService;
        }

        public string GetOrganizationTwinId(string name)
        {
            return this.GetTwinId(ModelIds.Organization, name);
        }

        public Slot GetSlot(string organizationId, string deviceName, int slotNumber, bool containsBottleTwin = false)
        {
            var query = $"SELECT slot.$dtId AS slotTwinId, " +
                        $"winerack.$dtId AS wineRackTwinId, " +
                        $"org.$dtId AS organizationTwinId " +
                        $"FROM DIGITALTWINS " +
                        $"MATCH (slot)-[:partOf]->(winerack)-[:ownedBy]->(org) " +
                        $"WHERE org.$dtId = '{organizationId}' " +
                        $"AND winerack.name = '{deviceName}' " +
                        $"AND slot.slotNumber = {slotNumber}";

            if (containsBottleTwin)
            {
                query = $"SELECT slot.$dtId AS slotTwinId, " +
                        $"bottle.$dtId AS bottleTwinId, " +
                        $"winerack.$dtId AS wineRackTwinId, " +
                        $"org.$dtId AS organizationTwinId " +
                        $"FROM DIGITALTWINS " +
                        $"MATCH (bottle)-[:storedIn]->(slot)-[:partOf]->(winerack)-[:ownedBy]->(org) " +
                        $"WHERE org.$dtId = '{organizationId}' " +
                        $"AND winerack.name = '{deviceName}' " +
                        $"AND slot.slotNumber = {slotNumber}";
            }
            
            var slot = this.Query<Slot>(query, pageable =>
            {
                var twin = pageable.SingleAsync().Result;

                return new Slot
                {
                    SlotTwinId = twin.Contents["slotTwinId"].ToString(),
                    BottleTwinId = containsBottleTwin ? twin.Contents["bottleTwinId"].ToString() : null,
                    WineRackTwinId = twin.Contents["wineRackTwinId"].ToString(),
                    OrganizationTwinId = twin.Contents["organizationTwinId"].ToString(),
                };
            });

            return slot;
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

            await this.dtClient.UpdateDigitalTwinAsync(id, patchDoc);
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
                    { "name", name },
                });
        }

        public async Task<BasicDigitalTwin> CreateSlotTwin(int slotNumber)
        {
            return await this.CreateTwin(
                ModelIds.WineRackSlot,
                new Dictionary<string, object>
                {
                    { "slotNumber", slotNumber },
                    { "name", $"Slot-{slotNumber}" },
                    { "occupied", false },
                });
        }

        public async Task<BasicRelationship> CreateRelationship(
            string name,
            string sourceTwinId,
            string targetTwinId,
            IDictionary<string, object> properties = null)
        {
            var id = this.twinIdService.CreateId();

            var relationship = new BasicRelationship
            {
                Id = id,
                SourceId = sourceTwinId,
                TargetId = targetTwinId,
                Name = name,
                Properties = properties,
            };

            var response = await this.dtClient.CreateOrReplaceRelationshipAsync(
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
                    { "upcCode", upcCode },
                });
        }

        public async Task DeleteTwin(string twinId)
        {
            await this.dtClient.DeleteDigitalTwinAsync(twinId);
        }

        public async Task<BasicRelationship> GetRelationship(string twinId)
        {
            return await this.dtClient.GetRelationshipsAsync<BasicRelationship>(twinId).SingleAsync();
        }

        public async Task DeleteRelationship(string twinId, string relationshipId)
        {
            await this.dtClient.DeleteRelationshipAsync(twinId, relationshipId);
        }

        private string GetTwinId(string query)
        {
            var id = this.Query<string>(query, pageable =>
            {
                var twin = pageable.SingleAsync().Result;
                return twin.Id;
            });

            return id;
        }

        private string GetTwinId(string model, string twinName)
        {
            return this.GetTwinId(
                $"SELECT T.$dtId FROM DIGITALTWINS T WHERE IS_OF_MODEL('{model}') AND T.name = '{twinName}'");
        }

        private async Task<BasicDigitalTwin> CreateTwin(string modelId, IDictionary<string, object> contents)
        {
            var id = this.twinIdService.CreateId();

            var twin = new BasicDigitalTwin
            {
                Id = id,
                Metadata = { ModelId = modelId },
                Contents = contents,
            };

            var response = await this.dtClient.CreateOrReplaceDigitalTwinAsync(id, twin);

            return response.Value;
        }

        private T Query<T>(string query, Func<AsyncPageable<BasicDigitalTwin>, T> mapper)
        {
            var queryResult = this.dtClient.QueryAsync<BasicDigitalTwin>(query);
            return mapper(queryResult);
        }
    }
}