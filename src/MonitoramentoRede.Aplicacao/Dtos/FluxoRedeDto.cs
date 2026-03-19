namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class FluxoRedeDto
{
    public long Id { get; init; }
    public long DispositivoRedeId { get; init; }
    public string Dispositivo { get; init; } = string.Empty;
    public string IpDestino { get; init; } = string.Empty;
    public int PortaDestino { get; init; }
    public string Protocolo { get; init; } = string.Empty;
    public long BytesEnviados { get; init; }
    public long BytesRecebidos { get; init; }
    public string? DominioCorrelacionado { get; init; }
    public DateTime DataInicioUtc { get; init; }
    public DateTime DataFimUtc { get; init; }
}
