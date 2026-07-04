namespace Bot.Models;

public record DeviceDto(int Id, string Name, string Type, int RoomId, string RoomName, bool IsOn, int RatedPowerWatts, DateTime LastChangedAt);

public record RoomStatusDto(int RoomId, string RoomName, string Code, List<DeviceDto> Devices, int DevicesOnCount, int CurrentPowerWatts);

public record RoomUsageBreakdownDto(int RoomId, string RoomName, int CurrentWatts);

public record UsageDto(int CurrentTotalWatts, double TodayEstimatedKwh, List<RoomUsageBreakdownDto> RoomBreakdown);

public record AlertDto(int Id, string Type, string Message, string? RoomName, string? DeviceName, DateTime TriggeredAt);
