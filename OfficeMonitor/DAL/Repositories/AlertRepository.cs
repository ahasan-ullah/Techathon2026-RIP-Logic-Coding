using DAL.Data;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(OfficeMonitorContext context) : base(context) { }

        public async Task<IReadOnlyList<Alert>> GetActiveAsync() =>
            await DbSet.AsNoTracking()
                .Include(a => a.Room)
                .Include(a => a.Device)
                .Where(a => a.ResolvedAt == null)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

        public async Task<Alert?> GetActiveByDeviceAndTypeAsync(int deviceId, AlertType type) =>
            await DbSet.FirstOrDefaultAsync(a =>
                a.DeviceId == deviceId && a.Type == type && a.ResolvedAt == null);

        public async Task<Alert?> GetActiveByRoomAndTypeAsync(int roomId, AlertType type) =>
            await DbSet.FirstOrDefaultAsync(a =>
                a.RoomId == roomId && a.Type == type && a.ResolvedAt == null);
    }
}
