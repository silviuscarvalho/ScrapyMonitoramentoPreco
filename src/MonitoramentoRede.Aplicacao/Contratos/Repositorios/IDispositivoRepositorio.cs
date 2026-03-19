using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência para dispositivos monitorados.
/// </summary>
public interface IDispositivoRepositorio
{
    Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken);
    Task<DispositivoDto?> ObterDtoPorIdAsync(long id, CancellationToken cancellationToken);
    Task<DispositivoRede?> ObterPorChavesRedeAsync(string ip, string mac, CancellationToken cancellationToken);
    Task<long> InserirAsync(DispositivoRede dispositivo, CancellationToken cancellationToken);
    Task AtualizarAsync(DispositivoRede dispositivo, CancellationToken cancellationToken);
    Task<int> ContarAtivosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TopDestinoDto>> ObterTopDestinosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken);
}
