using System.ComponentModel.DataAnnotations;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class CriarUsuarioDto
{
    [Required]
    public string Nome { get; init; } = string.Empty;

    [Required]
    public string Login { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; init; } = string.Empty;

    public long PerfilAcessoId { get; init; }
    public bool Ativo { get; init; } = true;
}
