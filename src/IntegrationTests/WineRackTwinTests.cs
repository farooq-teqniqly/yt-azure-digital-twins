using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using WineRackMessageProcessor.Models;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace IntegrationTests
{
    public class WineRackTwinTests : TestBed<TestFixture>
    {
        private readonly DigitalTwinsClient dtClient;

        public WineRackTwinTests(ITestOutputHelper testOutputHelper, TestFixture fixture) : base(testOutputHelper, fixture)
        {
            this.dtClient = fixture.GetService<DigitalTwinsClient>(testOutputHelper);
        }

        public async Task CanCommissionWineRack()
        {
            var message = new CommissionWineRackMessage
            {
                OrganizationName = "Integration Test Org",
                DeviceName = "Integration Test Wine Rack",
                ScannerSerialNumber = "scanner123",
                SlotCount = 2,
                WineRackSerialNumber = "winerack123",
            };

            var wineRackTwin = messageProcessor.ProcessMessage(message);

            wineRackTwin.Id.Should().NotBeNull();
            wineRackTwin.Name.Should().Be(message.DeviceName);
            wineRackTwin.SlotCount.Should().Be(message.SlotCount);
            wineRackTwin.SerialNumber.Should().Be(message.WineRackSerialNumber);

            wineRackTwin.Scanner.Id.Should().NotBeNull();
            wineRackTwin.Scanner.SerialNumber.Should().Be(message.ScannerSerialNumber);

            wineRackTwin.Organization.Id.Should().NotBeNull();
            wineRackTwin.Organization.Name.Should().Be(message.OrganizationName);

        }

    }
}
