using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Voltei.Api.Data;

namespace Voltei.Api.IntegrationTests.Fixtures;

public class VolteiApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string BaseConnectionString =
        "Host=localhost;Port=5434;Username=voltei;Password=voltei-dev";

    private readonly string _dbName = $"voltei_test_{Guid.NewGuid():N}"[..40];

    private string TestConnectionString => $"{BaseConnectionString};Database={_dbName}";

    public async Task InitializeAsync()
    {
        // Criar database de teste isolado
        await using var conn = new NpgsqlConnection($"{BaseConnectionString};Database=voltei");
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE \"{_dbName}\"";
        await cmd.ExecuteNonQueryAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();

        // Dropar database de teste
        try
        {
            await using var conn = new NpgsqlConnection($"{BaseConnectionString};Database=voltei");
            await conn.OpenAsync();

            // Forçar desconexão de qualquer sessão ativa
            await using var terminateCmd = conn.CreateCommand();
            terminateCmd.CommandText = $"""
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{_dbName}' AND pid <> pg_backend_pid()
                """;
            await terminateCmd.ExecuteNonQueryAsync();

            await using var dropCmd = conn.CreateCommand();
            dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{_dbName}\"";
            await dropCmd.ExecuteNonQueryAsync();
        }
        catch
        {
            // Best effort cleanup
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Apontar ContentRoot para o diretório do projeto API
        // para que os certs e appsettings.Local.json sejam encontrados.
        var apiProjectDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Voltei.Api"));
        if (Directory.Exists(apiProjectDir))
        {
            builder.UseContentRoot(apiProjectDir);
        }

        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext/EF/Npgsql registrations from production config
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                    d.ServiceType.FullName?.Contains("Npgsql") == true)
                .ToList();
            foreach (var d in descriptorsToRemove) services.Remove(d);

            // Add DbContext with test PostgreSQL database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(TestConnectionString);
            });

            // Create schema
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
