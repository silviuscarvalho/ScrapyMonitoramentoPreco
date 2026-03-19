using System.Text;
using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Implementa ingestão e consulta de fluxos de rede.
/// </summary>
public sealed class ServicoFluxoRede : IServicoFluxoRede
{
    private readonly IDispositivoRepositorio _dispositivoRepositorio;
    private readonly IFluxoRedeRepositorio _fluxoRedeRepositorio;
    private readonly IAlertaRedeRepositorio _alertaRedeRepositorio;
    private readonly IConfiguracaoSistemaRepositorio _configuracaoRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoFluxoRede(
        IDispositivoRepositorio dispositivoRepositorio,
        IFluxoRedeRepositorio fluxoRedeRepositorio,
        IAlertaRedeRepositorio alertaRedeRepositorio,
        IConfiguracaoSistemaRepositorio configuracaoRepositorio,
        IServicoAuditoria servicoAuditoria)
    {
        _dispositivoRepositorio = dispositivoRepositorio;
        _fluxoRedeRepositorio = fluxoRedeRepositorio;
        _alertaRedeRepositorio = alertaRedeRepositorio;
        _configuracaoRepositorio = configuracaoRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken) =>
        _fluxoRedeRepositorio.ListarAsync(filtro, cancellationToken);

    public async Task<long> IngerirAsync(FluxoRedeEntradaDto dto, CancellationToken cancellationToken)
    {
        var dispositivo = await ObterOuCriarDispositivoAsync(dto.IpDispositivo, dto.MacDispositivo, dto.Hostname, cancellationToken);

        var fluxo = new FluxoRede
        {
            DispositivoRedeId = dispositivo.Id,
            IpDestino = dto.IpDestino,
            PortaDestino = dto.PortaDestino,
            Protocolo = dto.Protocolo,
            BytesEnviados = dto.BytesEnviados,
            BytesRecebidos = dto.BytesRecebidos,
            DominioCorrelacionado = dto.DominioCorrelacionado,
            DataInicioUtc = dto.DataInicioUtc,
            DataFimUtc = dto.DataFimUtc
        };

        var id = await _fluxoRedeRepositorio.InserirAsync(fluxo, cancellationToken);
        await AvaliarAlertasAsync(dispositivo.Id, fluxo, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(null, "Sistema", "IngestaoFluxo", "FluxoRede", dto.IpDestino, true, dto.IpDispositivo, cancellationToken);
        return id;
    }

    public async Task<string> GerarCsvAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken)
    {
        filtro.Pagina = 1;
        filtro.TamanhoPagina = 5000;
        var resultado = await _fluxoRedeRepositorio.ListarAsync(filtro, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("Id,Dispositivo,IpDestino,PortaDestino,Protocolo,BytesEnviados,BytesRecebidos,DominioCorrelacionado,DataInicioUtc,DataFimUtc");

        foreach (var item in resultado.Itens)
        {
            sb.AppendLine($"{item.Id},\"{item.Dispositivo}\",{item.IpDestino},{item.PortaDestino},{item.Protocolo},{item.BytesEnviados},{item.BytesRecebidos},\"{item.DominioCorrelacionado}\",{item.DataInicioUtc:o},{item.DataFimUtc:o}");
        }

        return sb.ToString();
    }

    private async Task<DispositivoRede> ObterOuCriarDispositivoAsync(string ip, string mac, string hostname, CancellationToken cancellationToken)
    {
        var dispositivo = await _dispositivoRepositorio.ObterPorChavesRedeAsync(ip, mac, cancellationToken);
        if (dispositivo is not null)
        {
            dispositivo.Hostname = hostname;
            dispositivo.Status = StatusDispositivo.Ativo;
            dispositivo.UltimaDeteccaoUtc = DateTime.UtcNow;
            await _dispositivoRepositorio.AtualizarAsync(dispositivo, cancellationToken);
            return dispositivo;
        }

        dispositivo = new DispositivoRede
        {
            Ip = ip,
            Mac = mac,
            Hostname = hostname,
            Status = StatusDispositivo.Ativo,
            PrimeiroVistoUtc = DateTime.UtcNow,
            UltimaDeteccaoUtc = DateTime.UtcNow
        };

        dispositivo.Id = await _dispositivoRepositorio.InserirAsync(dispositivo, cancellationToken);
        return dispositivo;
    }

    private async Task AvaliarAlertasAsync(long dispositivoId, FluxoRede fluxo, CancellationToken cancellationToken)
    {
        var configuracao = await _configuracaoRepositorio.ObterAsync(cancellationToken);
        var totalBytes = fluxo.BytesEnviados + fluxo.BytesRecebidos;

        if (totalBytes >= configuracao.LimitePicoTrafegoBytes)
        {
            await _alertaRedeRepositorio.InserirAsync(new AlertaRede
            {
                DispositivoRedeId = dispositivoId,
                Tipo = TipoAlerta.PicoTrafego,
                Severidade = SeveridadeAlerta.Alta,
                Status = StatusAlerta.Aberto,
                Titulo = "Pico de tráfego detectado",
                Descricao = $"Fluxo para {fluxo.IpDestino}:{fluxo.PortaDestino} excedeu o limite configurado.",
                DataCriacaoUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        var portasIncomuns = configuracao.PortasIncomuns
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToHashSet();

        if (portasIncomuns.Contains(fluxo.PortaDestino))
        {
            await _alertaRedeRepositorio.InserirAsync(new AlertaRede
            {
                DispositivoRedeId = dispositivoId,
                Tipo = TipoAlerta.PortaIncomum,
                Severidade = SeveridadeAlerta.Media,
                Status = StatusAlerta.Aberto,
                Titulo = "Porta incomum observada",
                Descricao = $"Fluxo para a porta {fluxo.PortaDestino} foi identificado.",
                DataCriacaoUtc = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
