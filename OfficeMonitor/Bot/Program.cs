using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Bot.Configuration;
using Bot.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("Api"));

builder.Services.AddHttpClient<OfficeApiClient>((sp, http) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    http.BaseAddress = new Uri(apiSettings.BaseUrl);
});

builder.Services.AddSingleton(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
});
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<CommandHandlingService>();

builder.Services.AddHostedService<DiscordBotHostedService>();
builder.Services.AddHostedService<AlertWatcherService>();

var host = builder.Build();
await host.RunAsync();

public partial class Program { }
