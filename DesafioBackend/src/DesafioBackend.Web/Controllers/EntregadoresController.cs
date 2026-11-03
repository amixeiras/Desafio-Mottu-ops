using DesafioBackend.Web.Models.Entregadores;
using DesafioBackend.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Web.Controllers;

[Authorize]
public class EntregadoresController(ApiClient apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var entregadores = await apiClient.GetAsync<IReadOnlyList<EntregadorViewModel>>("entregadores", cancellationToken) ?? [];
        return View(entregadores);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View(new CadastrarEntregadorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(CadastrarEntregadorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string base64Image;
        try
        {
            base64Image = await ConvertFileToBase64Async(model.FotoCnh);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(model.FotoCnh), ex.Message);
            return View(model);
        }

        var payload = new
        {
            identificador = model.Identificador.Trim(),
            nome = model.Nome.Trim(),
            cnpj = model.Cnpj.Trim(),
            data_nascimento = model.DataNascimento,
            numero_cnh = model.NumeroCnh.Trim(),
            tipo_cnh = model.TipoCnh.Trim().ToUpperInvariant(),
            imagem_cnh = base64Image
        };

        var result = await apiClient.PostAsync("entregadores", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao cadastrar entregador.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Entregador '{model.Nome}' cadastrado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult EnviarFoto(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        return View(new EnviarFotoCnhViewModel { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarFoto(EnviarFotoCnhViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string base64Image;
        try
        {
            base64Image = await ConvertFileToBase64Async(model.FotoCnh);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(model.FotoCnh), ex.Message);
            return View(model);
        }

        var payload = new
        {
            imagem_cnh = base64Image
        };

        var result = await apiClient.PostAsync($"entregadores/{model.Id}/cnh", payload, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao enviar imagem da CNH.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Foto da CNH atualizada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    private static async Task<string> ConvertFileToBase64Async(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            throw new InvalidOperationException("Selecione um arquivo válido.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".png" && extension != ".bmp")
        {
            throw new InvalidOperationException("Formato inválido. Apenas imagens .PNG ou .BMP são aceitas.");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }
}

