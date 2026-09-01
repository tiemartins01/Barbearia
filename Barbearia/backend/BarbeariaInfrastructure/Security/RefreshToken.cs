using BarbeariaCore.Domain.Entities;

namespace BarbeariaInfrastructure.Security;

public sealed class RefreshToken
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiraEm { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public bool Revogado { get;  private set; }
    public Guid FamilyId { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? RevocationReason { get; private set; }
    public string? CreatedByIp { get; private set; }

    private RefreshToken() { }

    public RefreshToken(int usuarioId,
            string token,
            DateTime expiraEm,
            Guid familyId,
            string? createdByIp,
            DateTime criadoEm)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(usuarioId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException(
                "Refresh token é obrigatório.",
                nameof(token));

        UsuarioId = usuarioId;
        Token = token;
        ExpiraEm = expiraEm;
        FamilyId = familyId;
        CreatedByIp = createdByIp;
        CriadoEm = criadoEm;
        Revogado = false;
    }

    public bool EstaExpirado(
            DateTime agoraUtc)
    {
        return ExpiraEm <= agoraUtc;
    }

    public bool EstaAtivo(
        DateTime agoraUtc)
    {
        return !Revogado &&
               !EstaExpirado(agoraUtc);
    }

    public void Revogar(
        DateTime agoraUtc,
        string? substituidoPor,
        string? motivo)
    {
        if (Revogado)
            return;

        Revogado = true;
        RevokedAtUtc = agoraUtc;
        ReplacedByToken = substituidoPor;
        RevocationReason = motivo;
    }
}
