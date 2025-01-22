using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        private readonly IStoreService _service;
        private readonly ISchedulingService _schedulingService;
        public StoreController(IStoreService service, ISchedulingService schedulingService)
        {
            _service = service;
            _schedulingService = schedulingService;
        }

        [HttpPost("PostStoreAsync")]
        public async Task<IActionResult> PostStoreAsync([FromBody] Store store)
        {
            Scheduling scheduling = new()
            {
                Store = store.Name
            };
            Ok(_schedulingService.PostSchedulingAsync(scheduling));
            return Ok(await _service.PostStoreAsync(store));
        }

        [HttpGet("GetStoreByIdAsync")]
        public async Task<IActionResult> GetStoresByIdAsync(int id)
        {
            return Ok(await _service.GetStoresByIdAsync(id));
        }

        [HttpPut("PutStoreByIdAsync")]
        public async Task<IActionResult> PutStoreByIdAsync(int id, [FromBody] Store store)
        {   
            return Ok(await _service.PutStoreByIdAsync(id, store));
        }
    }
}

