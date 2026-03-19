using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class EventoDnsDto
{
    public long Id { get; init; }
    public long DispositivoRedeId { get; init; }
    public string Dispositivo { get; init; } = string.Empty;
    public string Dominio { get; init; } = string.Empty;
    public TipoRegistroDns TipoRegistro { get; init; }
    public StatusConsultaDns StatusConsulta { get; init; }
    public string? Resposta { get; init; }
    public int TempoRespostaMs { get; init; }
    public DateTime DataEventoUtc { get; init; }
}
