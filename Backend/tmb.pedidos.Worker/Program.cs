using tmb.pedidos.Infrastructure.Module;
using tmb.pedidos.Worker;
using tmb.pedidos.Worker.Outbox;

var builder =
    Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddScoped<OrderProcessingService>();

builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddHostedService<OutboxPublisher>();

var host = builder.Build();

host.Run();