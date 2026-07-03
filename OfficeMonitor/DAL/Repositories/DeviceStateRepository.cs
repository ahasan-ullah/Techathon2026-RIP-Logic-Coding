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
    public class DeviceStateRepository : GenericRepository<DeviceState>, IDeviceStateRepository
    {
        public DeviceStateRepository(OfficeMonitorContext context) : base(context) { }

        public async Task<DeviceState?> GetByDeviceIdAsync(int deviceId) =>
            await DbSet.FirstOrDefaultAsync(s => s.DeviceId == deviceId);
    }
}
