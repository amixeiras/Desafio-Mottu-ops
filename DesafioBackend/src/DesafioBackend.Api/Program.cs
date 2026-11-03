using DesafioBackend.Api.Middlewares;
using DesafioBackend.Application;
using DesafioBackend.Infrastructure;
using DesafioBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Enable legacy timestamp behavior for Npgsql to ensure DateTime without Kind=Utc doesn't throw
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key deve possuir pelo menos 32 caracteres.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    var token = authHeader.Trim();
                    while (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring(7).Trim();
                    }
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Manutenção de Motos",
        Version = "1.0.0",
        Description = "API do Desafio Backend Mottu para gerenciamento de motos, entregadores e locações."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticação JWT via cabeçalho Authorization. Insira 'Bearer {seu_token}' ou apenas o token no campo abaixo.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Clean Architecture / DDD Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Automatically apply migrations on startup if relational database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<AppDbContext>();

    try
    {
        if (context.Database.IsRelational())
        {
            logger.LogInformation("Aplicando migrações pendentes no banco de dados...");
            // Espera com retentativas caso o banco esteja inicializando no docker-compose
            var retries = 5;
            while (retries > 0)
            {
                try
                {
                    context.Database.Migrate();
                    logger.LogInformation("Migrações aplicadas com sucesso.");
                    break;
                }
                catch (Exception ex)
                {
                    retries--;
                    logger.LogWarning(ex, "Tentativa de conexão com o banco falhou. Retentativas restantes: {Retries}", retries);
                    if (retries == 0) throw;
                    Thread.Sleep(3000);
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falha ao aplicar migrações automáticas no banco de dados.");
    }
}

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger in all environments for ease of testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Manutenção de Motos v1.0.0");
    c.RoutePrefix = string.Empty; // Define o Swagger na raiz "/"
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// For Integration Tests
public partial class Program { }
