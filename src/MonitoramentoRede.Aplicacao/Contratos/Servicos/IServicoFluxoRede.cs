using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra consultas e ingestão de fluxos de rede.
/// </summary>
public interface IServicoFluxoRede
{
    Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken);
    Task<long> IngerirAsync(FluxoRedeEntradaDto dto, CancellationToken cancellationToken);
    Task<string> GerarCsvAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken);
}
