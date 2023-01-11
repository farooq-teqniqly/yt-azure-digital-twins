// <copyright file="SellBottleCommandTests.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

using System;

namespace SmartWineRackTests.CommandTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using SmartWineRack.Commands.Bottle;
    using SmartWineRack.Data.Dto;
    using SmartWineRack.Data.Repositories;
    using Xunit;

    public class SellBottleCommandTests
    {
        [Fact]
        public async Task When_Parameters_Dict_Null_Throw_Exception()
        {
            var mockRepository = new Mock<IRepository>();
            var command = new SellBottleCommand(mockRepository.Object);

            Func<Task> act = () => command.Execute();

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData("nnnSlot", "1")]
        public async Task When_Required_Params_Missing_Throw_Exception(string key1, string val1)
        {
            var commandParams = new Dictionary<string, object>
            {
                { key1, val1 },
            };

            var mockRepository = new Mock<IRepository>();
            var command = new SellBottleCommand(mockRepository.Object);

            Func<Task> act = () => command.Execute(commandParams);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RemoveBottleCommand_Returns_Bottle_Snapshot()
        {
            var slot = 1;
            var upcCode = "abc123";

            var commandParams = new Dictionary<string, object>
            {
                { "slot", slot },
            };

            var mockRepository = new Mock<IRepository>();

            mockRepository.Setup(r => r.GetBottles()).ReturnsAsync(new List<BottleDto>
            {
                new () { Slot = slot, UpcCode = upcCode },
            });

            var command = new SellBottleCommand(mockRepository.Object);
            var snapshot = await command.Execute(commandParams);

            snapshot.Bottles.Count().Should().Be(1);

            var bottle = snapshot.Bottles.Single();

            bottle.Slot.Should().Be(slot);
            bottle.UpcCode.Should().Be(upcCode);

            mockRepository.Verify(m => m.RemoveBottle(slot));
        }
    }
}