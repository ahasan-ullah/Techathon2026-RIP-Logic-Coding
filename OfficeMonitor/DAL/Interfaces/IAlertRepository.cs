using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAlertRepository : IGenericRepository<Alert>
    {
        Task<IReadOnlyList<Alert>> GetActiveAsync();
        Task<IReadOnlyList<Alert>> GetActiveByTypeAsync(Enums.AlertType type);
        Task<Alert?> GetActiveByDeviceAndTypeAsync(int deviceId, Enums.AlertType type);
        Task<Alert?> GetActiveByRoomAndTypeAsync(int roomId, Enums.AlertType type);
    }
}
