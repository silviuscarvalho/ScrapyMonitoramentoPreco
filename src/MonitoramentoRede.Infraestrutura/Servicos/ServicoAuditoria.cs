using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Implementa o fluxo de auditoria da aplicação.
/// </summary>
public sealed class ServicoAuditoria : IServicoAuditoria
{
    private readonly IAuditoriaRepositorio _auditoriaRepositorio;

    public ServicoAuditoria(IAuditoriaRepositorio auditoriaRepositorio)
    {
        _auditoriaRepositorio = auditoriaRepositorio;
    }

    public async Task RegistrarAsync(long? usuarioId, string usuarioNome, string acao, string entidade, string? dados, bool sucesso, string? enderecoIp, CancellationToken cancellationToken)
    {
        var log = new LogAuditoria
        {
            UsuarioSistemaId = usuarioId,
            UsuarioNome = string.IsNullOrWhiteSpace(usuarioNome) ? "Sistema" : usuarioNome,
            Acao = acao,
            Entidade = entidade,
            Dados = dados,
            EnderecoIp = enderecoIp,
            Sucesso = sucesso,
            DataEventoUtc = DateTime.UtcNow
        };

        await _auditoriaRepositorio.InserirAsync(log, cancellationToken);
    }

    public Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken) =>
        _auditoriaRepositorio.ListarAsync(filtro, cancellationToken);

    public Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken) =>
        _auditoriaRepositorio.ListarRecentesAsync(quantidade, cancellationToken);
}
