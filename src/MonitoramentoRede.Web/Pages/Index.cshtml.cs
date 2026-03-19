using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;

namespace MonitoramentoRede.Web.Pages;

/// <summary>
/// Exibe o dashboard principal com indicadores de rede.
/// </summary>
public sealed class IndexModel : PageModel
{
    private readonly IServicoDashboard _servicoDashboard;

    public IndexModel(IServicoDashboard servicoDashboard)
    {
        _servicoDashboard = servicoDashboard;
    }

    public DashboardResumoDto Resumo { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resumo = await _servicoDashboard.ObterResumoAsync(cancellationToken);
    }

    public async Task<IActionResult> OnGetResumoAsync(CancellationToken cancellationToken)
    {
        var resumo = await _servicoDashboard.ObterResumoAsync(cancellationToken);
        return new JsonResult(resumo);
    }

    public int CalcularPercentualDominio(int total)
    {
        var maximo = Resumo.TopDominios.Any() ? Resumo.TopDominios.Max(x => x.Total) : 1;
        return maximo == 0 ? 0 : (int)Math.Round((double)total / maximo * 100);
    }

    public int CalcularPercentualTrafego(long total)
    {
        var maximo = Resumo.TopDispositivosTrafego.Any() ? Resumo.TopDispositivosTrafego.Max(x => x.TotalBytes) : 1;
        return maximo == 0 ? 0 : (int)Math.Round((double)total / maximo * 100);
    }
}
