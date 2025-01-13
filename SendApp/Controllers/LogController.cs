using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        private readonly ILogService _service;
        public LogController(ILogService service)
        {
            _service = service;
        }

        [HttpPost("PostLogAsync")]
        public async Task<IActionResult> PostLogAsync([FromBody] Log log)
        {
            return Ok(await _service.PostLogAsync(log));
        }

        [HttpGet("GetLogsByDateAsync")]
        public async Task<IActionResult> GetLogsByDateAsync(int currentMonth)
        {
            return Ok(await _service.GetLogsByDateAsync(currentMonth));
        }
    }

}

