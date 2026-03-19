using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência de usuários do sistema.
/// </summary>
public interface IUsuarioRepositorio
{
    Task<UsuarioSistema?> ObterPorLoginAsync(string login, CancellationToken cancellationToken);
    Task<UsuarioSistema?> ObterPorIdAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UsuarioSistemaDto>> ListarAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PerfilAcessoDto>> ListarPerfisAsync(CancellationToken cancellationToken);
    Task<long> InserirAsync(UsuarioSistema usuario, CancellationToken cancellationToken);
    Task AtualizarAsync(UsuarioSistema usuario, CancellationToken cancellationToken);
    Task AtualizarUltimoLoginAsync(long usuarioId, DateTime dataUtc, CancellationToken cancellationToken);
}
