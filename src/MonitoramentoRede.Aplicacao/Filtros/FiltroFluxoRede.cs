using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Filtros;

public sealed class FiltroFluxoRede : RequisicaoPaginada
{
    public long? DispositivoRedeId { get; set; }
    public string? Dispositivo { get; set; }
    public string? IpDestino { get; set; }
    public int? PortaDestino { get; set; }
    public string? Protocolo { get; set; }
    public long? BytesMinimos { get; set; }
    public string? DominioCorrelacionado { get; set; }
    public DateTime? InicioUtc { get; set; }
    public DateTime? FimUtc { get; set; }
}
