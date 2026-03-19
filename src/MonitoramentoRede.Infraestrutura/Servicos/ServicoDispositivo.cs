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
/// Implementa o caso de uso de dispositivos.
/// </summary>
public sealed class ServicoDispositivo : IServicoDispositivo
{
    private readonly IDispositivoRepositorio _dispositivoRepositorio;
    private readonly IEventoDnsRepositorio _eventoDnsRepositorio;
    private readonly IFluxoRedeRepositorio _fluxoRedeRepositorio;
    private readonly IAlertaRedeRepositorio _alertaRedeRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoDispositivo(
        IDispositivoRepositorio dispositivoRepositorio,
        IEventoDnsRepositorio eventoDnsRepositorio,
        IFluxoRedeRepositorio fluxoRedeRepositorio,
        IAlertaRedeRepositorio alertaRedeRepositorio,
        IServicoAuditoria servicoAuditoria)
    {
        _dispositivoRepositorio = dispositivoRepositorio;
        _eventoDnsRepositorio = eventoDnsRepositorio;
        _fluxoRedeRepositorio = fluxoRedeRepositorio;
        _alertaRedeRepositorio = alertaRedeRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken) =>
        _dispositivoRepositorio.ListarAsync(filtro, cancellationToken);

    public async Task<DispositivoDetalhesDto?> ObterDetalhesAsync(long id, CancellationToken cancellationToken)
    {
        var dispositivo = await _dispositivoRepositorio.ObterDtoPorIdAsync(id, cancellationToken);
        if (dispositivo is null)
        {
            return null;
        }

        return new DispositivoDetalhesDto
        {
            Dispositivo = dispositivo,
            TopDominios = await _dispositivoRepositorio.ObterTopDominiosAsync(id, 5, cancellationToken),
            TopDestinos = await _dispositivoRepositorio.ObterTopDestinosAsync(id, 5, cancellationToken),
            EventosDns = await _eventoDnsRepositorio.ListarPorDispositivoAsync(id, 20, cancellationToken),
            FluxosRede = await _fluxoRedeRepositorio.ListarPorDispositivoAsync(id, 20, cancellationToken),
            Alertas = await _alertaRedeRepositorio.ListarPorDispositivoAsync(id, cancellationToken)
        };
    }

    public async Task<long> RegistrarDeteccaoAsync(DispositivoDetectadoEntradaDto dto, CancellationToken cancellationToken)
    {
        var dispositivo = await _dispositivoRepositorio.ObterPorChavesRedeAsync(dto.Ip, dto.Mac, cancellationToken);
        var ehNovo = dispositivo is null;

        if (dispositivo is null)
        {
            dispositivo = new DispositivoRede
            {
                Ip = dto.Ip,
                Mac = dto.Mac,
                Hostname = dto.Hostname,
                Status = StatusDispositivo.Ativo,
                SistemaOperacional = dto.SistemaOperacional,
                PrimeiroVistoUtc = dto.DataDeteccaoUtc,
                UltimaDeteccaoUtc = dto.DataDeteccaoUtc
            };

            dispositivo.Id = await _dispositivoRepositorio.InserirAsync(dispositivo, cancellationToken);
        }
        else
        {
            dispositivo.Hostname = dto.Hostname;
            dispositivo.Status = StatusDispositivo.Ativo;
            dispositivo.SistemaOperacional = dto.SistemaOperacional;
            dispositivo.UltimaDeteccaoUtc = dto.DataDeteccaoUtc;
            await _dispositivoRepositorio.AtualizarAsync(dispositivo, cancellationToken);
        }

        if (ehNovo)
        {
            await _alertaRedeRepositorio.InserirAsync(new AlertaRede
            {
                DispositivoRedeId = dispositivo.Id,
                Tipo = TipoAlerta.DispositivoNovo,
                Severidade = SeveridadeAlerta.Media,
                Status = StatusAlerta.Aberto,
                Titulo = "Novo dispositivo detectado",
                Descricao = $"Dispositivo {dto.Hostname} ({dto.Ip}) detectado pela primeira vez.",
                DataCriacaoUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        await _servicoAuditoria.RegistrarAsync(null, "Sistema", "DeteccaoDispositivo", "DispositivoRede", $"{dto.Hostname}|{dto.Ip}", true, null, cancellationToken);
        return dispositivo.Id;
    }
}
