using System.Data;

namespace MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;

/// <summary>
/// Cria conexões para o banco SQL Server.
/// </summary>
public interface IFabricaConexaoSql
{
    Task<IDbConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken);
}
