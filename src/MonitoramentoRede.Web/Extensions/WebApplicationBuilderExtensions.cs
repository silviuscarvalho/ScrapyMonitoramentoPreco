using Microsoft.AspNetCore.Authentication.Cookies;
using MonitoramentoRede.Compartilhado.Constantes;
using MonitoramentoRede.Infraestrutura.Extensoes;
using MonitoramentoRede.Web.Autenticacao;

namespace MonitoramentoRede.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AdicionarServicosMonitoramento(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/");
            options.Conventions.AllowAnonymousToPage("/Conta/Login");
            options.Conventions.AllowAnonymousToPage("/Conta/AcessoNegado");
            options.Conventions.AllowAnonymousToPage("/Error");
            options.Conventions.AuthorizeFolder("/Administracao", PerfisSistema.Administrador);
        });

        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ValidarApiInternaFiltro>();

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Conta/Login";
                options.AccessDeniedPath = "/Conta/AcessoNegado";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(PerfisSistema.Administrador, policy => policy.RequireRole(PerfisSistema.Administrador));
        });

        builder.Services.AdicionarInfraestruturaMonitoramentoRede(builder.Configuration);
        return builder;
    }
}
