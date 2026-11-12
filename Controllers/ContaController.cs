using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VetCare.Models;

namespace VetCare.Controllers;

public class ContaController : Controller
{
    private static Conta? contaCadastrada;
    private readonly IConfiguration configuracao;
    private readonly PasswordHasher<Conta> senhaHasher = new();

    public ContaController(IConfiguration configuracao)
    {
        this.configuracao = configuracao;
    }

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
        if (conta is null || !EmailPermitido(email) ||
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
        if (!EmailPermitido(email))
        {
            ModelState.AddModelError(nameof(email),
                $"Somente o e-mail autorizado ({EmailAutorizado()}) pode criar uma conta.");
        }

        if (senha.Length < 6)
        {
            ModelState.AddModelError(nameof(senha), "A senha deve ter pelo menos 6 caracteres.");
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

    private bool EmailPermitido(string email) =>
        string.Equals(email.Trim(), EmailAutorizado(), StringComparison.OrdinalIgnoreCase);

    private string EmailAutorizado() =>
        configuracao["Autenticacao:EmailPermitido"] ?? "admin@vetcare.com";

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home")!;
}
