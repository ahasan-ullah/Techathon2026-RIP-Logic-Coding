using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OfficeMonitor.API.Hubs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService alertService;
        private readonly IHubContext<DeviceHub> hubContext;

        public AlertsController(IAlertService alertService, IHubContext<DeviceHub> hubContext)
        {
            this.alertService = alertService;
            this.hubContext = hubContext;
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<AlertDto>>> GetActiveAlerts()
        {
            var alerts = await alertService.GetActiveAlertsAsync();
            return Ok(alerts);
        }

        [HttpPost("evaluate")]
        public async Task<ActionResult<List<AlertDto>>> Evaluate()
        {
            await alertService.EvaluateAlertsAsync();
            var alerts = await alertService.GetActiveAlertsAsync();
            await hubContext.Clients.All.SendAsync("AlertsUpdated", alerts);
            return Ok(alerts);
        }
    }
}
