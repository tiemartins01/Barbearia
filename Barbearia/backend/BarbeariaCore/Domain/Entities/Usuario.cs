using Barbearia.Core.Domain.Common;
using Barbearia.Core.Domain.Events;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.Enum;
using Barbearia.Core.Exceptions;

namespace Barbearia.Core.Domain.Entities;

/// <summary>
/// Aggregate Root responsável pelas regras de identidade, autenticação,
/// bloqueio, ativação e recuperação de senha do usuário.
/// </summary>
public sealed class Usuario : AggregateRoot // Acrescenta principalmente a capacidade de armazenar eventos de domínio
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;
    public Cpf CPF { get; private set; } = null!;
    public string Login { get; private set; } = string.Empty;
    public Senha Senha { get; private set; } = null!;
    public RolePerson Role { get; private set; }
    public bool Ativado { get; private set; }
    public string? Foto { get; private set; }
    public int TentativasLogin { get; private set; }
    public DateTime? BloqueioAte { get; private set; }
    public string? Codigo { get; private set; }
    public DateTime? CodigoRecuperacaoExpiraEm { get; private set; }
    public int TentativasCodigo { get; private set; }
    public bool CodigoAtivo { get; private set; }


    private Usuario() { }

    public Usuario(
        string nome,
        Email email,
        Phone phone,
        Cpf cpf,
        string login,
        Senha senha,
        RolePerson role,
        bool ativado,
        string? foto)
    {
        Nome = ValidarNome(nome);
        Login = NormalizarLogin(login);
        Email = email ?? throw new DomainException("USER_INVALID_EMAIL", "E-mail é obrigatório.");
        Phone = phone ?? throw new DomainException("USER_INVALID_PHONE", "Telefone é obrigatório.");
        CPF = cpf ?? throw new DomainException("USER_INVALID_CPF", "CPF é obrigatório.");
        Senha = senha ?? throw new DomainException("USER_INVALID_passwordHash", "Senha é obrigatória.");
        Role = role;
        Ativado = ativado;
        Foto = string.IsNullOrWhiteSpace(foto) ? null : foto.Trim();

        AddDomainEvent(new UsuarioCriadoDomainEvent(Id, Login));
    }

    public void AlterarDados(string nome, Email email, Phone telefone, Cpf cpf)
    {
        Nome = ValidarNome(nome);
        Email = email ?? throw new DomainException("USER_INVALID_EMAIL", "E-mail é obrigatório.");
        Phone = telefone ?? throw new DomainException("USER_INVALID_PHONE", "Telefone é obrigatório.");
        CPF = cpf ?? throw new DomainException("USER_INVALID_CPF", "CPF é obrigatório.");
    }

    public void AlterarSenhaPerfil(Senha senha)
    {
        DefinirNovaSenha(senha);
        AddDomainEvent(new SenhaAlteradaDomainEvent(Id));
    }

    public bool PodeLogar() => PodeLogar(DateTime.Now);

    public bool PodeLogar(DateTime agora) => BloqueioAte is null || BloqueioAte <= agora;

    public void RegistrarFalhaLogin() => RegistrarFalhaLogin(DateTime.Now);

    public void RegistrarFalhaLogin(DateTime agora)
    {
        TentativasLogin++;

        if (TentativasLogin < 5)
            return;

        BloqueioAte = agora.AddMinutes(5);
        AddDomainEvent(new UsuarioBloqueadoDomainEvent(Id, BloqueioAte.Value));
    }

    public void ResetarTentativasLogin()
    {
        TentativasLogin = 0;
        BloqueioAte = null;
    }

    public bool VerificarSituacao() => Ativado;

    public void GerarCodigo(string codigo) => GerarCodigo(codigo, DateTime.Now);

    public void GerarCodigo(string codigo, DateTime agora)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new DomainException("PASSWORD_RECOVERY_INVALID_CODE", "Código inválido.");

        Codigo = codigo.Trim();
        CodigoRecuperacaoExpiraEm = agora.AddMinutes(15);
        CodigoAtivo = true;
        TentativasCodigo = 0;

        AddDomainEvent(new RecuperacaoSenhaSolicitadaDomainEvent(Id, CodigoRecuperacaoExpiraEm.Value));
    }

    public bool PodeTrocarSenha(string codigoDigitado) => PodeTrocarSenha(codigoDigitado, DateTime.Now);

    public bool PodeTrocarSenha(string codigoDigitado, DateTime agora)
    {
        if (string.IsNullOrWhiteSpace(codigoDigitado))
            return false;

        return CodigoIsValido(agora) && Codigo == codigoDigitado.Trim();
    }

    public void AtivarUsuario()
    {
        if (Ativado)
            throw new DomainException("USER_ALREADY_ACTIVE", "Usuário já ativado.");

        Ativado = true;
        AddDomainEvent(new UsuarioAtivacaoAlteradaDomainEvent(Id, true));
    }

    public void DesativarUsuario()
    {
        if (!Ativado)
            throw new DomainException("USER_ALREADY_INACTIVE", "Usuário já desativado.");

        Ativado = false;
        AddDomainEvent(new UsuarioAtivacaoAlteradaDomainEvent(Id, false));
    }

    public bool CodigoIsValido() => CodigoIsValido(DateTime.Now);

    public bool CodigoIsValido(DateTime agora) =>
        CodigoAtivo &&
        CodigoRecuperacaoExpiraEm.HasValue &&
        CodigoRecuperacaoExpiraEm.Value > agora;

    public void RegistrarFalhaTrocaSenha()
    {
        TentativasCodigo++;

        if (TentativasCodigo >= 5)
            CodigoAtivo = false;
    }

    public void AlterarSenha(Senha novaSenha)
    {
        DefinirNovaSenha(novaSenha);
        LimparCodigo();
        AddDomainEvent(new SenhaAlteradaDomainEvent(Id));
    }

    public void DefinirNovaSenha(Senha senha)
    {
        Senha = senha ?? throw new DomainException(
            "USER_INVALID_PASSWORD",
            "Senha é obrigatória.");
    }

    public void LimparCodigo()
    {
        Codigo = null;
        CodigoRecuperacaoExpiraEm = null;
        TentativasCodigo = 0;
        CodigoAtivo = false;
    }

    // Mantidos apenas para compatibilidade com testes legados.
    public void ValorCodigo(string codigo) => Codigo = codigo;
    public void ValorTempo(DateTime tempo) => CodigoRecuperacaoExpiraEm = tempo;

    private static string ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("USER_INVALID_NAME", "Nome é obrigatório.");

        return nome.Trim();
    }

    private static string NormalizarLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new DomainException("USER_INVALID_LOGIN", "Login é obrigatório.");

        return login.Trim().ToLowerInvariant();
    }
}
