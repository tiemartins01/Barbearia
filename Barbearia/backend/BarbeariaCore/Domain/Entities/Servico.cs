using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.Common;

namespace BarbeariaCore.Domain.Entities
{
    public sealed class Servico : AggregateRoot
    {

        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public int Duracao { get; private set; }
        public decimal Preco { get; private set; }
        public bool Ativo { get; private set; }


        private Servico() { }

        public Servico(string nome, int duracao, decimal preco, bool ativo)
        {
            if(string.IsNullOrWhiteSpace(nome))
                throw new DomainException("SERVICE_INVALID_NAME", "Nome do serviço é obrigatório.");

            if(duracao <= 0 )
                throw new DomainException("SERVICE_INVALID_DURATION","Duração deve ser maior que 0.");

            if (preco <= 0)
                throw new DomainException("SERVICE_INVALID_PRICE", "Preço deve ser maior que 0.");

            Nome = nome.Trim();
            Duracao = duracao;
            Preco = preco;
            Ativo = ativo;
        }

        public void AlterarPreco(decimal novoPreco)
        {
            if(novoPreco <= 0)
                throw new DomainException("SERVICE_INVALID_PRICE", "Preço deve ser maior que 0.");

            Preco = novoPreco;                
        }

        public void Ativar()
        {
            if(Ativo)
                throw new DomainException("SERVICE_ALREADY_ACTIVE", "Serviço já ativo!");

            Ativo = true;
        }

        public void Desativar()
        {
            if (!Ativo)
                throw new DomainException("SERVICE_ALREADY_INACTIVE", "Serviço já desativado!");

            Ativo = false;
        }

        public void AlterarNome(string nomeServico)
        {
            if (string.IsNullOrWhiteSpace(nomeServico))
                throw new DomainException("SERVICE_INVALID_NAME", "Necessário inserir nome do serviço!");

            Nome = nomeServico.Trim();
        }

        public void AlterarDuracao(int duracao)
        {
            if (duracao <= 0)
                throw new DomainException("SERVICE_INVALID_DURATION","Duração deve ser maior que 0.");

            Duracao = duracao;
        }

    }
}
