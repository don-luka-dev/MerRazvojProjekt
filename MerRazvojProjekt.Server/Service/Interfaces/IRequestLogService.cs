using MerRazvojProjekt.Server.Models.Dto;

namespace MerRazvojProjekt.Server.Service.Interfaces
{
    public interface IRequestLogService
    {
        Task<List<RequestLogDto>> GetAllAsync();

    }
}
