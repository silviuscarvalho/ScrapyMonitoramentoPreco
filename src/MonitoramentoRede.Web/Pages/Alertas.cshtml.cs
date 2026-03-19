using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Web.Autenticacao;

namespace MonitoramentoRede.Web.Pages;

/// <summary>
/// Lista e permite resolver alertas de rede.
/// </summary>
public sealed class AlertasModel : PageModel
{
    private readonly IServicoAlertaRede _servicoAlertaRede;

    public AlertasModel(IServicoAlertaRede servicoAlertaRede)
    {
        _servicoAlertaRede = servicoAlertaRede;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroAlertaRede Filtro { get; set; } = new();

    public ResultadoPaginado<AlertaRedeDto> Resultado { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resultado = await _servicoAlertaRede.ListarAsync(Filtro, cancellationToken);
    }

    public async Task<IActionResult> OnPostResolverAsync(long id, string observacaoOperador, CancellationToken cancellationToken)
    {
        if (!(User.IsInRole("Administrador") || User.IsInRole("Operador")))
        {
            return Forbid();
        }

        await _servicoAlertaRede.ResolverAsync(new ResolverAlertaDto
        {
            Id = id,
            ObservacaoOperador = observacaoOperador,
            UsuarioId = User.ObterIdUsuario(),
            UsuarioNome = User.Identity?.Name ?? "Operador"
        }, cancellationToken);

        return RedirectToPage(new { Filtro.Pagina, Filtro.Tipo, Filtro.Severidade, Filtro.Status });
    }
}
