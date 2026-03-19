using System.ComponentModel.DataAnnotations;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class AtualizarUsuarioDto
{
    public long Id { get; init; }

    [Required]
    public string Nome { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string? Senha { get; init; }
    public long PerfilAcessoId { get; init; }
    public bool Ativo { get; init; }
}
