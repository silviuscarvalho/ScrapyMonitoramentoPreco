using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class AlertaRedeDto
{
    public long Id { get; init; }
    public long? DispositivoRedeId { get; init; }
    public string? Dispositivo { get; init; }
    public TipoAlerta Tipo { get; init; }
    public SeveridadeAlerta Severidade { get; init; }
    public StatusAlerta Status { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? ObservacaoOperador { get; init; }
    public DateTime DataCriacaoUtc { get; init; }
    public DateTime? DataResolucaoUtc { get; init; }
}
