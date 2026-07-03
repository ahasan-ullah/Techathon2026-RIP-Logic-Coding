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
    public class DeviceStateHistoryRepository : GenericRepository<DeviceStateHistory>, IDeviceStateHistoryRepository
    {
        public DeviceStateHistoryRepository(OfficeMonitorContext context) : base(context) { }

        public async Task<IReadOnlyList<DeviceStateHistory>> GetForDeviceSinceAsync(int deviceId, DateTime since) =>
            await DbSet.AsNoTracking()
                .Where(h => h.DeviceId == deviceId && h.ChangedAt >= since)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();

        public async Task<DeviceStateHistory?> GetLastEntryBeforeAsync(int deviceId, DateTime beforeUtc) =>
            await DbSet.AsNoTracking()
                .Where(h => h.DeviceId == deviceId && h.ChangedAt < beforeUtc)
                .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefaultAsync();

        public async Task<IReadOnlyList<DeviceStateHistory>> GetAllSinceAsync(DateTime since) =>
            await DbSet.AsNoTracking()
                .Where(h => h.ChangedAt >= since)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
    }
}
