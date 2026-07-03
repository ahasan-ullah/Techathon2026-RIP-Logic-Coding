using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork uow;

        public DeviceService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await uow.Devices.GetAllWithStateAsync();
            return devices.Select(MapToDto).ToList();
        }

        public async Task<RoomStatusDto?> GetRoomStatusAsync(string roomCode)
        {
            var room = await uow.Rooms.GetByCodeAsync(roomCode);
            if (room is null) return null;

            var devices = await uow.Devices.GetByRoomIdWithStateAsync(room.Id);
            return BuildRoomStatus(room, devices);
        }

        public async Task<List<RoomStatusDto>> GetAllRoomStatusesAsync()
        {
            var rooms = await uow.Rooms.GetAllWithDevicesAndStateAsync();
            return rooms.Select(r => BuildRoomStatus(r, r.Devices)).ToList();
        }

        public async Task<DeviceDto> ToggleDeviceAsync(int deviceId, bool isOn)
        {
            var device = await uow.Devices.GetByIdAsync(deviceId)
                ?? throw new KeyNotFoundException($"Device {deviceId} not found");

            var state = await uow.DeviceStates.GetByDeviceIdAsync(deviceId)
                ?? throw new InvalidOperationException($"DeviceState missing for device {deviceId}");

            if (state.IsOn == isOn)
            {
                var unchanged = await uow.Devices.GetByIdWithStateAsync(deviceId);
                return MapToDto(unchanged!);
            }

            var now = DateTime.UtcNow;

            state.IsOn = isOn;
            state.LastChangedAt = now;
            uow.DeviceStates.Update(state);

            await uow.DeviceStateHistories.AddAsync(new DeviceStateHistory
            {
                DeviceId = deviceId,
                IsOn = isOn,
                ChangedAt = now
            });

            await uow.SaveChangesAsync();

            var updated = await uow.Devices.GetByIdWithStateAsync(deviceId);
            return MapToDto(updated!);
        }


        private static DeviceDto MapToDto(Device d) => new()
        {
            Id = d.Id,
            Name = d.Name,
            Type = d.Type.ToString(),
            RoomId = d.RoomId,
            RoomName = d.Room?.Name ?? string.Empty,
            IsOn = d.DeviceState?.IsOn ?? false,
            RatedPowerWatts = d.RatedPowerWatts,
            LastChangedAt = d.DeviceState?.LastChangedAt ?? DateTime.MinValue
        };

        private static RoomStatusDto BuildRoomStatus(Room room, IEnumerable<Device> devices)
        {
            var deviceDtos = devices.Select(MapToDto).ToList();
            return new RoomStatusDto
            {
                RoomId = room.Id,
                RoomName = room.Name,
                Code = room.Code,
                Devices = deviceDtos,
                DevicesOnCount = deviceDtos.Count(d => d.IsOn),
                CurrentPowerWatts = deviceDtos.Where(d => d.IsOn).Sum(d => d.RatedPowerWatts)
            };
        }
    }
}
