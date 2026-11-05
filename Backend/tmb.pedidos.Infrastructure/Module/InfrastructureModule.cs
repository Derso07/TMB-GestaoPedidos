using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Infrastructure.Persistence;
using tmb.pedidos.Infrastructure.Repositories;

namespace tmb.pedidos.Infrastructure.Module;

    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);

            services.AddScoped<IOrderService, OrderRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<PedidosDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
