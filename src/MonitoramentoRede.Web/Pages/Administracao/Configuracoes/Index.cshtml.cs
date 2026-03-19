using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Web.Autenticacao;

namespace MonitoramentoRede.Web.Pages.Administracao.Configuracoes;

/// <summary>
/// Permite administrar parâmetros operacionais do sistema.
/// </summary>
public sealed class IndexModel : PageModel
{
    private readonly IServicoConfiguracaoSistema _servicoConfiguracaoSistema;

    public IndexModel(IServicoConfiguracaoSistema servicoConfiguracaoSistema)
    {
        _servicoConfiguracaoSistema = servicoConfiguracaoSistema;
    }

    [BindProperty]
    public AtualizarConfiguracaoSistemaDto Configuracao { get; set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var configuracao = await _servicoConfiguracaoSistema.ObterAsync(cancellationToken);
        Configuracao = new AtualizarConfiguracaoSistemaDto
        {
            Id = configuracao.Id,
            ChaveApiInterna = configuracao.ChaveApiInterna,
            RetencaoDadosDias = configuracao.RetencaoDadosDias,
            IntervaloAtualizacaoDashboardSegundos = configuracao.IntervaloAtualizacaoDashboardSegundos,
            LimitePicoTrafegoBytes = configuracao.LimitePicoTrafegoBytes,
            JanelaExcessoDnsMinutos = configuracao.JanelaExcessoDnsMinutos,
            LimiteConsultasDnsJanela = configuracao.LimiteConsultasDnsJanela,
            PortasIncomuns = configuracao.PortasIncomuns
        };
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await _servicoConfiguracaoSistema.AtualizarAsync(Configuracao, User.ObterIdUsuario(), User.Identity?.Name ?? "Administrador", cancellationToken);
        return RedirectToPage();
    }
}
