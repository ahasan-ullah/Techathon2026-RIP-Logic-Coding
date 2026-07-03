using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository Rooms { get; }
        IDeviceRepository Devices { get; }
        IDeviceStateRepository DeviceStates { get; }
        IDeviceStateHistoryRepository DeviceStateHistories { get; }
        IAlertRepository Alerts { get; }

        Task<int> SaveChangesAsync();
    }
}
