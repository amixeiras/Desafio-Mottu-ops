using DesafioBackend.Web.Models.Admin;
using DesafioBackend.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Web.Controllers;

[Authorize]
public sealed class AdminController(ApiClient apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var motorcycles = await apiClient.GetAsync<IReadOnlyList<MotorcycleItem>>("motos", cancellationToken) ?? [];
            var drivers = await apiClient.GetAsync<IReadOnlyList<DriverItem>>("entregadores", cancellationToken) ?? [];
            var rentals = await apiClient.GetAsync<IReadOnlyList<RentalItem>>("locacao", cancellationToken) ?? [];
            var availableDeliveries = await apiClient.GetAsync<IReadOnlyList<DeliveryItem>>("entregas/disponiveis", cancellationToken) ?? [];
            var acceptedDeliveries = await apiClient.GetAsync<IReadOnlyList<DeliveryItem>>("entregas?status=Accepted", cancellationToken) ?? [];
            var completedDeliveries = await apiClient.GetAsync<IReadOnlyList<DeliveryItem>>("entregas?status=Completed", cancellationToken) ?? [];

            var activeRentals = rentals.Where(r => r.DataDevolucao == null).ToList();
            var activeRentalByDriverId = activeRentals
                .GroupBy(r => r.EntregadorId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var eligibleDrivers = drivers
                .Where(d => activeRentalByDriverId.ContainsKey(d.Identificador))
                .Select(d => new EligibleDriverItem(
                    d.Identificador,
                    d.Nome,
                    d.NumeroCnh,
                    activeRentalByDriverId[d.Identificador].MotoId))
                .ToList();

            return View(new AdminDashboardViewModel
            {
                Motorcycles = motorcycles,
                Drivers = drivers,
                Rentals = rentals,
                AvailableDeliveries = availableDeliveries,
                AcceptedDeliveries = acceptedDeliveries,
                CompletedDeliveries = completedDeliveries,
                EligibleDrivers = eligibleDrivers
            });
        }
        catch (HttpRequestException)
        {
            return View(new AdminDashboardViewModel
            {
                ErrorMessage = "Não foi possível consultar a API. Verifique se a API está em execução."
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VincularEntrega(string entregaId, string entregadorId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entregaId) || string.IsNullOrWhiteSpace(entregadorId))
        {
            TempData["ErrorMessage"] = "Selecione um entregador válido com locação ativa para vincular à entrega.";
            return RedirectToAction(nameof(Index));
        }

        var payload = new { entregador_id = entregadorId.Trim() };
        var result = await apiClient.PostAsync($"entregas/{entregaId}/aceitar", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Erro ao vincular entrega ao entregador.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"Entrega vinculada com sucesso ao entregador '{entregadorId}'!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConcluirEntrega(string entregaId, string entregadorId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entregaId) || string.IsNullOrWhiteSpace(entregadorId))
        {
            TempData["ErrorMessage"] = "Dados inválidos para conclusão da entrega.";
            return RedirectToAction(nameof(Index));
        }

        var payload = new { entregador_id = entregadorId.Trim() };
        var result = await apiClient.PutAsync($"entregas/{entregaId}/concluir", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Erro ao concluir entrega.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Entrega concluída com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarEntrega(string pedidoId, string descricao, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(pedidoId) || string.IsNullOrWhiteSpace(descricao))
        {
            TempData["ErrorMessage"] = "O identificador do pedido e a descrição são obrigatórios.";
            return RedirectToAction(nameof(Index));
        }

        var payload = new { pedido_id = pedidoId.Trim(), descricao = descricao.Trim() };
        var result = await apiClient.PostAsync("entregas", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Erro ao registrar pedido de entrega.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"Pedido '{pedidoId}' registrado e disponível para os entregadores na plataforma!";
        return RedirectToAction(nameof(Index));
    }
}
