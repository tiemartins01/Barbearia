namespace BarbeariaCore.Application.Models;

public sealed record RefreshTokenData
{
    public int Id { get; init; }
    public int UsuarioId { get; init; }

    public string Token { get; init; } = string.Empty;

    public DateTime CriadoEm { get; init; }
    public DateTime ExpiraEm { get; init; }

    public bool Revogado { get; init; }

    public Guid FamilyId { get; init; }

    public string? CreatedByIp { get; init; }
    public string? ReplacedByToken { get; init; }
    public string? RevocationReason { get; init; }

    public bool EstaExpirado(DateTime agora)
    {
        return ExpiraEm <= agora;
    }
}