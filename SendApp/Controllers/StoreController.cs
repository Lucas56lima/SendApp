using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        private readonly IStoreService _service;
        public StoreController(IStoreService service)
        {
            _service = service;
        }

        [HttpPost("PostStoreAsync")]
        public async Task<IActionResult> PostStoreAsync([FromBody] Store store)
        {
            return Ok(await _service.PostStoreAsync(store));
        }

        [HttpGet("GetStoreByIdAsync")]
        public async Task<IActionResult> GetStoresByIdAsync(int id)
        {
            return Ok(await _service.GetStoresByIdAsync(id));
        }

        [HttpPut("PutStoreByIdAsync")]
        public async Task<IActionResult> PutStoreByIdAsync(int id,[FromBody] Store store)
        {
            return Ok(await _service.PutStoreByIdAsync(id,store));
        }
    }
}

