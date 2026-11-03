using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DesafioBackend.Application.DTOs;
using Xunit;

namespace DesafioBackend.IntegrationTests;

public class MotosEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MotosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostMotos_WithValidData_Returns201Created()
    {
        // Arrange
        var request = new CreateMotorcycleRequest("moto_test_1", 2024, "Sport", "TST-1111");

        // Act
        var response = await _client.PostAsJsonAsync("/motos", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostMotos_WhenDuplicatePlate_Returns400BadRequest()
    {
        // Arrange
        var request1 = new CreateMotorcycleRequest("moto_test_2", 2024, "Sport", "DUP-2222");
        var request2 = new CreateMotorcycleRequest("moto_test_3", 2023, "Sport", "DUP-2222");

        // Act
        await _client.PostAsJsonAsync("/motos", request1);
        var response2 = await _client.PostAsJsonAsync("/motos", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        var body = await response2.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("mensagem", out _));
    }

    [Fact]
    public async Task GetMotos_ReturnsListOfMotos()
    {
        // Arrange
        var request = new CreateMotorcycleRequest("moto_test_get", 2024, "Sport", "GET-3333");
        await _client.PostAsJsonAsync("/motos", request);

        // Act
        var response = await _client.GetAsync("/motos?placa=GET-3333");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var motos = await response.Content.ReadFromJsonAsync<List<MotorcycleResponse>>();
        Assert.NotNull(motos);
        Assert.Contains(motos, m => m.Placa == "GET-3333");
    }

    [Fact]
    public async Task PutMotoPlaca_UpdatesPlateSuccessfully()
    {
        // Arrange
        var request = new CreateMotorcycleRequest("moto_test_put", 2024, "Sport", "OLD-4444");
        await _client.PostAsJsonAsync("/motos", request);

        var updateRequest = new UpdateMotorcyclePlateRequest("NEW-4444");

        // Act
        var response = await _client.PutAsJsonAsync("/motos/moto_test_put/placa", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<MessageResponse>();
        Assert.NotNull(body);
        Assert.Equal("Placa modificada com sucesso", body.Mensagem);
    }
}

