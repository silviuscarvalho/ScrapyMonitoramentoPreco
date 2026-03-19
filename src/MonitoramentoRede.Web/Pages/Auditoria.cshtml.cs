using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Web.Pages;

/// <summary>
/// Exibe logs de login e operações administrativas.
/// </summary>
public sealed class AuditoriaModel : PageModel
{
    private readonly IServicoAuditoria _servicoAuditoria;

    public AuditoriaModel(IServicoAuditoria servicoAuditoria)
    {
        _servicoAuditoria = servicoAuditoria;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroAuditoria Filtro { get; set; } = new();

    public ResultadoPaginado<LogAuditoriaDto> Resultado { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resultado = await _servicoAuditoria.ListarAsync(Filtro, cancellationToken);
    }
}
