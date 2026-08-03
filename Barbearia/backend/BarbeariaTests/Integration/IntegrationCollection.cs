namespace BarbeariaTests.Integration;

[CollectionDefinition(Name)]
public sealed class IntegrationCollection : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "integration-tests";
}
