namespace MonitoramentoRede.Compartilhado.Modelos.Seguranca;

public sealed class UsuarioAutenticado
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Login { get; init; } = string.Empty;
    public string Perfil { get; init; } = string.Empty;
}
