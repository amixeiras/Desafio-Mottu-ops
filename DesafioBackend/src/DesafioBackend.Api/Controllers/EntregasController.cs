using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Api.Controllers;

[ApiController]
[Route("entregas")]
[Authorize]
public sealed class EntregasController(IDeliveryService deliveryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CriarPedido(
        CreateDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await deliveryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ConsultarEntrega), new { id }, new { identificador = id });
    }

    [HttpGet("disponiveis")]
    [ProducesResponseType(typeof(IReadOnlyList<DeliveryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarDisponiveis(CancellationToken cancellationToken)
    {
        return Ok(await deliveryService.GetAvailableAsync(cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DeliveryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Consultar(
        [FromQuery(Name = "entregador_id")] string? entregadorId,
        [FromQuery(Name = "status")] string? status,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(entregadorId))
            return Ok(await deliveryService.GetByDriverAsync(entregadorId, cancellationToken));

        if (!string.IsNullOrWhiteSpace(status))
            return Ok(await deliveryService.GetByStatusAsync(status, cancellationToken));

        return Ok(await deliveryService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarEntrega(string id, CancellationToken cancellationToken)
    {
        return Ok(await deliveryService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("{id}/aceitar")]
    public async Task<IActionResult> Aceitar(
        string id,
        AcceptDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        await deliveryService.AcceptAsync(id, request, cancellationToken);
        return Ok(new { mensagem = "Entrega aceita com sucesso." });
    }

    [HttpPut("{id}/concluir")]
    public async Task<IActionResult> Concluir(
        string id,
        AcceptDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        await deliveryService.CompleteAsync(id, request, cancellationToken);
        return Ok(new { mensagem = "Entrega concluída com sucesso." });
    }
}
