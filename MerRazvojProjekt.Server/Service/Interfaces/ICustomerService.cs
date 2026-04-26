using MerRazvojProjekt.Server.Models.Dto.CustomerDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;
using MerRazvojProjekt.Server.Models.Dto.StatisticDto;

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
