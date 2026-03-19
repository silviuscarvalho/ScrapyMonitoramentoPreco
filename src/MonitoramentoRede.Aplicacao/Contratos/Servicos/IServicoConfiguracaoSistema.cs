using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra leitura e atualização das configurações do sistema.
/// </summary>
public interface IServicoConfiguracaoSistema
{
    Task<ConfiguracaoSistemaDto> ObterAsync(CancellationToken cancellationToken);
    Task AtualizarAsync(AtualizarConfiguracaoSistemaDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken);
}
