# Stage 1: Build frontend
FROM oven/bun:1 AS frontend-build
WORKDIR /app/client
COPY client/package.json client/bun.lock ./
RUN bun install --frozen-lockfile
COPY client/ .
RUN bun run build

# Stage 2: Build backend
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS api-build
WORKDIR /src
COPY server/Voltei.Api.slnx ./
COPY server/Voltei.Api/Voltei.Api.csproj Voltei.Api/
COPY server/Voltei.Api.UnitTests/Voltei.Api.UnitTests.csproj Voltei.Api.UnitTests/
COPY server/Voltei.Api.IntegrationTests/Voltei.Api.IntegrationTests.csproj Voltei.Api.IntegrationTests/
RUN dotnet restore Voltei.Api/Voltei.Api.csproj
COPY server/ .
RUN dotnet publish Voltei.Api/Voltei.Api.csproj -c Release -o /app --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
LABEL service="voltei"
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=api-build /app .
COPY --from=frontend-build /app/client/dist ./wwwroot
COPY server/certs/pass-cert.p12 /app/certs/pass-cert.p12

EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000

ENTRYPOINT ["dotnet", "Voltei.Api.dll"]
