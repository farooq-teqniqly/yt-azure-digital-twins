using SmartWineRack.Commands.Models;
using SmartWineRack.Data.Repositories;

namespace SmartWineRack.Commands.Bottle;

public class SellBottleCommand : Command<BottleSnapshot>
{
    public SellBottleCommand(IRepository repository) : base(repository)
    {
    }

    public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
    {
        await Repository.RemoveBottle((int)parameters["slot"]);

        var bottleDtos = await Repository.GetBottles();
        var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

        return new BottleSnapshot(bottles);
    }
}