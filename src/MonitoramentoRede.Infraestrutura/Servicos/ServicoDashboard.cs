using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Consolida métricas do dashboard principal.
/// </summary>
public sealed class ServicoDashboard : IServicoDashboard
{
    private readonly IDispositivoRepositorio _dispositivoRepositorio;
    private readonly IEventoDnsRepositorio _eventoDnsRepositorio;
    private readonly IFluxoRedeRepositorio _fluxoRedeRepositorio;
    private readonly IAlertaRedeRepositorio _alertaRedeRepositorio;
    private readonly IAuditoriaRepositorio _auditoriaRepositorio;

    public ServicoDashboard(
        IDispositivoRepositorio dispositivoRepositorio,
        IEventoDnsRepositorio eventoDnsRepositorio,
        IFluxoRedeRepositorio fluxoRedeRepositorio,
        IAlertaRedeRepositorio alertaRedeRepositorio,
        IAuditoriaRepositorio auditoriaRepositorio)
    {
        _dispositivoRepositorio = dispositivoRepositorio;
        _eventoDnsRepositorio = eventoDnsRepositorio;
        _fluxoRedeRepositorio = fluxoRedeRepositorio;
        _alertaRedeRepositorio = alertaRedeRepositorio;
        _auditoriaRepositorio = auditoriaRepositorio;
    }

    public async Task<DashboardResumoDto> ObterResumoAsync(CancellationToken cancellationToken)
    {
        return new DashboardResumoDto
        {
            TotalDispositivosAtivos = await _dispositivoRepositorio.ContarAtivosAsync(cancellationToken),
            TotalEventosDns24h = await _eventoDnsRepositorio.ContarUltimas24HorasAsync(cancellationToken),
            TotalFluxos24h = await _fluxoRedeRepositorio.ContarUltimas24HorasAsync(cancellationToken),
            TotalAlertasAbertos = await _alertaRedeRepositorio.ContarAbertosAsync(cancellationToken),
            AtividadesRecentes = await _auditoriaRepositorio.ListarRecentesAsync(10, cancellationToken),
            TopDominios = await _eventoDnsRepositorio.ObterTopDominiosAsync(5, cancellationToken),
            TopDispositivosTrafego = await _fluxoRedeRepositorio.ObterTopDispositivosTrafegoAsync(5, cancellationToken)
        };
    }
}
