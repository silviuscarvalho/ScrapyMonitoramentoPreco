using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoramentoRede.Aplicacao.Contratos.Infraestrutura;
using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Infraestrutura.Configuracoes;
using MonitoramentoRede.Infraestrutura.Dados;
using MonitoramentoRede.Infraestrutura.Repositorios;
using MonitoramentoRede.Infraestrutura.Servicos;

namespace MonitoramentoRede.Infraestrutura.Extensoes;

public static class RegistroServicosInfraestruturaExtensions
{
    public static IServiceCollection AdicionarInfraestruturaMonitoramentoRede(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcaoBancoDados>(configuration.GetSection(OpcaoBancoDados.Secao));

        services.AddScoped<IFabricaConexaoSql, FabricaConexaoSql>();

        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IDispositivoRepositorio, DispositivoRepositorio>();
        services.AddScoped<IEventoDnsRepositorio, EventoDnsRepositorio>();
        services.AddScoped<IFluxoRedeRepositorio, FluxoRedeRepositorio>();
        services.AddScoped<IAlertaRedeRepositorio, AlertaRedeRepositorio>();
        services.AddScoped<IAuditoriaRepositorio, AuditoriaRepositorio>();
        services.AddScoped<IConfiguracaoSistemaRepositorio, ConfiguracaoSistemaRepositorio>();

        services.AddScoped<IServicoAuditoria, ServicoAuditoria>();
        services.AddScoped<IServicoDashboard, ServicoDashboard>();
        services.AddScoped<IServicoUsuario, ServicoUsuario>();
        services.AddScoped<IServicoConfiguracaoSistema, ServicoConfiguracaoSistema>();
        services.AddScoped<IServicoDispositivo, ServicoDispositivo>();
        services.AddScoped<IServicoEventoDns, ServicoEventoDns>();
        services.AddScoped<IServicoFluxoRede, ServicoFluxoRede>();
        services.AddScoped<IServicoAlertaRede, ServicoAlertaRede>();

        return services;
    }
}
