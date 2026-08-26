namespace BarbeariaCore.Domain.Policies
{
    public static class PoliticaAutenticacao
    {
        public const int LimiteTentativas = 5;
        public static readonly TimeSpan DuracaoBloqueio = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan TempoCodigo = TimeSpan.FromMinutes(15);
    }
}
