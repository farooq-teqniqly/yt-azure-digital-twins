// <copyright file="WineRackDbRepository.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using Microsoft.EntityFrameworkCore;
    using SmartWineRack.Db;

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
            await this.db.Configs.AddAsync(new WineRackConfig { Id = this.idFactory.CreateId(), Name = name, Value = value });
            await this.db.SaveChangesAsync();
        }

        public async Task AddConfigs(IEnumerable<KeyValuePair<string, string>> items)
        {
            await this.db.Configs.AddRangeAsync(
                items.Select(
                    item => new WineRackConfig
                    {
                        Id = this.idFactory.CreateId(), Name = item.Key, Value = item.Value,
                    }));

            await this.db.SaveChangesAsync();
        }

        public async Task<string> GetConfig(string name)
        {
            return (await this.db.Configs.SingleAsync(c => c.Name == name)).Value;
        }

        public async Task<IEnumerable<WineRackConfig>> ListConfigs()
        {
            return await this.db.Configs.ToListAsync();
        }

        public async Task RemoveConfig(string name)
        {
            var config = await this.db.Configs.SingleAsync(c => c.Name == name);
            this.db.Configs.Remove(config);
            await this.db.SaveChangesAsync();
        }

        public async Task AddWineRack(int slotCount)
        {
            var slots = new List<WineRackSlot>(slotCount);

            for (var i = 0; i < slotCount; i++)
            {
                slots.Add(
                    new WineRackSlot
                    {
                        Id = this.idFactory.CreateId(),
                        SlotNumber = i + 1,
                    });
            }

            var wineRack = new WineRack
            {
                Id = this.idFactory.CreateId(),
                Scanner = new Scanner { Id = this.idFactory.CreateId() },
                WineRackSlots = slots,
            };

            await this.db.WineRacks.AddAsync(wineRack);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<WineRackSlot>> GetSlots()
        {
            var wineRack = await this.db.WineRacks
                    .Include(r => r.WineRackSlots)
                    .ThenInclude(s => s.Bottle)
                .SingleAsync();

            return wineRack.WineRackSlots
                .OrderBy(s => s.SlotNumber);
        }

        public async Task AddBottle(string upcCode, int slotNumber)
        {
            (await this.GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle = new Bottle { UpcCode = upcCode, BottleState = BottleState.InPlace };
            await this.db.SaveChangesAsync();
        }

        public async Task RemoveBottle(int slotNumber)
        {
            (await this.GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle.BottleState = BottleState.RemoveNotScanned;
            await this.db.SaveChangesAsync();
        }

        public async Task ReturnBottle(int slotNumber)
        {
            (await this.GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle.BottleState = BottleState.InPlace;
            await this.db.SaveChangesAsync();
        }

        public async Task ScanBottle(int slotNumber)
        {
            (await this.GetSlots()).Single(s => s.SlotNumber == slotNumber).Bottle = null;
            await this.db.SaveChangesAsync();
        }

        public async Task DecomissionWineRack()
        {
            await this.db.Database.ExecuteSqlAsync($"DELETE FROM Bottle;");
            await this.db.Database.ExecuteSqlAsync($"DELETE FROM WineRackSlot;");
            await this.db.Database.ExecuteSqlAsync($"DELETE FROM Scanner;");
            await this.db.Database.ExecuteSqlAsync($"DELETE FROM WineRack;");
            await this.db.Database.ExecuteSqlAsync($"DELETE FROM WineRackConfig;");
        }
    }
}