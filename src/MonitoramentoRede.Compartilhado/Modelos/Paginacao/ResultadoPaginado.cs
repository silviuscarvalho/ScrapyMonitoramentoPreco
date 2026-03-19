namespace MonitoramentoRede.Compartilhado.Modelos.Paginacao;

public sealed class ResultadoPaginado<T>
{
    public IReadOnlyCollection<T> Itens { get; init; } = [];
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
}
