using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Bot.Configuration;
using Bot.Models;

namespace Bot.Services;

public class AlertWatcherService : BackgroundService
{
    private readonly DiscordSocketClient discord;
    private readonly ApiSettings apiSettings;
    private readonly DiscordSettings discordSettings;
    private readonly HashSet<int> seenAlertIds = new();
    private HubConnection? hubConnection;

    public AlertWatcherService(DiscordSocketClient discord,
        IOptions<ApiSettings> apiSettings, IOptions<DiscordSettings> discordSettings)
    {
        this.discord = discord;
        this.apiSettings = apiSettings.Value;
        this.discordSettings = discordSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitUntilDiscordReadyAsync(stoppingToken);

        hubConnection = new HubConnectionBuilder()
            .WithUrl(apiSettings.HubUrl)
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<List<AlertDto>>("AlertsUpdated", async alerts =>
        {
            var newAlerts = alerts.Where(a => seenAlertIds.Add(a.Id)).ToList();
            foreach (var alert in newAlerts)
                await PostAlertAsync(alert);
        });

        await hubConnection.StartAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { });
    }

    private async Task WaitUntilDiscordReadyAsync(CancellationToken ct)
    {
        if (discord.ConnectionState == ConnectionState.Connected) return;

        var tcs = new TaskCompletionSource();
        Task OnReady() { tcs.TrySetResult(); return Task.CompletedTask; }

        discord.Ready += OnReady;
        try
        {
            using var reg = ct.Register(() => tcs.TrySetCanceled());
            await tcs.Task;
        }
        finally
        {
            discord.Ready -= OnReady;
        }
    }

    private async Task PostAlertAsync(AlertDto alert)
    {
        if (discord.GetChannel(discordSettings.AlertChannelId) is not IMessageChannel channel) return;
        await channel.SendMessageAsync(embed: EmbedFactory.BuildAlertEmbed(alert));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (hubConnection is not null) await hubConnection.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
