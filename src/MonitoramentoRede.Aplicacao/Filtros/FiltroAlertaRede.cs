using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Filtros;

public sealed class FiltroAlertaRede : RequisicaoPaginada
{
    public TipoAlerta? Tipo { get; set; }
    public SeveridadeAlerta? Severidade { get; set; }
    public StatusAlerta? Status { get; set; }
    public DateTime? InicioUtc { get; set; }
    public DateTime? FimUtc { get; set; }
}
