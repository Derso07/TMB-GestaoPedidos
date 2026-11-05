using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Infrastructure.Module;
using tmb.pedidos.Infrastructure.Persistence;
using tmb.pedidos.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<OrderProcessingService>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

var host = builder.Build();
host.Run();