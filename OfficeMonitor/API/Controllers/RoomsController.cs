using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace OfficeMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IDeviceService deviceService;

    public RoomsController(IDeviceService deviceService)
    {
        this.deviceService = deviceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomStatusDto>>> GetAll()
    {
        return Ok(await deviceService.GetAllRoomStatusesAsync());
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<RoomStatusDto>> GetByCode(string code)
    {
        var status = await deviceService.GetRoomStatusAsync(code);
        return status is null ? NotFound($"Room '{code}' not found") : Ok(status);
    }
}