using Barbearia.Core.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barbearia.Core.Domain.Common;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    [NotMapped] // Não crie coluna nem relacionamento para essa propriedade.
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly(); // Algo relevante para o negócio que já aconteceu.

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent); // evita alguem enviar vazio
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}

//Construtor do Usuario
//    ↓
//valida nome, login, e-mail etc.
//    ↓
//preenche as propriedades
//    ↓
//cria UsuarioCriadoDomainEvent
//    ↓
//AddDomainEvent(...)
//    ↓
//_domainEvents.Add(evento)
