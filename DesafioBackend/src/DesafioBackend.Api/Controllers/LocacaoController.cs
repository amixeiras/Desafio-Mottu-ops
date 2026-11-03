using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DesafioBackend.Api.Controllers;

[ApiController]
[Route("locacao")]
[Authorize]
public class LocacaoController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public LocacaoController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RentalResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarLocacoes(CancellationToken cancellationToken)
    {
        return Ok(await _rentalService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlugarMoto([FromBody] CreateRentalRequest request, CancellationToken cancellationToken)
    {
        await _rentalService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RentalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarLocacaoPorId(string id, CancellationToken cancellationToken)
    {
        var result = await _rentalService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/devolucao")]
    [ProducesResponseType(typeof(ReturnRentalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InformarDevolucao(string id, [FromBody] ReturnRentalRequest request, CancellationToken cancellationToken)
    {
        var total = await _rentalService.InformReturnDateAsync(id, request, cancellationToken);
        return Ok(new ReturnRentalResponse("Data de devolução informada com sucesso", total));
    }
}

