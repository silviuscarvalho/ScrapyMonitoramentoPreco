using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência de fluxos de rede.
/// </summary>
public interface IFluxoRedeRepositorio
{
    Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<FluxoRedeDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken);
    Task<long> InserirAsync(FluxoRede fluxoRede, CancellationToken cancellationToken);
    Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TopDispositivoTrafegoDto>> ObterTopDispositivosTrafegoAsync(int quantidade, CancellationToken cancellationToken);
}
