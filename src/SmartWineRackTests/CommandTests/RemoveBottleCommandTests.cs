﻿// <copyright file="RemoveBottleCommandTests.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

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

    public class RemoveBottleCommandTests
    {
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