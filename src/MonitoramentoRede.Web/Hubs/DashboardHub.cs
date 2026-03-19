using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MonitoramentoRede.Web.Hubs;

/// <summary>
/// Hub responsável por atualizar partes do dashboard em tempo real.
/// </summary>
[Authorize]
public sealed class DashboardHub : Hub
{
}
