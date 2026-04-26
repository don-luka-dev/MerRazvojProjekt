using MerRazvojProjekt.Server.Models.Dto.MiscDto;

namespace MerRazvojProjekt.Server.Service.Interfaces
{
    public interface IRequestLogService
    {
        Task<List<RequestLogDto>> GetAllAsync();

    }
}
