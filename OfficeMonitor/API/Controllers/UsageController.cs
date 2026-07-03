using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace OfficeMonitor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsageController : ControllerBase
{
    private readonly IPowerCalculationService powerService;

    public UsageController(IPowerCalculationService powerService)
    {
        this.powerService = powerService;
    }

    [HttpGet]
    public async Task<ActionResult<UsageDto>> Get()
    {
        return Ok(await powerService.GetCurrentUsageAsync());
    }
}