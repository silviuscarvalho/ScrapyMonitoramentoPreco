using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Compartilhado.Modelos.Seguranca;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Infraestrutura.Repositorios;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Implementa autenticação e gestão de usuários do sistema.
/// </summary>
public sealed class ServicoUsuario : IServicoUsuario
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoUsuario(IUsuarioRepositorio usuarioRepositorio, IServicoAuditoria servicoAuditoria)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public async Task<UsuarioAutenticado?> AutenticarAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepositorio.ObterPorLoginAsync(dto.Login, cancellationToken);
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        var senhaHash = UsuarioRepositorio.CalcularHashSenha(dto.Senha);
        if (!string.Equals(usuario.SenhaHash, senhaHash, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        await _usuarioRepositorio.AtualizarUltimoLoginAsync(usuario.Id, DateTime.UtcNow, cancellationToken);

        return new UsuarioAutenticado
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Login = usuario.Login,
            Perfil = usuario.PerfilAcesso?.Nome ?? string.Empty
        };
    }

    public Task<IReadOnlyCollection<UsuarioSistemaDto>> ListarAsync(CancellationToken cancellationToken) =>
        _usuarioRepositorio.ListarAsync(cancellationToken);

    public Task<IReadOnlyCollection<PerfilAcessoDto>> ListarPerfisAsync(CancellationToken cancellationToken) =>
        _usuarioRepositorio.ListarPerfisAsync(cancellationToken);

    public async Task<UsuarioSistemaDto?> ObterPorIdAsync(long id, CancellationToken cancellationToken)
    {
        var itens = await _usuarioRepositorio.ListarAsync(cancellationToken);
        return itens.FirstOrDefault(x => x.Id == id);
    }

    public async Task<long> CriarAsync(CriarUsuarioDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken)
    {
        var usuario = new UsuarioSistema
        {
            Nome = dto.Nome,
            Login = dto.Login,
            Email = dto.Email,
            SenhaHash = UsuarioRepositorio.CalcularHashSenha(dto.Senha),
            PerfilAcessoId = dto.PerfilAcessoId,
            Ativo = dto.Ativo
        };

        var id = await _usuarioRepositorio.InserirAsync(usuario, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(usuarioExecutorId, usuarioExecutorNome, "CriacaoUsuario", "UsuarioSistema", $"Usuario {dto.Login} criado", true, null, cancellationToken);
        return id;
    }

    public async Task AtualizarAsync(AtualizarUsuarioDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken)
    {
        var atual = await _usuarioRepositorio.ObterPorIdAsync(dto.Id, cancellationToken) ?? throw new InvalidOperationException("Usuário não encontrado.");

        atual.Nome = dto.Nome;
        atual.Email = dto.Email;
        atual.PerfilAcessoId = dto.PerfilAcessoId;
        atual.Ativo = dto.Ativo;

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            atual.SenhaHash = UsuarioRepositorio.CalcularHashSenha(dto.Senha);
        }

        await _usuarioRepositorio.AtualizarAsync(atual, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(usuarioExecutorId, usuarioExecutorNome, "EdicaoUsuario", "UsuarioSistema", $"Usuario {dto.Id} atualizado", true, null, cancellationToken);
    }
}
