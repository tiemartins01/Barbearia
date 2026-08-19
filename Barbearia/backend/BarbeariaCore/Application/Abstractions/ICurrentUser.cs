namespace BarbeariaCore.Application.Abstractions;
// COM ISSO NÃO É MAIS NECESSÁRIO FICAR CHAMANDO JWT NO CONTROLLER TODA VEZ QUANDO QUER CONFERIR O USUÁRIO
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    int UserId { get; }
    string Name { get; }
    string Role { get; }
}
