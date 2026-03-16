using MerRazvojProjekt.Server.Models.Dto;
using MerRazvojProjekt.Server.Models.DTO;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace MerRazvojProjekt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<ActionResult<GetCustomerDto>> Add([FromBody] UpsertCustomerDto dto)
        {
            var result = await _customerService.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetCustomerDto>> GetById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (result is null)
                return NotFound();     
                return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<GetCustomerDto>>> GetAll([FromQuery] CustomerFilterDto query)
        {
            var result = await _customerService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _customerService.SoftDeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GetCustomerDto>> Update(int id, [FromBody] UpsertCustomerDto dto)
        {
            var result = await _customerService.UpdateAsync(id, dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("bulk-deactivate")]
        public async Task<IActionResult> BulkDeactivate([FromBody] List<int> customerIds)
        {
            var updatedCount = await _customerService.BulkDeactivateAsync(customerIds);

            return Ok(new { updatedCount });
        }

        [HttpGet("stats")]
        public async Task<ActionResult<StatsDto>> GetStats()
        {
            var result = await _customerService.GetStatsAsync();
            return Ok(result);
        }
    }
}
