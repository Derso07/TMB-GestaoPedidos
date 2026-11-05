using tmb.pedidos.API.Services.Orders;
using tmb.pedidos.Infrastructure.Module;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CreateOrderService>();
builder.Services.AddScoped<GetOrderByIdService>();
builder.Services.AddScoped<GetOrdersService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Serve o JSON do Swagger
    app.UseSwaggerUI(); // Serve a interface do Swagger
}

// Configure the HTTP request pipeline.
app.MapControllers();

app.UseHttpsRedirection();

app.Run();