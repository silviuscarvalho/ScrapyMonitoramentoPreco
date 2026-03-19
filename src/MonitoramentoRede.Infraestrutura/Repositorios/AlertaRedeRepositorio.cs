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

public sealed class AlertaRedeRepositorio : RepositorioDapperBase, IAlertaRedeRepositorio
{
    public AlertaRedeRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<ResultadoPaginado<AlertaRedeDto>> ListarAsync(FiltroAlertaRede filtro, CancellationToken cancellationToken)
    {
        var sqlBase = new StringBuilder("""
            FROM AlertaRede a
            LEFT JOIN DispositivoRede d ON d.Id = a.DispositivoRedeId
            WHERE 1 = 1
            """);

        var parametros = new DynamicParameters();

        if (filtro.Tipo.HasValue)
        {
            sqlBase.AppendLine("AND a.Tipo = @Tipo");
            parametros.Add("Tipo", filtro.Tipo.Value);
        }

        if (filtro.Severidade.HasValue)
        {
            sqlBase.AppendLine("AND a.Severidade = @Severidade");
            parametros.Add("Severidade", filtro.Severidade.Value);
        }

        if (filtro.Status.HasValue)
        {
            sqlBase.AppendLine("AND a.Status = @Status");
            parametros.Add("Status", filtro.Status.Value);
        }

        if (filtro.InicioUtc.HasValue)
        {
            sqlBase.AppendLine("AND a.DataCriacaoUtc >= @InicioUtc");
            parametros.Add("InicioUtc", filtro.InicioUtc.Value);
        }

        if (filtro.FimUtc.HasValue)
        {
            sqlBase.AppendLine("AND a.DataCriacaoUtc <= @FimUtc");
            parametros.Add("FimUtc", filtro.FimUtc.Value);
        }

        parametros.Add("Offset", (filtro.Pagina - 1) * filtro.TamanhoPagina);
        parametros.Add("Fetch", filtro.TamanhoPagina);

        var sqlConsulta = $"""
            SELECT
                a.Id,
                a.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                a.Tipo,
                a.Severidade,
                a.Status,
                a.Titulo,
                a.Descricao,
                a.ObservacaoOperador,
                a.DataCriacaoUtc,
                a.DataResolucaoUtc
            {sqlBase}
            ORDER BY a.DataCriacaoUtc DESC
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase};";

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<AlertaRedeDto>(CriarComando(sqlConsulta, parametros, cancellationToken));
        var total = await conexao.ExecuteScalarAsync<int>(CriarComando(sqlTotal, parametros, cancellationToken));

        return new ResultadoPaginado<AlertaRedeDto>
        {
            Itens = itens.ToArray(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<IReadOnlyCollection<AlertaRedeDto>> ListarPorDispositivoAsync(long dispositivoId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                a.Id,
                a.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                a.Tipo,
                a.Severidade,
                a.Status,
                a.Titulo,
                a.Descricao,
                a.ObservacaoOperador,
                a.DataCriacaoUtc,
                a.DataResolucaoUtc
            FROM AlertaRede a
            LEFT JOIN DispositivoRede d ON d.Id = a.DispositivoRedeId
            WHERE a.DispositivoRedeId = @DispositivoId
            ORDER BY a.DataCriacaoUtc DESC;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<AlertaRedeDto>(CriarComando(sql, new { DispositivoId = dispositivoId }, cancellationToken));
        return itens.ToArray();
    }

    public async Task<long> InserirAsync(AlertaRede alerta, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO AlertaRede (DispositivoRedeId, Tipo, Severidade, Status, Titulo, Descricao, ObservacaoOperador, DataCriacaoUtc, DataResolucaoUtc)
            VALUES (@DispositivoRedeId, @Tipo, @Severidade, @Status, @Titulo, @Descricao, @ObservacaoOperador, @DataCriacaoUtc, @DataResolucaoUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, alerta, cancellationToken));
    }

    public async Task AtualizarAsync(AlertaRede alerta, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE AlertaRede
            SET Status = @Status,
                ObservacaoOperador = @ObservacaoOperador,
                DataResolucaoUtc = @DataResolucaoUtc
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(CriarComando(sql, alerta, cancellationToken));
    }

    public async Task<AlertaRede?> ObterPorIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 Id, DispositivoRedeId, Tipo, Severidade, Status, Titulo, Descricao, ObservacaoOperador, DataCriacaoUtc, DataResolucaoUtc
            FROM AlertaRede
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.QuerySingleOrDefaultAsync<AlertaRede>(CriarComando(sql, new { Id = id }, cancellationToken));
    }

    public async Task<int> ContarAbertosAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM AlertaRede WHERE Status <> 3;";
        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<int>(CriarComando(sql, null, cancellationToken));
    }
}
