using MonitoramentoRede.Compartilhado.Modelos.Configuracao;
using MonitoramentoRede.Coletor.Fluxo.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<OpcaoApiInterna>(builder.Configuration.GetSection(OpcaoApiInterna.Secao));
builder.Services.Configure<OpcaoColetorArquivos>(builder.Configuration.GetSection(OpcaoColetorArquivos.Secao));
builder.Services.AddHttpClient();
builder.Services.AddHostedService<TrabalhadorColetorFluxo>();

var host = builder.Build();
host.Run();
