using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RandomStringCreator;
using SmartWineRack.Db;
using static System.Reflection.Metadata.BlobBuilder;

namespace SmartWineRack.Repositories
{
    public interface IWineRackDbRepository
    {
        public Task AddConfig(string name, string value);
        public Task AddConfigs(IEnumerable<KeyValuePair<string, string>> items);

        public Task<string> GetConfig(string name);

        public Task<IEnumerable<WineRackConfig>> ListConfigs();
        public Task RemoveConfig(string name);

        public Task AddWineRack(int slotCount);

        public Task<IEnumerable<WineRackSlot>> GetSlots();

        public Task AddBottle(string upcCode, int slotNumber);
        public Task RemoveBottle(int slotNumber);
        public Task ReturnBottle(int slotNumber);
        public Task ScanBottle(int slotNumber);
    }

    public class WineRackDbRepository : IWineRackDbRepository
    {
        private readonly WineRackDbContext db;
        private readonly StringCreator idCreator;

        public WineRackDbRepository(WineRackDbContext db, StringCreator idCreator)
        {
            this.db = db;
            this.idCreator = idCreator;
        }
        public async Task AddConfig(string name, string value)
        {
            await db.Configs.AddAsync(new WineRackConfig { Id = idCreator.Get(10), Name = name, Value = value });
            await db.SaveChangesAsync();
        }

        public async Task AddConfigs(IEnumerable<KeyValuePair<string, string>> items)
        {
            await db.Configs.AddRangeAsync(
                items.Select(
                    item => new WineRackConfig
                    {
                        Id = idCreator.Get(10), Name = item.Key, Value = item.Value
                    }));

            await db.SaveChangesAsync();
        }

        public async Task<string> GetConfig(string name)
        {
            return (await db.Configs.SingleAsync(c => c.Name == name)).Value;
        }

        public async Task<IEnumerable<WineRackConfig>> ListConfigs()
        {
            return await db.Configs.ToListAsync();
        }

        public async Task RemoveConfig(string name)
        {
            var config = await db.Configs.SingleAsync(c => c.Name == name);
            db.Configs.Remove(config);
            await db.SaveChangesAsync();
        }

        public async Task AddWineRack(int slotCount)
        {
            var slots = new List<WineRackSlot>(slotCount);

            for (var i = 0; i < slotCount; i++)
            {
                slots.Add(
                    new WineRackSlot
                        {
                            Id = idCreator.Get(10), 
                            SlotNumber = i + 1
                        });
            }

            var wineRack = new WineRack
            {
                Id = idCreator.Get(10),
                Scanner = new Scanner { Id = idCreator.Get(10) },
                WineRackSlots = slots
            };

            await db.WineRacks.AddAsync(wineRack);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<WineRackSlot>> GetSlots()
        {
            var wineRack = await(db.WineRacks
                    .Include(r => r.WineRackSlots)
                    .ThenInclude(s => s.Bottle))
                .SingleAsync();

            return wineRack.WineRackSlots
                .OrderBy(s => s.SlotNumber);
        }

        public async Task AddBottle(string upcCode, int slotNumber)
        {
            (await GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle = new Bottle { UpcCode = upcCode, BottleState = BottleState.InPlace };
            await db.SaveChangesAsync();

        }

        public async Task RemoveBottle(int slotNumber)
        {
            (await GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle.BottleState = BottleState.RemoveNotScanned;
            await db.SaveChangesAsync();
        }

        public async Task ReturnBottle(int slotNumber)
        {
            (await GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle.BottleState = BottleState.InPlace;
            await db.SaveChangesAsync();
        }

        public async Task ScanBottle(int slotNumber)
        {
            (await GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle = null;
            await db.SaveChangesAsync();
        }
    }
}
