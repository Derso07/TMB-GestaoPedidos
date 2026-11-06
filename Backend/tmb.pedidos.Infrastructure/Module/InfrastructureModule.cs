using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Domain.Interfaces.MessagingPublish;
using tmb.pedidos.Infrastructure.Messaging;
using tmb.pedidos.Infrastructure.Outbox;
using tmb.pedidos.Infrastructure.Persistence;
using tmb.pedidos.Infrastructure.Repositories;

namespace tmb.pedidos.Infrastructure.Module;

    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);
            
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "A connection string DefaultConnection não foi configurada.");
            }
            services.AddDbContext<PedidosDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            services.AddScoped<IOrderService, OrderRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<
                IMessagePublisher,
                RabbitMqPublisher>();

            return services;
        }
    }