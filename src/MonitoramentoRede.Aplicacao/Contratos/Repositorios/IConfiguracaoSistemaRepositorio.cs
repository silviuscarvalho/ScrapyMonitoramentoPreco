using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Aplicacao.Contratos.Repositorios;

/// <summary>
/// Define operações de persistência de configurações do sistema.
/// </summary>
public interface IConfiguracaoSistemaRepositorio
{
    Task<ConfiguracaoSistema> ObterAsync(CancellationToken cancellationToken);
    Task AtualizarAsync(ConfiguracaoSistema configuracao, CancellationToken cancellationToken);
}
