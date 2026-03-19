namespace MonitoramentoRede.Aplicacao.Dtos;

public sealed class TopDispositivoTrafegoDto
{
    public string Dispositivo { get; init; } = string.Empty;
    public long TotalBytes { get; init; }
}
