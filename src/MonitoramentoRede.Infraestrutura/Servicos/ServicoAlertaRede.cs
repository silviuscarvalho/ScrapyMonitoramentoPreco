using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Enums;

namespace MonitoramentoRede.Infraestrutura.Servicos;

/// <summary>
/// Implementa listagem e resolução de alertas.
/// </summary>
public sealed class ServicoAlertaRede : IServicoAlertaRede
{
    private readonly IAlertaRedeRepositorio _alertaRedeRepositorio;
    private readonly IServicoAuditoria _servicoAuditoria;

    public ServicoAlertaRede(IAlertaRedeRepositorio alertaRedeRepositorio, IServicoAuditoria servicoAuditoria)
    {
        _alertaRedeRepositorio = alertaRedeRepositorio;
        _servicoAuditoria = servicoAuditoria;
    }

    public Task<ResultadoPaginado<AlertaRedeDto>> ListarAsync(FiltroAlertaRede filtro, CancellationToken cancellationToken) =>
        _alertaRedeRepositorio.ListarAsync(filtro, cancellationToken);

    public async Task ResolverAsync(ResolverAlertaDto dto, CancellationToken cancellationToken)
    {
        var alerta = await _alertaRedeRepositorio.ObterPorIdAsync(dto.Id, cancellationToken) ?? throw new InvalidOperationException("Alerta não encontrado.");
        alerta.Status = StatusAlerta.Resolvido;
        alerta.ObservacaoOperador = dto.ObservacaoOperador;
        alerta.DataResolucaoUtc = DateTime.UtcNow;

        await _alertaRedeRepositorio.AtualizarAsync(alerta, cancellationToken);
        await _servicoAuditoria.RegistrarAsync(dto.UsuarioId, dto.UsuarioNome, "ResolucaoAlerta", "AlertaRede", $"Alerta {dto.Id} resolvido", true, null, cancellationToken);
    }
}
