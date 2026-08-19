using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Entities
{
    public sealed class Servicos
    {

        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public int Duracao { get; private set; }
        public decimal Preco { get; private set; }
        public bool Ativo { get; private set; }


        private Servicos() { }

        public Servicos(string nome, int duracao, decimal preco, bool ativo)
        {
            if(string.IsNullOrEmpty(nome))
                throw new DomainException("Nome do serviço é obrigatório.");

            Nome = nome;
            Duracao = duracao;
            Preco = preco;
            Ativo = ativo;
        }

        public void AlterarValor(int NovoValor)
        {
            if(NovoValor > 0)
                Preco = NovoValor;
            else
                throw new DomainException("Preço deve ser maior que 0.");
        }

        public void AtivarServico(string NomeServico)
        {
            if (string.IsNullOrEmpty(NomeServico))
                throw new DomainException("Necessário inserir nome do serviço!");

            if(Ativo)
                throw new DomainException("Serviço já ativo!");

            Ativo = true;
        }

        public void DesativarServico(string NomeServico)
        {
            if (string.IsNullOrEmpty(NomeServico))
                throw new DomainException("Necessário inserir nome do serviço!");

            if (!Ativo)
                throw new DomainException("Serviço já desativado!");

            Ativo = false;
        }

    }
}
