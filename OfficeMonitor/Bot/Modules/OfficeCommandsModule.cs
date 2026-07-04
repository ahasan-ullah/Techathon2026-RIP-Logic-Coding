using Discord.Commands;
using Bot.Services;

namespace Bot.Modules;

public class OfficeCommandsModule : ModuleBase<SocketCommandContext>
{
    private readonly OfficeApiClient api;

    public OfficeCommandsModule(OfficeApiClient api)
    {
        this.api = api;
    }

    [Command("status")]
    [Summary("Shows ON/OFF status for every room.")]
    public async Task StatusAsync()
    {
        await Context.Channel.TriggerTypingAsync();

        try
        {
            var rooms = await api.GetRoomsAsync();
            if (rooms is null || rooms.Count == 0)
            {
                await ReplyAsync("⚠️ The office backend returned no rooms — is the database seeded?");
                return;
            }

            await Context.Channel.SendMessageAsync(embed: EmbedFactory.BuildStatusEmbed(rooms));
        }
        catch (HttpRequestException)
        {
            await ReplyAsync("⚠️ I couldn't reach the office backend right now — try again in a moment.");
        }
    }

    [Command("room")]
    [Summary("Shows ON/OFF status for one room. Usage: !room work1")]
    public async Task RoomAsync([Remainder] string code)
    {
        await Context.Channel.TriggerTypingAsync();

        try
        {
            var normalized = code.Trim().ToLowerInvariant();
            var room = await api.GetRoomAsync(normalized);
            if (room is null)
            {
                await ReplyAsync($"⚠️ I don't know a room called \"{code}\". Try `drawing`, `work1`, or `work2`.");
                return;
            }

            await Context.Channel.SendMessageAsync(embed: EmbedFactory.BuildRoomEmbed(room));
        }
        catch (HttpRequestException)
        {
            await ReplyAsync("⚠️ I couldn't reach the office backend right now — try again in a moment.");
        }
    }

    [Command("usage")]
    [Summary("Shows current power draw and today's estimated usage.")]
    public async Task UsageAsync()
    {
        await Context.Channel.TriggerTypingAsync();

        try
        {
            var usage = await api.GetUsageAsync();
            if (usage is null)
            {
                await ReplyAsync("⚠️ The office backend returned no usage data.");
                return;
            }

            await Context.Channel.SendMessageAsync(embed: EmbedFactory.BuildUsageEmbed(usage));
        }
        catch (HttpRequestException)
        {
            await ReplyAsync("⚠️ I couldn't reach the office backend right now — try again in a moment.");
        }
    }
}
