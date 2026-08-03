using Barbearia.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.BackgroundServices;

public sealed class OutboxProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorService> _logger;

    public OutboxProcessorService(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar o Outbox.");
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var messages = await context.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null && x.RetryCount < 10)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(50)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                // Ponto de integração com RabbitMQ, Kafka ou Service Bus.
                // Nesta fase o evento é publicado no pipeline operacional por log estruturado.
                _logger.LogInformation(
                    "Domain event publicado pelo Outbox. EventId={EventId} EventType={EventType} Payload={Payload}",
                    message.Id,
                    message.Type,
                    message.Payload);

                message.MarkProcessed(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                message.RegisterFailure(ex.Message);
                _logger.LogWarning(ex, "Falha ao publicar evento do Outbox. EventId={EventId}", message.Id);
            }
        }

        if (messages.Count > 0)
            await context.SaveChangesAsync(cancellationToken);
    }
}
