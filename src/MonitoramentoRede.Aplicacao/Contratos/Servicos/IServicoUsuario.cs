using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Compartilhado.Modelos.Seguranca;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra autenticação e administração de usuários.
/// </summary>
public interface IServicoUsuario
{
    Task<UsuarioAutenticado?> AutenticarAsync(LoginDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UsuarioSistemaDto>> ListarAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PerfilAcessoDto>> ListarPerfisAsync(CancellationToken cancellationToken);
    Task<UsuarioSistemaDto?> ObterPorIdAsync(long id, CancellationToken cancellationToken);
    Task<long> CriarAsync(CriarUsuarioDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken);
    Task AtualizarAsync(AtualizarUsuarioDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken);
}
