using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Filtros;

public sealed class FiltroAuditoria : RequisicaoPaginada
{
    public string? Usuario { get; set; }
    public string? Acao { get; set; }
    public DateTime? InicioUtc { get; set; }
    public DateTime? FimUtc { get; set; }
}
