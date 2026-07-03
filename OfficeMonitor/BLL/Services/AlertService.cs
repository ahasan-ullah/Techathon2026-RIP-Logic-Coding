using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork uow;
        private static readonly TimeSpan OfficeStart = TimeSpan.FromHours(9);
        private static readonly TimeSpan OfficeEnd = TimeSpan.FromHours(17);
        private static readonly TimeSpan ContinuousThreshold = TimeSpan.FromHours(2);

        public AlertService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<AlertDto>> GetActiveAlertsAsync()
        {
            var alerts = await uow.Alerts.GetActiveAsync();
            return alerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type.ToString(),
                Message = a.Message,
                RoomName = a.Room?.Name,
                DeviceName = a.Device?.Name,
                TriggeredAt = a.TriggeredAt
            }).ToList();
        }

        public async Task EvaluateAlertsAsync()
        {
            var now = DateTime.UtcNow;
            var localTime = now.TimeOfDay;
            var devices = await uow.Devices.GetAllWithStateAsync();

            bool isAfterHours = localTime < OfficeStart || localTime > OfficeEnd;
            if (isAfterHours)
            {
                foreach (var device in devices.Where(d => d.DeviceState?.IsOn == true))
                {
                    await RaiseIfNotActiveAsync(
                        AlertType.AfterHours,
                        deviceId: device.Id,
                        roomId: device.RoomId,
                        message: $"{device.Name} in {device.Room.Name} is still ON outside office hours.");
                }
            }
            else
            {
                await ResolveByTypeAsync(AlertType.AfterHours);
            }

            var rooms = devices.GroupBy(d => d.RoomId);
            foreach (var group in rooms)
            {
                var allOn = group.All(d => d.DeviceState?.IsOn == true);
                if (!allOn) continue;

                var earliestOnSince = group.Max(d => d.DeviceState!.LastChangedAt);
                if (now - earliestOnSince >= ContinuousThreshold)
                {
                    var roomName = group.First().Room.Name;
                    await RaiseIfNotActiveAsync(
                        AlertType.ContinuousUsage,
                        deviceId: null,
                        roomId: group.Key,
                        message: $"{roomName} has had all devices ON continuously for over 2 hours.");
                }
            }

            await uow.SaveChangesAsync();
        }

        private async Task RaiseIfNotActiveAsync(AlertType type, int? deviceId, int? roomId, string message)
        {
            Alert? existing = deviceId.HasValue
                ? await uow.Alerts.GetActiveByDeviceAndTypeAsync(deviceId.Value, type)
                : roomId.HasValue ? await uow.Alerts.GetActiveByRoomAndTypeAsync(roomId.Value, type) : null;

            if (existing is not null) return;

            await uow.Alerts.AddAsync(new Alert
            {
                Type = type,
                DeviceId = deviceId,
                RoomId = roomId,
                Message = message,
                TriggeredAt = DateTime.UtcNow
            });
        }

        private async Task ResolveByTypeAsync(AlertType type)
        {
            var active = await uow.Alerts.GetActiveAsync();
            foreach (var alert in active.Where(a => a.Type == type))
            {
                alert.ResolvedAt = DateTime.UtcNow;
                uow.Alerts.Update(alert);
            }
        }
    }
}
