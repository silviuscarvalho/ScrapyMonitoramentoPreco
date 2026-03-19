using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Compartilhado.Modelos.Api;
using MonitoramentoRede.Web.Autenticacao;
using MonitoramentoRede.Web.Hubs;

namespace MonitoramentoRede.Web.Controllers.Interno;

[ApiController]
[Route("api/interno/ingestao")]
[ServiceFilter(typeof(ValidarApiInternaFiltro))]
[IgnoreAntiforgeryToken]
public sealed class IngestaoInternaController : ControllerBase
{
    private readonly IServicoEventoDns _servicoEventoDns;
    private readonly IServicoFluxoRede _servicoFluxoRede;
    private readonly IServicoDispositivo _servicoDispositivo;
    private readonly IServicoAuditoria _servicoAuditoria;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<IngestaoInternaController> _logger;

    public IngestaoInternaController(
        IServicoEventoDns servicoEventoDns,
        IServicoFluxoRede servicoFluxoRede,
        IServicoDispositivo servicoDispositivo,
        IServicoAuditoria servicoAuditoria,
        IHubContext<DashboardHub> hubContext,
        ILogger<IngestaoInternaController> logger)
    {
        _servicoEventoDns = servicoEventoDns;
        _servicoFluxoRede = servicoFluxoRede;
        _servicoDispositivo = servicoDispositivo;
        _servicoAuditoria = servicoAuditoria;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("dns")]
    public async Task<IActionResult> ReceberEventoDns([FromBody] EventoDnsEntradaDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(RespostaPadronizada<string>.Falha("Payload inválido.", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray()));
        }

        try
        {
            var id = await _servicoEventoDns.IngerirAsync(dto, cancellationToken);
            await NotificarDashboardAsync(cancellationToken);
            return Ok(RespostaPadronizada<object>.Ok(new { Id = id }, "Evento DNS processado com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ingerir evento DNS.");
            await _servicoAuditoria.RegistrarAsync(null, "Sistema", "ErroIngestaoDns", "EventoDns", ex.Message, false, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            return StatusCode(StatusCodes.Status500InternalServerError, RespostaPadronizada<string>.Falha("Erro ao processar o evento DNS."));
        }
    }

    [HttpPost("fluxos")]
    public async Task<IActionResult> ReceberFluxo([FromBody] FluxoRedeEntradaDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(RespostaPadronizada<string>.Falha("Payload inválido.", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray()));
        }

        try
        {
            var id = await _servicoFluxoRede.IngerirAsync(dto, cancellationToken);
            await NotificarDashboardAsync(cancellationToken);
            return Ok(RespostaPadronizada<object>.Ok(new { Id = id }, "Fluxo processado com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ingerir fluxo.");
            await _servicoAuditoria.RegistrarAsync(null, "Sistema", "ErroIngestaoFluxo", "FluxoRede", ex.Message, false, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            return StatusCode(StatusCodes.Status500InternalServerError, RespostaPadronizada<string>.Falha("Erro ao processar o fluxo."));
        }
    }

    [HttpPost("dispositivos")]
    public async Task<IActionResult> ReceberDispositivo([FromBody] DispositivoDetectadoEntradaDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(RespostaPadronizada<string>.Falha("Payload inválido.", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray()));
        }

        try
        {
            var id = await _servicoDispositivo.RegistrarDeteccaoAsync(dto, cancellationToken);
            await NotificarDashboardAsync(cancellationToken);
            return Ok(RespostaPadronizada<object>.Ok(new { Id = id }, "Dispositivo processado com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ingerir dispositivo.");
            await _servicoAuditoria.RegistrarAsync(null, "Sistema", "ErroIngestaoDispositivo", "DispositivoRede", ex.Message, false, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            return StatusCode(StatusCodes.Status500InternalServerError, RespostaPadronizada<string>.Falha("Erro ao processar o dispositivo."));
        }
    }

    private async Task NotificarDashboardAsync(CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync("AtualizarDashboard", cancellationToken);
        await _hubContext.Clients.All.SendAsync("NovaAtividade", cancellationToken);
        await _hubContext.Clients.All.SendAsync("NovoAlerta", cancellationToken);
    }
}
