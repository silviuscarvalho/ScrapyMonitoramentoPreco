using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Filtros;

public sealed class FiltroDispositivo : RequisicaoPaginada
{
    public string? Ip { get; set; }
    public string? Mac { get; set; }
    public string? Hostname { get; set; }
    public StatusDispositivo? Status { get; set; }
}
