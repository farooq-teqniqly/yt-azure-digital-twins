using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure.DigitalTwins.Core;
using DigitalTwinTests.Models;
using FluentAssertions;
using Newtonsoft.Json;
using WineRackMessageProcessor.Services;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace DigitalTwinTests
{
    public class DigitalTwinQueryTests : TestBed<TestFixture>
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly ITwinRepository repository;
        private readonly DigitalTwinsClient dtClient;
        private readonly IMapper mapper;

        public DigitalTwinQueryTests(ITestOutputHelper testOutputHelper, TestFixture fixture) : base(testOutputHelper, fixture)
        {
            this.testOutputHelper = testOutputHelper;
            repository = _fixture.GetService<ITwinRepository>(this.testOutputHelper);
            dtClient = _fixture.GetService<DigitalTwinsClient>(this.testOutputHelper);
            mapper = _fixture.GetService<IMapper>(this.testOutputHelper);
        }

        [Fact]
        public void GetWineRackGraph()
        {
            var orgName = "My Org";
            var wineRackName = "testwinerack";
            var slotNumber = 3;
            var orgId = repository.GetOrganizationTwinId(orgName);

            var query = "SELECT winerack, slot, org " +
                        "FROM DIGITALTWINS slot " +
                        "JOIN winerack RELATED slot.partOf " +
                        "JOIN org RELATED winerack.ownedBy " +
                        $"WHERE org.$dtId = '{orgId}' " +
                        $"AND  winerack.name = '{wineRackName}' " +
                        $"AND slot.slotNumber = {slotNumber}";

            var result = dtClient.QueryAsync<BasicDigitalTwin>(query).SingleAsync().Result;
            
            var slot = DigitalTwinSerializer.Deserialize<Slot>(result, "slot");

            slot.SlotNumber.Should().Be(slotNumber);
            slot.Name.Should().Be("Slot-3");
            slot.Id.Should().Be("vfsddkd6qd");

            var org = DigitalTwinSerializer.Deserialize<Organization>(result, "org");

            org.Id.Should().Be("cpgkkzvsig");
            org.Name.Should().Be("My Org");
        }

        [Fact]
        public void CanGetOrgId()
        {
            var id = repository.GetOrganizationTwinId("My Org");

            id.Should().Be("cpgkkzvsig");
        }

        [Fact]
        public void CanGetSlot()
        {
            var orgTwinId = "cpgkkzvsig";
            var slot = repository.GetSlot("cpgkkzvsig", "testwinerack", 3);

            slot.OrganizationTwinId.Should().Be(orgTwinId);
            slot.SlotTwinId.Should().Be("vfsddkd6qd");
            slot.WineRackTwinId.Should().Be("538sn2vtza");
            slot.BottleTwinId.Should().BeNull();
        }
    }
}
