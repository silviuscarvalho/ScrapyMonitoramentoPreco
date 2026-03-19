using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitoramentoRede.Aplicacao.Contratos.Servicos;
using MonitoramentoRede.Aplicacao.Dtos;
using MonitoramentoRede.Aplicacao.Dtos.Entradas;
using MonitoramentoRede.Web.Autenticacao;

namespace MonitoramentoRede.Web.Pages.Administracao.Usuarios;

/// <summary>
/// Permite listar, criar e editar usuários internos.
/// </summary>
public sealed class IndexModel : PageModel
{
    private readonly IServicoUsuario _servicoUsuario;

    public IndexModel(IServicoUsuario servicoUsuario)
    {
        _servicoUsuario = servicoUsuario;
    }

    [BindProperty(SupportsGet = true)]
    public long? IdEdicao { get; set; }

    [BindProperty]
    public FormularioUsuario Formulario { get; set; } = new();

    public IReadOnlyCollection<UsuarioSistemaDto> Usuarios { get; private set; } = [];
    public IReadOnlyCollection<PerfilAcessoDto> Perfis { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await CarregarAsync(cancellationToken);

        if (IdEdicao.HasValue)
        {
            var usuario = await _servicoUsuario.ObterPorIdAsync(IdEdicao.Value, cancellationToken);
            if (usuario is not null)
            {
                Formulario = new FormularioUsuario
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Login = usuario.Login,
                    Email = usuario.Email,
                    PerfilAcessoId = usuario.PerfilAcessoId,
                    Ativo = usuario.Ativo
                };
            }
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Formulario.Id > 0)
        {
            await _servicoUsuario.AtualizarAsync(new AtualizarUsuarioDto
            {
                Id = Formulario.Id,
                Nome = Formulario.Nome,
                Email = Formulario.Email,
                PerfilAcessoId = Formulario.PerfilAcessoId,
                Senha = Formulario.Senha,
                Ativo = Formulario.Ativo
            }, User.ObterIdUsuario(), User.Identity?.Name ?? "Administrador", cancellationToken);
        }
        else
        {
            await _servicoUsuario.CriarAsync(new CriarUsuarioDto
            {
                Nome = Formulario.Nome,
                Login = Formulario.Login,
                Email = Formulario.Email,
                Senha = Formulario.Senha ?? "Admin@123",
                PerfilAcessoId = Formulario.PerfilAcessoId,
                Ativo = Formulario.Ativo
            }, User.ObterIdUsuario(), User.Identity?.Name ?? "Administrador", cancellationToken);
        }

        return RedirectToPage();
    }

    private async Task CarregarAsync(CancellationToken cancellationToken)
    {
        Usuarios = await _servicoUsuario.ListarAsync(cancellationToken);
        Perfis = await _servicoUsuario.ListarPerfisAsync(cancellationToken);
    }

    public sealed class FormularioUsuario
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Senha { get; set; }
        public long PerfilAcessoId { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
