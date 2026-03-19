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

public sealed class DispositivoRepositorio : RepositorioDapperBase, IDispositivoRepositorio
{
    public DispositivoRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken)
    {
        var sqlBase = new StringBuilder("""
            FROM DispositivoRede
            WHERE 1 = 1
            """);

        var parametros = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filtro.Ip))
        {
            sqlBase.AppendLine("AND Ip LIKE @Ip");
            parametros.Add("Ip", $"%{filtro.Ip}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Mac))
        {
            sqlBase.AppendLine("AND Mac LIKE @Mac");
            parametros.Add("Mac", $"%{filtro.Mac}%");
        }

        if (!string.IsNullOrWhiteSpace(filtro.Hostname))
        {
            sqlBase.AppendLine("AND Hostname LIKE @Hostname");
            parametros.Add("Hostname", $"%{filtro.Hostname}%");
        }

        if (filtro.Status.HasValue)
        {
            sqlBase.AppendLine("AND Status = @Status");
            parametros.Add("Status", filtro.Status.Value);
        }

        parametros.Add("Offset", (filtro.Pagina - 1) * filtro.TamanhoPagina);
        parametros.Add("Fetch", filtro.TamanhoPagina);

        var sqlConsulta = $"""
            SELECT Id, Ip, Mac, Hostname, Status, SistemaOperacional, PrimeiroVistoUtc, UltimaDeteccaoUtc
            {sqlBase}
            ORDER BY UltimaDeteccaoUtc DESC
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            """;

        var sqlTotal = $"SELECT COUNT(1) {sqlBase};";

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<DispositivoDto>(CriarComando(sqlConsulta, parametros, cancellationToken));
        var total = await conexao.ExecuteScalarAsync<int>(CriarComando(sqlTotal, parametros, cancellationToken));

        return new ResultadoPaginado<DispositivoDto>
        {
            Itens = itens.ToArray(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<DispositivoDto?> ObterDtoPorIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 Id, Ip, Mac, Hostname, Status, SistemaOperacional, PrimeiroVistoUtc, UltimaDeteccaoUtc
            FROM DispositivoRede
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.QuerySingleOrDefaultAsync<DispositivoDto>(CriarComando(sql, new { Id = id }, cancellationToken));
    }

    public async Task<DispositivoRede?> ObterPorChavesRedeAsync(string ip, string mac, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 Id, Ip, Mac, Hostname, Status, SistemaOperacional, Observacoes, PrimeiroVistoUtc, UltimaDeteccaoUtc
            FROM DispositivoRede
            WHERE Ip = @Ip AND Mac = @Mac;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.QuerySingleOrDefaultAsync<DispositivoRede>(CriarComando(sql, new { Ip = ip, Mac = mac }, cancellationToken));
    }

    public async Task<long> InserirAsync(DispositivoRede dispositivo, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO DispositivoRede (Ip, Mac, Hostname, Status, SistemaOperacional, Observacoes, PrimeiroVistoUtc, UltimaDeteccaoUtc)
            VALUES (@Ip, @Mac, @Hostname, @Status, @SistemaOperacional, @Observacoes, @PrimeiroVistoUtc, @UltimaDeteccaoUtc);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<long>(CriarComando(sql, dispositivo, cancellationToken));
    }

    public async Task AtualizarAsync(DispositivoRede dispositivo, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE DispositivoRede
            SET Hostname = @Hostname,
                Status = @Status,
                SistemaOperacional = @SistemaOperacional,
                Observacoes = @Observacoes,
                UltimaDeteccaoUtc = @UltimaDeteccaoUtc
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(CriarComando(sql, dispositivo, cancellationToken));
    }

    public async Task<int> ContarAtivosAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM DispositivoRede WHERE Status = 1;";
        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<int>(CriarComando(sql, null, cancellationToken));
    }

    public async Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                Dominio,
                COUNT(1) AS Total
            FROM EventoDns
            WHERE DispositivoRedeId = @DispositivoId
            GROUP BY Dominio
            ORDER BY Total DESC, Dominio;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<TopDominioDto>(CriarComando(sql, new { DispositivoId = dispositivoId, Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }

    public async Task<IReadOnlyCollection<TopDestinoDto>> ObterTopDestinosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (@Quantidade)
                IpDestino AS Destino,
                SUM(BytesEnviados + BytesRecebidos) AS Total
            FROM FluxoRede
            WHERE DispositivoRedeId = @DispositivoId
            GROUP BY IpDestino
            ORDER BY Total DESC, Destino;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        var itens = await conexao.QueryAsync<TopDestinoDto>(CriarComando(sql, new { DispositivoId = dispositivoId, Quantidade = quantidade }, cancellationToken));
        return itens.ToArray();
    }
}
