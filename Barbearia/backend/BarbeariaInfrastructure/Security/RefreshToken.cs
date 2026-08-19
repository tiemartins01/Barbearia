namespace BarbeariaInfrastructure.Security;

public sealed class RefreshToken
{
    public int Id { get; set; }
    public int Id_usuario { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEM { get; set; }
    public DateTime CriadoEM { get; set; }
    public bool Revogado { get; set; }
    public Guid FamilyId { get; set; }
    public string? ReplacedByToken { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevocationReason { get; set; }
    public string? CreatedByIp { get; set; }

    public bool IsExpired(DateTime utcNow) => ExpiraEM <= utcNow;
    public bool IsActive(DateTime utcNow) => !Revogado && !IsExpired(utcNow);
}
