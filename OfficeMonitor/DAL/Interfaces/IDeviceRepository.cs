using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IDeviceRepository : IGenericRepository<Device>
    {
        Task<Device?> GetByIdWithStateAsync(int id);
        Task<IReadOnlyList<Device>> GetAllWithStateAsync();
        Task<IReadOnlyList<Device>> GetByRoomIdWithStateAsync(int roomId);
    }

}
