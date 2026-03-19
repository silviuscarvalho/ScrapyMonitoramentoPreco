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

public sealed class FluxoRedeRepositorio : RepositorioDapperBase, IFluxoRedeRepositorio
{
    public FluxoRedeRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken)
    {
        var sqlBase = new StringBuilder("""
            FROM FluxoRede f
            INNER JOIN DispositivoRede d ON d.Id = f.DispositivoRedeId
            WHERE 1 = 1
            """);

        var parametros = new DynamicParameters();

        if (filtro.DispositivoRedeId.HasValue)
        {
            sqlBase.AppendLine("AND f.DispositivoRedeId = @DispositivoRedeId");
            parametros.Add("DispositivoRedeId", filtro.DispositivoRedeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Dispositivo))
        {
            sqlBase.AppendLine("AND d.Hostname LIKE @Dispositivo");
            parametros.Add("Dispositivo", $"%{filtro.Dispositivo}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.IpDestino))
        {
            sqlBase.AppendLine("AND f.IpDestino LIKE @IpDestino");
            parametros.Add("IpDestino", $"%{filtro.IpDestino}%");
        }

        if (filtro.PortaDestino.HasValue)
        {
            sqlBase.AppendLine("AND f.PortaDestino = @PortaDestino");
            parametros.Add("PortaDestino", filtro.PortaDestino.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Protocolo))
        {
            sqlBase.AppendLine("AND f.Protocolo = @Protocolo");
            parametros.Add("Protocolo", filtro.Protocolo);
        }

        if (filtro.BytesMinimos.HasValue)
        {
            sqlBase.AppendLine("AND (f.BytesEnviados + f.BytesRecebidos) >= @BytesMinimos");
            parametros.Add("BytesMinimos", filtro.BytesMinimos.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.DominioCorrelacionado))
        {
            sqlBase.AppendLine("AND f.DominioCorrelacionado LIKE @DominioCorrelacionado");
            parametros.Add("DominioCorrelacionado", $"%{filtro.DominioCorrelacionado}%");
        }

        if (filtro.InicioUtc.HasValue)
        {
            sqlBase.AppendLine("AND f.DataInicioUtc >= @InicioUtc");
            parametros.Add("InicioUtc", filtro.InicioUtc.Value);
        }

        if (filtro.FimUtc.HasValue)
        {
            sqlBase.AppendLine("AND f.DataFimUtc <= @FimUtc");
            parametros.Add("FimUtc", filtro.FimUtc.Value);
        }

        parametros.Add("Offset", (filtro.Pagina - 1) * filtro.TamanhoPagina);
        parametros.Add("Fetch", filtro.TamanhoPagina);

        var sqlConsulta = $"""
            SELECT
                f.Id,
                f.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                f.IpDestino,
                f.PortaDestino,
                f.Protocolo,
                f.BytesEnviados,
                f.BytesRecebidos,
                f.DominioCorrelacionado,
                f.DataInicioUtc,
                f.DataFimUtc
            {sqlBase}
            ORDER BY f.DataInicioUtc DESC
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase};";

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<FluxoRedeDto>(CriarComando(sqlConsulta, parametros, cancellationToken));
        var total = await conexao.ExecuteScalarAsync<int>(CriarComando(sqlTotal, parametros, cancellationToken));

        return new ResultadoPaginado<FluxoRedeDto>
        {
            Itens = itens.ToArray(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<IReadOnlyCollection<FluxoRedeDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                f.Id,
                f.DispositivoRedeId,
                d.Hostname AS Dispositivo,
                f.IpDestino,
                f.PortaDestino,
                f.Protocolo,
                f.BytesEnviados,
                f.BytesRecebidos,
                f.DominioCorrelacionado,
                f.DataInicioUtc,
                f.DataFimUtc
            FROM FluxoRede f
            INNER JOIN DispositivoRede d ON d.Id = f.DispositivoRedeId
            WHERE f.DispositivoRedeId = @DispositivoId
            ORDER BY f.DataInicioUtc DESC;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<FluxoRedeDto>(CriarComando(sql, new { DispositivoId = dispositivoId, Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }

    public async Task<long> InserirAsync(FluxoRede fluxoRede, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO FluxoRede (DispositivoRedeId, IpDestino, PortaDestino, Protocolo, BytesEnviados, BytesRecebidos, DominioCorrelacionado, DataInicioUtc, DataFimUtc)
            VALUES (@DispositivoRedeId, @IpDestino, @PortaDestino, @Protocolo, @BytesEnviados, @BytesRecebidos, @DominioCorrelacionado, @DataInicioUtc, @DataFimUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, fluxoRede, cancellationToken));
    }

    public async Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM FluxoRede WHERE DataInicioUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME());";
        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<int>(CriarComando(sql, null, cancellationToken));
    }

    public async Task<IReadOnlyCollection<TopDispositivoTrafegoDto>> ObterTopDispositivosTrafegoAsync(int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                d.Hostname AS Dispositivo,
                SUM(f.BytesEnviados + f.BytesRecebidos) AS TotalBytes
            FROM FluxoRede f
            INNER JOIN DispositivoRede d ON d.Id = f.DispositivoRedeId
            WHERE f.DataInicioUtc >= DATEADD(HOUR, -24, SYSUTCDATETIME())
            GROUP BY d.Hostname
            ORDER BY TotalBytes DESC, d.Hostname;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<TopDispositivoTrafegoDto>(CriarComando(sql, new { Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }
}
