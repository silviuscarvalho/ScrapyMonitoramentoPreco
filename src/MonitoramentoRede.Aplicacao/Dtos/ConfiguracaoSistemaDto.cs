namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class ConfiguracaoSistemaDto
{
    public long Id { get; init; }
    public string ChaveApiInterna { get; init; } = string.Empty;
    public int RetencaoDadosDias { get; init; }
    public int IntervaloAtualizacaoDashboardSegundos { get; init; }
    public long LimitePicoTrafegoBytes { get; init; }
    public int JanelaExcessoDnsMinutos { get; init; }
    public int LimiteConsultasDnsJanela { get; init; }
    public string PortasIncomuns { get; init; } = string.Empty;
    public DateTime DataAtualizacaoUtc { get; init; }
}
