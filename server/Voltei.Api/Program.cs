using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Voltei.Api.Configuration;
using Voltei.Api.Data;
using Voltei.Api.Services;
using Voltei.Api.Services.Wallet;

var builder = WebApplication.CreateBuilder(args);

// Load local secrets (gitignored)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

// Database — env var DATABASE_URL tem prioridade sobre appsettings
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Authentication — env var JWT_SECRET tem prioridade
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]!;
// Sobrescrever config para que TokenService use o mesmo secret
builder.Configuration["Jwt:Secret"] = jwtSecret;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

// Configuration
builder.Services.Configure<GoogleWalletOptions>(builder.Configuration.GetSection("GoogleWallet"));
builder.Services.Configure<AppleWalletOptions>(builder.Configuration.GetSection("AppleWallet"));

// Services
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<GoogleWalletService>();
builder.Services.AddSingleton<AppleWalletService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<CheckinService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Auto-migrate database (skip in test environment — tests use EnsureCreated)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Servir frontend estático (wwwroot) em produção
if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}

app.Run();
