using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.UnitTests.IntegrationTestDB.Shared;

public class TestDbFixture : IDisposable
{
    public readonly HttpClient Client;
    private readonly WebApplicationFactory<Program> _factory;

    public TestDbFixture()
    {
        _factory = new TestDbWebApplicationFactory();
        Client = _factory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}
