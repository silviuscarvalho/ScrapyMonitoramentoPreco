using System.Text;
using Dapper;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;
using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Infraestrutura.Dados;

namespace MonitoramentoRede.Infraestrutura.Repositorios;

public sealed class AuditoriaRepositorio : RepositorioDapperBase, IAuditoriaRepositorio
{
    public AuditoriaRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<long> InserirAsync(LogAuditoria logAuditoria, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO LogAuditoria (UsuarioSistemaId, UsuarioNome, Acao, Entidade, Dados, EnderecoIp, Sucesso, DataEventoUtc)
            VALUES (@UsuarioSistemaId, @UsuarioNome, @Acao, @Entidade, @Dados, @EnderecoIp, @Sucesso, @DataEventoUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, logAuditoria, cancellationToken));
    }

    public async Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken)
    {
        var sqlBase = new StringBuilder("""
            FROM LogAuditoria
            WHERE 1 = 1
            """);

        var parametros = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filtro.Usuario))
        {
            sqlBase.AppendLine("AND UsuarioNome LIKE @Usuario");
            parametros.Add("Usuario", $"%{filtro.Usuario}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Acao))
        {
            sqlBase.AppendLine("AND Acao LIKE @Acao");
            parametros.Add("Acao", $"%{filtro.Acao}%");
        }

        if (filtro.InicioUtc.HasValue)
        {
            sqlBase.AppendLine("AND DataEventoUtc >= @InicioUtc");
            parametros.Add("InicioUtc", filtro.InicioUtc.Value);
        }

        if (filtro.FimUtc.HasValue)
        {
            sqlBase.AppendLine("AND DataEventoUtc <= @FimUtc");
            parametros.Add("FimUtc", filtro.FimUtc.Value);
        }

        parametros.Add("Offset", (filtro.Pagina - 1) * filtro.TamanhoPagina);
        parametros.Add("Fetch", filtro.TamanhoPagina);

        var sqlConsulta = $"""
            SELECT Id, UsuarioNome, Acao, Entidade, Dados, Sucesso, EnderecoIp, DataEventoUtc
            {sqlBase}
            ORDER BY DataEventoUtc DESC
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase};";

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<LogAuditoriaDto>(CriarComando(sqlConsulta, parametros, cancellationToken));
        var total = await conexao.ExecuteScalarAsync<int>(CriarComando(sqlTotal, parametros, cancellationToken));

        return new ResultadoPaginado<LogAuditoriaDto>
        {
            Itens = itens.ToArray(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                Id,
                UsuarioNome,
                Acao,
                Entidade,
                Dados,
                Sucesso,
                EnderecoIp,
                DataEventoUtc
            FROM LogAuditoria
            ORDER BY DataEventoUtc DESC;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<LogAuditoriaDto>(CriarComando(sql, new { Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }
}
