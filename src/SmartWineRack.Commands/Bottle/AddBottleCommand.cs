using SmartWineRack.Commands.Models;
using SmartWineRack.Data.Dto;
using SmartWineRack.Data.Repositories;

namespace SmartWineRack.Commands.Bottle
{
    public class AddBottleCommand : Command<BottleSnapshot>
    {
        public AddBottleCommand(IRepository repository) : base(repository)
        {
        }

        public override async Task<BottleSnapshot> Execute(IDictionary<string, object>? parameters = null)
        {
            await Repository.AddBottle(new BottleDto {Slot = (int)parameters["slot"], UpcCode = (string)parameters["upcCode"] });

            var bottleDtos = await Repository.GetBottles();
            var bottles = bottleDtos.Select(bottleDto => new Models.Bottle(bottleDto.Slot, bottleDto.UpcCode)).ToList();

            return new BottleSnapshot(bottles);
        }
    }
}
