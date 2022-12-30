using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WineRackMessageProcessor.Models;

namespace WineRackMessageProcessor
{
    public class Function1
    {
        private readonly DigitalTwinsClient dtClient;
        private readonly ITwinIdService twinIdService;
        private readonly ILogger<Function1> logger;

        public Function1(DigitalTwinsClient dtClient,ITwinIdService twinIdService, ILogger<Function1> logger)
        {
            this.dtClient = dtClient;
            this.twinIdService = twinIdService;
            this.logger = logger;
        }

        [FunctionName("Function1")]
        public async Task Run([EventHubTrigger(eventHubName: "%EventHubName%", Connection = "IoTHubConnectionString", ConsumerGroup = "$Default")] EventData[] events)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    this.logger.LogInformation($"C# Event Hub trigger function received a message: {messageBody}");

                    var messageType = GetMessageType(messageBody);

                    if (messageType == MessageTypes.OnboardTwin)
                    {
                       await ProcessOnboardTwinMessage(messageBody);
                    }
                    else if (messageType == MessageTypes.BottleAdded)
                    {
                        await ProcessBottleAddedMessage(messageBody);
                    }
                    // Replace these two lines with your processing logic.
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        private async Task ProcessBottleAddedMessage(string messageBody)
        {
            var bottleAddedMessage = JsonConvert.DeserializeObject<BottleAddedMessage>(messageBody);

            var orgQueryResult = this.dtClient
                .QueryAsync<object>(
                    $"SELECT T.$dtId FROM DIGITALTWINS T WHERE IS_OF_MODEL('dtmi:com:thewineshoppe:Organization;1') AND T.name = '{bottleAddedMessage.Organization}'")
                .SingleAsync()
                .Result;

            var orgId = ((System.Text.Json.JsonElement)orgQueryResult).GetProperty("$dtId").GetString();
            this.logger.LogInformation($"Processing BottleAdded message. Org id: {orgId}");

            var slotQueryResult = this.dtClient
                .QueryAsync<BasicDigitalTwin>(
                    $"SELECT slot from DIGITALTWINS MATCH (slot)-[:partOf]->(winerack)-[:ownedBy]->(org) WHERE org.$dtId = '{orgId}' and winerack.name = '{bottleAddedMessage.DeviceName}' and slot.slotNumber = {bottleAddedMessage.Slot}")
                .SingleAsync()
                .Result;

        }

        private async Task ProcessOnboardTwinMessage(string messageBody)
        {
            var onboardTwinMessage = JsonConvert.DeserializeObject<OnboardTwinMessage>(messageBody);
            
            var orgTwin = await this.CreateTwin(
                ModelIds.Organization,
                new Dictionary<string, object> { { "name", onboardTwinMessage.Organization } });

            var wineRackTwin = await this.CreateTwin(
                ModelIds.WineRack,
                new Dictionary<string, object>
                {
                    { "serialNo", onboardTwinMessage.WineRackSerialNumber },
                    { "name", onboardTwinMessage.DeviceName },
                    { "slotCount", onboardTwinMessage.SlotCount },
                });

            var scannerTwin = await this.CreateTwin(
                ModelIds.Scanner,
                new Dictionary<string, object>
                {
                    { "serialNo", onboardTwinMessage.ScannerSerialNumber },
                    { "name", "Scanner" }
                });

            var ownedByRelationship = await this.CreateRelationship(Relationships.OwnedBy, wineRackTwin, orgTwin);
            var attachedToRelationship = await this.CreateRelationship(Relationships.AttachedTo, scannerTwin, wineRackTwin);

            for (var i = 0; i < onboardTwinMessage.SlotCount; i++)
            {
                var slotNumber = i + 1;

                var slotTwin = await this.CreateTwin(
                    ModelIds.WineRackSlot,
                    new Dictionary<string, object>
                    {
                        { "slotNumber", slotNumber },
                        {"name", $"Slot-{slotNumber}"},
                        { "occupied", false }
                    });

                var partOfRelationship = await this.CreateRelationship(Relationships.PartOf, slotTwin, wineRackTwin);
            }
        }

        private string GetMessageType(string messageBody)
        {
            var regex = Regex.Match(messageBody, @"\""mtype\"":\""(?<messageType>\w+)\""",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var type = regex.Groups["messageType"].Value;

            if (type.ToLower() == "onboardtwin")
            {
                return MessageTypes.OnboardTwin;
            }

            if (type.ToLower() == "bottleadded")
            {
                return MessageTypes.BottleAdded;
            }

            throw new InvalidOperationException("Unknown message type.");
        }

        private async Task<BasicDigitalTwin> CreateTwin(string modelId, IDictionary<string, object> contents)
        {
            var id = this.twinIdService.CreateId();

            var twin = new BasicDigitalTwin
            {
                Id = id,
                Metadata = { ModelId = modelId },
                Contents = contents
            };

            var response = await this.dtClient.CreateOrReplaceDigitalTwinAsync(id, twin);
            this.logger.LogInformation($"Created twin. Model id: '{modelId}';Twin id: '{response.Value.Id}'");

            return response.Value;
        }

        private async Task<BasicRelationship> CreateRelationship(
            string name,
            BasicDigitalTwin sourceTwin, 
            BasicDigitalTwin targetTwin,
            IDictionary<string, object> properties = null)
        {
            var id = this.twinIdService.CreateId();

            var relationship = new BasicRelationship
            {
                Id = id,
                SourceId = sourceTwin.Id,
                TargetId = targetTwin.Id,
                Name = name,
                Properties = properties
            };

            var response = await this.dtClient.CreateOrReplaceRelationshipAsync(
                relationship.SourceId, 
                relationship.Id,
                    relationship);

            this.logger.LogInformation($"Created relationship. Name: '{response.Value.Name}';Source twin id: '{response.Value.SourceId}';Target twin id: '{response.Value.TargetId}'");

            return response.Value;
        }
    }
}
