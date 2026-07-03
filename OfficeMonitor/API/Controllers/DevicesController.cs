using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OfficeMonitor.API.Hubs;

namespace OfficeMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService deviceService;
    private readonly IHubContext<DeviceHub> hubContext;

    public DevicesController(IDeviceService deviceService, IHubContext<DeviceHub> hubContext)
    {
        this.deviceService = deviceService;
        this.hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<DeviceDto>>> GetAll()
    {
        return Ok(await deviceService.GetAllDevicesAsync());
    }

    [HttpPost("{id:int}/toggle")]
    public async Task<ActionResult<DeviceDto>> Toggle(int id, [FromBody] DeviceToggleRequestDto request)
    {
        try
        {
            var updated = await deviceService.ToggleDeviceAsync(id, request.IsOn);

            await hubContext.Clients.All.SendAsync("DeviceStateChanged", updated);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}