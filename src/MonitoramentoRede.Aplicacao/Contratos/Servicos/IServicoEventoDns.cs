using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra consultas e ingestão de eventos DNS.
/// </summary>
public interface IServicoEventoDns
{
    Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken);
    Task<long> IngerirAsync(EventoDnsEntradaDto dto, CancellationToken cancellationToken);
    Task<string> GerarCsvAsync(FiltroEventoDns filtro, CancellationToken cancellationToken);
}
