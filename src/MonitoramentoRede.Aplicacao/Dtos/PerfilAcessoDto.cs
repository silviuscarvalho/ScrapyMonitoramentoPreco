namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class PerfilAcessoDto
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
}
