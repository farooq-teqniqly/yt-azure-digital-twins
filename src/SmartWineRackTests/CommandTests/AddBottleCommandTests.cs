using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartWineRack.Commands;
using SmartWineRack.Commands.Bottle;
using SmartWineRack.Data.Dto;
using SmartWineRack.Data.Repositories;
using Xunit;

namespace SmartWineRackTests.CommandTests
{
    public class AddBottleCommandTests
    {
        [Fact]
        public async Task AddBottleCommand_Returns_Bottle_Snapshot()
        {
            var slot = 1;
            var upcCode = "abc123";

            var commandParams = new Dictionary<string, object>
            {
                { "slot", slot },
                { "upcCode", upcCode },
            };

            var mockRepository = new Mock<IRepository>();
            
            mockRepository.Setup(r => r.GetBottles()).ReturnsAsync(new List<BottleDto>
            {
                new() { Slot = slot, UpcCode = upcCode }
            });

            var command = new AddBottleCommand(mockRepository.Object);
            var snapshot = await command.Execute(commandParams);
            
            snapshot.Bottles.Count().Should().Be(1);

            var bottle = snapshot.Bottles.Single();

            bottle.Slot.Should().Be(slot);
            bottle.UpcCode.Should().Be(upcCode);

            mockRepository.Verify(m => m.AddBottle(It.Is<BottleDto>(dto =>
                dto.Slot == slot && dto.UpcCode == upcCode)));
        }
    }
}
