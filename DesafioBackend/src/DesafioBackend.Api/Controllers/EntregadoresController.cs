using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DesafioBackend.Api.Controllers;

[ApiController]
[Route("entregadores")]
[Authorize]
public class EntregadoresController : ControllerBase
{
    private readonly IDeliveryDriverService _driverService;

    public EntregadoresController(IDeliveryDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DeliveryDriverResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarEntregadores(CancellationToken cancellationToken)
    {
        return Ok(await _driverService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeliveryDriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(string id, CancellationToken cancellationToken)
    {
        return Ok(await _driverService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarEntregador([FromBody] CreateDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        await _driverService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("{id}/cnh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnviarFotoCnh(string id, [FromBody] UpdateCnhImageRequest request, CancellationToken cancellationToken)
    {
        await _driverService.UpdateCnhImageAsync(id, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }
}

