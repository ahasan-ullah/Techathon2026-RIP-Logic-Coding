using System.Net;
using System.Net.Http.Json;
using Bot.Models;

namespace Bot.Services;

public class OfficeApiClient
{
    private readonly HttpClient http;

    public OfficeApiClient(HttpClient http)
    {
        this.http = http; // BaseAddress is set via DI registration in Program.cs
    }

    public Task<List<RoomStatusDto>?> GetRoomsAsync(CancellationToken ct = default) =>
        http.GetFromJsonAsync<List<RoomStatusDto>>("rooms", ct);

    public async Task<RoomStatusDto?> GetRoomAsync(string code, CancellationToken ct = default)
    {
        try
        {
            return await http.GetFromJsonAsync<RoomStatusDto>($"rooms/{code}", ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // unknown room code — not a backend failure
        }
    }

    public Task<UsageDto?> GetUsageAsync(CancellationToken ct = default) =>
        http.GetFromJsonAsync<UsageDto>("usage", ct);
}
