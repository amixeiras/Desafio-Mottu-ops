using DesafioBackend.Web.Models.Motos;
using DesafioBackend.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Web.Controllers;

[Authorize]
public class MotosController(ApiClient apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? placa, CancellationToken cancellationToken)
    {
        var path = string.IsNullOrWhiteSpace(placa) ? "motos" : $"motos?placa={Uri.EscapeDataString(placa.Trim())}";
        var motos = await apiClient.GetAsync<IReadOnlyList<MotoViewModel>>(path, cancellationToken) ?? [];
        ViewBag.FiltroPlaca = placa;
        return View(motos);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View(new CadastrarMotoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(CadastrarMotoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var payload = new
        {
            identificador = model.Identificador.Trim(),
            ano = model.Ano,
            modelo = model.Modelo.Trim(),
            placa = model.Placa.Trim()
        };

        var result = await apiClient.PostAsync("motos", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao cadastrar moto.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Moto '{model.Identificador}' cadastrada com sucesso! Evento publicado via RabbitMQ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditarPlaca(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var moto = await apiClient.GetAsync<MotoViewModel>($"motos/{id}", cancellationToken);
        if (moto is null)
        {
            TempData["ErrorMessage"] = "Moto não encontrada.";
            return RedirectToAction(nameof(Index));
        }

        return View(new EditarPlacaViewModel
        {
            Id = moto.Identificador,
            Placa = moto.Placa
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPlaca(EditarPlacaViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var payload = new
        {
            placa = model.Placa.Trim()
        };

        var result = await apiClient.PutAsync($"motos/{model.Id}/placa", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao alterar placa da moto.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Placa atualizada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var result = await apiClient.DeleteAsync($"motos/{id}", cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Erro ao remover a moto.";
        }
        else
        {
            TempData["SuccessMessage"] = "Moto removida com sucesso!";
        }

        return RedirectToAction(nameof(Index));
    }
}

