namespace MonitoramentoRede.Dominio.Entidades;

public sealed class FluxoRede : EntidadeBase
{
    public long DispositivoRedeId { get; set; }
    public string IpDestino { get; set; } = string.Empty;
    public int PortaDestino { get; set; }
    public string Protocolo { get; set; } = "TCP";
    public long BytesEnviados { get; set; }
    public long BytesRecebidos { get; set; }
    public string? DominioCorrelacionado { get; set; }
    public DateTime DataInicioUtc { get; set; }
    public DateTime DataFimUtc { get; set; }
}
