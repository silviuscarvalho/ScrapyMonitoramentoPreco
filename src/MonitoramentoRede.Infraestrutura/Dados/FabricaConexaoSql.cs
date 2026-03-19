using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;
using MonitoramentoRede.Infraestrutura.Configuracoes;

namespace MonitoramentoRede.Infraestrutura.Dados;

/// <summary>
/// Cria conexões abertas para o SQL Server.
/// </summary>
public sealed class FabricaConexaoSql : IFabricaConexaoSql
{
    private readonly string _connectionString;

    public FabricaConexaoSql(IOptions<OpcaoBancoDados> opcaoBancoDados)
    {
        _connectionString = opcaoBancoDados.Value.ConnectionString;
    }

    public async Task<IDbConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("A connection string do SQL Server não foi configurada.");
        }

        var conexao = new SqlConnection(_connectionString);
        await conexao.OpenAsync(cancellationToken);
        return conexao;
    }
}
