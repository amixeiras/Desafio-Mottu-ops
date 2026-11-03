using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DesafioBackend.Api.Controllers;

[ApiController]
[Route("motos")]
[Authorize]
public class MotosController : ControllerBase
{
    private readonly IMotorcycleService _motorcycleService;

    public MotosController(IMotorcycleService motorcycleService)
    {
        _motorcycleService = motorcycleService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarMoto([FromBody] CreateMotorcycleRequest request, CancellationToken cancellationToken)
    {
        await _motorcycleService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MotorcycleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarMotos([FromQuery] string? placa, CancellationToken cancellationToken)
    {
        var result = await _motorcycleService.GetAllAsync(placa, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MotorcycleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMotoPorId(string id, CancellationToken cancellationToken)
    {
        var result = await _motorcycleService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/placa")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ModificarPlaca(string id, [FromBody] UpdateMotorcyclePlateRequest request, CancellationToken cancellationToken)
    {
        await _motorcycleService.UpdatePlateAsync(id, request, cancellationToken);
        return Ok(new MessageResponse("Placa modificada com sucesso"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoverMoto(string id, CancellationToken cancellationToken)
    {
        await _motorcycleService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}

