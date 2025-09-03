namespace DbApp.Tests.Fixtures;

/// <summary>
/// Class used for Collection Definition, ensuring database fixture is shared across tests in the collection.
/// </summary>
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
