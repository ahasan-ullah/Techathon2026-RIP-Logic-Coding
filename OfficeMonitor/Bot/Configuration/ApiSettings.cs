namespace Bot.Configuration;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty; // e.g. https://localhost:7081/api/
    public string HubUrl { get; set; } = string.Empty;  // e.g. https://localhost:7081/hub/devices
}
