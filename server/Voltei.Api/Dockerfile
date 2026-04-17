FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copiar solution e csproj primeiro (cache de restore)
COPY Voltei.Api.slnx ./
COPY Voltei.Api/Voltei.Api.csproj Voltei.Api/
COPY Voltei.Api.UnitTests/Voltei.Api.UnitTests.csproj Voltei.Api.UnitTests/
COPY Voltei.Api.IntegrationTests/Voltei.Api.IntegrationTests.csproj Voltei.Api.IntegrationTests/
RUN dotnet restore Voltei.Api/Voltei.Api.csproj

# Copiar todo o código e publicar
COPY . .
RUN dotnet publish Voltei.Api/Voltei.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Copiar certs do Apple Wallet (se existirem)
COPY --from=build /app .

# .NET 10 usa porta 8080 por padrão em containers
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Voltei.Api.dll"]
