using DAL.Data;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OfficeMonitorContext context;

        public IRoomRepository Rooms { get; }
        public IDeviceRepository Devices { get; }
        public IDeviceStateRepository DeviceStates { get; }
        public IDeviceStateHistoryRepository DeviceStateHistories { get; }
        public IAlertRepository Alerts { get; }

        public UnitOfWork(OfficeMonitorContext context)
        {
            this.context = context;
            Rooms = new RoomRepository(context);
            Devices = new DeviceRepository(context);
            DeviceStates = new DeviceStateRepository(context);
            DeviceStateHistories = new DeviceStateHistoryRepository(context);
            Alerts = new AlertRepository(context);
        }

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();

        public void Dispose() => context.Dispose();
    }
}
