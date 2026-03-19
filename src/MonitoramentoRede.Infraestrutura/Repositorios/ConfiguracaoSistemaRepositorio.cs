using Dapper;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;
using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Infraestrutura.Dados;

namespace MonitoramentoRede.Infraestrutura.Repositorios;

public sealed class ConfiguracaoSistemaRepositorio : RepositorioDapperBase, IConfiguracaoSistemaRepositorio
{
    public ConfiguracaoSistemaRepositorio(IFabricaConexaoSql fabricaConexaoSql) : base(fabricaConexaoSql)
    {
    }

    public async Task<ConfiguracaoSistema> ObterAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1
                Id,
                ChaveApiInterna,
                RetencaoDadosDias,
                IntervaloAtualizacaoDashboardSegundos,
                LimitePicoTrafegoBytes,
                JanelaExcessoDnsMinutos,
                LimiteConsultasDnsJanela,
                PortasIncomuns,
                DataAtualizacaoUtc
            FROM ConfiguracaoSistema
            ORDER BY Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        return await conexao.QuerySingleAsync<ConfiguracaoSistema>(CriarComando(sql, null, cancellationToken));
    }

    public async Task AtualizarAsync(ConfiguracaoSistema configuracao, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE ConfiguracaoSistema
            SET ChaveApiInterna = @ChaveApiInterna,
                RetencaoDadosDias = @RetencaoDadosDias,
                IntervaloAtualizacaoDashboardSegundos = @IntervaloAtualizacaoDashboardSegundos,
                LimitePicoTrafegoBytes = @LimitePicoTrafegoBytes,
                JanelaExcessoDnsMinutos = @JanelaExcessoDnsMinutos,
                LimiteConsultasDnsJanela = @LimiteConsultasDnsJanela,
                PortasIncomuns = @PortasIncomuns,
                DataAtualizacaoUtc = @DataAtualizacaoUtc
            WHERE Id = @Id;
            """;

        using var conexao = await ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(CriarComando(sql, configuracao, cancellationToken));
    }
}
