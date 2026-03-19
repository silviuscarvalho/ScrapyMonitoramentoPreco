namespace MonitoramentoRede.Compartilhado.Modelos.Paginacao;

public abstract class RequisicaoPaginada
{
    private int _pagina = 1;
    private int _tamanhoPagina = 20;

    public int Pagina
    {
        get => _pagina;
        set => _pagina = value <= 0 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value is <= 0 or > 200 ? 20 : value;
    }
}
