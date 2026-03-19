using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência de alertas de rede.
/// </summary>
public interface IAlertaRedeRepositorio
{
    Task<ResultadoPaginado<AlertaRedeDto>> ListarAsync(FiltroAlertaRede filtro, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AlertaRedeDto>> ListarPorDispositivoAsync(long dispositivoId, CancellationToken cancellationToken);
    Task<long> InserirAsync(AlertaRede alerta, CancellationToken cancellationToken);
    Task AtualizarAsync(AlertaRede alerta, CancellationToken cancellationToken);
    Task<AlertaRede?> ObterPorIdAsync(long id, CancellationToken cancellationToken);
    Task<int> ContarAbertosAsync(CancellationToken cancellationToken);
}
