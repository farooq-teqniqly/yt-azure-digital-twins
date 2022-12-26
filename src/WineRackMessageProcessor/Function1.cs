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

        private async Task ProcessOnboardTwinMessage(string messageBody)
        {
            var organization = JsonConvert.DeserializeObject<Organization>(messageBody);

            this.logger.LogInformation($"Creating twin for Organization '{organization.Name}'.");

            var twinId = this.twinIdService.CreateId();

            var twin = new BasicDigitalTwin
            {
                Id = twinId,
                Metadata = { ModelId = ModelIds.Organization },
                Contents =
                {
                    { "name", organization.Name }
                }
            };

            var response = await this.dtClient.CreateOrReplaceDigitalTwinAsync(twinId, twin);

            this.logger.LogInformation($"Created twin for Organization '{organization.Name}'. Twin id: {response.Value.Id}");
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

            throw new InvalidOperationException("Unknown message type.");
        }
    }
}
