using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;

namespace MonitoramentoRede.Aplicacao.Contratos.Servicos;

/// <summary>
/// Orquestra operações de consulta e cadastro de dispositivos.
/// </summary>
public interface IServicoDispositivo
{
    Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken);
    Task<DispositivoDetalhesDto?> ObterDetalhesAsync(long id, CancellationToken cancellationToken);
    Task<long> RegistrarDeteccaoAsync(DispositivoDetectadoEntradaDto dto, CancellationToken cancellationToken);
}
