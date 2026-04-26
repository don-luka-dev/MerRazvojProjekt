using MerRazvojProjekt.Server.Models.Dto.CarDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;

namespace MerRazvojProjekt.Server.Service.Interfaces
{
    public interface ICarService
    {
        Task<PagedResultDto<GetCarDto>> GetAllCarsAsync(CarFilterDto dto);
        Task<GetCarDto> GetCarByIdAsync(int id);
        Task<GetCarDto> AddCarAsync(UpsertCarDto dto);
        Task<GetCarDto> UpdateCarAsync(int id, UpsertCarDto dto);
        Task<bool> SoftDeleteCarAsync(int id);
        Task<int> BulkDeactivateAsync(IEnumerable<int> carIds);

    }
}
