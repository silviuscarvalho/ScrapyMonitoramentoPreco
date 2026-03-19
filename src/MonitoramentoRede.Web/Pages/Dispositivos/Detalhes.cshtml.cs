using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;

namespace MonitoramentoRede.Web.Pages.Dispositivos;

/// <summary>
/// Exibe a visão detalhada de um dispositivo monitorado.
/// </summary>
public sealed class DetalhesModel : PageModel
{
    private readonly IServicoDispositivo _servicoDispositivo;

    public DetalhesModel(IServicoDispositivo servicoDispositivo)
    {
        _servicoDispositivo = servicoDispositivo;
    }

    public DispositivoDetalhesDto? Detalhes { get; private set; }

    public async Task OnGetAsync(long id, CancellationToken cancellationToken)
    {
        Detalhes = await _servicoDispositivo.ObterDetalhesAsync(id, cancellationToken);
    }
}
