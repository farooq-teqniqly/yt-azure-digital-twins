// <copyright file="IWineRackDbRepository.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using SmartWineRack.Db;

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
}
