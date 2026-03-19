using System.Data;
using Dapper;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;

namespace MonitoramentoRede.Infraestrutura.Dados;

public abstract class RepositorioDapperBase
{
    private readonly IFabricaConexaoSql _fabricaConexaoSql;

    protected RepositorioDapperBase(IFabricaConexaoSql fabricaConexaoSql)
    {
        _fabricaConexaoSql = fabricaConexaoSql;
    }

    protected async Task<IDbConnection> ObterConexaoAsync(CancellationToken cancellationToken)
    {
        return await _fabricaConexaoSql.CriarConexaoAbertaAsync(cancellationToken);
    }

    protected static CommandDefinition CriarComando(string sql, object? parametros, CancellationToken cancellationToken) =>
        new(sql, parametros, cancellationToken: cancellationToken);
}
