using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WineRackMessageProcessor.Models;
using WineRackMessageProcessor.Services;

namespace WineRackMessageProcessor
{
    public class Function1
    {
        private readonly ITwinRepository _twinRepository;
        private readonly ILogger<Function1> logger;

        public Function1(ITwinRepository twinRepository, ILogger<Function1> logger)
        {
            _twinRepository = twinRepository;
            this.logger = logger;
        }

        [FunctionName("Function1")]
        public async Task Run([EventHubTrigger(eventHubName: "%EventHubName%", Connection = "IoTHubConnectionString", ConsumerGroup = "$Default")] EventData[] events)
        {
            var exceptions = new List<Exception>();

            foreach (var eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

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
                    else if (messageType == MessageTypes.BottleRemoved)
                    {
                        await ProcessBottleRemovedMessage(messageBody);
                    }
                    else if (messageType == MessageTypes.BottleScanned)
                    {
                        await ProcessBottleScannedMessage(messageBody);
                    }

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            
            switch (exceptions.Count)
            {
                case > 1:
                    throw new AggregateException(exceptions);
                case 1:
                    throw exceptions.Single();
            }
        }

        private async Task ProcessBottleAddedMessage(string messageBody)
        {
            var bottleAddedMessage = JsonConvert.DeserializeObject<BottleAddedMessage>(messageBody);
            var orgId = this._twinRepository.GetOrganizationTwinId(bottleAddedMessage.Organization);
            var slot = this._twinRepository.GetSlot(orgId, bottleAddedMessage.DeviceName, bottleAddedMessage.Slot);

            this.logger.LogInformation($"Processing BottleAdded message. Slot id: {slot.SlotTwinId}; Wine Rack id: {slot.WineRackTwinId}; Org id: {slot.OrganizationTwinId}; ");

            await this._twinRepository.UpdateTwin(slot.SlotTwinId, "/occupied", true);

            var bottleTwin = await this._twinRepository.CreateBottleTwin(bottleAddedMessage.UpcCode);

            var relationship = await this._twinRepository.CreateRelationship(Relationships.StoredIn, bottleTwin.Id, slot.SlotTwinId);

            this.logger.LogInformation($"Created relationship. Name: '{relationship.Name}';Source twin id: '{relationship.SourceId}';Target twin id: '{relationship.TargetId}'");
        }

        private async Task ProcessBottleRemovedMessage(string messageBody)
        {
            var bottleRemovedMessage = JsonConvert.DeserializeObject<BottleRemovedMessage>(messageBody);
            var orgId = this._twinRepository.GetOrganizationTwinId(bottleRemovedMessage.Organization);
            var slot = this._twinRepository.GetSlot(orgId, bottleRemovedMessage.DeviceName, bottleRemovedMessage.Slot);
            
            this.logger.LogInformation($"Processing BottleRemoved message. Slot id: {slot.SlotTwinId}; Wine Rack id: {slot.WineRackTwinId}; Org id: {slot.OrganizationTwinId}; ");

            await this._twinRepository.UpdateTwin(slot.SlotTwinId, "/occupied", false);
        }

        private async Task ProcessBottleScannedMessage(string messageBody)
        {
            var bottleScannedMessage = JsonConvert.DeserializeObject<BottleScannedMessage>(messageBody);
            var orgId = this._twinRepository.GetOrganizationTwinId(bottleScannedMessage.Organization);
            var slot = this._twinRepository.GetSlot(orgId, bottleScannedMessage.DeviceName, bottleScannedMessage.Slot, true);

            this.logger.LogInformation($"Processing BottleScanned message. Slot id: {slot.SlotTwinId}; Wine Rack id: {slot.WineRackTwinId}; Org id: {slot.OrganizationTwinId}; ");

            await this._twinRepository.UpdateTwin(slot.SlotTwinId, "/occupied", false);

            var relationship = await this._twinRepository.GetRelationship(slot.BottleTwinId);

            await this._twinRepository.DeleteRelationship(slot.BottleTwinId, relationship.Id);
            await this._twinRepository.DeleteTwin(slot.BottleTwinId);
        }

        private async Task ProcessOnboardTwinMessage(string messageBody)
        {
            var onboardTwinMessage = JsonConvert.DeserializeObject<OnboardTwinMessage>(messageBody);

            var orgTwin = await this._twinRepository.CreateOrganizationTwin(onboardTwinMessage.Organization);

            var wineRackTwin = await this._twinRepository.CreateWineRackTwin(
                onboardTwinMessage.WineRackSerialNumber,
                onboardTwinMessage.DeviceName,
                onboardTwinMessage.SlotCount);

            var scannerTwin = await this._twinRepository.CreateScannerTwin(
                onboardTwinMessage.ScannerSerialNumber,
                "Scanner");

            await this._twinRepository.CreateRelationship(Relationships.OwnedBy, wineRackTwin.Id, orgTwin.Id);
            await this._twinRepository.CreateRelationship(Relationships.AttachedTo, scannerTwin.Id, wineRackTwin.Id);

            for (var i = 0; i < onboardTwinMessage.SlotCount; i++)
            {
                var slotNumber = i + 1;

                var slotTwin = await this._twinRepository.CreateSlotTwin(slotNumber);

                var relationship = await this._twinRepository.CreateRelationship(Relationships.PartOf, slotTwin.Id, wineRackTwin.Id);

                this.logger.LogInformation($"Created relationship. Name: '{relationship.Name}';Source twin id: '{relationship.SourceId}';Target twin id: '{relationship.TargetId}'");
            }
        }

        private string GetMessageType(string messageBody)
        {
            var regex = Regex.Match(messageBody, @"\""mtype\"":\""(?<messageType>\w+)\""",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var type = regex.Groups["messageType"].Value;

            return type.ToLower() switch
            {
                "onboardtwin" => MessageTypes.OnboardTwin,
                "bottleadded" => MessageTypes.BottleAdded,
                "bottleremoved" => MessageTypes.BottleRemoved,
                "bottlescanned" => MessageTypes.BottleScanned,
                _ => throw new InvalidOperationException("Unknown message type.")
            };
        }
        
    }
}
