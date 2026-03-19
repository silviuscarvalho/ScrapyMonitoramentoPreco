using System.Security.Cryptography;
using System.Text;
using Dapper;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;
using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Infraestrutura.Dados;

namespace MonitoramentoRede.Infraestrutura.Repositorios;

public sealed class UsuarioRepositorio : RepositorioDapperBase, IUsuarioRepositorio
{
    public UsuarioRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<UsuarioSistema?> ObterPorLoginAsync(string login, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1
                u.Id,
                u.Nome,
                u.Login,
                u.Email,
                u.SenhaHash,
                u.PerfilAcessoId,
                u.Ativo,
                u.UltimoLoginUtc,
                p.Id,
                p.Nome,
                p.Descricao
            FROM UsuarioSistema u
            INNER JOIN PerfilAcesso p ON p.Id = u.PerfilAcessoId
            WHERE u.Login = @Login;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var usuarios = await conexao.QueryAsync<UsuarioSistema, PerfilAcesso, UsuarioSistema>(
            CriarComando(sql, new { Login = login }, cancellationToken),
            (usuario, perfil) =>
            {
                usuario.PerfilAcesso = perfil;
                return usuario;
            },
            splitOn: "Id");

        return usuarios.FirstOrDefault();
    }

    public async Task<UsuarioSistema?> ObterPorIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 Id, Nome, Login, Email, SenhaHash, PerfilAcessoId, Ativo, UltimoLoginUtc
            FROM UsuarioSistema
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.QuerySingleOrDefaultAsync<UsuarioSistema>(CriarComando(sql, new { Id = id }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<UsuarioSistemaDto>> ListarAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                u.Id,
                u.Nome,
                u.Login,
                u.Email,
                u.PerfilAcessoId,
                p.Nome AS PerfilNome,
                u.Ativo,
                u.UltimoLoginUtc
            FROM UsuarioSistema u
            INNER JOIN PerfilAcesso p ON p.Id = u.PerfilAcessoId
            ORDER BY u.Nome;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<UsuarioSistemaDto>(CriarComando(sql, null, cancellationToken));
        return itens.ToArray();
    }

    public async Task<IReadOnlyCollection<PerfilAcessoDto>> ListarPerfisAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Nome, Descricao
            FROM PerfilAcesso
            ORDER BY Nome;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<PerfilAcessoDto>(CriarComando(sql, null, cancellationToken));
        return itens.ToArray();
    }

    public async Task<long> InserirAsync(UsuarioSistema usuario, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO UsuarioSistema (Nome, Login, Email, SenhaHash, PerfilAcessoId, Ativo, UltimoLoginUtc)
            VALUES (@Nome, @Login, @Email, @SenhaHash, @PerfilAcessoId, @Ativo, @UltimoLoginUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, usuario, cancellationToken));
    }

    public async Task AtualizarAsync(UsuarioSistema usuario, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE UsuarioSistema
            SET Nome = @Nome,
                Email = @Email,
                SenhaHash = @SenhaHash,
                PerfilAcessoId = @PerfilAcessoId,
                Ativo = @Ativo
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(CriarComando(sql, usuario, cancellationToken));
    }

    public async Task AtualizarUltimoLoginAsync(long usuarioId, DateTime dataUtc, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE UsuarioSistema
            SET UltimoLoginUtc = @DataUtc
            WHERE Id = @UsuarioId;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(CriarComando(sql, new { UsuarioId = usuarioId, DataUtc = dataUtc }, cancellationToken));
    }

    public static string CalcularHashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
