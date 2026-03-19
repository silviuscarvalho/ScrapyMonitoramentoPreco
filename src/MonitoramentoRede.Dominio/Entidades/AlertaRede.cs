using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Dominio.Entidades;

public sealed class AlertaRede : EntidadeBase
{
    public long? DispositivoRedeId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public SeveridadeAlerta Severidade { get; set; }
    public StatusAlerta Status { get; set; } = StatusAlerta.Aberto;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? ObservacaoOperador { get; set; }
    public DateTime DataCriacaoUtc { get; set; }
    public DateTime? DataResolucaoUtc { get; set; }
}
