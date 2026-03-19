using MonitoramentoRede.Compartilhado.Modelos.Configuracao;
using MonitoramentoRede.Coletor.Descoberta.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<OpcaoApiInterna>(builder.Configuration.GetSection(OpcaoApiInterna.Secao));
builder.Services.Configure<OpcaoColetorArquivos>(builder.Configuration.GetSection(OpcaoColetorArquivos.Secao));
builder.Services.AddHttpClient();
builder.Services.AddHostedService<TrabalhadorColetorDescoberta>();

var host = builder.Build();
host.Run();
