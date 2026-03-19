using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Testes.Integracao;

namespace MonitoramentoRede.Testes.Integracao;

public sealed class IngestaoInternaControllerTestes : IClassFixture<FabricaAplicacaoWeb>
{
    private readonly FabricaAplicacaoWeb _fabrica;

    public IngestaoInternaControllerTestes(FabricaAplicacaoWeb fabrica)
    {
        _fabrica = fabrica;
    }

    [Fact]
    public async Task PostDispositivo_DeveRetornarUnauthorized_QuandoHeaderNaoForInformado()
    {
        using var cliente = _fabrica.CreateClient();
        var resposta = await cliente.PostAsJsonAsync("/api/interno/ingestao/dispositivos", new DispositivoDetectadoEntradaDto
        {
            Ip = "10.0.0.20",
            Mac = "11-22-33-44-55-66",
            Hostname = "sem-chave"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task PostDispositivo_DeveRetornarOk_QuandoHeaderForValido()
    {
        using var cliente = _fabrica.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-Api-Key", "teste-api");

        var resposta = await cliente.PostAsJsonAsync("/api/interno/ingestao/dispositivos", new DispositivoDetectadoEntradaDto
        {
            Ip = "10.0.0.21",
            Mac = "AA-22-33-44-55-66",
            Hostname = "com-chave"
        });

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }
}

public sealed class FabricaAplicacaoWeb : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IServicoConfiguracaoSistema>(new ServicoConfiguracaoSistemaTeste());
            services.AddSingleton<IServicoDispositivo>(new ServicoDispositivoTeste());
            services.AddSingleton<IServicoEventoDns>(new ServicoEventoDnsTeste());
            services.AddSingleton<IServicoFluxoRede>(new ServicoFluxoRedeTeste());
            services.AddSingleton<IServicoAuditoria>(new ServicoAuditoriaTeste());
        });
    }

    private sealed class ServicoConfiguracaoSistemaTeste : IServicoConfiguracaoSistema
    {
        public Task AtualizarAsync(AtualizarConfiguracaoSistemaDto dto, long usuarioExecutorId, string usuarioExecutorNome, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<ConfiguracaoSistemaDto> ObterAsync(CancellationToken cancellationToken) => Task.FromResult(new ConfiguracaoSistemaDto { Id = 1, ChaveApiInterna = "teste-api", PortasIncomuns = "23", DataAtualizacaoUtc = DateTime.UtcNow });
    }

    private sealed class ServicoDispositivoTeste : IServicoDispositivo
    {
        public Task<DispositivoDetalhesDto?> ObterDetalhesAsync(long id, CancellationToken cancellationToken) => Task.FromResult<DispositivoDetalhesDto?>(null);
        public Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<DispositivoDto>());
        public Task<long> RegistrarDeteccaoAsync(DispositivoDetectadoEntradaDto dto, CancellationToken cancellationToken) => Task.FromResult(1L);
    }

    private sealed class ServicoEventoDnsTeste : IServicoEventoDns
    {
        public Task<string> GerarCsvAsync(FiltroEventoDns filtro, CancellationToken cancellationToken) => Task.FromResult(string.Empty);
        public Task<long> IngerirAsync(EventoDnsEntradaDto dto, CancellationToken cancellationToken) => Task.FromResult(1L);
        public Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<EventoDnsDto>());
    }

    private sealed class ServicoFluxoRedeTeste : IServicoFluxoRede
    {
        public Task<string> GerarCsvAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken) => Task.FromResult(string.Empty);
        public Task<long> IngerirAsync(FluxoRedeEntradaDto dto, CancellationToken cancellationToken) => Task.FromResult(1L);
        public Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<FluxoRedeDto>());
    }

    private sealed class ServicoAuditoriaTeste : IServicoAuditoria
    {
        public Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<LogAuditoriaDto>());
        public Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<LogAuditoriaDto>>([]);
        public Task RegistrarAsync(long? usuarioId, string usuarioNome, string acao, string entidade, string? dados, bool sucesso, string? enderecoIp, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
