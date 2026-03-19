namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class ResolverAlertaDto
{
    public long Id { get; init; }
    public string ObservacaoOperador { get; init; } = string.Empty;
    public long UsuarioId { get; init; }
    public string UsuarioNome { get; init; } = string.Empty;
}
