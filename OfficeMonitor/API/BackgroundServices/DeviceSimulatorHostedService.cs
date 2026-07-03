using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using OfficeMonitor.API.Hubs;

namespace OfficeMonitor.API.BackgroundServices;

public class DeviceSimulatorHostedService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHubContext<DeviceHub> hubContext;
    private readonly ILogger<DeviceSimulatorHostedService> logger;
    private readonly bool enabled;
    private readonly TimeSpan interval;
    private static readonly Random Rng = new();

    public DeviceSimulatorHostedService(
        IServiceScopeFactory scopeFactory,
        IHubContext<DeviceHub> hubContext,
        IConfiguration configuration,
        ILogger<DeviceSimulatorHostedService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.hubContext = hubContext;
        this.logger = logger;
        enabled = configuration.GetValue("SimulatorSettings:Enabled", true);
        var intervalSeconds = configuration.GetValue("SimulatorSettings:IntervalSeconds", 15);
        interval = TimeSpan.FromSeconds(Math.Max(5, intervalSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!enabled) return;

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RunTickAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Device simulator tick failed.");
            }
        }
    }

    private async Task RunTickAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();
        var powerService = scope.ServiceProvider.GetRequiredService<IPowerCalculationService>();

        var devices = await deviceService.GetAllDevicesAsync();
        if (devices.Count == 0) return;

        var flipCount = Rng.Next(1, 3);
        var candidates = devices.OrderBy(_ => Rng.Next()).Take(flipCount);

        foreach (var candidate in candidates)
        {
            var updated = await deviceService.ToggleDeviceAsync(candidate.Id, !candidate.IsOn);
            await hubContext.Clients.All.SendAsync("DeviceStateChanged", updated, ct);
        }

        await alertService.EvaluateAlertsAsync();
        var alerts = await alertService.GetActiveAlertsAsync();
        await hubContext.Clients.All.SendAsync("AlertsUpdated", alerts, ct);

        var usage = await powerService.GetCurrentUsageAsync();
        await hubContext.Clients.All.SendAsync("UsageUpdated", usage, ct);
    }
}
