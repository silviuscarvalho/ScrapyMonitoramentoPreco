using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Web.Autenticacao;

namespace MonitoramentoRede.Web.Pages.Conta;

[AllowAnonymous]
public sealed class LoginModel : PageModel
{
    private readonly IServicoUsuario _servicoUsuario;
    private readonly IServicoAuditoria _servicoAuditoria;

    public LoginModel(IServicoUsuario servicoUsuario, IServicoAuditoria servicoAuditoria)
    {
        _servicoUsuario = servicoUsuario;
        _servicoAuditoria = servicoAuditoria;
    }

    [BindProperty]
    public LoginDto Entrada { get; set; } = new();

    public string? MensagemErro { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var usuario = await _servicoUsuario.AutenticarAsync(Entrada, cancellationToken);
        if (usuario is null)
        {
            MensagemErro = "Credenciais inválidas.";
            await _servicoAuditoria.RegistrarAsync(null, Entrada.Login, "Login", "Conta", "Falha de autenticação", false, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil),
            new(ClaimTypes.NameIdentifier, usuario.Login),
            new(AtributosClaims.IdUsuario, usuario.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        await _servicoAuditoria.RegistrarAsync(usuario.Id, usuario.Nome, "Login", "Conta", "Login efetuado", true, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostSairAsync(CancellationToken cancellationToken)
    {
        await _servicoAuditoria.RegistrarAsync(User.ObterIdUsuario(), User.Identity?.Name ?? "Usuário", "Logout", "Conta", "Logout efetuado", true, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage();
    }
}
