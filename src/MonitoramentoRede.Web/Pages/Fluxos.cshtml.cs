using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Web.Pages;

/// <summary>
/// Consulta fluxos de rede com filtros e exportação.
/// </summary>
public sealed class FluxosModel : PageModel
{
    private readonly IServicoFluxoRede _servicoFluxoRede;

    public FluxosModel(IServicoFluxoRede servicoFluxoRede)
    {
        _servicoFluxoRede = servicoFluxoRede;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroFluxoRede Filtro { get; set; } = new();

    public ResultadoPaginado<FluxoRedeDto> Resultado { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resultado = await _servicoFluxoRede.ListarAsync(Filtro, cancellationToken);
    }

    public async Task<FileContentResult> OnGetExportarAsync(CancellationToken cancellationToken)
    {
        var csv = await _servicoFluxoRede.GerarCsvAsync(Filtro, cancellationToken);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "fluxos.csv");
    }
}
