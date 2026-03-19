using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência para auditoria.
/// </summary>
public interface IAuditoriaRepositorio
{
    Task<long> InserirAsync(LogAuditoria logAuditoria, CancellationToken cancellationToken);
    Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken);
}
