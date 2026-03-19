namespace MonitoramentoRede.Infraestrutura.Configuracoes;

public sealed class OpcaoBancoDados
{
    public const string Secao = "BancoDados";

    public string ConnectionString { get; set; } = string.Empty;
}
