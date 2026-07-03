using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDeviceService
    {
        Task<List<DeviceDto>> GetAllDevicesAsync();
        Task<RoomStatusDto?> GetRoomStatusAsync(string roomCode);
        Task<List<RoomStatusDto>> GetAllRoomStatusesAsync();
        Task<DeviceDto> ToggleDeviceAsync(int deviceId, bool isOn);
    }
}
