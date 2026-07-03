using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        public readonly IAlertService alertService;
        public AlertsController(IAlertService alertService)
        {
            this.alertService = alertService;
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<AlertDto>>> GetActiveAlerts()
        {
            var alerts = await alertService.GetActiveAlertsAsync();
            return Ok(alerts);
        }
}
}
