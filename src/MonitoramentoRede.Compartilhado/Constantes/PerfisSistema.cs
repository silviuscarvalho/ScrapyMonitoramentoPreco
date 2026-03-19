namespace MonitoramentoRede.Compartilhado.Constantes;

public static class PerfisSistema
{
    public const string Administrador = "Administrador";
    public const string Operador = "Operador";
    public const string SomenteLeitura = "SomenteLeitura";

    public static readonly string[] Todos = [Administrador, Operador, SomenteLeitura];
    public static readonly string[] Operacionais = [Administrador, Operador];
}
