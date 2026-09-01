using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbeariaCore.Application.Policies
{
    public sealed class UsuarioUnicidadePolicy
    {

        private readonly IUsuarioRepository _usuarios;

        public UsuarioUnicidadePolicy(IUsuarioRepository usuarios)
        {
           _usuarios = usuarios;
        }

        public async Task ValidarAsync(
            Email email,
            Cpf cpf,
            Telefone telefone,
            string login,
            CancellationToken cancellationToken = default)
        {
            if (await _usuarios.ObterPorEmailAsync(email.Valor, cancellationToken) is not null)
                throw new DomainException(
                    "USER_EMAIL_ALREADY_EXISTS",
                    "E-mail já cadastrado.");

            if (await _usuarios.ObterPorCpfAsync(cpf.Valor,cancellationToken) is not null)
                throw new DomainException(
                    "USER_CPF_ALREADY_EXISTS",
                    "CPF já cadastrado.");

            if (await _usuarios.ObterPorTelefoneAsync(telefone.Valor,cancellationToken) is not null)
                throw new DomainException(
                    "USER_PHONE_ALREADY_EXISTS",
                    "Telefone já cadastrado.");

            if (await _usuarios.ObterPorLoginAsync(login,cancellationToken) is not null)
                throw new DomainException(
                    "USER_LOGIN_ALREADY_EXISTS",
                    "Login já cadastrado.");
        }
    }
}
