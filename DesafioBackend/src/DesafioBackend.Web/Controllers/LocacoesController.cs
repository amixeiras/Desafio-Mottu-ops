using DesafioBackend.Web.Models.Entregadores;
using DesafioBackend.Web.Models.Locacoes;
using DesafioBackend.Web.Models.Motos;
using DesafioBackend.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesafioBackend.Web.Controllers;

[Authorize]
public class LocacoesController(ApiClient apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var locacoes = await apiClient.GetAsync<IReadOnlyList<LocacaoViewModel>>("locacao", cancellationToken) ?? [];
        return View(locacoes);
    }

    [HttpGet]
    public async Task<IActionResult> Nova(CancellationToken cancellationToken)
    {
        var model = new NovaLocacaoViewModel();
        await PopulateDropdownsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nova(NovaLocacaoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model, cancellationToken);
            return View(model);
        }

        var createdAt = DateTime.UtcNow;
        var dataInicio = createdAt.Date.AddDays(1);
        var dataPrevisaoTermino = dataInicio.AddDays(model.Plano);
        var dataTermino = dataPrevisaoTermino;

        var payload = new
        {
            entregador_id = model.EntregadorId.Trim(),
            moto_id = model.MotoId.Trim(),
            data_inicio = dataInicio,
            data_termino = dataTermino,
            data_previsao_termino = dataPrevisaoTermino,
            plano = model.Plano
        };

        var result = await apiClient.PostAsync("locacao", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao realizar a locação.");
            await PopulateDropdownsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Locação criada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Devolver(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var locacao = await apiClient.GetAsync<LocacaoViewModel>($"locacao/{id}", cancellationToken);
        if (locacao is null)
        {
            TempData["ErrorMessage"] = "Locação não encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(new DevolverLocacaoViewModel
        {
            Id = locacao.Identificador,
            EntregadorId = locacao.EntregadorId,
            MotoId = locacao.MotoId,
            ValorDiaria = locacao.ValorDiaria,
            DataInicio = locacao.DataInicio,
            DataPrevisaoTermino = locacao.DataPrevisaoTermino,
            DataDevolucao = DateTime.UtcNow.Date
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(DevolverLocacaoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var payload = new
        {
            data_devolucao = model.DataDevolucao
        };

        var result = await apiClient.PutAsync<object, ReturnRentalResponse>($"locacao/{model.Id}/devolucao", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao informar devolução da locação.");
            return View(model);
        }

        var total = result.Data?.ValorTotal;
        TempData["SuccessMessage"] = total.HasValue
            ? $"Data de devolução informada com sucesso! Tarifa total calculada: {total.Value:C2}"
            : "Data de devolução informada com sucesso! O valor da locação foi atualizado.";

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(NovaLocacaoViewModel model, CancellationToken cancellationToken)
    {
        var drivers = await apiClient.GetAsync<IReadOnlyList<EntregadorViewModel>>("entregadores", cancellationToken) ?? [];
        var motos = await apiClient.GetAsync<IReadOnlyList<MotoViewModel>>("motos", cancellationToken) ?? [];

        model.EntregadoresDisponiveis = drivers.Select(d => new SelectListItem
        {
            Value = d.Identificador,
            Text = $"{d.Nome} (CNH: {d.NumeroCnh} - Tipo: {d.TipoCnh})"
        });

        model.MotosDisponiveis = motos.Select(m => new SelectListItem
        {
            Value = m.Identificador,
            Text = $"{m.Modelo} - Placa: {m.Placa} (Ano: {m.Ano})"
        });
    }
}

