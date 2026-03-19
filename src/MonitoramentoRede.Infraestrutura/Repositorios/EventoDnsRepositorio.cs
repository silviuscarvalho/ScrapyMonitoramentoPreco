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

public sealed class EventoDnsRepositorio : RepositorioDapperBase, IEventoDnsRepositorio
{
    public EventoDnsRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken)
    {
        var sqlBase = new StringBuilder("""
            FROM EventoDns e
            INNER JOIN DispositivoRede d ON d.Id = e.DispositivoRedeId
            WHERE 1 = 1
            """);

        var parametros = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filtro.Dominio))
        {
            sqlBase.AppendLine("AND e.Dominio LIKE @Dominio");
            parametros.Add("Dominio", $"%{filtro.Dominio}%");
        }

        if (filtro.DispositivoRedeId.HasValue)
        {
            sqlBase.AppendLine("AND e.DispositivoRedeId = @DispositivoRedeId");
            parametros.Add("DispositivoRedeId", filtro.DispositivoRedeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.HostnameDispositivo))
        {
            sqlBase.AppendLine("AND d.Hostname LIKE @HostnameDispositivo");
            parametros.Add("HostnameDispositivo", $"%{filtro.HostnameDispositivo}%");
        }

        if (filtro.TipoRegistro.HasValue)
        {
            sqlBase.AppendLine("AND e.TipoRegistro = @TipoRegistro");
            parametros.Add("TipoRegistro", filtro.TipoRegistro.Value);
        }

        if (filtro.StatusConsulta.HasValue)
        {
            sqlBase.AppendLine("AND e.StatusConsulta = @StatusConsulta");
            parametros.Add("StatusConsulta", filtro.StatusConsulta.Value);
        }

        if (filtro.InicioUtc.HasValue)
        {
            sqlBase.AppendLine("AND e.DataEventoUtc >= @InicioUtc");
            parametros.Add("InicioUtc", filtro.InicioUtc.Value);
        }

        if (filtro.FimUtc.HasValue)
        {
            sqlBase.AppendLine("AND e.DataEventoUtc <= @FimUtc");
            parametros.Add("FimUtc", filtro.FimUtc.Value);
        }

        parametros.Add("Offset", (filtro.Pagina - 1) * filtro.TamanhoPagina);
        parametros.Add("Fetch", filtro.TamanhoPagina);

        var ordem = filtro.OrdenacaoDescendente ? "DESC" : "ASC";

        var sqlConsulta = $"""
            SELECT
                e.Id,
                e.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                e.Dominio,
                e.TipoRegistro,
                e.StatusConsulta,
                e.Resposta,
                e.TempoRespostaMs,
                e.DataEventoUtc
            {sqlBase}
            ORDER BY e.DataEventoUtc {ordem}
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase};";

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<EventoDnsDto>(CriarComando(sqlConsulta, parametros, cancellationToken));
        var total = await conexao.ExecuteScalarAsync<int>(CriarComando(sqlTotal, parametros, cancellationToken));

        return new ResultadoPaginado<EventoDnsDto>
        {
            Itens = itens.ToArray(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<IReadOnlyCollection<EventoDnsDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                e.Id,
                e.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                e.Dominio,
                e.TipoRegistro,
                e.StatusConsulta,
                e.Resposta,
                e.TempoRespostaMs,
                e.DataEventoUtc
            FROM EventoDns e
            INNER JOIN DispositivoRede d ON d.Id = e.DispositivoRedeId
            WHERE e.DispositivoRedeId = @DispositivoId
            ORDER BY e.DataEventoUtc DESC;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<EventoDnsDto>(CriarComando(sql, new { DispositivoId = dispositivoId, Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }

    public async Task<long> InserirAsync(EventoDns eventoDns, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO EventoDns (DispositivoRedeId, Dominio, TipoRegistro, StatusConsulta, Resposta, TempoRespostaMs, EnderecoOrigem, DataEventoUtc)
            VALUES (@DispositivoRedeId, @Dominio, @TipoRegistro, @StatusConsulta, @Resposta, @TempoRespostaMs, @EnderecoOrigem, @DataEventoUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, eventoDns, cancellationToken));
    }

    public async Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM EventoDns WHERE DataEventoUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME());";
        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<int>(CriarComando(sql, null, cancellationToken));
    }

    public async Task<bool> DominioJaVistoAsync(string dominio, CancellationToken cancellationToken)
    {
        const string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM EventoDns WHERE Dominio = @Dominio) THEN 1 ELSE 0 END;";
        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<bool>(CriarComando(sql, new { Dominio = dominio }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade) Dominio, COUNT(1) AS Total
            FROM EventoDns
            WHERE DataEventoUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME())
            GROUP BY Dominio
            ORDER BY Total DESC, Dominio;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<TopDominioDto>(CriarComando(sql, new { Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }

    public async Task<int> ContarConsultasPorJanelaAsync(long dispositivoId, string dominio, DateTime inicioUtc, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM EventoDns
            WHERE DispositivoRedeId = @DispositivoId
              AND Dominio = @Dominio
              AND DataEventoUtc >= @InicioUtc;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<int>(CriarComando(sql, new { DispositivoId = dispositivoId, Dominio = dominio, InicioUtc = inicioUtc }, cancellationToken));
    }
}
