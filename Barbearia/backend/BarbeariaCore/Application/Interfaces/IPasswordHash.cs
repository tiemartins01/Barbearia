namespace BarbeariaCore.Application.Interfaces
{
    public interface IPasswordHash
    {
        string Hash(string senha);
        bool Verify(string senha, string hash);

    }
}
