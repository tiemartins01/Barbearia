using Barbearia.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace BarbeariaTests.Integration;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("barbearia_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .EnableDetailedErrors()
            .Options;

        return new AppDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();

        // Limpa somente as tabelas que realmente existem no banco de teste.
        // Isso evita o erro 42P01 quando uma tabela citada manualmente,
        // como "avaliacao", ainda não existe nas migrations atuais.
        const string sql = """
            DO $$
            DECLARE
                tabela RECORD;
            BEGIN
                FOR tabela IN
                    SELECT tablename
                    FROM pg_tables
                    WHERE schemaname = 'public'
                      AND tablename <> '__EFMigrationsHistory'
                LOOP
                    EXECUTE format(
                        'TRUNCATE TABLE %I.%I RESTART IDENTITY CASCADE',
                        'public',
                        tabela.tablename);
                END LOOP;
            END
            $$;
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
        context.ChangeTracker.Clear();
    }
}
