namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class LogAuditoriaDto
{
    public long Id { get; init; }
    public string UsuarioNome { get; init; } = string.Empty;
    public string Acao { get; init; } = string.Empty;
    public string Entidade { get; init; } = string.Empty;
    public string? Dados { get; init; }
    public bool Sucesso { get; init; }
    public string? EnderecoIp { get; init; }
    public DateTime DataEventoUtc { get; init; }
}
