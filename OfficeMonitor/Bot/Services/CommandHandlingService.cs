using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Bot.Configuration;

namespace Bot.Services;

public class CommandHandlingService
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commands;
    private readonly IServiceProvider services;
    private readonly string prefix;

    public CommandHandlingService(DiscordSocketClient client, CommandService commands,
        IServiceProvider services, IOptions<DiscordSettings> discordSettings)
    {
        this.client = client;
        this.commands = commands;
        this.services = services;
        prefix = discordSettings.Value.Prefix;

        this.commands.CommandExecuted += OnCommandExecutedAsync;
        this.client.MessageReceived += OnMessageReceivedAsync;
    }

    public Task InitializeAsync() => commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

    private async Task OnMessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage message) return;
        if (message.Source != MessageSource.User) return;

        int argPos = 0;
        if (!message.HasStringPrefix(prefix, ref argPos)) return;

        var context = new SocketCommandContext(client, message);
        await commands.ExecuteAsync(context, argPos, services);
    }

    private Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            Console.WriteLine($"[Command Error] {command.Value?.Name}: {result.ErrorReason}");
        return Task.CompletedTask;
    }
}
