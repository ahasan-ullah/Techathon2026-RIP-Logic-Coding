using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IDeviceStateHistoryRepository : IGenericRepository<DeviceStateHistory>
    {
        Task<IReadOnlyList<DeviceStateHistory>> GetForDeviceSinceAsync(int deviceId, DateTime since);
        Task<DeviceStateHistory?> GetLastEntryBeforeAsync(int deviceId, DateTime beforeUtc);
        Task<IReadOnlyList<DeviceStateHistory>> GetAllSinceAsync(DateTime since);
        Task<IReadOnlyList<DeviceStateHistory>> GetLastEntriesBeforeAsync(DateTime beforeUtc);
    }

}
