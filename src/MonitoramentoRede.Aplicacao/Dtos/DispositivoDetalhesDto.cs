namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class DispositivoDetalhesDto
{
    public DispositivoDto? Dispositivo { get; init; }
    public IReadOnlyCollection<TopDominioDto> TopDominios { get; init; } = [];
    public IReadOnlyCollection<TopDestinoDto> TopDestinos { get; init; } = [];
    public IReadOnlyCollection<EventoDnsDto> EventosDns { get; init; } = [];
    public IReadOnlyCollection<FluxoRedeDto> FluxosRede { get; init; } = [];
    public IReadOnlyCollection<AlertaRedeDto> Alertas { get; init; } = [];
}
