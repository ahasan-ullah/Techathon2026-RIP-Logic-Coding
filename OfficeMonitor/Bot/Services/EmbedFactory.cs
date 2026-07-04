using Discord;
using Bot.Models;

namespace Bot.Services;

public static class EmbedFactory
{
    public static Embed BuildStatusEmbed(List<RoomStatusDto> rooms)
    {
        var builder = new EmbedBuilder()
            .WithTitle("📋 Office Status")
            .WithColor(rooms.Any(r => r.DevicesOnCount > 0) ? Color.Gold : Color.Green)
            .WithCurrentTimestamp();

        foreach (var room in rooms)
        {
            var fans = room.Devices.Count(d => d.Type == "Fan" && d.IsOn);
            var lights = room.Devices.Count(d => d.Type == "Light" && d.IsOn);

            var value = room.DevicesOnCount == 0
                ? "✅ All off"
                : $"🌀 {fans} fan{(fans == 1 ? "" : "s")} ON\n💡 {lights} light{(lights == 1 ? "" : "s")} ON\n⚡ {room.CurrentPowerWatts}W";

            builder.AddField(room.RoomName, value, inline: true);
        }

        return builder.Build();
    }

    public static Embed BuildRoomEmbed(RoomStatusDto room)
    {
        var deviceLines = room.Devices
            .OrderBy(d => d.Type).ThenBy(d => d.Name)
            .Select(d => $"{(d.IsOn ? "🟢" : "⚫")} {(d.Type == "Fan" ? "🌀" : "💡")} {d.Name}");

        var builder = new EmbedBuilder()
            .WithTitle($"🏠 {room.RoomName}")
            .WithColor(room.DevicesOnCount > 0 ? Color.Gold : Color.Green)
            .WithFooter($"Room code: {room.Code}")
            .WithCurrentTimestamp()
            .AddField("Devices", string.Join("\n", deviceLines), inline: false)
            .AddField("On", $"{room.DevicesOnCount}/{room.Devices.Count}", inline: true)
            .AddField("Current Draw", $"{room.CurrentPowerWatts} W", inline: true);

        return builder.Build();
    }

    public static Embed BuildUsageEmbed(UsageDto usage)
    {
        var breakdown = string.Join("\n", usage.RoomBreakdown.Select(r => $"**{r.RoomName}:** {r.CurrentWatts} W"));

        var builder = new EmbedBuilder()
            .WithTitle("⚡ Power Usage")
            .WithColor(usage.CurrentTotalWatts > 0 ? Color.Gold : Color.Green)
            .WithCurrentTimestamp()
            .AddField("Right Now", $"{usage.CurrentTotalWatts} W", inline: true)
            .AddField("Today (est.)", $"{usage.TodayEstimatedKwh:F2} kWh", inline: true)
            .AddField("Per Room", string.IsNullOrWhiteSpace(breakdown) ? "—" : breakdown, inline: false);

        return builder.Build();
    }

    public static Embed BuildAlertEmbed(AlertDto alert)
    {
        var isContinuous = alert.Type == "ContinuousUsage";

        var builder = new EmbedBuilder()
            .WithTitle(isContinuous ? "🔥 Continuous Usage Alert" : "⚠️ After-Hours Alert")
            .WithDescription(alert.Message)
            .WithColor(isContinuous ? Color.Orange : Color.Red)
            .WithTimestamp(new DateTimeOffset(alert.TriggeredAt, TimeSpan.Zero));

        if (alert.RoomName is not null) builder.AddField("Room", alert.RoomName, inline: true);
        if (alert.DeviceName is not null) builder.AddField("Device", alert.DeviceName, inline: true);

        return builder.Build();
    }
}
