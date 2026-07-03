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

        private async Task<double> CalculateTodayKwhAsync(IEnumerable<Device> devices)
        {
            var todayStart = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;
            double totalKwh = 0;

            // Two bulk queries cover every device instead of two round trips per device.
            var historySince = (await uow.DeviceStateHistories.GetAllSinceAsync(todayStart))
                .GroupBy(h => h.DeviceId)
                .ToDictionary(g => g.Key, g => g.OrderBy(h => h.ChangedAt).ToList());
            var priorEntries = (await uow.DeviceStateHistories.GetLastEntriesBeforeAsync(todayStart))
                .ToDictionary(h => h.DeviceId);

            foreach (var device in devices)
            {
                var history = historySince.TryGetValue(device.Id, out var h) ? h : new List<DeviceStateHistory>();
                var wasOn = priorEntries.TryGetValue(device.Id, out var priorEntry) && priorEntry.IsOn;
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
