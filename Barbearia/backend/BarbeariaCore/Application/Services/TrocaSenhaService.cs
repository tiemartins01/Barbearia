using Barbearia.Core.DTO;
using Barbearia.Core.Excepetion;
using Barbearia.Core.Interface;
using Barbearia.Core.Domain.Entities;

namespace Barbearia.Core.Service
{
    public sealed class TrocaSenhaService : ITrocaSenhaService
    {

        private readonly ITrocaSenhaRepository _repository;
        private readonly IUnitOfWork _uow;
        public TrocaSenhaService(ITrocaSenhaRepository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task<DTOResposta> RealizarTrocaSenha(string codigo, string email, string senha, string senharepetida)
        {
            var usuario = await _repository.PegaInformacaoUsuario(email);

            if (usuario is null || !usuario.Ativado)
                throw new DomainException("PASSWORD_RESET_INVALID_DATA", "Dados inválidos!");

            if (!usuario.CodigoIsValido())
                throw new DomainException("PASSWORD_RESET_CODE_EXPIRED", "Codigo expirado! Solicite um novo código!");

            if (!usuario.PodeTrocarSenha(codigo))
            {
                await RegistrarFalhaAsync(usuario);
                throw new DomainException("PASSWORD_RESET_INVALID_CODE", "Dados inválidos!");
            }

            if (!string.Equals(senha,senharepetida,StringComparison.Ordinal))
            {
                throw new DomainException("PASSWORD_RESET_PASSWORD_MISMATCH", "Dados inválidos!");
            }

            usuario.AlterarSenha(senha);

            await _repository.AtualizaUsuario(usuario);

            await _uow.SaveChangesAsync();

            return new DTOResposta
            {
                Sucesso = true,
                Mensagem = "Senha alterada!"

            };
        }

        private async Task RegistrarFalhaAsync(Usuario usuario)
        {
            usuario.RegistrarFalhaTrocaSenha();
            await _repository.AtualizaUsuario(usuario);
            await _uow.SaveChangesAsync();
        }

    }
}
