using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Dominio.Entidades;

public sealed class EventoDns : EntidadeBase
{
    public long DispositivoRedeId { get; set; }
    public string Dominio { get; set; } = string.Empty;
    public TipoRegistroDns TipoRegistro { get; set; }
    public StatusConsultaDns StatusConsulta { get; set; }
    public string? Resposta { get; set; }
    public int TempoRespostaMs { get; set; }
    public string EnderecoOrigem { get; set; } = string.Empty;
    public DateTime DataEventoUtc { get; set; }
}
