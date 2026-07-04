namespace Bot.Configuration;

public class DiscordSettings
{
    public string Token { get; set; } = string.Empty;
    public string Prefix { get; set; } = "!";
    public ulong AlertChannelId { get; set; }
}
