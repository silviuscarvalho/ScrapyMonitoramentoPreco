using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Web.Pages;

/// <summary>
/// Consulta eventos DNS com filtros e exportação.
/// </summary>
public sealed class DnsModel : PageModel
{
    private readonly IServicoEventoDns _servicoEventoDns;

    public DnsModel(IServicoEventoDns servicoEventoDns)
    {
        _servicoEventoDns = servicoEventoDns;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroEventoDns Filtro { get; set; } = new();

    public ResultadoPaginado<EventoDnsDto> Resultado { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resultado = await _servicoEventoDns.ListarAsync(Filtro, cancellationToken);
    }

    public async Task<FileContentResult> OnGetExportarAsync(CancellationToken cancellationToken)
    {
        var csv = await _servicoEventoDns.GerarCsvAsync(Filtro, cancellationToken);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "eventos-dns.csv");
    }
}
