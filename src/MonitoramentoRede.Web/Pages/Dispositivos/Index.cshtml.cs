using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Aplicacao.Dtos;

namespace MonitoramentoRede.Web.Pages.Dispositivos;

/// <summary>
/// Lista dispositivos monitorados com paginação server-side.
/// </summary>
public sealed class IndexModel : PageModel
{
    private readonly IServicoDispositivo _servicoDispositivo;

    public IndexModel(IServicoDispositivo servicoDispositivo)
    {
        _servicoDispositivo = servicoDispositivo;
    }

    [BindProperty(SupportsGet = true)]
    public FiltroDispositivo Filtro { get; set; } = new();

    public ResultadoPaginado<DispositivoDto> Resultado { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Resultado = await _servicoDispositivo.ListarAsync(Filtro, cancellationToken);
    }
}
