using MonitoramentoRede.Aplicacao.Contratos.Repositorios;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Aplicacao.Filtros;
using MonitoramentoRede.Compartilhado.Modelos.Paginacao;
using MonitoramentoRede.Dominio.Entidades;
using MonitoramentoRede.Infraestrutura.Repositorios;
using MonitoramentoRede.Infraestrutura.Servicos;

namespace MonitoramentoRede.Testes.Unitarios;

public sealed class ServicoUsuarioTestes
{
    [Fact]
    public async Task AutenticarAsync_DeveRetornarUsuario_QuandoCredenciaisForemValidas()
    {
        var repositorio = new UsuarioRepositorioFalso
        {
            Usuario = new UsuarioSistema
            {
                Id = 10,
                Nome = "Operador",
                Login = "operador",
                Ativo = true,
                SenhaHash = UsuarioRepositorio.CalcularHashSenha("Senha@123"),
                PerfilAcesso = new PerfilAcesso { Id = 2, Nome = "Operador" }
            }
        };

        var servico = new ServicoUsuario(repositorio, new ServicoAuditoriaFalso());
        var autenticado = await servico.AutenticarAsync(new LoginDto { Login = "operador", Senha = "Senha@123" }, CancellationToken.None);

        Assert.NotNull(autenticado);
        Assert.Equal("Operador", autenticado!.Perfil);
    }

    [Fact]
    public async Task RegistrarDeteccaoAsync_DeveCriarAlerta_QuandoDispositivoForNovo()
    {
        var dispositivoRepositorio = new DispositivoRepositorioFalso();
        var alertaRepositorio = new AlertaRepositorioFalso();
        var servico = new ServicoDispositivo(
            dispositivoRepositorio,
            new EventoDnsRepositorioFalso(),
            new FluxoRedeRepositorioFalso(),
            alertaRepositorio,
            new ServicoAuditoriaFalso());

        var id = await servico.RegistrarDeteccaoAsync(new DispositivoDetectadoEntradaDto
        {
            Ip = "10.0.0.10",
            Mac = "AA-BB-CC-DD-EE-FF",
            Hostname = "host-novo",
            DataDeteccaoUtc = DateTime.UtcNow
        }, CancellationToken.None);

        Assert.True(id > 0);
        Assert.Single(alertaRepositorio.Itens);
    }

    private sealed class UsuarioRepositorioFalso : IUsuarioRepositorio
    {
        public UsuarioSistema? Usuario { get; set; }

        public Task AtualizarAsync(UsuarioSistema usuario, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task AtualizarUltimoLoginAsync(long usuarioId, DateTime dataUtc, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<long> InserirAsync(UsuarioSistema usuario, CancellationToken cancellationToken) => Task.FromResult(1L);
        public Task<IReadOnlyCollection<UsuarioSistemaDto>> ListarAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<UsuarioSistemaDto>>([]);
        public Task<IReadOnlyCollection<PerfilAcessoDto>> ListarPerfisAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<PerfilAcessoDto>>([]);
        public Task<UsuarioSistema?> ObterPorIdAsync(long id, CancellationToken cancellationToken) => Task.FromResult<UsuarioSistema?>(Usuario);
        public Task<UsuarioSistema?> ObterPorLoginAsync(string login, CancellationToken cancellationToken) => Task.FromResult(Usuario);
    }

    private sealed class DispositivoRepositorioFalso : IDispositivoRepositorio
    {
        private long _contador = 1;
        public List<DispositivoRede> Itens { get; } = [];

        public Task AtualizarAsync(DispositivoRede dispositivo, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<int> ContarAtivosAsync(CancellationToken cancellationToken) => Task.FromResult(Itens.Count);
        public Task<long> InserirAsync(DispositivoRede dispositivo, CancellationToken cancellationToken)
        {
            dispositivo.Id = _contador++;
            Itens.Add(dispositivo);
            return Task.FromResult(dispositivo.Id);
        }
        public Task<ResultadoPaginado<DispositivoDto>> ListarAsync(FiltroDispositivo filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<DispositivoDto>());
        public Task<DispositivoDto?> ObterDtoPorIdAsync(long id, CancellationToken cancellationToken) => Task.FromResult<DispositivoDto?>(null);
        public Task<DispositivoRede?> ObterPorChavesRedeAsync(string ip, string mac, CancellationToken cancellationToken) => Task.FromResult<DispositivoRede?>(null);
        public Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<TopDominioDto>>([]);
        public Task<IReadOnlyCollection<TopDestinoDto>> ObterTopDestinosAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<TopDestinoDto>>([]);
    }

    private sealed class AlertaRepositorioFalso : IAlertaRedeRepositorio
    {
        public List<AlertaRede> Itens { get; } = [];

        public Task<int> ContarAbertosAsync(CancellationToken cancellationToken) => Task.FromResult(Itens.Count);
        public Task<long> InserirAsync(AlertaRede alerta, CancellationToken cancellationToken)
        {
            Itens.Add(alerta);
            return Task.FromResult((long)Itens.Count);
        }
        public Task<ResultadoPaginado<AlertaRedeDto>> ListarAsync(FiltroAlertaRede filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<AlertaRedeDto>());
        public Task<IReadOnlyCollection<AlertaRedeDto>> ListarPorDispositivoAsync(long dispositivoId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<AlertaRedeDto>>([]);
        public Task<AlertaRede?> ObterPorIdAsync(long id, CancellationToken cancellationToken) => Task.FromResult<AlertaRede?>(null);
        public Task AtualizarAsync(AlertaRede alerta, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class EventoDnsRepositorioFalso : IEventoDnsRepositorio
    {
        public Task<int> ContarConsultasPorJanelaAsync(long dispositivoId, string dominio, DateTime inicioUtc, CancellationToken cancellationToken) => Task.FromResult(0);
        public Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken) => Task.FromResult(0);
        public Task<bool> DominioJaVistoAsync(string dominio, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<string> Dummy() => Task.FromResult(string.Empty);
        public Task<long> InserirAsync(EventoDns eventoDns, CancellationToken cancellationToken) => Task.FromResult(0L);
        public Task<ResultadoPaginado<EventoDnsDto>> ListarAsync(FiltroEventoDns filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<EventoDnsDto>());
        public Task<IReadOnlyCollection<EventoDnsDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<EventoDnsDto>>([]);
        public Task<IReadOnlyCollection<TopDominioDto>> ObterTopDominiosAsync(int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<TopDominioDto>>([]);
    }

    private sealed class FluxoRedeRepositorioFalso : IFluxoRedeRepositorio
    {
        public Task<int> ContarUltimas24HorasAsync(CancellationToken cancellationToken) => Task.FromResult(0);
        public Task<long> InserirAsync(FluxoRede fluxoRede, CancellationToken cancellationToken) => Task.FromResult(0L);
        public Task<ResultadoPaginado<FluxoRedeDto>> ListarAsync(FiltroFluxoRede filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<FluxoRedeDto>());
        public Task<IReadOnlyCollection<FluxoRedeDto>> ListarPorDispositivoAsync(long dispositivoId, int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<FluxoRedeDto>>([]);
        public Task<IReadOnlyCollection<TopDispositivoTrafegoDto>> ObterTopDispositivosTrafegoAsync(int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<TopDispositivoTrafegoDto>>([]);
    }

    private sealed class ServicoAuditoriaFalso : IServicoAuditoria
    {
        public Task<ResultadoPaginado<LogAuditoriaDto>> ListarAsync(FiltroAuditoria filtro, CancellationToken cancellationToken) => Task.FromResult(new ResultadoPaginado<LogAuditoriaDto>());
        public Task<IReadOnlyCollection<LogAuditoriaDto>> ListarRecentesAsync(int quantidade, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<LogAuditoriaDto>>([]);
        public Task RegistrarAsync(long? usuarioId, string usuarioNome, string acao, string entidade, string? dados, bool sucesso, string? enderecoIp, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
