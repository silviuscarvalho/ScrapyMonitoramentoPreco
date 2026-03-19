using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Dominio.Entidades;

public sealed class DispositivoRede : EntidadeBase
{
    public string Ip { get; set; } = string.Empty;
    public string Mac { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public StatusDispositivo Status { get; set; } = StatusDispositivo.Ativo;
    public string? SistemaOperacional { get; set; }
    public string? Observacoes { get; set; }
    public DateTime PrimeiroVistoUtc { get; set; }
    public DateTime UltimaDeteccaoUtc { get; set; }
}
