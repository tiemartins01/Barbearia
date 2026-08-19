namespace BarbeariaCore.Domain.Common;

public interface IDomainEvent
{ // Com isso, todo sistema consegue ser tratado da mesma maneira, mesmo sendo eventos diferentes.
    DateTime OccurredAtUtc { get; }
}

//Exemplo conceitual:

//IDomainEvent evento1 =
//    new UsuarioCriadoDomainEvent(...);

//    IDomainEvent evento2 =
//        new SenhaAlteradaDomainEvent(...);

//    Os dois podem ser armazenados na mesma lista: