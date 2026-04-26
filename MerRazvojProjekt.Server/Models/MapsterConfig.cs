using Mapster;
using MerRazvojProjekt.Server.Models.Dto.CarDto;
using MerRazvojProjekt.Server.Models.Dto.CustomerDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;
using System.Reflection;

namespace MerRazvojProjekt.Server.Models
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Customer, GetCustomerDto>
                .NewConfig();

            TypeAdapterConfig<RequestLog, RequestLogDto>
                .NewConfig();

            TypeAdapterConfig<Car, GetCarDto>
                .NewConfig();
        }
    }
}

