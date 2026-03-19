using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra consulta e tratamento de alertas.
/// </summary>
public interface IServicoAlertaRede
{
    Task<ResultadoPaginado<AlertaRedeDto>> ListarAsync(FiltroAlertaRede filtro, CancellationToken cancellationToken);
    Task ResolverAsync(ResolverAlertaDto dto, CancellationToken cancellationToken);
}
