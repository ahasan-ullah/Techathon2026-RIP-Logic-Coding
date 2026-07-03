using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(OfficeMonitorContext context) : base(context) { }

        public async Task<Room?> GetByCodeAsync(string code) =>
            await DbSet.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code);

        public async Task<IReadOnlyList<Room>> GetAllWithDevicesAndStateAsync() =>
            await DbSet.AsNoTracking()
                .Include(r => r.Devices)
                    .ThenInclude(d => d.DeviceState)
                .ToListAsync();
    }
}
