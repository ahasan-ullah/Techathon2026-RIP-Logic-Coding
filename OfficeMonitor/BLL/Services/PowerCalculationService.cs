using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PowerCalculationService : IPowerCalculationService
    {
        private readonly IUnitOfWork uow;

        public PowerCalculationService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<UsageDto> GetCurrentUsageAsync()
        {
            var rooms = await uow.Rooms.GetAllWithDevicesAndStateAsync();

            var breakdown = rooms.Select(r => new RoomUsageBreakdownDto
            {
                RoomId = r.Id,
                RoomName = r.Name,
                CurrentWatts = r.Devices
                    .Where(d => d.DeviceState?.IsOn == true)
                    .Sum(d => d.RatedPowerWatts)
            }).ToList();

            var totalWatts = breakdown.Sum(b => b.CurrentWatts);
            var todayKwh = await CalculateTodayKwhAsync(rooms.SelectMany(r => r.Devices));

            return new UsageDto
            {
                CurrentTotalWatts = totalWatts,
                TodayEstimatedKwh = Math.Round(todayKwh, 2),
                RoomBreakdown = breakdown
            };
        }

        private async Task<double> CalculateTodayKwhAsync(IEnumerable<DAL.Entities.Device> devices)
        {
            var todayStart = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;
            double totalKwh = 0;

            foreach (var device in devices)
            {
                var history = await uow.DeviceStateHistories.GetForDeviceSinceAsync(device.Id, todayStart);

                var priorEntry = await uow.DeviceStateHistories.GetLastEntryBeforeAsync(device.Id, todayStart);
                var wasOn = priorEntry?.IsOn ?? false;
                var segmentStart = todayStart;

                foreach (var entry in history)
                {
                    if (wasOn)
                    {
                        totalKwh += HoursOn(segmentStart, entry.ChangedAt) * device.RatedPowerWatts / 1000.0;
                    }
                    wasOn = entry.IsOn;
                    segmentStart = entry.ChangedAt;
                }

                if (wasOn)
                {
                    totalKwh += HoursOn(segmentStart, now) * device.RatedPowerWatts / 1000.0;
                }
            }

            return totalKwh;
        }

        private static double HoursOn(DateTime from, DateTime to) =>
            Math.Max(0, (to - from).TotalHours);
    }
}
