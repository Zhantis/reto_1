using Microsoft.AspNetCore.Mvc;
using ScrapperEy.Models;
using ScrapperEy.Services;

namespace ScrapperEy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OffShoreController : ControllerBase
    {
        private readonly ILogger<OffShoreController> _logger;
        private readonly OffShoreService _offShoreService;

        public OffShoreController(ILogger<OffShoreController> logger, OffShoreService offshoreService)
        {
            _logger = logger;
            _offShoreService = offshoreService;
        }

        [HttpGet(Name = "/listarOffShoreEntities")]
        public async Task<IActionResult> ListarOffShoreEntities([FromQuery] string querySearch)
        {
            List<OffShoreEntity> entities;
            try
            {
                entities = await _offShoreService.ListarOffShoreEntitiesByName(querySearch.Trim());
                return Ok(new { success = true, data = entities });
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR en controller", ex);
                //return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
