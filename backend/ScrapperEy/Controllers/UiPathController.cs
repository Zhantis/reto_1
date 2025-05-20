using Microsoft.AspNetCore.Mvc;
using ScrapperEy.Services;
using System.Text.Json;

namespace ScrapperEy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UiPathController : ControllerBase
    {
        private readonly UiPathService _uipathService;

        public UiPathController(UiPathService uipathService)
        {
            _uipathService = uipathService;
        }

        [HttpPost("start-job")]
        public async Task<IActionResult> StartJob([FromBody] string query)
        {
            var result = await _uipathService.StartJobAsync();
            return Ok(JsonDocument.Parse(result));
        }
    }
}
