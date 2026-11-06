FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["tmb.pedidos.api/tmb.pedidos.API.csproj", "tmb.pedidos.api/"]
COPY ["tmb.pedidos.Domain/tmb.pedidos.Domain.csproj", "tmb.pedidos.Domain/"]
COPY ["tmb.pedidos.Infrastructure/tmb.pedidos.Infrastructure.csproj", "tmb.pedidos.Infrastructure/"]

RUN dotnet restore "tmb.pedidos.api/tmb.pedidos.API.csproj"

COPY . .

WORKDIR "/src/tmb.pedidos.api"

RUN dotnet publish "tmb.pedidos.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "tmb.pedidos.API.dll"]
