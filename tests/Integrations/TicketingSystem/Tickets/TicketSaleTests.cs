using System.Net;
using DbApp.Tests.Fixtures;

namespace DbApp.Tests.Integrations.TicketingSystem.Tickets;

[Collection("Database")]
public class TicketSaleTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task GetTicketSalesStats_ReturnsSuccess()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Call the API endpoint.
        var response = await client.GetAsync("/api/ticketing/tickets/sales/stats/search");

        // Should return 200 OK (even if no data).
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
