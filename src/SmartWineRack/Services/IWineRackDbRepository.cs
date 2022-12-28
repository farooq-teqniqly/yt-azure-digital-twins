using Microsoft.EntityFrameworkCore;
using RandomStringCreator;
using SmartWineRack.Db;

namespace SmartWineRack.Services
{
    public interface IIdFactory
    {
        public string CreateId();
    }

    public class IdFactory: IIdFactory
    {
        private readonly StringCreator stringCreator = new();
        
        public string CreateId()
        {
            return stringCreator.Get(10);
        }
    }

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

        public Task DecomissionWineRack();
    }

    public class WineRackDbRepository : IWineRackDbRepository
    {
        private readonly WineRackDbContext db;
        private readonly IIdFactory idFactory;

        public WineRackDbRepository(WineRackDbContext db, IIdFactory idFactory)
        {
            this.db = db;
            this.idFactory = idFactory;
        }
        public async Task AddConfig(string name, string value)
        {
            await db.Configs.AddAsync(new WineRackConfig { Id = idFactory.CreateId(), Name = name, Value = value });
            await db.SaveChangesAsync();
        }

        public async Task AddConfigs(IEnumerable<KeyValuePair<string, string>> items)
        {
            await db.Configs.AddRangeAsync(
                items.Select(
                    item => new WineRackConfig
                    {
                        Id = idFactory.CreateId(), Name = item.Key, Value = item.Value
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
                            Id = idFactory.CreateId(), 
                            SlotNumber = i + 1
                        });
            }

            var wineRack = new WineRack
            {
                Id = idFactory.CreateId(),
                Scanner = new Scanner { Id = idFactory.CreateId() },
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

        public async Task DecomissionWineRack()
        {
            await db.Database.ExecuteSqlAsync($"DELETE FROM Bottle;");
            await db.Database.ExecuteSqlAsync($"DELETE FROM WineRackSlot;");
            await db.Database.ExecuteSqlAsync($"DELETE FROM Scanner;");
            await db.Database.ExecuteSqlAsync($"DELETE FROM WineRack;");
            await db.Database.ExecuteSqlAsync($"DELETE FROM WineRackConfig;");
        }
    }
}
