namespace MonitoramentoRede.Compartilhado.Modelos.Api;

public sealed class RespostaPadronizada<T>
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public T? Dados { get; init; }
    public IReadOnlyCollection<string> Erros { get; init; } = [];

    public static RespostaPadronizada<T> Ok(T? dados, string mensagem = "") =>
        new()
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados
        };

    public static RespostaPadronizada<T> Falha(string mensagem, params string[] erros) =>
        new()
        {
            Sucesso = false,
            Mensagem = mensagem,
            Erros = erros
        };
}
