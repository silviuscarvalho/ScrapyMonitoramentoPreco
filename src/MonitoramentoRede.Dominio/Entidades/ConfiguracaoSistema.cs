namespace MonitoramentoRede.Dominio.Entidades;

public sealed class ConfiguracaoSistema : EntidadeBase
{
    public string ChaveApiInterna { get; set; } = string.Empty;
    public int RetencaoDadosDias { get; set; }
    public int IntervaloAtualizacaoDashboardSegundos { get; set; }
    public long LimitePicoTrafegoBytes { get; set; }
    public int JanelaExcessoDnsMinutos { get; set; }
    public int LimiteConsultasDnsJanela { get; set; }
    public string PortasIncomuns { get; set; } = string.Empty;
    public DateTime DataAtualizacaoUtc { get; set; }
}
