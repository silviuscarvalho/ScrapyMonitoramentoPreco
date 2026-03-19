namespace MonitoramentoRede.Dominio.Entidades;

public sealed class PerfilAcesso : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
