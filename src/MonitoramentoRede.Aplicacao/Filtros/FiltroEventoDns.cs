using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Filtros;

public sealed class FiltroEventoDns : RequisicaoPaginada
{
    public string? Dominio { get; set; }
    public long? DispositivoRedeId { get; set; }
    public string? HostnameDispositivo { get; set; }
    public TipoRegistroDns? TipoRegistro { get; set; }
    public StatusConsultaDns? StatusConsulta { get; set; }
    public DateTime? InicioUtc { get; set; }
    public DateTime? FimUtc { get; set; }
    public bool OrdenacaoDescendente { get; set; } = true;
}
