using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class DispositivoDto
{
    public long Id { get; init; }
    public string Ip { get; init; } = string.Empty;
    public string Mac { get; init; } = string.Empty;
    public string Hostname { get; init; } = string.Empty;
    public StatusDispositivo Status { get; init; }
    public string? SistemaOperacional { get; init; }
    public DateTime PrimeiroVistoUtc { get; init; }
    public DateTime UltimaDeteccaoUtc { get; init; }
}
