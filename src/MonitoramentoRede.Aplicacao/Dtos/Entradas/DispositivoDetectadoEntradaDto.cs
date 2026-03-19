using System.ComponentModel.DataAnnotations;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class DispositivoDetectadoEntradaDto
{
    [Required]
    public string Ip { get; init; } = string.Empty;

    [Required]
    public string Mac { get; init; } = string.Empty;

    [Required]
    public string Hostname { get; init; } = string.Empty;

    public string? SistemaOperacional { get; init; }
    public DateTime DataDeteccaoUtc { get; init; } = DateTime.UtcNow;
}
