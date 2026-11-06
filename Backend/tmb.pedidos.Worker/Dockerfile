FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["tmb.pedidos.Worker/tmb.pedidos.Worker.csproj", "tmb.pedidos.Worker/"]
COPY ["tmb.pedidos.Domain/tmb.pedidos.Domain.csproj", "tmb.pedidos.Domain/"]
COPY ["tmb.pedidos.Infrastructure/tmb.pedidos.Infrastructure.csproj", "tmb.pedidos.Infrastructure/"]

RUN dotnet restore "tmb.pedidos.Worker/tmb.pedidos.Worker.csproj"

COPY . .

WORKDIR "/src/tmb.pedidos.Worker"

RUN dotnet publish "tmb.pedidos.Worker.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "tmb.pedidos.Worker.dll"]
