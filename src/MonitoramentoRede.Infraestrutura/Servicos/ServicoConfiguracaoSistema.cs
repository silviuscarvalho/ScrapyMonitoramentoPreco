using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Dominio.Entidades;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Implementa leitura e atualização de parâmetros operacionais do sistema.
/// </summary>
public sealed class ServicoConfiguracaoSistema : IServicoConfiguracaoSistema
{
    private readonly IConfiguracaoSistemaRepositorio _configuracaoRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoConfiguracaoSistema(IConfiguracaoSistemaRepositorio configuracaoRepositorio, IServicoAuditoria servicoAuditoria)
    {
        _configuracaoRepositorio = configuracaoRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public async Task<ConfiguracaoSistemaDto> ObterAsync(CancellationToken cancellationToken)
    {
        var configuracao = await _configuracaoRepositorio.ObterAsync(cancellationToken);
        return Mapear(configuracao);
    }

    public async Task AtualizarAsync(AtualizarConfiguracaoSistemaDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken)
    {
        var entidade = new ConfiguracaoSistema
        {
            Id = dto.Id,
            ChaveApiInterna = dto.ChaveApiInterna,
            RetencaoDadosDias = dto.RetencaoDadosDias,
            IntervaloAtualizacaoDashboardSegundos = dto.IntervaloAtualizacaoDashboardSegundos,
            LimitePicoTrafegoBytes = dto.LimitePicoTrafegoBytes,
            JanelaExcessoDnsMinutos = dto.JanelaExcessoDnsMinutos,
            LimiteConsultasDnsJanela = dto.LimiteConsultasDnsJanela,
            PortasIncomuns = dto.PortasIncomuns,
            DataAtualizacaoUtc = DateTime.UtcNow
        };

        await _configuracaoRepositorio.AtualizarAsync(entidade, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(usuarioExecutorId, usuarioExecutorNome, "AtualizacaoConfiguracao", "ConfiguracaoSistema", $"Configuracao {dto.Id} atualizada", true, null, cancellationToken);
    }

    private static ConfiguracaoSistemaDto Mapear(ConfiguracaoSistema configuracao) =>
        new()
        {
            Id = configuracao.Id,
            ChaveApiInterna = configuracao.ChaveApiInterna,
            RetencaoDadosDias = configuracao.RetencaoDadosDias,
            IntervaloAtualizacaoDashboardSegundos = configuracao.IntervaloAtualizacaoDashboardSegundos,
            LimitePicoTrafegoBytes = configuracao.LimitePicoTrafegoBytes,
            JanelaExcessoDnsMinutos = configuracao.JanelaExcessoDnsMinutos,
            LimiteConsultasDnsJanela = configuracao.LimiteConsultasDnsJanela,
            PortasIncomuns = configuracao.PortasIncomuns,
            DataAtualizacaoUtc = configuracao.DataAtualizacaoUtc
        };
}
