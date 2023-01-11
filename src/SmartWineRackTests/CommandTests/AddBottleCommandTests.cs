// <copyright file="AddBottleCommandTests.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRackTests.CommandTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using SmartWineRack.Commands.Bottle;
    using SmartWineRack.Data.Dto;
    using SmartWineRack.Data.Repositories;
    using Xunit;

    public class AddBottleCommandTests
    {
        [Fact]
        public async Task When_Parameters_Dict_Null_Throw_Exception()
        {
            var mockRepository = new Mock<IRepository>();
            var command = new AddBottleCommand(mockRepository.Object);

            Func<Task> act = () => command.Execute();

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData("foo", "12345", "upcCode", "12345")]
        [InlineData("slot", "1", "foo", "12345")]
        public async Task When_Required_Params_Missing_Throw_Exception(string key1, string val1, string key2, string val2)
        {
            var commandParams = new Dictionary<string, object>
            {
                { key1, val1 },
                { key2, val2 },
            };

            var mockRepository = new Mock<IRepository>();
            var command = new AddBottleCommand(mockRepository.Object);

            Func<Task> act = () => command.Execute(commandParams);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

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
                new () { Slot = slot, UpcCode = upcCode },
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
