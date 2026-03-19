using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra persistência e consulta de auditoria.
/// </summary>
public interface IServicoAuditoria
{
    Task RegistrarAsync(long? usuarioId, string usuarioNome, string acao, string entidade, string? dados, bool sucesso, string? enderecoIp, CancellationToken cancellationToken);
    Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken);
}
