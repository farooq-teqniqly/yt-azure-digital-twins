using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWineRack.Data.Dto;

namespace SmartWineRack.Data.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<BottleDto>> GetBottles();
        Task AddBottle(BottleDto bottle);
        Task RemoveBottle(int slot);
        Task SaveConfig(WineRackConfigDto configDto);
        Task<WineRackConfigDto> GetConfig();
    }
}
