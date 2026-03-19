using MonitoramentoRede.Aplicacao.Dtos;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Fornece dados agregados do dashboard.
/// </summary>
public interface IServicoDashboard
{
    Task<DashboardResumoDto> ObterResumoAsync(CancellationToken cancellationToken);
}
