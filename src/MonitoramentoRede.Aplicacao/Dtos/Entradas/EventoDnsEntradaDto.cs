using System.ComponentModel.DataAnnotations;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class EventoDnsEntradaDto
{
    [Required]
    public string IpDispositivo { get; init; } = string.Empty;

    [Required]
    public string MacDispositivo { get; init; } = string.Empty;

    [Required]
    public string Hostname { get; init; } = string.Empty;

    [Required]
    public string Dominio { get; init; } = string.Empty;

    [Required]
    public TipoRegistroDns TipoRegistro { get; init; }

    [Required]
    public StatusConsultaDns StatusConsulta { get; init; }

    public string? Resposta { get; init; }
    public int TempoRespostaMs { get; init; }
    public DateTime DataEventoUtc { get; init; } = DateTime.UtcNow;
}
