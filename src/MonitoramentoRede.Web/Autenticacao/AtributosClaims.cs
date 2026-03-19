using System.Security.Claims;

namespace MonitoramentoRede.Web.Autenticacao;

public static class AtributosClaims
{
    public const string IdUsuario = "IdUsuario";

    public static long ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirstValue(IdUsuario);
        return long.TryParse(valor, out var id) ? id : 0;
    }
}
