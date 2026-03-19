using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MonitoramentoRede.Web.Pages.Conta;

[AllowAnonymous]
public sealed class AcessoNegadoModel : PageModel
{
    public void OnGet()
    {
    }
}
