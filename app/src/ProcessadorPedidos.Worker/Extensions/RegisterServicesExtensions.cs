using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessadorPedidos.Application.Ouvintes;
using ProcessadorPedidos.Infrastructure.Ouvintes;

namespace ProcessadorPedidos.Worker.Extensions
{
    public static class RegisterServicesExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services.AddSingleton<IOuvintePedidos, OuvintePedidos>();
        }
    }
}