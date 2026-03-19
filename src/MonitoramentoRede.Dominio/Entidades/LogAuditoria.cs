namespace MonitoramentoRede.Dominio.Entidades;

public sealed class LogAuditoria : EntidadeBase
{
    public long? UsuarioSistemaId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string? Dados { get; set; }
    public string? EnderecoIp { get; set; }
    public bool Sucesso { get; set; }
    public DateTime DataEventoUtc { get; set; }
}
