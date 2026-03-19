using System.ComponentModel.DataAnnotations;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class FluxoRedeEntradaDto
{
    [Required]
    public string IpDispositivo { get; init; } = string.Empty;

    [Required]
    public string MacDispositivo { get; init; } = string.Empty;

    [Required]
    public string Hostname { get; init; } = string.Empty;

    [Required]
    public string IpDestino { get; init; } = string.Empty;

    public int PortaDestino { get; init; }
    public string Protocolo { get; init; } = "TCP";
    public long BytesEnviados { get; init; }
    public long BytesRecebidos { get; init; }
    public string? DominioCorrelacionado { get; init; }
    public DateTime DataInicioUtc { get; init; } = DateTime.UtcNow;
    public DateTime DataFimUtc { get; init; } = DateTime.UtcNow;
}
