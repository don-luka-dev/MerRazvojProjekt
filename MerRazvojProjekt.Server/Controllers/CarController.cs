using MerRazvojProjekt.Server.Models.Dto.CarDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MerRazvojProjekt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController(ICarService carService) : ControllerBase
    {
        private readonly ICarService _carService = carService;

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] UpsertCarDto dto)
        {
            var result = await _carService.AddCarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetCarDto>> GetById(int id)
        {
            var result = await _carService.GetCarByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<GetCarDto>>> GetAll([FromQuery] CarFilterDto query)
        {
            var result = await _carService.GetAllCarsAsync(query);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> SoftDelete(int id)
        {
            await _carService.SoftDeleteCarAsync(id);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GetCarDto>> Update(int id, [FromBody] UpsertCarDto dto)
        {
            var result = await _carService.UpdateCarAsync(id, dto);
            return Ok(result);
        }

        [HttpPost("bulk-deactivate")]
        public async Task<IActionResult> BulkDeactivate([FromBody] List<int> carIds)
        {
            var result = await _carService.BulkDeactivateAsync(carIds);
            return Ok(new { deactivatedCount = result });
        }
    }
}
