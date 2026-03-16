using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MerRazvojProjekt.Server.Controllers
{
    [ApiController]
    [Route("api/request-logs")]
    public class RequestLogsController : ControllerBase
    {
        private readonly IRequestLogService _requestLogService;

        public RequestLogsController(IRequestLogService requestLogService)
        {
            _requestLogService = requestLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _requestLogService.GetAllAsync();
            return Ok(logs);
        }
    }
}
