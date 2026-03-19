using System.ComponentModel.DataAnnotations;

namespace MonitoramentoRede.Aplicacao.Dtos.Entradas;

public sealed class LoginDto
{
    [Required]
    public string Login { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Senha { get; init; } = string.Empty;
}
