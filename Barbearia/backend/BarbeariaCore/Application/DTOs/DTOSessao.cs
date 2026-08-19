namespace BarbeariaCore.Application.DTOs;

public sealed class DTOSessao
{
    public int Id { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime ExpiraEm { get; init; }
    public bool Revogado { get; init; }
    public bool Atual { get; init; }
}
