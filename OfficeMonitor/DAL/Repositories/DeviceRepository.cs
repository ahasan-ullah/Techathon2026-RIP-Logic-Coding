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
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(OfficeMonitorContext context) : base(context) { }

        public async Task<Device?> GetByIdWithStateAsync(int id) =>
            await DbSet.AsNoTracking()
                .Include(d => d.DeviceState)
                .Include(d => d.Room)
                .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<IReadOnlyList<Device>> GetAllWithStateAsync() =>
            await DbSet.AsNoTracking()
                .Include(d => d.DeviceState)
                .Include(d => d.Room)
                .ToListAsync();

        public async Task<IReadOnlyList<Device>> GetByRoomIdWithStateAsync(int roomId) =>
            await DbSet.AsNoTracking()
                .Where(d => d.RoomId == roomId)
                .Include(d => d.DeviceState)
                .Include(d => d.Room)
                .ToListAsync();
    }
}
