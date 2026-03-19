using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Compartilhado.Modelos.Api;

namespace MonitoramentoRede.Web.Autenticacao;

public sealed class ValidarApiInternaFiltro : IAsyncActionFilter
{
    private const string CabecalhoChaveApi = "X-Api-Key";
    private readonly IServicoConfiguracaoSistema _servicoConfiguracaoSistema;
    private readonly ILogger<ValidarApiInternaFiltro> _logger;

    public ValidarApiInternaFiltro(IServicoConfiguracaoSistema servicoConfiguracaoSistema, ILogger<ValidarApiInternaFiltro> logger)
    {
        _servicoConfiguracaoSistema = servicoConfiguracaoSistema;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(CabecalhoChaveApi, out var chaveInformada))
        {
            context.Result = new UnauthorizedObjectResult(RespostaPadronizada<string>.Falha("Cabeçalho X-Api-Key não informado."));
            return;
        }

        var configuracao = await _servicoConfiguracaoSistema.ObterAsync(context.HttpContext.RequestAborted);
        if (!string.Equals(configuracao.ChaveApiInterna, chaveInformada.ToString(), StringComparison.Ordinal))
        {
            _logger.LogWarning("Tentativa de ingestão com chave inválida. Origem: {Origem}", context.HttpContext.Connection.RemoteIpAddress);
            context.Result = new UnauthorizedObjectResult(RespostaPadronizada<string>.Falha("Chave de API inválida."));
            return;
        }

        await next();
    }
}
