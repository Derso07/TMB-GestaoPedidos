using Microsoft.EntityFrameworkCore;
using tmb.pedidos.API.Services.Orders;
using tmb.pedidos.Infrastructure.Module;
using tmb.pedidos.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CreateOrderService>();
builder.Services.AddScoped<GetOrderByIdService>();
builder.Services.AddScoped<GetOrdersService>();

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173" )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Sistema de Gestão de Pedidos",
        Description = "Desafio técnico TMB",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ederson Henrique",
            Email = "edersonhenrique2016@gmail.com"
        }
    });
});

var app = builder.Build();

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<PedidosDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Serve o JSON do Swagger
    app.UseSwaggerUI(); // Serve a interface do Swagger
}

// Configure the HTTP request pipeline.
app.MapControllers();

app.UseHttpsRedirection();

app.Run();