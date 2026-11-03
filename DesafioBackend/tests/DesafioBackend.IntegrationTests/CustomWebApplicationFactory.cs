using DesafioBackend.Application.Common.Interfaces;
using DesafioBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace DesafioBackend.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Set RabbitMQ Host to empty so it doesn't try to connect to localhost during test runs
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMQ:Host"] = ""
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove real DbContext
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Add InMemory DbContext for isolation
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Mock message bus for integration tests so no broker is needed
            var busDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageBus));
            if (busDescriptor != null)
            {
                services.Remove(busDescriptor);
            }

            var mockBus = new Mock<IMessageBus>();
            services.AddSingleton(mockBus.Object);
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                "development-only-change-this-key-before-production-123456")),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims:
            [
                new Claim(ClaimTypes.Name, "integration-test"),
                new Claim(ClaimTypes.Role, "Administrator")
            ],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            new JwtSecurityTokenHandler().WriteToken(token));
    }
}
