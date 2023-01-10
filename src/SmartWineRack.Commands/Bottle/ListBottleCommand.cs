﻿using SmartWineRack.Commands.Models;
using SmartWineRack.Data.Repositories;

namespace SmartWineRack.Commands.Bottle;

public class ListBottleCommand : Command<BottleSnapshot>
{
    public ListBottleCommand(IRepository repository) : base(repository)
    {
    }

    public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
    {
        var bottleDtos = await Repository.GetBottles();
        var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

        return new BottleSnapshot(bottles);
    }
}