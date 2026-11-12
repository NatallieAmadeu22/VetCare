using System.Security.Claims;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VetCare.Models;

namespace VetCare.Controllers;

public class ContaController : Controller
{
    private static Conta? contaCadastrada;
    private readonly PasswordHasher<Conta> senhaHasher = new();

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string senha, string? returnUrl = null)
    {
        var conta = contaCadastrada;
        if (conta is null ||
            !string.Equals(email.Trim(), conta.Email, StringComparison.OrdinalIgnoreCase) ||
            senhaHasher.VerifyHashedPassword(conta, conta.SenhaHash, senha) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        await Entrar(conta.Email);
        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    public IActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastro(string email, string senha, string confirmarSenha)
    {
        if (!EmailValido(email))
        {
            ModelState.AddModelError(nameof(email), "Informe um e-mail válido.");
        }

        if (senha.Length < 8 ||
            !senha.Any(char.IsUpper) ||
            !senha.Any(char.IsLower) ||
            !senha.Any(char.IsDigit) ||
            senha.All(char.IsLetterOrDigit))
        {
            ModelState.AddModelError(
                nameof(senha),
                "A senha deve ter pelo menos 8 caracteres, incluindo maiúscula, minúscula, número e símbolo.");
        }

        if (senha != confirmarSenha)
        {
            ModelState.AddModelError(nameof(confirmarSenha), "As senhas não conferem.");
        }

        if (contaCadastrada is not null)
        {
            ModelState.AddModelError(string.Empty, "Este acesso já foi cadastrado.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        var conta = new Conta { Email = email.Trim().ToLowerInvariant() };
        conta.SenhaHash = senhaHasher.HashPassword(conta, senha);
        contaCadastrada = conta;

        TempData["Mensagem"] = "Cadastro realizado. Agora entre com seu e-mail e senha.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task Entrar(string email)
    {
        var identidade = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, email)],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade));
    }

    private static bool EmailValido(string email)
    {
        try
        {
            var endereco = new MailAddress(email.Trim());
            return string.Equals(endereco.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home")!;
}
