namespace MonitoramentoRede.Dominio.Entidades;

public sealed class UsuarioSistema : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public long PerfilAcessoId { get; set; }
    public bool Ativo { get; set; }
    public DateTime? UltimoLoginUtc { get; set; }
    public PerfilAcesso? PerfilAcesso { get; set; }
}
