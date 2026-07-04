using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Bot.Configuration;

namespace Bot.Services;

public class DiscordBotHostedService : IHostedService
{
    private readonly DiscordSocketClient client;
    private readonly CommandHandlingService commandHandler;
    private readonly DiscordSettings settings;

    public DiscordBotHostedService(DiscordSocketClient client, CommandHandlingService commandHandler,
        IOptions<DiscordSettings> settings)
    {
        this.client = client;
        this.commandHandler = commandHandler;
        this.settings = settings.Value;
        this.client.Log += LogAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await commandHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, settings.Token);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
        await client.LogoutAsync();
    }

    private static Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}
