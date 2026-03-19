namespace MonitoramentoRede.Compartilhado.Modelos.Configuracao;

public sealed class OpcaoApiInterna
{
    public const string Secao = "ApiInterna";

    public string UrlBase { get; set; } = "https://localhost:5001";
    public string Chave { get; set; } = "alterar-em-producao";
}
