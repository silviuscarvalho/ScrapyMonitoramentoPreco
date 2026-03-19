namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class DashboardResumoDto
{
    public int TotalDispositivosAtivos { get; init; }
    public int TotalEventosDns24h { get; init; }
    public int TotalFluxos24h { get; init; }
    public int TotalAlertasAbertos { get; init; }
    public IReadOnlyCollection<LogAuditoriaDto> AtividadesRecentes { get; init; } = [];
    public IReadOnlyCollection<TopDominioDto> TopDominios { get; init; } = [];
    public IReadOnlyCollection<TopDispositivoTrafegoDto> TopDispositivosTrafego { get; init; } = [];
}
