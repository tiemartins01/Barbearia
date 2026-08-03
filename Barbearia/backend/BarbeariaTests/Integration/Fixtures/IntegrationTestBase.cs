namespace BarbeariaTests.Integration;

[Collection(IntegrationCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected IntegrationTestBase(PostgreSqlFixture fixture)
    {
        Fixture = fixture;
    }

    protected PostgreSqlFixture Fixture { get; }

    public Task InitializeAsync() => Fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
