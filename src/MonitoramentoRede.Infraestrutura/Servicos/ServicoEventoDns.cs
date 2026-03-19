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
/// Implementa ingestão e consulta de eventos DNS.
/// </summary>
public sealed class ServicoEventoDns : IServicoEventoDns
{
    private readonly IDispositivoRepositorio _dispositivoRepositorio;
    private readonly IEventoDnsRepositorio _eventoDnsRepositorio;
    private readonly IAlertaRedeRepositorio _alertaRedeRepositorio;
    private readonly IConfiguracaoSistemaRepositorio _configuracaoRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoEventoDns(
        IDispositivoRepositorio dispositivoRepositorio,
        IEventoDnsRepositorio eventoDnsRepositorio,
        IAlertaRedeRepositorio alertaRedeRepositorio,
        IConfiguracaoSistemaRepositorio configuracaoRepositorio,
        IServicoAuditoria servicoAuditoria)
    {
        _dispositivoRepositorio = dispositivoRepositorio;
        _eventoDnsRepositorio = eventoDnsRepositorio;
        _alertaRedeRepositorio = alertaRedeRepositorio;
        _configuracaoRepositorio = configuracaoRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken) =>
        _eventoDnsRepositorio.ListarAsync(filtro, cancellationToken);

    public async Task<long> IngerirAsync(EventoDnsEntradaDto dto, CancellationToken cancellationToken)
    {
        var dispositivo = await ObterOuCriarDispositivoAsync(dto.IpDispositivo, dto.MacDispositivo, dto.Hostname, cancellationToken);
        var dominioJaVisto = await _eventoDnsRepositorio.DominioJaVistoAsync(dto.Dominio, cancellationToken);

        var evento = new EventoDns
        {
            DispositivoRedeId = dispositivo.Id,
            Dominio = dto.Dominio,
            TipoRegistro = dto.TipoRegistro,
            StatusConsulta = dto.StatusConsulta,
            Resposta = dto.Resposta,
            TempoRespostaMs = dto.TempoRespostaMs,
            EnderecoOrigem = dto.IpDispositivo,
            DataEventoUtc = dto.DataEventoUtc
        };

        var id = await _eventoDnsRepositorio.InserirAsync(evento, cancellationToken);
        await AvaliarAlertasAsync(dispositivo.Id, dto.Dominio, dominioJaVisto, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(null, "Sistema", "IngestaoDns", "EventoDns", dto.Dominio, true, dto.IpDispositivo, cancellationToken);
        return id;
    }

    public async Task<string> GerarCsvAsync(FiltroEventoDns filtro, CancellationToken cancellationToken)
    {
        filtro.Pagina = 1;
        filtro.TamanhoPagina = 5000;
        var resultado = await _eventoDnsRepositorio.ListarAsync(filtro, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("Id,Dispositivo,Dominio,TipoRegistro,StatusConsulta,Resposta,TempoRespostaMs,DataEventoUtc");

        foreach (var item in resultado.Itens)
        {
            sb.AppendLine($"{item.Id},\"{item.Dispositivo}\",\"{item.Dominio}\",{item.TipoRegistro},{item.StatusConsulta},\"{item.Resposta}\",{item.TempoRespostaMs},{item.DataEventoUtc:o}");
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

    private async Task AvaliarAlertasAsync(long dispositivoId, string dominio, bool dominioJaVisto, CancellationToken cancellationToken)
    {
        var configuracao = await _configuracaoRepositorio.ObterAsync(cancellationToken);

        if (!dominioJaVisto)
        {
            await _alertaRedeRepositorio.InserirAsync(new AlertaRede
            {
                DispositivoRedeId = dispositivoId,
                Tipo = TipoAlerta.DominioNuncaVisto,
                Severidade = SeveridadeAlerta.Media,
                Status = StatusAlerta.Aberto,
                Titulo = "Domínio nunca visto antes",
                Descricao = $"O domínio {dominio} foi observado pela primeira vez.",
                DataCriacaoUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        var janelaInicioUtc = DateTime.UtcNow.AddMinutes(-configuracao.JanelaExcessoDnsMinutos);
        var total = await _eventoDnsRepositorio.ContarConsultasPorJanelaAsync(dispositivoId, dominio, janelaInicioUtc, cancellationToken);
        if (total >= configuracao.LimiteConsultasDnsJanela)
        {
            await _alertaRedeRepositorio.InserirAsync(new AlertaRede
            {
                DispositivoRedeId = dispositivoId,
                Tipo = TipoAlerta.ExcessoConsultasDns,
                Severidade = SeveridadeAlerta.Alta,
                Status = StatusAlerta.Aberto,
                Titulo = "Excesso de consultas DNS",
                Descricao = $"O domínio {dominio} excedeu o limite de {configuracao.LimiteConsultasDnsJanela} consultas na janela.",
                DataCriacaoUtc = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
