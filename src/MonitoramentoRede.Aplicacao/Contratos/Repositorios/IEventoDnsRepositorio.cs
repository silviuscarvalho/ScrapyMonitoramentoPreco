using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência de eventos DNS.
/// </summary>
public interface IEventoDnsRepositorio
{
    Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<EventoDnsDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken);
    Task<long> InserirAsync(EventoDns eventoDns, CancellationToken cancellationToken);
    Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken);
    Task<bool> DominioJaVistoAsync(string dominio, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(int quantidade, CancellationToken cancellationToken);
    Task<int> ContarConsultasPorJanelaAsync(long dispositivoId, string dominio, DateTime inicioUtc, CancellationToken cancellationToken);
}
