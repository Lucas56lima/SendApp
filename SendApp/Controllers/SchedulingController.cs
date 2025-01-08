using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulingController : Controller
    {
        private readonly ISchedulingService _service;
        public SchedulingController(ISchedulingService service)
        {
            _service = service;
        }
        
        [HttpPost("PostSchedulingAsync")]        
        public async Task<IActionResult> PostSchedulingAsync([FromBody] Scheduling scheduling)
        {
            return Ok(await _service.PostSchedulingAsync(scheduling));
        }

        [HttpGet("GetSchedulingByStatusAsync")]
        public async Task<IActionResult> GetSchedulingByStatusAsync(string status)
        {
            return Ok(await _service.GetSchedulingByStatusAsync(status));
        }
    }
}
