using MerRazvojProjekt.Server.Models.Dto.CustomerDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;
using MerRazvojProjekt.Server.Models.Dto.StatisticDto;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MerRazvojProjekt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;

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
            await _customerService.SoftDeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GetCustomerDto>> Update(int id, [FromBody] UpsertCustomerDto dto)
        {
            var result = await _customerService.UpdateAsync(id, dto);
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
