namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class UsuarioSistemaDto
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public long PerfilAcessoId { get; init; }
    public string PerfilNome { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime? UltimoLoginUtc { get; init; }
}
