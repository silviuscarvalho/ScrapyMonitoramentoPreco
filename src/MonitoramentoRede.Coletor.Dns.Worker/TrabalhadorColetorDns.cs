using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Compartilhado.Modelos.Configuracao;

namespace MonitoramentoRede.Coletor.Dns.Worker;

public sealed class TrabalhadorColetorDns : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TrabalhadorColetorDns> _logger;
    private readonly OpcaoApiInterna _apiInterna;
    private readonly OpcaoColetorArquivos _coletor;

    public TrabalhadorColetorDns(
        IHttpClientFactory httpClientFactory,
        IOptions<OpcaoApiInterna> apiInterna,
        IOptions<OpcaoColetorArquivos> coletor,
        ILogger<TrabalhadorColetorDns> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiInterna = apiInterna.Value;
        _coletor = coletor.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PrepararPastas();

        while (!stoppingToken.IsCancellationRequested)
        {
            var arquivos = Directory.GetFiles(_coletor.PastaEntrada, "*.json");
            foreach (var arquivo in arquivos)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(arquivo, stoppingToken);
                    var payload = JsonSerializer.Deserialize<EventoDnsEntradaDto>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))
                        ?? throw new InvalidOperationException("Arquivo DNS inválido.");

                    var cliente = _httpClientFactory.CreateClient();
                    cliente.DefaultRequestHeaders.Add("X-Api-Key", _apiInterna.Chave);
                    var resposta = await cliente.PostAsJsonAsync($"{_apiInterna.UrlBase}/api/interno/ingestao/dns", payload, stoppingToken);
                    resposta.EnsureSuccessStatusCode();

                    File.Move(arquivo, Path.Combine(_coletor.PastaHistorico, Path.GetFileName(arquivo)), true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar arquivo DNS {Arquivo}", arquivo);
                    File.Move(arquivo, Path.Combine(_coletor.PastaRejeitados, Path.GetFileName(arquivo)), true);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(_coletor.IntervaloSegundos), stoppingToken);
        }
    }

    private void PrepararPastas()
    {
        Directory.CreateDirectory(_coletor.PastaEntrada);
        Directory.CreateDirectory(_coletor.PastaHistorico);
        Directory.CreateDirectory(_coletor.PastaRejeitados);
    }
}
