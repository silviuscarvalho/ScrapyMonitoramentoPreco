namespace MonitoramentoRede.Compartilhado.Modelos.Configuracao;

public sealed class OpcaoColetorArquivos
{
    public const string Secao = "Coletor";

    public string PastaEntrada { get; set; } = "dados/entrada";
    public string PastaHistorico { get; set; } = "dados/historico";
    public string PastaRejeitados { get; set; } = "dados/rejeitados";
    public int IntervaloSegundos { get; set; } = 30;
}
