using System;

namespace BarbeariaCore.Infrastructure.Data.Operational;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = string.Empty; // Guarda qual evento originou essa mensagem.
    public string Payload { get; private set; } = string.Empty; // É o evento convertido para JSON.
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public int RetryCount { get; private set; } // Conta quantas vezes a publicação falhou.
    public string? LastError { get; private set; }

    private OutboxMessage() { }
    //  evita o problema que é conhecido como inconsistência entre banco e mensageria

    //Observe que:

    //ProcessedAtUtc

    //RetryCount

    //LastError

    //não são preenchidos.

    //Porque a mensagem acabou de nascer.

    //Ela ainda:

    //não foi publicada;
    //    nunca falhou.

    public OutboxMessage(Guid id, string type, string payload, DateTime occurredAtUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
    }

    //Ao invés de enviar imediatamente para RabbitMQ:

    //UsuarioCriadoDomainEvent
    //        ↓
    //OutboxMessage
    //        ↓
    //Banco


    //Tudo fica salvo na mesma transação.

    //Depois:

    //OutboxProcessor
    //        ↓
    //RabbitMQ


    //Mesmo que RabbitMQ esteja fora do ar, a mensagem continua guardada.

    public void MarkProcessed(DateTime processedAtUtc)
    {
        if (ProcessedAtUtc.HasValue)
            throw new InvalidOperationException(
                "A mensagem já foi processada.");


        ProcessedAtUtc = processedAtUtc;
        LastError = null;
    }

    public void RegisterFailure(string error)
    {
        if (ProcessedAtUtc.HasValue)
            throw new InvalidOperationException(
                "A mensagem já foi processada.");

        RetryCount++;
        LastError = error.Length <= 2000 ? error : error[..2000];
    }
}
