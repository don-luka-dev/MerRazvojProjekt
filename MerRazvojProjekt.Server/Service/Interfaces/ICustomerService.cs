
using MerRazvojProjekt.Server.Models.Dto;
using MerRazvojProjekt.Server.Models.DTO;

namespace MerRazvojProjekt.Server.Service.Interfaces
{
    public interface ICustomerService
    {
        Task<GetCustomerDto?> GetByIdAsync(int id);
        Task<GetCustomerDto> AddAsync(UpsertCustomerDto dto);
        Task<GetCustomerDto?> UpdateAsync(int id, UpsertCustomerDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<PagedResultDto<GetCustomerDto>> GetAllAsync(CustomerFilterDto dto);
        Task<int> BulkDeactivateAsync(IEnumerable<int> customerIds);
        Task<StatsDto> GetStatsAsync();
    }
}
