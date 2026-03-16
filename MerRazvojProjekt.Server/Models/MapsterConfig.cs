using Mapster;
using MerRazvojProjekt.Server.Models.Dto;
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
        }
    }
}

