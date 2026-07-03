using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<Room?> GetByCodeAsync(string code);
        Task<IReadOnlyList<Room>> GetAllWithDevicesAndStateAsync();
    }
}
